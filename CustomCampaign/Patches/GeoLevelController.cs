using HarmonyLib;
using PhoenixPoint.Geoscape.Levels;

namespace CustomCampaign.Patches.GeoLevelControllerPatches
{
	[HarmonyPatch(typeof(GeoLevelController), "ChangeWorldPopulation")]
	class ChangeWorldPopulation
	{
		static void Prefix(ref int delta) {
			//if (Geoscape.Mod != null)
			delta = Geoscape.Mod.GetPopulationChange(delta);
		}
	}
}
