using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities.Equipments;
using System.Collections.Generic;

namespace CustomCampaign
{
	public class Tactical : ModTactical
	{
		public class InstanceData
		{
			public bool NoAmmo = false;
		}

		public static Tactical Mod => ModMain.Main.TacticalMod;
		public new ModMain Main => (ModMain)base.Main;

		public bool NoAmmo => _data?.NoAmmo ?? false;

		private InstanceData _data = null;

		private Dictionary<TacticalItemDef, TacticalItem> _runtimeGeneratedAmmo = new Dictionary<TacticalItemDef, TacticalItem>();

		public override void OnTacticalStart() {

			if (_data == null) {
				if (Controller.IsFromSaveGame) {
					// Loaded old save without mod, don't enable anything
					_data = new InstanceData();
				} else {
					// New tactical with mod enabled, use mod settings
					// What about geoscape settings???
					_data = new InstanceData()
					{
						NoAmmo = Main.Config.NoAmmo,
					};
				}
			}

			if (_data.NoAmmo)
				Main.Logger.LogInfo("No ammo is enabled!");
		}

		public override void OnTacticalEnd() {
			foreach (TacticalItem item in _runtimeGeneratedAmmo.Values)
				item.Destroy();

			_runtimeGeneratedAmmo.Clear();
		}


		public override object RecordTacticalInstanceData() {
			return _data;
		}

		public override void ProcessTacticalInstanceData(object data) {
			_data = (InstanceData)data;
		}

		public TacticalItem GetAmmoItem(TacticalItemDef ammoDef) {
			if (ammoDef == null) return null;

			if (!_runtimeGeneratedAmmo.TryGetValue(ammoDef, out TacticalItem result)) {
				result = new TacticalItem();
				result.Init(ammoDef, null);
				_runtimeGeneratedAmmo.Add(ammoDef, result);
			}
			return result;
		}

	}
}
