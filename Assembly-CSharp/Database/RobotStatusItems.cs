using System;

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
			this.CantReachStation.resolveStringCallback = (string str, object data) => str;
			this.LowBattery = new StatusItem("LowBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			this.LowBattery.resolveStringCallback = (string str, object data) => str;
			this.DustBinFull = new StatusItem("DustBinFull", "ROBOTS", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.DustBinFull.resolveStringCallback = (string str, object data) => str;
			this.Working = new StatusItem("Working", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.Working.resolveStringCallback = (string str, object data) => str;
			this.MovingToChargeStation = new StatusItem("MovingToChargeStation", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.MovingToChargeStation.resolveStringCallback = (string str, object data) => str;
			this.UnloadingStorage = new StatusItem("UnloadingStorage", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.UnloadingStorage.resolveStringCallback = (string str, object data) => str;
			this.ReactPositive = new StatusItem("ReactPositive", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.ReactPositive.resolveStringCallback = (string str, object data) => str;
			this.ReactNegative = new StatusItem("ReactNegative", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.ReactNegative.resolveStringCallback = (string str, object data) => str;
		}

		public StatusItem LowBattery;

		public StatusItem CantReachStation;

		public StatusItem DustBinFull;

		public StatusItem Working;

		public StatusItem UnloadingStorage;

		public StatusItem ReactPositive;

		public StatusItem ReactNegative;

		public StatusItem MovingToChargeStation;
	}
}
