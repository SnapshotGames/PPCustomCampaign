using HarmonyLib;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Equipments;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaign.Patches.ReloadAbilityPatches
{
	[HarmonyPatch(typeof(ReloadAbility), "Reload")]
	public class Reload
	{
		static bool Prefix(Equipment equipment, TacticalItem ammoClip) {
			if (!Tactical.Mod.NoAmmo) return true;

			var ammoClone = new TacticalItem();
			ammoClone.Init(ammoClip.ItemDef, null);
			int missingCharges = ammoClip.ChargesMax - ammoClip.CommonItemData.CurrentCharges;
			ammoClone.CommonItemData.ModifyCharges(-missingCharges);

			equipment.CommonItemData.Ammo.LoadMagazine(ammoClone);

			ModMain.Main.Logger.LogInfo("Weapon reloaded without consuming ammo.");

			return false;
		}
	}

	[HarmonyPatch(typeof(TacticalAbility), "GetTargetInventoryCompatibleItemEquipmentPairs")]
	public class GetTargetInventoryCompatibleItemEquipmentPairs
	{
		static bool Prefix(TacticalAbility __instance, ref IEnumerable<TacticalAbilityTarget> __result, TacticalActorBase sourceActor) {

			if (!(__instance is ReloadAbility)) return true;

			if (!Tactical.Mod.NoAmmo) return true;

			List<TacticalAbilityTarget> result = new List<TacticalAbilityTarget>();
			__result = result;

			var tacActor = sourceActor as TacticalActor;

			if (tacActor == null || tacActor.Equipments == null) {
				return false;
			}

			foreach (var equipment in tacActor.Equipments.Equipments) {
				var ammoDef = equipment.TacticalItemDef.CompatibleAmmunition.FirstOrDefault();
				var ammo = Tactical.Mod.GetAmmoItem(ammoDef);
				if (ammo == null) continue;

				result.Add(new TacticalAbilityTarget(tacActor)
				{
					TacticalItem = ammo,
					Equipment = equipment
				});
			}

			return false;
		}
	}
}
