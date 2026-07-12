using System;
using KSerialization;

public class LogicBroadcaster : KMonoBehaviour, ISimEveryTick
{
	public int BroadCastChannelID
	{
		get
		{
			return this.broadcastChannelID;
		}
		private set
		{
			this.broadcastChannelID = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.LogicBroadcasters.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.LogicBroadcasters.Remove(this);
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<LogicBroadcaster>(-801688580, LogicBroadcaster.OnLogicValueChangedDelegate);
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.operational.SetFlag(LogicBroadcaster.spaceVisible, this.IsSpaceVisible());
		this.wasOperational = !this.operational.IsOperational;
		this.OnOperationalChanged(null);
	}

	public bool IsSpaceVisible()
	{
		return base.gameObject.GetMyWorld().IsModuleInterior || Grid.ExposedToSunlight[Grid.PosToCell(base.gameObject)] > 0;
	}

	public int GetCurrentValue()
	{
		return base.GetComponent<LogicPorts>().GetInputValue(this.PORT_ID);
	}

	private void OnLogicValueChanged(object data)
	{
	}

	public void SimEveryTick(float dt)
	{
		bool flag = this.IsSpaceVisible();
		this.operational.SetFlag(LogicBroadcaster.spaceVisible, flag);
		if (!flag)
		{
			if (this.spaceNotVisibleStatusItem == Guid.Empty)
			{
				this.spaceNotVisibleStatusItem = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight, null);
				return;
			}
		}
		else if (this.spaceNotVisibleStatusItem != Guid.Empty)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(this.spaceNotVisibleStatusItem, false);
			this.spaceNotVisibleStatusItem = Guid.Empty;
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

	public static int RANGE = 5;

	private static int INVALID_CHANNEL_ID = -1;

	public string PORT_ID = "";

	private bool wasOperational;

	[Serialize]
	private int broadcastChannelID = LogicBroadcaster.INVALID_CHANNEL_ID;

	private static readonly EventSystem.IntraObjectHandler<LogicBroadcaster> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicBroadcaster>(delegate(LogicBroadcaster component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	public static readonly Operational.Flag spaceVisible = new Operational.Flag("spaceVisible", Operational.Flag.Type.Requirement);

	private Guid spaceNotVisibleStatusItem = Guid.Empty;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private KBatchedAnimController animController;
}
