using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Levels.MapGeneration;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Geoscape.Core;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Tactical.Entities.ActorsInstance;

namespace CustomCampaign.Patches.GeoMissionPatches
{
	[HarmonyPatch(typeof(GeoMission), "ApplyTacticalMissionResult")]
	public class ApplyTacticalMissionResult
	{
		static bool Prefix(TacMissionResult result, GeoSquad squad) {
			Geoscape.Mod.UpdateDifficulty();
			return true;
		}

		static void Postfix() => Geoscape.Mod.RestoreDifficulty();
	}
	[HarmonyPatch(typeof(GeoMission), "GetMissionThreatLevel")]
	public class GetMissionThreatLevel
	{
		static void Postfix(ref DifficultyThreatLevel __result) => Geoscape.Mod.UpdateThreatLevel(ref __result);
	}
	[HarmonyPatch(typeof(GeoMission), "PrepareTacticalGame")]
	public class PrepareTacticalGame
	{
		static bool Prefix(GeoSite site, GeoSquad squad) {

			Geoscape.Mod.UpdateDifficulty();
			return true;
		}

		static void Postfix() => Geoscape.Mod.RestoreDifficulty();
	}
	[HarmonyPatch(typeof(GeoMission), "GenerateTacticalReward")]
	public class GenerateTacticalReward
	{
		static void Postfix(TacMissionResult result, GeoFactionReward reward) {
			if (!Geoscape.Mod.NoAmmo) return;

			Geoscape.Mod.RemoveAmmoFromStorage(reward.Items);
		}
	}
	[HarmonyPatch(typeof(GeoMission), "TryReloadItem")]
	public class TryReloadItem
	{
		static bool Prefix(GeoItem item, ItemStorage storage, string storageName, ref bool __result) {
			if (!Geoscape.Mod.NoAmmo) return true;

			var ammo = item.CommonItemData.Ammo;
			if (ammo != null) {
				int deltaCharges = ammo.ParentItem.ItemDef.ChargesMax - ammo.CurrentCharges;

				if (deltaCharges > 0) {
					__result = item.CommonItemData.Ammo.ReloadCharges(deltaCharges);
					ModMain.Main.Logger.LogInfo($"Free relaod: {item.ItemDef} + {deltaCharges} = {__result}");
					return false;
				}
			}
			
			return true;
		}
	}
	[HarmonyPatch(typeof(GeoMission), "AddCratesToMissionData")]
	public class AddCratesToMissionData
	{
		static void Postfix(GeoMission __instance, TacMissionData missionData, MapPlotDef plotDef, bool allowResourceCrates ) {
			if (!Geoscape.Mod.NoAmmo) return;

			foreach(var factionData in missionData.MissionParticipants) {
				foreach(var deployData in factionData.ActorDeployData) {
					if (!(deployData.ActorInstance is TacEquipmentCrateData crateData))
						continue;

					Geoscape.Mod.RemoveAmmoFromCrateDeployment(crateData);
				}
			}
		}
	}
}
