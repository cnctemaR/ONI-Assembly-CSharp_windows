using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;

public class Uncoverable : KMonoBehaviour
{
	private bool IsAnyCellShowing()
	{
		int num = Grid.PosToCell(this);
		bool flag = this.occupyArea.TestArea(num, null, new Func<int, object, bool>(Uncoverable.IsCellBlocked));
		return !flag;
	}

	private static bool IsCellBlocked(int cell, object data)
	{
		return Grid.Element[cell].IsSolid && !Grid.Foundation[cell];
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.IsAnyCellShowing())
		{
			this.hasBeenUncovered = true;
		}
		if (!this.hasBeenUncovered)
		{
			base.GetComponent<KSelectable>().IsSelectable = false;
			Extents extents = this.occupyArea.GetExtents();
			this.partitionerEntry = GameScenePartitioner.Instance.Add("Uncoverable.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		}
	}

	private void OnSolidChanged(object data)
	{
		bool flag = this.IsAnyCellShowing();
		if (flag && !this.hasBeenUncovered && this.partitionerEntry.IsValid())
		{
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
			this.hasBeenUncovered = true;
			base.GetComponent<KSelectable>().IsSelectable = true;
			Notification notification = new Notification(MISC.STATUSITEMS.BURIEDITEM.NOTIFICATION, NotificationType.Good, HashedString.Invalid, new Func<List<Notification>, object, string>(Uncoverable.OnNotificationToolTip), this, true, 0f, null, null, null);
			base.gameObject.AddOrGet<Notifier>().Add(notification, string.Empty);
		}
	}

	private static string OnNotificationToolTip(List<Notification> notifications, object data)
	{
		Uncoverable uncoverable = (Uncoverable)data;
		return MISC.STATUSITEMS.BURIEDITEM.NOTIFICATION_TOOLTIP.Replace("{Uncoverable}", uncoverable.GetProperName());
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
	}

	[MyCmpReq]
	private OccupyArea occupyArea;

	[Serialize]
	private bool hasBeenUncovered;

	private HandleVector<int>.Handle partitionerEntry;
}
