using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ObjectDispenser : Switch, IUserControlledCapacity
{
	public virtual float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(this.userMaxCapacity, base.GetComponent<Storage>().capacityKg);
		}
		set
		{
			this.userMaxCapacity = value;
			this.filteredStorage.FilterChanged();
		}
	}

	public float AmountStored
	{
		get
		{
			return base.GetComponent<Storage>().MassStored();
		}
	}

	public float MinCapacity
	{
		get
		{
			return 0f;
		}
	}

	public float MaxCapacity
	{
		get
		{
			return base.GetComponent<Storage>().capacityKg;
		}
	}

	public bool WholeValues
	{
		get
		{
			return false;
		}
	}

	public LocString CapacityUnits
	{
		get
		{
			return GameUtil.GetCurrentMassUnit(false);
		}
	}

	protected override void OnPrefabInit()
	{
		this.Initialize();
	}

	protected void Initialize()
	{
		base.OnPrefabInit();
		this.log = new LoggerFS("ObjectDispenser", 35);
		this.filteredStorage = new FilteredStorage(this, null, null, this, false, Db.Get().ChoreTypes.StorageFetch);
		base.Subscribe<ObjectDispenser>(-905833192, ObjectDispenser.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new ObjectDispenser.Instance(this, base.IsSwitchedOn);
		this.smi.StartSM();
		if (ObjectDispenser.infoStatusItem == null)
		{
			ObjectDispenser.infoStatusItem = new StatusItem("ObjectDispenserAutomationInfo", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ObjectDispenser.infoStatusItem.resolveStringCallback = new Func<string, object, string>(ObjectDispenser.ResolveInfoStatusItemString);
		}
		this.filteredStorage.FilterChanged();
		base.GetComponent<KSelectable>().ToggleStatusItem(ObjectDispenser.infoStatusItem, true, this.smi);
	}

	protected override void OnCleanUp()
	{
		this.filteredStorage.CleanUp();
		base.OnCleanUp();
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		ObjectDispenser component = gameObject.GetComponent<ObjectDispenser>();
		if (component == null)
		{
			return;
		}
		this.UserMaxCapacity = component.UserMaxCapacity;
	}

	public void DropHeldItems()
	{
		while (this.storage.Count > 0)
		{
			GameObject gameObject = this.storage.Drop(this.storage.items[0], true);
			if (this.rotatable != null)
			{
				gameObject.transform.SetPosition(base.transform.GetPosition() + this.rotatable.GetRotatedCellOffset(this.dropOffset).ToVector3());
			}
			else
			{
				gameObject.transform.SetPosition(base.transform.GetPosition() + this.dropOffset.ToVector3());
			}
		}
		this.smi.GetMaster().GetComponent<Storage>().DropAll(false, false, default(Vector3), true);
	}

	protected override void Toggle()
	{
		base.Toggle();
	}

	protected override void OnRefreshUserMenu(object data)
	{
		if (!this.smi.IsAutomated())
		{
			base.OnRefreshUserMenu(data);
		}
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		ObjectDispenser.Instance instance = (ObjectDispenser.Instance)data;
		string text = ((!instance.IsAutomated()) ? BUILDING.STATUSITEMS.OBJECTDISPENSER.MANUAL_CONTROL : BUILDING.STATUSITEMS.OBJECTDISPENSER.AUTOMATION_CONTROL);
		string text2 = ((!instance.IsOpened()) ? BUILDING.STATUSITEMS.OBJECTDISPENSER.CLOSED : BUILDING.STATUSITEMS.OBJECTDISPENSER.OPENED);
		return string.Format(text, text2);
	}

	public static readonly HashedString PORT_ID = "ObjectDispenser";

	private LoggerFS log;

	public CellOffset dropOffset;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Storage storage;

	[MyCmpGet]
	private Rotatable rotatable;

	private ObjectDispenser.Instance smi;

	private static StatusItem infoStatusItem;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	protected FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<ObjectDispenser> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ObjectDispenser>(delegate(ObjectDispenser component, object data)
	{
		component.OnCopySettings(data);
	});

	public class States : GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.opened;
			base.serializable = true;
			this.closed_pre.Enter(delegate(ObjectDispenser.Instance smi)
			{
				smi.SetActive(true);
			}).Exit(delegate(ObjectDispenser.Instance smi)
			{
				smi.SetActive(false);
			}).PlayAnim("off_pre")
				.OnAnimQueueComplete(this.closed);
			this.closed.PlayAnim("off").ParamTransition<bool>(this.should_open, this.opened_pre, GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.IsTrue);
			this.opened_pre.Enter(delegate(ObjectDispenser.Instance smi)
			{
				smi.SetActive(true);
			}).Exit(delegate(ObjectDispenser.Instance smi)
			{
				smi.SetActive(false);
			}).PlayAnim("on_pre")
				.OnAnimQueueComplete(this.opened);
			this.opened.Update(delegate(ObjectDispenser.Instance smi, float dt)
			{
				smi.master.DropHeldItems();
			}, UpdateRate.SIM_200ms, false).PlayAnim("on").ParamTransition<bool>(this.should_open, this.closed_pre, GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.IsFalse);
		}

		public GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State closed_pre;

		public GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State closed;

		public GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State opened_pre;

		public GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State opened;

		public StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.BoolParameter should_open;
	}

	public class Instance : GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.GameInstance
	{
		public Instance(ObjectDispenser master, bool manual_start_state)
			: base(master)
		{
			this.manual_on = manual_start_state;
			this.operational = base.GetComponent<Operational>();
			this.logic = base.GetComponent<LogicPorts>();
			base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
			base.Subscribe(-801688580, new Action<object>(this.OnLogicValueChanged));
			base.smi.sm.should_open.Set(true, base.smi);
		}

		public bool IsAutomated()
		{
			return this.logic.IsPortConnected(ObjectDispenser.PORT_ID);
		}

		public bool IsOpened()
		{
			return (!this.IsAutomated()) ? this.manual_on : this.logic_on;
		}

		public void SetSwitchState(bool on)
		{
			this.manual_on = on;
			this.UpdateShouldOpen();
		}

		public void SetActive(bool active)
		{
			this.operational.SetActive(this.operational.IsOperational && active, false);
		}

		private void OnOperationalChanged(object data)
		{
			this.UpdateShouldOpen();
		}

		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID != ObjectDispenser.PORT_ID)
			{
				return;
			}
			this.logic_on = logicValueChanged.newValue != 0;
			this.UpdateShouldOpen();
		}

		private void UpdateShouldOpen()
		{
			if (!this.operational.IsOperational)
			{
				return;
			}
			if (this.IsAutomated())
			{
				base.smi.sm.should_open.Set(this.logic_on, base.smi);
			}
			else
			{
				base.smi.sm.should_open.Set(this.manual_on, base.smi);
			}
		}

		private Operational operational;

		public LogicPorts logic;

		public bool logic_on = true;

		private bool manual_on;
	}
}
