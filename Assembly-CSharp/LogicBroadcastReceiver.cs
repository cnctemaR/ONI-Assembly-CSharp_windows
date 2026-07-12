using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class LogicBroadcastReceiver : KMonoBehaviour, ISimEveryTick
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.SetChannel(this.channel.Get());
		this.operational.SetFlag(LogicBroadcastReceiver.spaceVisible, this.IsSpaceVisible());
		this.operational.SetFlag(LogicBroadcastReceiver.validChannelInRange, this.CheckChannelValid() && LogicBroadcastReceiver.CheckRange(this.channel.Get().gameObject, base.gameObject));
		this.wasOperational = !this.operational.IsOperational;
		this.OnOperationalChanged(null);
	}

	public void SimEveryTick(float dt)
	{
		bool flag = this.IsSpaceVisible();
		this.operational.SetFlag(LogicBroadcastReceiver.spaceVisible, flag);
		if (!flag)
		{
			if (this.spaceNotVisibleStatusItem == Guid.Empty)
			{
				this.spaceNotVisibleStatusItem = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight, null);
			}
		}
		else if (this.spaceNotVisibleStatusItem != Guid.Empty)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(this.spaceNotVisibleStatusItem, false);
			this.spaceNotVisibleStatusItem = Guid.Empty;
		}
		bool flag2 = this.CheckChannelValid() && LogicBroadcastReceiver.CheckRange(this.channel.Get().gameObject, base.gameObject);
		this.operational.SetFlag(LogicBroadcastReceiver.validChannelInRange, flag2);
		if (flag2 && !this.syncToChannelComplete)
		{
			this.SyncWithBroadcast();
		}
	}

	public bool IsSpaceVisible()
	{
		return base.gameObject.GetMyWorld().IsModuleInterior || Grid.ExposedToSunlight[Grid.PosToCell(base.gameObject)] > 0;
	}

	private bool CheckChannelValid()
	{
		return this.channel.Get() != null && this.channel.Get().GetComponent<LogicPorts>().inputPorts != null;
	}

	public LogicBroadcaster GetChannel()
	{
		return this.channel.Get();
	}

	public void SetChannel(LogicBroadcaster broadcaster)
	{
		this.ClearChannel();
		if (broadcaster == null)
		{
			return;
		}
		this.channel.Set(broadcaster);
		this.syncToChannelComplete = false;
		this.channelEventListeners.Add(this.channel.Get().gameObject.Subscribe(-801688580, new Action<object>(this.OnChannelLogicEvent)));
		this.channelEventListeners.Add(this.channel.Get().gameObject.Subscribe(-592767678, new Action<object>(this.OnChannelLogicEvent)));
		this.SyncWithBroadcast();
	}

	private void ClearChannel()
	{
		if (this.CheckChannelValid())
		{
			for (int i = 0; i < this.channelEventListeners.Count; i++)
			{
				this.channel.Get().gameObject.Unsubscribe(this.channelEventListeners[i]);
			}
		}
		this.channelEventListeners.Clear();
	}

	private void OnChannelLogicEvent(object data)
	{
		if (!this.channel.Get().GetComponent<Operational>().IsOperational)
		{
			return;
		}
		this.SyncWithBroadcast();
	}

	private void SyncWithBroadcast()
	{
		if (!this.CheckChannelValid())
		{
			return;
		}
		bool flag = LogicBroadcastReceiver.CheckRange(this.channel.Get().gameObject, base.gameObject);
		this.UpdateRangeStatus(flag);
		if (!flag)
		{
			return;
		}
		base.GetComponent<LogicPorts>().SendSignal(this.PORT_ID, this.channel.Get().GetCurrentValue());
		this.syncToChannelComplete = true;
	}

	public static bool CheckRange(GameObject broadcaster, GameObject receiver)
	{
		return AxialUtil.GetDistance(broadcaster.GetMyWorldLocation(), receiver.GetMyWorldLocation()) <= LogicBroadcaster.RANGE;
	}

	private void UpdateRangeStatus(bool inRange)
	{
		if (!inRange && this.rangeStatusItem == Guid.Empty)
		{
			KSelectable component = base.GetComponent<KSelectable>();
			this.rangeStatusItem = component.AddStatusItem(Db.Get().BuildingStatusItems.BroadcasterOutOfRange, null);
			return;
		}
		if (this.rangeStatusItem != Guid.Empty)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(this.rangeStatusItem, false);
			this.rangeStatusItem = Guid.Empty;
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (this.operational.IsOperational)
		{
			if (!this.wasOperational)
			{
				this.wasOperational = true;
				this.animController.Queue("on_pre", KAnim.PlayMode.Once, 1f, 0f);
				this.animController.Queue("on", KAnim.PlayMode.Loop, 1f, 0f);
				return;
			}
		}
		else if (this.wasOperational)
		{
			this.wasOperational = false;
			this.animController.Queue("on_pst", KAnim.PlayMode.Once, 1f, 0f);
			this.animController.Queue("off", KAnim.PlayMode.Loop, 1f, 0f);
		}
	}

	protected override void OnCleanUp()
	{
		this.ClearChannel();
		base.OnCleanUp();
	}

	[Serialize]
	private Ref<LogicBroadcaster> channel = new Ref<LogicBroadcaster>();

	public string PORT_ID = "";

	private List<int> channelEventListeners = new List<int>();

	private bool syncToChannelComplete;

	public static readonly Operational.Flag spaceVisible = new Operational.Flag("spaceVisible", Operational.Flag.Type.Requirement);

	public static readonly Operational.Flag validChannelInRange = new Operational.Flag("validChannelInRange", Operational.Flag.Type.Requirement);

	[MyCmpGet]
	private Operational operational;

	private bool wasOperational;

	[MyCmpGet]
	private KBatchedAnimController animController;

	private Guid rangeStatusItem = Guid.Empty;

	private Guid spaceNotVisibleStatusItem = Guid.Empty;
}
