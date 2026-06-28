using System;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class PowerTransformer : Generator
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		int powerOutputCell = component.GetPowerOutputCell();
		this.partitionerEntry = GameScenePartitioner.Instance.Add("PowerTransformer", base.gameObject, powerOutputCell, GameScenePartitioner.Instance.wiresLayer, new Action<object>(this.OnConnectionChanged));
		this.battery = base.GetComponent<Battery>();
	}

	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		base.OnCleanUp();
	}

	private void OnConnectionChanged(object data)
	{
		Building component = base.GetComponent<Building>();
		int powerOutputCell = component.GetPowerOutputCell();
		bool flag = Grid.Objects[powerOutputCell, 19] != null;
		KSelectable component2 = base.GetComponent<KSelectable>();
		if (flag && this.wireConnectedStatusItem == Guid.Empty)
		{
			this.wireConnectedStatusItem = component2.AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
		}
		else
		{
			this.wireConnectedStatusItem = component2.RemoveStatusItem(this.wireConnectedStatusItem, false);
		}
	}

	public override void ApplyDeltaJoules(float joules_delta, bool can_over_power = false)
	{
		this.battery.ConsumeEnergy(-joules_delta);
		base.ApplyDeltaJoules(joules_delta, can_over_power);
	}

	public override float JoulesAvailable
	{
		get
		{
			return Math.Min(this.battery.JoulesAvailable, base.WattageRating * 0.25f);
		}
	}

	private GameScenePartitionerEntry partitionerEntry;

	private Guid wireConnectedStatusItem;

	private Battery battery;
}
