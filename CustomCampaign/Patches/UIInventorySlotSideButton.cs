using HarmonyLib;
using PhoenixPoint.Common.View.ViewControllers.Inventory;
using static PhoenixPoint.Common.View.ViewControllers.Inventory.UIInventorySlotSideButton;

namespace CustomCampaign.Patches.UIInventorySlotSideButtonPatches
{
	[HarmonyPatch(typeof(UIInventorySlotSideButton), "GetState")]
	class GetState
	{
		static void Postfix(ref GeneralState __result) {
			// Don't show add ammo button in geoscape
			if (Geoscape.Mod != null && Geoscape.Mod.NoAmmo) {
				if (__result.Action == SideButtonAction.AddAmmo ||
					__result.Action == SideButtonAction.LoadAmmo)
					__result = new GeneralState()
					{
						State = SideButtonState.Hidden
					};
			}
		}
	}

}
