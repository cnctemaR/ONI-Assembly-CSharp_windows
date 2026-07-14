using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class UnderwaterVentDrill : GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.noOperational;
		this.noOperational.TagTransition(GameTags.Operational, this.operational, false).Enter(new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.UpdateDiamondMeter)).PlayAnim("idle");
		this.operational.Enter(new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.UpdateDiamondMeter)).DefaultState(this.operational.idle);
		this.operational.idle.Target(this.Vent).EventTransition(GameHashes.VentBlocked, this.operational.working, new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.Transition.ConditionCallback(UnderwaterVentDrill.CanWork)).Target(this.masterTarget)
			.TagTransition(GameTags.Operational, this.noOperational, true)
			.EventTransition(GameHashes.OnStorageChange, this.operational.missingDiamonds, GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.Not(new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.Transition.ConditionCallback(UnderwaterVentDrill.HasAnyDiamond)))
			.PlayAnim("idle")
			.ToggleStatusItem(Db.Get().BuildingStatusItems.UnderwaterDrillIdle, null);
		this.operational.missingDiamonds.TagTransition(GameTags.Operational, this.noOperational, true).EventTransition(GameHashes.OnStorageChange, this.operational.idle, new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.Transition.ConditionCallback(UnderwaterVentDrill.HasAnyDiamond)).EventHandler(GameHashes.OnStorageChange, new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.UpdateDiamondMeter))
			.PlayAnim("idle");
		this.operational.working.ToggleStatusItem(Db.Get().BuildingStatusItems.UnderwaterDrillActive, null).Toggle("HeatProduction", new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.EnableHeatProduction), new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.DisableHeatProduction)).DefaultState(this.operational.working.pre);
		this.operational.working.pre.Target(this.Vent).PlayAnim("drill_pre").Target(this.masterTarget)
			.PlayAnim("working_pre")
			.OnAnimQueueComplete(this.operational.working.loop);
		this.operational.working.loop.Target(this.Vent).PlayAnim("drill_loop", KAnim.PlayMode.Loop).ToggleStatusItem(Db.Get().MiscStatusItems.UnderwaterVentBeingDrilled, null)
			.Target(this.masterTarget)
			.TagTransition(GameTags.Operational, this.operational.working.pst, true)
			.UpdateTransition(this.operational.working.pst, new Func<UnderwaterVentDrill.Instance, float, bool>(UnderwaterVentDrill.DrillUpdate), UpdateRate.SIM_200ms, false)
			.Toggle("ToggleProgressBar", new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.CreateProgressBar), new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.ClearProgressBar))
			.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		this.operational.working.pst.Target(this.Vent).PlayAnim("drill_pst").Target(this.masterTarget)
			.PlayAnim("working_pst")
			.OnAnimQueueComplete(this.operational.workEnded);
		this.operational.workEnded.ParamTransition<float>(this.DrillProgress, this.operational.completed, GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.IsGTEOne).GoTo(this.operational.missingDiamonds);
		this.operational.completed.Enter(new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.ResetDrillProgress)).Enter(new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State.Callback(UnderwaterVentDrill.UnblockVent)).EnterGoTo(this.operational.idle);
	}

	private static void EnableHeatProduction(UnderwaterVentDrill.Instance smi)
	{
		smi.SetOperationalActiveFlag(true);
	}

	private static void DisableHeatProduction(UnderwaterVentDrill.Instance smi)
	{
		smi.SetOperationalActiveFlag(false);
	}

	private static void ResetDrillProgress(UnderwaterVentDrill.Instance smi)
	{
		smi.sm.DrillProgress.Set(0f, smi, false);
	}

	private static void UnblockVent(UnderwaterVentDrill.Instance smi)
	{
		smi.UnblockVent();
	}

	private static void CreateProgressBar(UnderwaterVentDrill.Instance smi)
	{
		smi.CreateProgressBar();
	}

	private static void ClearProgressBar(UnderwaterVentDrill.Instance smi)
	{
		smi.ClearProgressBar();
	}

	private static bool DrillUpdate(UnderwaterVentDrill.Instance smi, float dt)
	{
		return smi.DrillUpdate(dt);
	}

	private static bool CanWork(UnderwaterVentDrill.Instance smi)
	{
		return smi.CanWork;
	}

	private static bool HasAnyDiamond(UnderwaterVentDrill.Instance smi)
	{
		return smi.HasAnyDiamond;
	}

	private static void UpdateDiamondMeter(UnderwaterVentDrill.Instance smi)
	{
		smi.UpdateDiamondMeter();
	}

	private const string OFF_ANIM_NAME = "idle";

	private const string IDLE_ANIM_NAME = "idle";

	private const string PRE_ANIM_NAME = "working_pre";

	private const string LOOP_ANIM_NAME = "working_loop";

	private const string PST_ANIM_NAME = "working_pst";

	private const string VENT_PRE_ANIM_NAME = "drill_pre";

	private const string VENT_LOOP_ANIM_NAME = "drill_loop";

	private const string VENT_PST_ANIM_NAME = "drill_pst";

	private const string METER_TARGET_NAME = "target_meter";

	private const string METER_ANIM_NAME = "meter";

	public GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State noOperational;

	public UnderwaterVentDrill.OperationalStates operational;

	public StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.FloatParameter DrillProgress = new StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.FloatParameter(0f);

	public StateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.TargetParameter Vent;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			string formattedMass = GameUtil.GetFormattedMass(this.DiamondConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.UNDERWATER_DRILL_DIAMOND_CONSUMPTION.Replace("{Rate}", formattedMass), UI.BUILDINGEFFECTS.TOOLTIPS.UNDERWATER_DRILL_DIAMOND_CONSUMPTION.Replace("{Rate}", formattedMass), Descriptor.DescriptorType.Requirement, false));
			return list;
		}

		public Tag DiamondTag = SimHashes.Diamond.CreateTag();

		public float DiamondConsumptionRate;

		public float WorkDuration;

		public Vector3 ProgressBarOffset = Vector3.zero;
	}

	public class OperationalStates : GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State
	{
		public GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State idle;

		public GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State missingDiamonds;

		public GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.PreLoopPostState working;

		public GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State workEnded;

		public GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.State completed;
	}

	public new class Instance : GameStateMachine<UnderwaterVentDrill, UnderwaterVentDrill.Instance, IStateMachineTarget, UnderwaterVentDrill.Def>.GameInstance
	{
		public float DrillProgress
		{
			get
			{
				return base.sm.DrillProgress.Get(this);
			}
		}

		public bool CanWork
		{
			get
			{
				return this.HasAnyDiamond && this.IsVentBlocked;
			}
		}

		public bool HasAnyDiamond
		{
			get
			{
				return this.storage.GetMassAvailable(base.def.DiamondTag) > 0f;
			}
		}

		public bool IsVentBlocked
		{
			get
			{
				return this.vent != null && this.vent.IsBlocked;
			}
		}

		public bool IsOff
		{
			get
			{
				return base.IsInsideState(base.sm.noOperational);
			}
		}

		public Instance(IStateMachineTarget master, UnderwaterVentDrill.Def def)
			: base(master, def)
		{
			this.storage = base.GetComponent<Storage>();
			this.operational = base.GetComponent<Operational>();
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			this.diamondMeter = new MeterController(component, "target_meter", "meter", Meter.Offset.Infront, Grid.SceneLayer.BuildingBack, Array.Empty<string>());
		}

		public override void StartSM()
		{
			int num = Grid.PosToCell(base.gameObject);
			GameObject gameObject = Grid.Objects[num, 1];
			this.vent = ((gameObject == null) ? null : gameObject.GetSMI<UnderwaterVent.Instance>());
			base.sm.Vent.Set((this.vent == null) ? null : this.vent.gameObject, this, false);
			base.StartSM();
			this.UpdateDiamondMeter();
		}

		public void SetOperationalActiveFlag(bool active)
		{
			this.operational.SetActive(active, false);
		}

		public bool DrillUpdate(float dt)
		{
			if (dt == 0f)
			{
				return false;
			}
			float num = dt * base.def.DiamondConsumptionRate;
			float massAvailable = this.storage.GetMassAvailable(base.def.DiamondTag);
			float num2 = Mathf.Min(num, massAvailable);
			float num3 = num2 / num;
			float num4 = dt / base.def.WorkDuration * num3;
			this.storage.ConsumeIgnoringDisease(base.def.DiamondTag, num2);
			float num5 = this.DrillProgress;
			num5 += num4;
			base.sm.DrillProgress.Set(num5, this, false);
			bool flag = !this.HasAnyDiamond || this.DrillProgress >= 1f;
			this.UpdateDiamondMeter();
			return flag;
		}

		public void UpdateDiamondMeter()
		{
			if (this.diamondMeter != null)
			{
				float num = (this.IsOff ? 0f : (this.storage.MassStored() / this.storage.Capacity()));
				this.diamondMeter.SetPositionPercent(num);
			}
		}

		public void UnblockVent()
		{
			if (this.vent != null)
			{
				this.vent.Unblock();
			}
		}

		public void CreateProgressBar()
		{
			this.progressBar = ProgressBar.CreateProgressBar(base.gameObject, () => this.DrillProgress, base.def.ProgressBarOffset);
			this.progressBar.SetVisibility(true);
		}

		public void ClearProgressBar()
		{
			if (this.progressBar != null)
			{
				Util.KDestroyGameObject(this.progressBar.gameObject);
				this.progressBar = null;
			}
		}

		private Storage storage;

		private UnderwaterVent.Instance vent;

		private Operational operational;

		private ProgressBar progressBar;

		private MeterController diamondMeter;
	}
}
