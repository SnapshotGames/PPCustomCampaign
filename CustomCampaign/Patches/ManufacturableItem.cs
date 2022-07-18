using HarmonyLib;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Geoscape.Levels;

namespace CustomCampaign.Patches
{
	[HarmonyPatch(typeof(ManufacturableItem), "GetCostInManufacturePoints")]
	public class GetCostInManufacturePoints
	{
		static void Postfix(GeoFaction faction, ref int __result) {
			if (faction != faction.GeoLevel.PhoenixFaction) return;

			__result = Geoscape.Mod.GetItemManufactureCost(__result);

		}
	}
}
