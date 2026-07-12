using System;
using UnityEngine;

public class BionicBedTimeMonitor : GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.notAllowed;
		this.notAllowed.ScheduleChange(this.bedTime, new StateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Transition.ConditionCallback(BionicBedTimeMonitor.CanGoToBedTime)).EventTransition(GameHashes.BionicOnline, this.bedTime, new StateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Transition.ConditionCallback(BionicBedTimeMonitor.CanGoToBedTime));
		this.bedTime.DefaultState(this.bedTime.runChore);
		this.bedTime.runChore.ToggleChore((BionicBedTimeMonitor.Instance smi) => new BionicBedTimeModeChore(smi.master), this.bedTime.choreEnded, this.bedTime.choreEnded).DefaultState(this.bedTime.runChore.notStarted);
		this.bedTime.runChore.notStarted.EventTransition(GameHashes.BeginChore, this.bedTime.runChore.running, new StateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Transition.ConditionCallback(BionicBedTimeMonitor.ChoreIsRunning)).ScheduleChange(this.notAllowed, GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Not(new StateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Transition.ConditionCallback(BionicBedTimeMonitor.CanGoToBedTime))).EventTransition(GameHashes.BionicOffline, this.notAllowed, null);
		this.bedTime.runChore.running.EventTransition(GameHashes.EndChore, this.bedTime.runChore.notStarted, GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Not(new StateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Transition.ConditionCallback(BionicBedTimeMonitor.ChoreIsRunning))).DefaultState(this.bedTime.runChore.running.traveling);
		this.bedTime.runChore.running.traveling.TagTransition(GameTags.BionicBedTime, this.bedTime.runChore.running.defragmenting, false);
		this.bedTime.runChore.running.defragmenting.TagTransition(GameTags.BionicBedTime, this.bedTime.runChore.running.traveling, true).Enter(new StateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State.Callback(BionicBedTimeMonitor.EnableLight)).Exit(new StateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State.Callback(BionicBedTimeMonitor.DisableLight));
		this.bedTime.choreEnded.ScheduleChange(this.notAllowed, GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Not(new StateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.Transition.ConditionCallback(BionicBedTimeMonitor.CanGoToBedTime))).EventTransition(GameHashes.BionicOffline, this.notAllowed, null).GoTo(this.bedTime.runChore);
	}

	public static bool CanGoToBedTime(BionicBedTimeMonitor.Instance smi)
	{
		return BionicBedTimeMonitor.IsOnline(smi) && BionicBedTimeMonitor.ScheduleIsInBedTime(smi);
	}

	private static void EnableLight(BionicBedTimeMonitor.Instance smi)
	{
		smi.EnableLight();
	}

	private static void DisableLight(BionicBedTimeMonitor.Instance smi)
	{
		smi.DisableLight();
	}

	private static bool IsOnline(BionicBedTimeMonitor.Instance smi)
	{
		return smi.IsOnline;
	}

	private static bool ScheduleIsInBedTime(BionicBedTimeMonitor.Instance smi)
	{
		return smi.IsScheduleInBedTime;
	}

	public static bool ChoreIsRunning(BionicBedTimeMonitor.Instance smi)
	{
		ChoreDriver component = smi.GetComponent<ChoreDriver>();
		Chore chore = ((component == null) ? null : component.GetCurrentChore());
		return chore != null && chore.choreType == Db.Get().ChoreTypes.BionicBedtimeMode;
	}

	private const float LIGHT_RADIUS = 3f;

	private const int LIGHT_LUX = 1800;

	public GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State notAllowed;

	public BionicBedTimeMonitor.BedTimeStates bedTime;

	public class Def : StateMachine.BaseDef
	{
	}

	public class DefragmentingStates : GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State
	{
		public GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State traveling;

		public GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State defragmenting;
	}

	public class ChoreStates : GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State
	{
		public GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State notStarted;

		public BionicBedTimeMonitor.DefragmentingStates running;
	}

	public class BedTimeStates : GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State
	{
		public BionicBedTimeMonitor.ChoreStates runChore;

		public GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.State choreEnded;
	}

	public new class Instance : GameStateMachine<BionicBedTimeMonitor, BionicBedTimeMonitor.Instance, IStateMachineTarget, BionicBedTimeMonitor.Def>.GameInstance
	{
		public bool IsOnline
		{
			get
			{
				return this.batteryMonitor != null && this.batteryMonitor.IsOnline;
			}
		}

		public bool IsBedTimeChoreRunning
		{
			get
			{
				return this.prefabID.HasTag(GameTags.BionicBedTime);
			}
		}

		public bool IsScheduleInBedTime
		{
			get
			{
				return this.schedulable.IsAllowed(Db.Get().ScheduleBlockTypes.Sleep);
			}
		}

		public Instance(IStateMachineTarget master, BionicBedTimeMonitor.Def def)
			: base(master, def)
		{
			this.batteryMonitor = base.gameObject.GetSMI<BionicBatteryMonitor.Instance>();
			this.prefabID = base.GetComponent<KPrefabID>();
			this.schedulable = base.GetComponent<Schedulable>();
		}

		public void EnableLight()
		{
			this.lightSymbolTracker = base.gameObject.AddOrGet<LightSymbolTracker>();
			this.lightSymbolTracker.targetSymbol = "snapTo_mouth";
			this.lightSymbolTracker.enabled = true;
			this.light = base.gameObject.AddOrGet<Light2D>();
			this.light.Lux = 1800;
			this.light.Range = 3f;
			this.light.enabled = true;
			this.light.drawOverlay = true;
			this.light.Color = new Color(0f, 0.3137255f, 1f, 1f);
			this.light.overlayColour = new Color(1f, 1f, 1f, 1f);
			this.light.FullRefresh();
		}

		public void DisableLight()
		{
			if (this.light != null)
			{
				this.light.enabled = false;
			}
			if (this.lightSymbolTracker != null)
			{
				this.lightSymbolTracker.enabled = false;
			}
		}

		private Light2D light;

		private LightSymbolTracker lightSymbolTracker;

		private BionicBatteryMonitor.Instance batteryMonitor;

		private Schedulable schedulable;

		private KPrefabID prefabID;
	}
}
