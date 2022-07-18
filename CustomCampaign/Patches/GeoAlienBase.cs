using HarmonyLib;
using PhoenixPoint.Geoscape.Entities;
using System.Reflection;

namespace CustomCampaign.Patches.GeoAlienBasePatches
{
	[HarmonyPatch(typeof(GeoAlienBase), "InitialHavenAttackCounter")]
	class InitialHavenAttackCounter
	{
		static void Postfix(ref int __result) {
			__result = Geoscape.Mod.GetHavenAttackCounter(__result);
		}
	}

	[HarmonyPatch(typeof(GeoAlienBase), "DecreaseHavenCounter")]
	class DecreaseHavenCounter
	{
		static FieldInfo field = null;

		static bool Prefix(GeoAlienBase __instance, int amount) {
			if (Geoscape.Mod.HavenDefeneseFrequency == 100) return true;
			if (field == null)
				field = __instance.GetType().GetField("_havenAttackCounter", BindingFlags.Instance | BindingFlags.NonPublic);

			int value = (int)field.GetValue(__instance);
			value -= amount;
			if (value < 0) value = 0;

			field.SetValue(__instance, value);

			return false;
		}
	}
}
