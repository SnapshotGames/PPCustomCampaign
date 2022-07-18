using HarmonyLib;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Levels.Factions;

namespace CustomCampaign.Patches.GeoAlienFactionPatches
{
	[HarmonyPatch(typeof(GeoAlienFaction), "ProgressEvolution")]
	class ProgressEvolution
	{
		static bool Prefix() {
			Geoscape.Mod.UpdateDifficulty();
			return true;
		}

		static void Postfix() => Geoscape.Mod.RestoreDifficulty();
	}

	[HarmonyPatch(typeof(GeoAlienFaction), "AlienBaseDestroyed")]
	class AlienBaseDestroyed
	{
		static bool Prefix(GeoAlienBase alienBase) {
			Geoscape.Mod.UpdateDifficulty();
			return true;
		}

		static void Postfix() => Geoscape.Mod.RestoreDifficulty();
	}

}
