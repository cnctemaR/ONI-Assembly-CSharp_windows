using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class RocketControlStation : StateMachineComponent<RocketControlStation.StatesInstance>, IGameObjectEffectDescriptor
{
	public bool RestrictWhenGrounded
	{
		get
		{
			return this.m_restrictWhenGrounded;
		}
		set
		{
			this.m_restrictWhenGrounded = value;
			base.Trigger(1861523068, null);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Components.RocketControlStations.Add(this);
		base.Subscribe<RocketControlStation>(-801688580, RocketControlStation.OnLogicValueChangedDelegate);
		base.Subscribe<RocketControlStation>(1861523068, RocketControlStation.OnRocketRestrictionChanged);
		this.UpdateRestrictionAnimSymbol(null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.RocketControlStations.Remove(this);
	}

	public bool BuildingRestrictionsActive
	{
		get
		{
			if (this.IsLogicInputConnected())
			{
				return this.m_logicUsageRestrictionState;
			}
			base.smi.sm.AquireClustercraft(base.smi, false);
			GameObject gameObject = base.smi.sm.clusterCraft.Get(base.smi);
			return this.RestrictWhenGrounded && gameObject != null && gameObject.gameObject.HasTag(GameTags.RocketOnGround);
		}
	}

	public bool IsLogicInputConnected()
	{
		return this.GetNetwork() != null;
	}

	public void OnLogicValueChanged(object data)
	{
		if (((LogicValueChanged)data).portID == RocketControlStation.PORT_ID)
		{
			LogicCircuitNetwork network = this.GetNetwork();
			int num = ((network != null) ? network.OutputValue : 1);
			bool flag = LogicCircuitNetwork.IsBitActive(0, num);
			this.m_logicUsageRestrictionState = flag;
			base.Trigger(1861523068, null);
		}
	}

	public void OnTagsChanged(object obj)
	{
		if (((Boxed<TagChangedEventData>)obj).value.tag == GameTags.RocketOnGround)
		{
			base.Trigger(1861523068, null);
		}
	}

	private LogicCircuitNetwork GetNetwork()
	{
		int portCell = base.GetComponent<LogicPorts>().GetPortCell(RocketControlStation.PORT_ID);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
	}

	private void UpdateRestrictionAnimSymbol(object o = null)
	{
		base.GetComponent<KAnimControllerBase>().SetSymbolVisiblity("restriction_sign", this.BuildingRestrictionsActive);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.ROCKETRESTRICTION_HEADER, UI.BUILDINGEFFECTS.TOOLTIPS.ROCKETRESTRICTION_HEADER, Descriptor.DescriptorType.Effect, false));
		string text = string.Join(", ", RocketControlStation.CONTROLLED_BUILDINGS.Select<Tag, string>((Tag t) => Strings.Get("STRINGS.BUILDINGS.PREFABS." + t.Name.ToUpper() + ".NAME").String).ToArray<string>());
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.ROCKETRESTRICTION_BUILDINGS.text.Replace("{buildinglist}", text), UI.BUILDINGEFFECTS.TOOLTIPS.ROCKETRESTRICTION_BUILDINGS.text.Replace("{buildinglist}", text), Descriptor.DescriptorType.Effect, false));
		return list;
	}

	public static List<Tag> CONTROLLED_BUILDINGS = new List<Tag>();

	private const int UNNETWORKED_VALUE = 1;

	[Serialize]
	public float TimeRemaining;

	private bool m_logicUsageRestrictionState;

	[Serialize]
	private bool m_restrictWhenGrounded;

	public static readonly HashedString PORT_ID = "LogicUsageRestriction";

	private static readonly EventSystem.IntraObjectHandler<RocketControlStation> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<RocketControlStation>(delegate(RocketControlStation component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketControlStation> OnRocketRestrictionChanged = new EventSystem.IntraObjectHandler<RocketControlStation>(delegate(RocketControlStation component, object data)
	{
		component.UpdateRestrictionAnimSymbol(data);
	});

	public class States : GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.ParamsOnly;
			default_state = this.unoperational;
			this.root.Enter("SetTarget", delegate(RocketControlStation.StatesInstance smi)
			{
				this.AquireClustercraft(smi, true);
			}).Target(this.masterTarget).Exit(delegate(RocketControlStation.StatesInstance smi)
			{
				this.SetRocketSpeedModifiers(smi, 0.5f, 1f);
			});
			this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational, false);
			this.operational.Enter(delegate(RocketControlStation.StatesInstance smi)
			{
				this.SetRocketSpeedModifiers(smi, 1f, smi.pilotSpeedMult);
			}).PlayAnim("on").TagTransition(GameTags.Operational, this.unoperational, true)
				.Transition(this.ready, new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.IsInFlight), UpdateRate.SIM_4000ms)
				.Target(this.clusterCraft)
				.EventTransition(GameHashes.RocketRequestLaunch, this.launch, new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.RocketReadyForLaunch))
				.EventTransition(GameHashes.LaunchConditionChanged, this.launch, new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.RocketReadyForLaunch))
				.Target(this.masterTarget)
				.Exit(delegate(RocketControlStation.StatesInstance smi)
				{
					this.timeRemaining.Set(120f, smi, false);
				});
			this.launch.Enter(delegate(RocketControlStation.StatesInstance smi)
			{
				this.SetRocketSpeedModifiers(smi, 1f, smi.pilotSpeedMult);
			}).ToggleChore(new Func<RocketControlStation.StatesInstance, Chore>(this.CreateLaunchChore), this.operational).Transition(this.launch.fadein, new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.IsInFlight), UpdateRate.SIM_200ms)
				.Target(this.clusterCraft)
				.EventTransition(GameHashes.RocketRequestLaunch, this.operational, GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Not(new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.RocketReadyForLaunch)))
				.EventTransition(GameHashes.LaunchConditionChanged, this.operational, GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Not(new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.RocketReadyForLaunch)))
				.Target(this.masterTarget);
			this.launch.fadein.Enter(delegate(RocketControlStation.StatesInstance smi)
			{
				if (CameraController.Instance.cameraActiveCluster == this.clusterCraft.Get(smi).GetComponent<WorldContainer>().id)
				{
					CameraController.Instance.FadeIn(0f, 1f, null);
				}
			});
			this.running.PlayAnim("on").TagTransition(GameTags.Operational, this.unoperational, true).Transition(this.operational, GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Not(new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.IsInFlight)), UpdateRate.SIM_200ms)
				.ParamTransition<float>(this.timeRemaining, this.ready, (RocketControlStation.StatesInstance smi, float p) => p <= 0f)
				.Enter(delegate(RocketControlStation.StatesInstance smi)
				{
					this.SetRocketSpeedModifiers(smi, 1f, smi.pilotSpeedMult);
				})
				.Update("Decrement time", new Action<RocketControlStation.StatesInstance, float>(this.DecrementTime), UpdateRate.SIM_200ms, false)
				.Exit(delegate(RocketControlStation.StatesInstance smi)
				{
					this.timeRemaining.Set(30f, smi, false);
				});
			this.ready.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<RocketControlStation.StatesInstance, Chore>(this.CreateChore), this.ready.post, this.ready)
				.Transition(this.operational, GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Not(new StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Transition.ConditionCallback(this.IsInFlight)), UpdateRate.SIM_200ms)
				.OnSignal(this.pilotSuccessful, this.ready.post)
				.Update("Decrement time", new Action<RocketControlStation.StatesInstance, float>(this.DecrementTime), UpdateRate.SIM_200ms, false);
			this.ready.idle.PlayAnim("on", KAnim.PlayMode.Loop).WorkableStartTransition((RocketControlStation.StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), this.ready.working).ParamTransition<float>(this.timeRemaining, this.ready.warning, (RocketControlStation.StatesInstance smi, float p) => p <= 15f);
			this.ready.warning.PlayAnim("on_alert", KAnim.PlayMode.Loop).WorkableStartTransition((RocketControlStation.StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), this.ready.working).ToggleMainStatusItem(Db.Get().BuildingStatusItems.PilotNeeded, null)
				.ParamTransition<float>(this.timeRemaining, this.ready.autopilot, (RocketControlStation.StatesInstance smi, float p) => p <= 0f);
			this.ready.autopilot.PlayAnim("on_failed", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().BuildingStatusItems.AutoPilotActive, null).WorkableStartTransition((RocketControlStation.StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), this.ready.working)
				.Enter(delegate(RocketControlStation.StatesInstance smi)
				{
					this.SetRocketSpeedModifiers(smi, 0.5f, smi.pilotSpeedMult);
				});
			this.ready.working.PlayAnim("working_pre").QueueAnim("working_loop", true, null).Enter(delegate(RocketControlStation.StatesInstance smi)
			{
				this.SetRocketSpeedModifiers(smi, 1f, smi.pilotSpeedMult);
			})
				.WorkableStopTransition((RocketControlStation.StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), this.ready.idle);
			this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete(this.running).Exit(delegate(RocketControlStation.StatesInstance smi)
			{
				this.timeRemaining.Set(120f, smi, false);
			});
		}

		public void AquireClustercraft(RocketControlStation.StatesInstance smi, bool force = false)
		{
			if (force || this.clusterCraft.IsNull(smi))
			{
				GameObject rocket = this.GetRocket(smi);
				this.clusterCraft.Set(rocket, smi, false);
				if (rocket != null)
				{
					rocket.Subscribe(-1582839653, new Action<object>(smi.master.OnTagsChanged));
				}
			}
		}

		private void DecrementTime(RocketControlStation.StatesInstance smi, float dt)
		{
			this.timeRemaining.Delta(-dt, smi);
		}

		private bool RocketReadyForLaunch(RocketControlStation.StatesInstance smi)
		{
			Clustercraft component = this.clusterCraft.Get(smi).GetComponent<Clustercraft>();
			return component.LaunchRequested && component.CheckReadyToLaunch();
		}

		private GameObject GetRocket(RocketControlStation.StatesInstance smi)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(smi.GetMyWorldId());
			if (world == null)
			{
				return null;
			}
			return world.gameObject.GetComponent<Clustercraft>().gameObject;
		}

		private void SetRocketSpeedModifiers(RocketControlStation.StatesInstance smi, float autoPilotSpeedMultiplier, float pilotSkillMultiplier = 1f)
		{
			this.clusterCraft.Get(smi).GetComponent<Clustercraft>().AutoPilotMultiplier = autoPilotSpeedMultiplier;
			this.clusterCraft.Get(smi).GetComponent<Clustercraft>().PilotSkillMultiplier = pilotSkillMultiplier;
		}

		private Chore CreateChore(RocketControlStation.StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<RocketControlStationIdleWorkable>();
			WorkChore<RocketControlStationIdleWorkable> workChore = new WorkChore<RocketControlStationIdleWorkable>(Db.Get().ChoreTypes.RocketControl, component, null, true, null, null, null, false, Db.Get().ScheduleBlockTypes.Work, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
			workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRocketControlStation);
			workChore.AddPrecondition(ChorePreconditions.instance.IsRocketTravelling, null);
			return workChore;
		}

		private Chore CreateLaunchChore(RocketControlStation.StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<RocketControlStationLaunchWorkable>();
			WorkChore<RocketControlStationLaunchWorkable> workChore = new WorkChore<RocketControlStationLaunchWorkable>(Db.Get().ChoreTypes.RocketControl, component, null, true, null, null, null, true, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.topPriority, 5, false, true);
			workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRocketControlStation);
			return workChore;
		}

		public void LaunchRocket(RocketControlStation.StatesInstance smi)
		{
			this.clusterCraft.Get(smi).GetComponent<Clustercraft>().Launch(false);
		}

		public bool IsInFlight(RocketControlStation.StatesInstance smi)
		{
			return this.clusterCraft.Get(smi).GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.InFlight;
		}

		public bool IsLaunching(RocketControlStation.StatesInstance smi)
		{
			return this.clusterCraft.Get(smi).GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.Launching;
		}

		public StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.TargetParameter clusterCraft;

		private GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State unoperational;

		private GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State operational;

		private GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State running;

		private RocketControlStation.States.ReadyStates ready;

		private RocketControlStation.States.LaunchStates launch;

		public StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.Signal pilotSuccessful;

		public StateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.FloatParameter timeRemaining;

		public class ReadyStates : GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State
		{
			public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State idle;

			public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State working;

			public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State post;

			public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State warning;

			public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State autopilot;
		}

		public class LaunchStates : GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State
		{
			public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State launch;

			public GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State fadein;
		}
	}

	public class StatesInstance : GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.GameInstance
	{
		public StatesInstance(RocketControlStation smi)
			: base(smi)
		{
		}

		public void LaunchRocket()
		{
			base.sm.LaunchRocket(this);
		}

		public void SetPilotSpeedMult(WorkerBase pilot)
		{
			AttributeConverter pilotingSpeed = Db.Get().AttributeConverters.PilotingSpeed;
			AttributeConverterInstance converter = pilot.GetComponent<AttributeConverters>().GetConverter(pilotingSpeed.Id);
			float num = 1f + converter.Evaluate();
			this.pilotSpeedMult = Mathf.Max(num, 0.1f);
		}

		public float pilotSpeedMult = 1f;
	}
}
