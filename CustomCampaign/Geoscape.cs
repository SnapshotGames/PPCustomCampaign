using Base;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities.ActorsInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomCampaign
{
	public class Geoscape : ModGeoscape
	{
		public class InstanceData
		{
			public int TacticalDifficultyModifier = 0;
			public int MissionThreatLevelModifier = 0;
			public int DefaultDiffucly = 0;
			public int HumanPopulationLost = 100;
			public int ItemManufactureSpeed = 100;
			public bool NoAmmo = false;
			public int AmbushChance = 100;
			public int HavenDefenseFrequency = 100;
			public int CurrentPopulation = 0;
		}

		public static Geoscape Mod => ModMain.Main.GeoscapeMod;
		private GameDifficultyLevelDef[] _difficultiesMap = null;
		private SharedData _sharedData;
		public new ModMain Main => (ModMain)base.Main;

		private InstanceData _data;

		public bool NoAmmo => _data?.NoAmmo ?? false;
		public int HavenDefeneseFrequency => _data?.HavenDefenseFrequency ?? 100;

		private bool _gameStarted = false;

		public override void Init() {
			_sharedData = SharedData.GetSharedDataFromGame();
			_difficultiesMap = SharedData.GetSharedDataFromGame().DifficultyLevels;
		}

		public override void OnGeoscapeStart() {
			_gameStarted = true;

			if (_data == null) {
				if (Controller.IsFromSaveGame) {
					// Loaded old save withot mod data, use default mod settings
					_data = new InstanceData();
				} else {
					// New game, use current mod settings
					_data = new InstanceData()
					{
						MissionThreatLevelModifier = Main.Config.MissionThreatLevelModifier,
						TacticalDifficultyModifier = Main.Config.TacticalDifficultyModifier,
						HumanPopulationLost = Main.Config.HumanPopulationLost,
						ItemManufactureSpeed = Main.Config.ItemManufactureSpeed,
						NoAmmo = Main.Config.NoAmmo,
						DefaultDiffucly = _difficultiesMap.IndexOf(Controller.CurrentDifficultyLevel),
						AmbushChance = Main.Config.AmbushChance,
						HavenDefenseFrequency = Main.Config.HavenDefenseFrequency,
					};
				}

			}

			if (NoAmmo) {
				RemoveAmmoFromManufacture();
				RemoveAmmoFromStorage(Controller.PhoenixFaction.ItemStorage);
				RemoveAmmoFromCharacters();
			}

			Controller.EventSystem.ExplorationAmbushChance = Mathf.Clamp(_data.AmbushChance, 0, 100);

			if (!Controller.IsFromSaveGame && HavenDefeneseFrequency != 100) {
				// reset all bases' counter
				foreach (var alienBase in Controller.AlienFaction.Bases)
					alienBase.ResetHavenAttackCounter();


			}

			if (!Controller.IsFromSaveGame)
				UpdateStartingResources();

			if (Controller.IsFromSaveGame) {
				UpdateCurrentWorldPopulation();
			}

		}

		private void UpdateCurrentWorldPopulation() {
			if (_data.HumanPopulationLost == 100 || _data.CurrentPopulation <= 0) return;

			int current = Controller.CurrentPopulation;
			var field = typeof(GeoLevelController).GetProperty(nameof(_data.CurrentPopulation), BindingFlags.Public | BindingFlags.Instance);
			field.SetValue(Controller, _data.CurrentPopulation);
			Main.Logger.LogInfo($"Chaning world popualtion from {current} to { _data.CurrentPopulation} = {Controller.CurrentPopulation}");

			// Refresh ui bar
			var method = typeof(UIModuleInfoBar).GetMethod("SetPopulationVisibility", BindingFlags.NonPublic | BindingFlags.Instance);
			method.Invoke(Controller.View.GeoscapeModules.ResourcesModule, new object[0]);

		}

		private void UpdateStartingResources() {
			if (Main.Config.StartingResources == 100) return;

			int mod = Main.Config.StartingResources;
			if (mod < 0) mod = 0;

			var field = typeof(Wallet).GetField("_resources", BindingFlags.NonPublic | BindingFlags.Instance);
			var dict = field.GetValue(Controller.PhoenixFaction.Wallet) as Dictionary<ResourceType, ResourceUnit>;
			ResourcePack pack = new ResourcePack();
			foreach (var val in dict.Values) {

				pack.Add(val.Type, (int)(val.Value * mod) / 100);
			}

			Controller.PhoenixFaction.Wallet.Clear(OperationReason.None);
			Controller.PhoenixFaction.Wallet.Apply(pack, OperationReason.Gift);
		}

		public void RemoveAmmoFromManufacture() {
			foreach (var faction in Controller.Factions) {
				if (faction.Manufacture == null) continue;
				faction.Manufacture.ManufacturableItems.RemoveAll(i => i.RelatedItemDef != null &&
						i.RelatedItemDef.Tags.Contains(_sharedData.SharedGameTags.AmmoTag));
			}

		}
		public void RemoveAmmoFromStorage(ItemStorage storage) {

			foreach (var def in storage.Items.Keys
				.Where(i => i.Tags.Contains(_sharedData.SharedGameTags.AmmoTag)).ToList()
				) {
				GeoItem item = storage.Items[def];
				while (item.CommonItemData.Count > 0)
					storage.RemoveItem(item);
			}

		}
		public void RemoveAmmoFromCharacters() {

			foreach (var character in Controller.PhoenixFaction.Characters) {
				var inventory = character.InventoryItems.Where(i => !i.ItemDef.Tags.Contains(_sharedData.SharedGameTags.AmmoTag)).ToList();
				var equipment = character.EquipmentItems.Where(i => !i.ItemDef.Tags.Contains(_sharedData.SharedGameTags.AmmoTag)).ToList();
				character.SetItems(null, equipment, inventory, true);
			}
		}
		public void RemoveAmmoFromCrateDeployment(TacEquipmentCrateData crateData) {

			if (crateData.Items != null && crateData.Items.Length > 0) {

				var items =
					crateData.Items.Where(i => !(i.ItemDef.Tags.Contains(_sharedData.SharedGameTags.AmmoTag))).ToArray();

				crateData.Items = items;
			}
			if (crateData.AdditionalItems != null && crateData.AdditionalItems.Count > 0) {
				crateData.AdditionalItems.RemoveAll(i => i.ItemDef.Tags.Contains(_sharedData.SharedGameTags.AmmoTag));
			}

		}

		public override object RecordGeoscapeInstanceData() {
			// Record current population only when playing, ignore autosave after tactical as population is not corrected yet according to mod values
			if (_gameStarted || _data.CurrentPopulation == 0)
				_data.CurrentPopulation = Controller.CurrentPopulation;
			return _data;
		}
		public override void ProcessGeoscapeInstanceData(object instanceData) {
			_data = (InstanceData)instanceData;
		}

		public void UpdateDifficulty() {
			if (_data.TacticalDifficultyModifier == 0)
				return;

			int tacDifficulty = _data.DefaultDiffucly + _data.TacticalDifficultyModifier;
			if (tacDifficulty < 0) tacDifficulty = 0;
			if (tacDifficulty >= _difficultiesMap.Length) tacDifficulty = _difficultiesMap.Length - 1;

			GameDifficultyLevelDef old = Controller.CurrentDifficultyLevel;
			Controller.CurrentDifficultyLevel = _difficultiesMap[tacDifficulty];
			//Main.Logger.LogInfo($"Changed tactical difficulty from '{old}' to '{Controller.CurrentDifficultyLevel}'");
		}
		public void RestoreDifficulty() {
			if (_data.TacticalDifficultyModifier == 0)
				return;

			Controller.CurrentDifficultyLevel = _difficultiesMap[_data.DefaultDiffucly];
		}
		public void UpdateThreatLevel(ref DifficultyThreatLevel __result) {

			if (_data.MissionThreatLevelModifier == 0) return;

			int threatLevel = (int)__result + _data.MissionThreatLevelModifier;
			if (threatLevel < 0) threatLevel = 0;
			if (threatLevel >= 3) threatLevel = 3;

			DifficultyThreatLevel old = __result;
			__result = (DifficultyThreatLevel)threatLevel;
			//Main.Logger.LogInfo($"Changed mission threat level from '{old}' to '{__result}'");
		}
		public int GetPopulationChange(int delta) {
			if (_data.HumanPopulationLost == 100) return delta;

			int old = delta;


			long multiplier = Math.Max(0, _data.HumanPopulationLost);
			long res = ((long)delta * multiplier) / 100;

			delta = (int)res;

			return delta;
		}
		public int GetItemManufactureCost(int cost) {
			if (_data == null) return cost;

			if (_data.ItemManufactureSpeed == 100) return cost;

			if (_data.ItemManufactureSpeed <= 0)
				return 0;

			int old = cost;

			int div = Math.Max(0, _data.ItemManufactureSpeed);
			cost = (cost * 100) / div;

			return cost;
		}

		public bool IsManufacturableItemAmmo(ItemDef item) {
			return (item.Tags.Contains(_sharedData.SharedGameTags.AmmoTag) ||
				(item.RelatedItemDef != null && item.RelatedItemDef.Tags.Contains(_sharedData.SharedGameTags.AmmoTag)));
		}

		public int GetHavenAttackCounter(int result) {
			if (HavenDefeneseFrequency == 100) return result;
			int old = result;
			if (HavenDefeneseFrequency <= 0) {
				result = ushort.MaxValue;
			} else {
				result = (result * 100) / HavenDefeneseFrequency;
				if (result < 0) result = 1;
			}
			Main.Logger.LogInfo($"Haven attack frequency changed from {old} to {result}");
			return result;
		}
	}


}
