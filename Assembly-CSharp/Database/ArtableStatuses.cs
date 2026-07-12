using System;

namespace Database
{
	public class ArtableStatuses : ResourceSet<StatusItem>
	{
		public ArtableStatuses(ResourceSet parent)
			: base("ArtableStatuses", parent)
		{
			this.Ready = new StatusItem("AwaitingArting", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Ugly = new StatusItem("LookingUgly", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Okay = new StatusItem("LookingOkay", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Great = new StatusItem("LookingGreat", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
		}

		public StatusItem Ready;

		public StatusItem Ugly;

		public StatusItem Okay;

		public StatusItem Great;
	}
}
