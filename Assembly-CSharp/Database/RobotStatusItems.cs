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
			this.CantReachStation = new StatusItem("CantReachStation", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			this.CantReachStation.resolveStringCallback = (string str, object data) => str;
			this.LowBattery = new StatusItem("LowBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			this.LowBattery.resolveStringCallback = (string str, object data) => str;
			this.DustBinFull = new StatusItem("DustBinFull", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			this.DustBinFull.resolveStringCallback = (string str, object data) => str;
		}

		public StatusItem LowBattery;

		public StatusItem CantReachStation;

		public StatusItem DustBinFull;
	}
}
