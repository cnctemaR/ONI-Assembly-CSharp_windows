using System;
using UnityEngine;

namespace Database
{
	public class RobotStatusItems : StatusItems
	{
		public RobotStatusItems(ResourceSet parent)
			: base("RobotStatusItems", parent)
		{
			this.CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			this.CantReachStation = new StatusItem("CantReachStation", "ROBOTS", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			this.CantReachStation.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject = (GameObject)data;
				return str.Replace("{0}", gameObject.name);
			};
			this.LowBattery = new StatusItem("LowBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			this.LowBattery.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject2 = (GameObject)data;
				return str.Replace("{0}", gameObject2.name);
			};
			this.LowBatteryNoCharge = new StatusItem("LowBatteryNoCharge", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			this.LowBatteryNoCharge.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject3 = (GameObject)data;
				return str.Replace("{0}", gameObject3.name);
			};
			this.DeadBattery = new StatusItem("DeadBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			this.DeadBattery.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject4 = (GameObject)data;
				return str.Replace("{0}", gameObject4.name);
			};
			this.DustBinFull = new StatusItem("DustBinFull", "ROBOTS", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.DustBinFull.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject5 = (GameObject)data;
				return str.Replace("{0}", gameObject5.name);
			};
			this.Working = new StatusItem("Working", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.Working.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject6 = (GameObject)data;
				return str.Replace("{0}", gameObject6.name);
			};
			this.MovingToChargeStation = new StatusItem("MovingToChargeStation", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.MovingToChargeStation.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject7 = (GameObject)data;
				return str.Replace("{0}", gameObject7.name);
			};
			this.UnloadingStorage = new StatusItem("UnloadingStorage", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.UnloadingStorage.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject8 = (GameObject)data;
				return str.Replace("{0}", gameObject8.name);
			};
			this.ReactPositive = new StatusItem("ReactPositive", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.ReactPositive.resolveStringCallback = (string str, object data) => str;
			this.ReactNegative = new StatusItem("ReactNegative", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.ReactNegative.resolveStringCallback = (string str, object data) => str;
		}

		public StatusItem LowBattery;

		public StatusItem LowBatteryNoCharge;

		public StatusItem DeadBattery;

		public StatusItem CantReachStation;

		public StatusItem DustBinFull;

		public StatusItem Working;

		public StatusItem UnloadingStorage;

		public StatusItem ReactPositive;

		public StatusItem ReactNegative;

		public StatusItem MovingToChargeStation;
	}
}
