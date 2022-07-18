using HarmonyLib;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.View.ViewModules;

namespace CustomCampaign.Patches.UIModuleWeaponSelectionModule
{
	[HarmonyPatch(typeof(UIModuleWeaponSelection), "SetAvailableMagazines")]
	public class SetAvailableMagazines
	{
		static void Postfix (UIModuleWeaponSelection __instance, Weapon weapon) {
			if (!Tactical.Mod.NoAmmo) return;

			if (!__instance.MagazineInfo.gameObject.activeInHierarchy)
				return;

			__instance.MagazineInfo.StatText.text = "∞";
			__instance.MagazineInfoExtended.StatText.text = "∞"; 
		}
	}
}
