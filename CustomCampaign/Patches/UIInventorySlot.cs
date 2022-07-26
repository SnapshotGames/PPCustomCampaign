using HarmonyLib;
using PhoenixPoint.Common.View.ViewControllers.Inventory;
using PhoenixPoint.Tactical.Entities.Equipments;
using System.Linq;

namespace CustomCampaign.Patches.UIInventorySlotPatches
{
	[HarmonyPatch(typeof(UIInventorySlot), "UpdateItem")]
	class UpdateItem
	{
		static void Postfix(UIInventorySlot __instance) {
			// Don't show ammo counter only in geoscape
			if (Geoscape.Mod != null) {
				if (Geoscape.Mod.NoAmmo) {
					if (__instance.Item != null && __instance.Item.ItemDef is TacticalItemDef tacDef && tacDef.CompatibleAmmunition.Any()) {
						__instance.AmmoImageNode.gameObject.SetActive(false);
						__instance.EmptyAmmoImageNode.gameObject.SetActive(false);
					}
				}
				return;
			}
		}
	}
}
