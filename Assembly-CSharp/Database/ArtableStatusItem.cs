using System;

namespace Database
{
	public class ArtableStatusItem : StatusItem
	{
		public ArtableStatusItem(string id, ArtableStatuses.ArtableStatusType statusType)
			: base(id, "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null)
		{
			this.StatusType = statusType;
		}

		public ArtableStatuses.ArtableStatusType StatusType;
	}
}
