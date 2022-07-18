using HarmonyLib;
using PhoenixPoint.Geoscape.Entities;

namespace CustomCampaign.Patches.GeoReseatchCompleteDataBindPatches
{
	[HarmonyPatch(typeof(ItemStorage), "AddItem")]
	class AddItem
	{
		static bool Prefix(GeoItem item) {
			if (!Geoscape.Mod.NoAmmo) return true;

			// Don't add ammo to any storage

			if (Geoscape.Mod.IsManufacturableItemAmmo(item.ItemDef)) return false;

			return true;
		}

		static void Postfix(GeoItem item) {
			if (!Geoscape.Mod.NoAmmo) return;

			var ammo = item.CommonItemData.Ammo;
			if (ammo != null) {
				item.ReloadForFree();
			}

		}
	}
}
