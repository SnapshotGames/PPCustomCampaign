using HarmonyLib;
using PhoenixPoint.Common.Entities.Items;

namespace CustomCampaign.Patches.ItemManufacturingPatches
{
	[HarmonyPatch(typeof(ItemManufacturing), "AddAvailableItem")]
	public class AddAvailableItem
	{
		static bool Prefix(ItemDef item) {
			// This can be called before our mod is ready, in this case don't do anything, our GS mod will do a cleanup after init

			if (!(Geoscape.Mod.NoAmmo) ) return true;

			if (Geoscape.Mod.IsManufacturableItemAmmo(item)) return false;
			return true;
		}
	}
}
