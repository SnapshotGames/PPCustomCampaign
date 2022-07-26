using HarmonyLib;
using PhoenixPoint.Common.View.ViewControllers.Inventory;

namespace CustomCampaign.Patches.UIInventoryListPatches
{
	[HarmonyPatch(typeof(UIInventoryList), "TryStripAmmo")]
	class TryStripAmmo
	{
		static bool Prefix() {
			if (Geoscape.Mod == null || !Geoscape.Mod.NoAmmo) return true;
			return false;
		}
	}

	[HarmonyPatch(typeof(UIInventoryList), "TryLoadItemWithItem")]
	class TryLoadItemWithItem
	{
		static bool Prefix() {
			if (Geoscape.Mod == null || !Geoscape.Mod.NoAmmo) return true;
			return false;
		}
	}
}
