using HarmonyLib;
using PhoenixPoint.Modding;

namespace CustomCampaign
{
	public class ModMain : PhoenixPoint.Modding.ModMain
	{
		public override bool CanSafelyDisable => true;

		public static ModMain Main { get; private set; }

		public new Configuration Config => (Configuration)base.Config;

		public new Harmony HarmonyInstance => (Harmony)base.HarmonyInstance;

		public new Geoscape GeoscapeMod => (Geoscape)base.GeoscapeMod;
		public new Tactical TacticalMod => (Tactical)base.TacticalMod;

		public override void OnModEnabled() {

			Main = this;
			HarmonyInstance.PatchAll(GetType().Assembly);
		}

		public override void OnModDisabled() {
			HarmonyInstance.UnpatchAll(HarmonyInstance.Id);

			Main = null;
		}
	}
}
