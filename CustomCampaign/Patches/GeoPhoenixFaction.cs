using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Entities.Sites;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Geoscape.Levels.Factions;
using PhoenixPoint.Tactical.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaign.Patches.GeoPhoenixFactionPatches
{
	[HarmonyPatch(typeof(GeoPhoenixFaction), "CreateInitialSquad")]
	public class CreateInitialSquad
	{
		// Because game is initializing, our geoscape mod is not ready, access ModMain directy
		static void Postfix(GeoPhoenixFaction __instance, GeoSite site) {
			if (ModMain.Main.Config.StartingSoldiersModifier == 0)
				return;

			GeoLevelController gs = __instance.GeoLevel;
			GameDifficultyLevelDef gameDifficulty = gs.CurrentDifficultyLevel;
			GeoPhoenixFaction faction = __instance;

			int startingSoldiersCount = gameDifficulty.StartingSquadTemplate.Length + ModMain.Main.Config.StartingSoldiersModifier;
			if (startingSoldiersCount < 1)
				startingSoldiersCount = 1;


			startingSoldiersCount = Math.Min(startingSoldiersCount, Configuration.MaxStartingSoldiers);

			ModMain.Main.Logger.LogInfo($"Changing starting squad from {gameDifficulty.StartingSquadTemplate.Length} to {startingSoldiersCount}.");
			TacCharacterDef[] squadTemplates = GenerateStartingSquad(startingSoldiersCount);

			var vehicle = faction.Vehicles.First();
			foreach (var c in vehicle.GetAllCharacters().ToList())
				vehicle.RemoveCharacter(c);

			var @base = site.GetComponent<GeoPhoenixBase>();

			foreach (var unitTemplate in squadTemplates) {
				//pass from options whether to randomize armor colors as well
				var unit = gs.CharacterGenerator.GenerateUnit(faction, unitTemplate);
				gs.CharacterGenerator.ApplyGenerationParameters(unit, gameDifficulty.StartingSquadGenerationParams);
				gs.CharacterGenerator.RandomizeIdentity(unit);

				GeoCharacter character = unit.SpawnAsCharacter();
				if (vehicle.FreeCharacterSpace >= character.OccupingSpace)
					vehicle.AddCharacter(character);
				else if (@base != null)
					@base.Site.AddCharacter(character);
			}

		}

		static TacCharacterDef[] GenerateStartingSquad(int count) {

			if (count == 0)
				return new TacCharacterDef[0];

			List<TacCharacterDef> squad = new List<TacCharacterDef>();

			// Base generation template on easiest difficulty
			TacCharacterDef[] baseTemplate = SharedData.GetSharedDataFromGame().DifficultyLevels[0].StartingSquadTemplate;


			var groups = baseTemplate
				.GroupBy(t => t)
				.OrderByDescending(t => t.Count())
				.ToList();

			List<TacCharacterDef> queue = new List<TacCharacterDef>();
			// Fill queue by adding from each class
			if (count < 3) {
				squad.AddRange(groups.Select(s => s.Key).Take(count));
			} else {
				queue.AddRange(groups.Select(s => s.Key));
				// Now add remaining of the squad by class

				foreach (var group in groups) {
					TacCharacterDef @class = group.Key;
					while (queue.Count(q => q == @class) < group.Count()) {
						queue.Add(@class);
					}
				}

				for (int i = 0; i < count; ++i) {
					int queuePos = i % queue.Count;
					TacCharacterDef soldier = queue[queuePos];
					squad.Add(soldier);
				}
			}

			return squad.ToArray();
		}
	}
}
