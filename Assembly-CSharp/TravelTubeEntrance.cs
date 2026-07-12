using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TravelTubeEntrance : StateMachineComponent<TravelTubeEntrance.SMInstance>, ISaveLoadable, ISim200ms
{
	public float AvailableJoules
	{
		get
		{
			return this.availableJoules;
		}
	}

	public float TotalCapacity
	{
		get
		{
			return this.jouleCapacity;
		}
	}

	public float UsageJoules
	{
		get
		{
			return this.joulesPerLaunch;
		}
	}

	public bool HasLaunchPower
	{
		get
		{
			return this.availableJoules > this.joulesPerLaunch;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.energyConsumer.OnConnectionChanged += this.OnConnectionChanged;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = (int)base.transform.GetPosition().x;
		int num2 = (int)base.transform.GetPosition().y + 2;
		Extents extents = new Extents(num, num2, 1, 1);
		UtilityConnections connections = Game.Instance.travelTubeSystem.GetConnections(Grid.XYToCell(num, num2), true);
		this.TubeConnectionsChanged(connections);
		this.tubeChangedEntry = GameScenePartitioner.Instance.Add("TravelTubeEntrance.TubeListener", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[35], new Action<object>(this.TubeChanged));
		base.Subscribe<TravelTubeEntrance>(-592767678, TravelTubeEntrance.OnOperationalChangedDelegate);
		this.meter = new MeterController(this, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
		this.CreateNewWaitReactable();
		Grid.RegisterTubeEntrance(Grid.PosToCell(this), Mathf.FloorToInt(this.availableJoules / this.joulesPerLaunch));
		base.smi.StartSM();
		this.UpdateCharge();
	}

	protected override void OnCleanUp()
	{
		if (this.travelTube != null)
		{
			this.travelTube.Unsubscribe(-1041684577, new Action<object>(this.TubeConnectionsChanged));
			this.travelTube = null;
		}
		Grid.UnregisterTubeEntrance(Grid.PosToCell(this));
		this.ClearWaitReactable();
		GameScenePartitioner.Instance.Free(ref this.tubeChangedEntry);
		base.OnCleanUp();
	}

	private void TubeChanged(object data)
	{
		if (this.travelTube != null)
		{
			this.travelTube.Unsubscribe(-1041684577, new Action<object>(this.TubeConnectionsChanged));
			this.travelTube = null;
		}
		GameObject gameObject = data as GameObject;
		if (data == null)
		{
			this.TubeConnectionsChanged(0);
			return;
		}
		TravelTube component = gameObject.GetComponent<TravelTube>();
		if (component != null)
		{
			component.Subscribe(-1041684577, new Action<object>(this.TubeConnectionsChanged));
			this.travelTube = component;
			return;
		}
		this.TubeConnectionsChanged(0);
	}

	private void TubeConnectionsChanged(object data)
	{
		bool flag = (UtilityConnections)data == UtilityConnections.Up;
		this.operational.SetFlag(TravelTubeEntrance.tubeConnected, flag);
	}

	private bool CanAcceptMorePower()
	{
		return this.operational.IsOperational && (this.button == null || this.button.IsEnabled) && this.energyConsumer.IsExternallyPowered && this.availableJoules < this.jouleCapacity;
	}

	public void Sim200ms(float dt)
	{
		if (this.CanAcceptMorePower())
		{
			this.availableJoules = Mathf.Min(this.jouleCapacity, this.availableJoules + this.energyConsumer.WattsUsed * dt);
			this.UpdateCharge();
		}
		this.energyConsumer.SetSustained(this.HasLaunchPower);
		this.UpdateActive();
		this.UpdateConnectionStatus();
	}

	public void Reserve(TubeTraveller.Instance traveller, int prefabInstanceID)
	{
		Grid.ReserveTubeEntrance(Grid.PosToCell(this), prefabInstanceID, true);
	}

	public void Unreserve(TubeTraveller.Instance traveller, int prefabInstanceID)
	{
		Grid.ReserveTubeEntrance(Grid.PosToCell(this), prefabInstanceID, false);
	}

	public bool IsTraversable(Navigator agent)
	{
		return Grid.HasUsableTubeEntrance(Grid.PosToCell(this), agent.gameObject.GetComponent<KPrefabID>().InstanceID);
	}

	public bool HasChargeSlotReserved(Navigator agent)
	{
		return Grid.HasReservedTubeEntrance(Grid.PosToCell(this), agent.gameObject.GetComponent<KPrefabID>().InstanceID);
	}

	public bool HasChargeSlotReserved(TubeTraveller.Instance tube_traveller, int prefabInstanceID)
	{
		return Grid.HasReservedTubeEntrance(Grid.PosToCell(this), prefabInstanceID);
	}

	public bool IsChargedSlotAvailable(TubeTraveller.Instance tube_traveller, int prefabInstanceID)
	{
		return Grid.HasUsableTubeEntrance(Grid.PosToCell(this), prefabInstanceID);
	}

	public bool ShouldWait(GameObject reactor)
	{
		if (!this.operational.IsOperational)
		{
			return false;
		}
		if (!this.HasLaunchPower)
		{
			return false;
		}
		if (this.launch_workable.worker == null)
		{
			return false;
		}
		TubeTraveller.Instance smi = reactor.GetSMI<TubeTraveller.Instance>();
		return this.HasChargeSlotReserved(smi, reactor.GetComponent<KPrefabID>().InstanceID);
	}

	public void ConsumeCharge(GameObject reactor)
	{
		if (this.HasLaunchPower)
		{
			this.availableJoules -= this.joulesPerLaunch;
			this.UpdateCharge();
		}
	}

	private void CreateNewWaitReactable()
	{
		if (this.wait_reactable == null)
		{
			this.wait_reactable = new TravelTubeEntrance.WaitReactable(this);
		}
	}

	private void OrphanWaitReactable()
	{
		this.wait_reactable = null;
	}

	private void ClearWaitReactable()
	{
		if (this.wait_reactable != null)
		{
			this.wait_reactable.Cleanup();
			this.wait_reactable = null;
		}
	}

	private void OnOperationalChanged(object data)
	{
		bool flag = (bool)data;
		Grid.SetTubeEntranceOperational(Grid.PosToCell(this), flag);
		this.UpdateActive();
	}

	private void OnConnectionChanged()
	{
		this.UpdateActive();
		this.UpdateConnectionStatus();
	}

	private void UpdateActive()
	{
		this.operational.SetActive(this.CanAcceptMorePower(), false);
	}

	private void UpdateCharge()
	{
		base.smi.sm.hasLaunchCharges.Set(this.HasLaunchPower, base.smi, false);
		float num = Mathf.Clamp01(this.availableJoules / this.jouleCapacity);
		this.meter.SetPositionPercent(num);
		this.energyConsumer.UpdatePoweredStatus();
		Grid.SetTubeEntranceReservationCapacity(Grid.PosToCell(this), Mathf.FloorToInt(this.availableJoules / this.joulesPerLaunch));
	}

	private void UpdateConnectionStatus()
	{
		bool flag = this.button != null && !this.button.IsEnabled;
		bool isConnected = this.energyConsumer.IsConnected;
		bool hasLaunchPower = this.HasLaunchPower;
		if (flag || !isConnected || hasLaunchPower)
		{
			this.connectedStatus = this.selectable.RemoveStatusItem(this.connectedStatus, false);
			return;
		}
		if (this.connectedStatus == Guid.Empty)
		{
			this.connectedStatus = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotEnoughPower, null);
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private TravelTubeEntrance.Work launch_workable;

	[MyCmpReq]
	private EnergyConsumerSelfSustaining energyConsumer;

	[MyCmpGet]
	private BuildingEnabledButton button;

	[MyCmpReq]
	private KSelectable selectable;

	public float jouleCapacity = 1f;

	public float joulesPerLaunch = 1f;

	[Serialize]
	private float availableJoules;

	private TravelTube travelTube;

	private TravelTubeEntrance.WaitReactable wait_reactable;

	private MeterController meter;

	private const int MAX_CHARGES = 3;

	private const float RECHARGE_TIME = 10f;

	private static readonly Operational.Flag tubeConnected = new Operational.Flag("tubeConnected", Operational.Flag.Type.Functional);

	private HandleVector<int>.Handle tubeChangedEntry;

	private static readonly EventSystem.IntraObjectHandler<TravelTubeEntrance> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<TravelTubeEntrance>(delegate(TravelTubeEntrance component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private Guid connectedStatus;

	private class LaunchReactable : WorkableReactable
	{
		public LaunchReactable(Workable workable, TravelTubeEntrance entrance)
			: base(workable, "LaunchReactable", Db.Get().ChoreTypes.TravelTubeEntrance, WorkableReactable.AllowedDirection.Any)
		{
			this.entrance = entrance;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (base.InternalCanBegin(new_reactor, transition))
			{
				Navigator component = new_reactor.GetComponent<Navigator>();
				return this.entrance.HasChargeSlotReserved(component);
			}
			return false;
		}

		private TravelTubeEntrance entrance;
	}

	private class WaitReactable : Reactable
	{
		public WaitReactable(TravelTubeEntrance entrance)
			: base(entrance.gameObject, "WaitReactable", Db.Get().ChoreTypes.TravelTubeEntrance, 2, 1, false, 0f, 0f, float.PositiveInfinity, 0f, ObjectLayer.NumLayers)
		{
			this.entrance = entrance;
			this.preventChoreInterruption = false;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (this.reactor != null)
			{
				return false;
			}
			if (this.entrance == null)
			{
				base.Cleanup();
				return false;
			}
			return this.entrance.ShouldWait(new_reactor);
		}

		protected override void InternalBegin()
		{
			KBatchedAnimController component = this.reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"), 1f);
			component.Play("idle_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
			this.entrance.OrphanWaitReactable();
			this.entrance.CreateNewWaitReactable();
		}

		public override void Update(float dt)
		{
			if (this.entrance == null)
			{
				base.Cleanup();
				return;
			}
			if (!this.entrance.ShouldWait(this.reactor))
			{
				base.Cleanup();
			}
		}

		protected override void InternalEnd()
		{
			if (this.reactor != null)
			{
				this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"));
			}
		}

		protected override void InternalCleanup()
		{
		}

		private TravelTubeEntrance entrance;
	}

	public class SMInstance : GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.GameInstance
	{
		public SMInstance(TravelTubeEntrance master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.notoperational;
			this.root.ToggleStatusItem(Db.Get().BuildingStatusItems.StoredCharge, null);
			this.notoperational.DefaultState(this.notoperational.normal).PlayAnim("off").TagTransition(GameTags.Operational, this.ready, false);
			this.notoperational.normal.EventTransition(GameHashes.OperationalFlagChanged, this.notoperational.notube, (TravelTubeEntrance.SMInstance smi) => !smi.master.operational.GetFlag(TravelTubeEntrance.tubeConnected));
			this.notoperational.notube.EventTransition(GameHashes.OperationalFlagChanged, this.notoperational.normal, (TravelTubeEntrance.SMInstance smi) => smi.master.operational.GetFlag(TravelTubeEntrance.tubeConnected)).ToggleStatusItem(Db.Get().BuildingStatusItems.NoTubeConnected, null);
			this.notready.PlayAnim("off").ParamTransition<bool>(this.hasLaunchCharges, this.ready, (TravelTubeEntrance.SMInstance smi, bool hasLaunchCharges) => hasLaunchCharges).TagTransition(GameTags.Operational, this.notoperational, true);
			this.ready.DefaultState(this.ready.free).ToggleReactable((TravelTubeEntrance.SMInstance smi) => new TravelTubeEntrance.LaunchReactable(smi.master.GetComponent<TravelTubeEntrance.Work>(), smi.master.GetComponent<TravelTubeEntrance>())).ParamTransition<bool>(this.hasLaunchCharges, this.notready, (TravelTubeEntrance.SMInstance smi, bool hasLaunchCharges) => !hasLaunchCharges)
				.TagTransition(GameTags.Operational, this.notoperational, true);
			this.ready.free.PlayAnim("on").WorkableStartTransition((TravelTubeEntrance.SMInstance smi) => smi.GetComponent<TravelTubeEntrance.Work>(), this.ready.occupied);
			this.ready.occupied.PlayAnim("working_pre").QueueAnim("working_loop", true, null).WorkableStopTransition((TravelTubeEntrance.SMInstance smi) => smi.GetComponent<TravelTubeEntrance.Work>(), this.ready.post);
			this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete(this.ready);
		}

		public StateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.BoolParameter hasLaunchCharges;

		public TravelTubeEntrance.States.NotOperationalStates notoperational;

		public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State notready;

		public TravelTubeEntrance.States.ReadyStates ready;

		public class NotOperationalStates : GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State
		{
			public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State normal;

			public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State notube;
		}

		public class ReadyStates : GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State
		{
			public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State free;

			public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State occupied;

			public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State post;
		}
	}

	[AddComponentMenu("KMonoBehaviour/Workable/Work")]
	public class Work : Workable, IGameObjectEffectDescriptor
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.resetProgressOnStop = true;
			this.showProgressBar = false;
			this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_tube_launcher_kanim") };
			this.workLayer = Grid.SceneLayer.BuildingUse;
		}

		protected override void OnStartWork(Worker worker)
		{
			base.SetWorkTime(1f);
		}
	}
}
