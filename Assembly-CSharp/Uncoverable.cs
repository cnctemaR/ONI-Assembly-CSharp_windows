using System;
using System.Collections.Generic;
using KSerialization;

public class Uncoverable : KMonoBehaviour
{
	private bool IsAnyCellShowing()
	{
		int num = Grid.PosToCell(this);
		bool flag = this.occupyArea.TestArea(num, null, new Func<int, object, bool>(this.IsCellBlocked));
		return !flag;
	}

	private bool IsCellBlocked(int cell, object data)
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
		if (flag && !this.hasBeenUncovered && this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
			this.hasBeenUncovered = true;
			base.GetComponent<KSelectable>().IsSelectable = true;
			Notification notification = new Notification("Buried Object Discovered!", NotificationType.Good, HashedString.Invalid, new Func<List<Notification>, object, string>(Uncoverable.OnNotificationToolTip), this, true, 0f, null, null, null);
			base.gameObject.AddOrGet<Notifier>().Add(notification, string.Empty);
		}
	}

	private static string OnNotificationToolTip(List<Notification> notifications, object data)
	{
		Uncoverable uncoverable = (Uncoverable)data;
		return "Miners have uncovered a {Uncoverable}!\n\nClick to jump to its location.".Replace("{Uncoverable}", uncoverable.GetProperName());
	}

	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
	}

	[MyCmpReq]
	private OccupyArea occupyArea;

	[Serialize]
	private bool hasBeenUncovered;

	private GameScenePartitionerEntry partitionerEntry;
}
