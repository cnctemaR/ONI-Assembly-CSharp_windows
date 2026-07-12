using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class RadiationMonitor : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.root.Update(new Action<RadiationMonitor.Instance, float>(RadiationMonitor.CheckRadiationLevel), UpdateRate.SIM_1000ms, false);
		this.idle.DoNothing().ParamTransition<float>(this.radiationExposure, this.sick.deadly, RadiationMonitor.COMPARE_GTE_DEADLY).ParamTransition<float>(this.radiationExposure, this.sick.extreme, RadiationMonitor.COMPARE_GTE_EXTREME)
			.ParamTransition<float>(this.radiationExposure, this.sick.major, RadiationMonitor.COMPARE_GTE_MAJOR)
			.ParamTransition<float>(this.radiationExposure, this.sick.minor, RadiationMonitor.COMPARE_GTE_MINOR);
		this.sick.ParamTransition<float>(this.radiationExposure, this.idle, RadiationMonitor.COMPARE_LT_MINOR).Enter(delegate(RadiationMonitor.Instance smi)
		{
			smi.sm.isSick.Set(true, smi);
		}).Exit(delegate(RadiationMonitor.Instance smi)
		{
			smi.sm.isSick.Set(false, smi);
		});
		this.sick.minor.ToggleEffect("RadiationExposureMinor").ParamTransition<float>(this.radiationExposure, this.sick.deadly, RadiationMonitor.COMPARE_GTE_DEADLY).ParamTransition<float>(this.radiationExposure, this.sick.extreme, RadiationMonitor.COMPARE_GTE_EXTREME)
			.ParamTransition<float>(this.radiationExposure, this.sick.major, RadiationMonitor.COMPARE_GTE_MAJOR)
			.ToggleAnims("anim_loco_radiation1_kanim", 4f, "")
			.ToggleAnims("anim_idle_radiation1_kanim", 4f, "")
			.ToggleExpression(Db.Get().Expressions.Radiation1, null)
			.DefaultState(this.sick.minor.waiting);
		this.sick.minor.reacting.ToggleChore(new Func<RadiationMonitor.Instance, Chore>(this.CreateVomitChore), this.sick.minor.waiting);
		this.sick.major.ToggleEffect("RadiationExposureMajor").ParamTransition<float>(this.radiationExposure, this.sick.deadly, RadiationMonitor.COMPARE_GTE_DEADLY).ParamTransition<float>(this.radiationExposure, this.sick.extreme, RadiationMonitor.COMPARE_GTE_EXTREME)
			.ToggleAnims("anim_loco_radiation2_kanim", 4f, "")
			.ToggleAnims("anim_idle_radiation2_kanim", 4f, "")
			.ToggleExpression(Db.Get().Expressions.Radiation2, null)
			.DefaultState(this.sick.major.waiting);
		this.sick.major.waiting.ScheduleGoTo(120f, this.sick.major.vomiting);
		this.sick.major.vomiting.ToggleChore(new Func<RadiationMonitor.Instance, Chore>(this.CreateVomitChore), this.sick.major.waiting);
		this.sick.extreme.ParamTransition<float>(this.radiationExposure, this.sick.deadly, RadiationMonitor.COMPARE_GTE_DEADLY).ToggleEffect("RadiationExposureExtreme").ToggleAnims("anim_loco_radiation3_kanim", 4f, "")
			.ToggleAnims("anim_idle_radiation3_kanim", 4f, "")
			.ToggleExpression(Db.Get().Expressions.Radiation3, null)
			.DefaultState(this.sick.extreme.waiting);
		this.sick.extreme.waiting.ScheduleGoTo(60f, this.sick.extreme.vomiting);
		this.sick.extreme.vomiting.ToggleChore(new Func<RadiationMonitor.Instance, Chore>(this.CreateVomitChore), this.sick.extreme.waiting);
		this.sick.deadly.ToggleAnims("anim_loco_radiation4_kanim", 4f, "").ToggleAnims("anim_idle_radiation4_kanim", 4f, "").ToggleExpression(Db.Get().Expressions.Radiation4, null)
			.Enter(delegate(RadiationMonitor.Instance smi)
			{
				smi.GetComponent<Health>().Incapacitate(GameTags.RadiationSicknessIncapacitation);
			});
	}

	private Chore CreateVomitChore(RadiationMonitor.Instance smi)
	{
		Notification notification = new Notification(DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null, true);
		return new VomitChore(Db.Get().ChoreTypes.Vomit, smi.master, Db.Get().DuplicantStatusItems.Vomiting, notification, null);
	}

	private static void RadiationRecovery(RadiationMonitor.Instance smi, float dt)
	{
		smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).ApplyDelta(Db.Get().Attributes.RadiationRecovery.Lookup(smi.gameObject).GetTotalValue() * dt);
	}

	private static void CheckRadiationLevel(RadiationMonitor.Instance smi, float dt)
	{
		RadiationMonitor.RadiationRecovery(smi, dt);
		smi.sm.timeUntilNextExposureReact.Delta(-dt, smi);
		smi.sm.timeUntilNextSickReact.Delta(-dt, smi);
		int num = Grid.PosToCell(smi.gameObject);
		if (Grid.IsValidCell(num))
		{
			float num2 = 1f - Db.Get().Attributes.RadiationResistance.Lookup(smi.gameObject).GetTotalValue();
			float num3 = Grid.Radiation[num] * 1f * num2 / 600f * dt;
			smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).ApplyDelta(num3);
			smi.sm.currentExposurePerCycle.Set(num3 / dt * 600f, smi);
			if (smi.sm.timeUntilNextExposureReact.Get(smi) <= 0f && num3 > 0.16666667f / dt)
			{
				smi.sm.timeUntilNextExposureReact.Set(120f, smi);
				ReactionMonitor.Instance smi2 = smi.master.gameObject.GetSMI<ReactionMonitor.Instance>();
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(smi.master.gameObject, "RadiationReact", Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_radiation_kanim", 0f, 20f, float.PositiveInfinity);
				selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
				{
					anim = "react_radiation_glare"
				});
				smi2.AddOneshotReactable(selfEmoteReactable);
			}
		}
		if (smi.sm.timeUntilNextSickReact.Get(smi) <= 0f && smi.sm.isSick.Get(smi))
		{
			smi.sm.timeUntilNextSickReact.Set(60f, smi);
			ReactionMonitor.Instance smi3 = smi.master.gameObject.GetSMI<ReactionMonitor.Instance>();
			SelfEmoteReactable selfEmoteReactable2 = new SelfEmoteReactable(smi.master.gameObject, "RadiationReact", Db.Get().ChoreTypes.RadiationPain, "anim_react_radiation_kanim", 0f, 20f, float.PositiveInfinity);
			selfEmoteReactable2.AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react_radiation_itch"
			});
			smi3.AddOneshotReactable(selfEmoteReactable2);
		}
		smi.sm.radiationExposure.Set(smi.master.gameObject.GetComponent<KSelectable>().GetAmounts().GetValue("RadiationBalance"), smi);
	}

	public const float BASE_ABSORBTION_RATE = 1f;

	public const float REACT_THRESHOLD = 0.16666667f;

	public const float MIN_TIME_BETWEEN_EXPOSURE_REACTS = 120f;

	public const float MIN_TIME_BETWEEN_SICK_REACTS = 60f;

	public const int VOMITS_PER_CYCLE_MAJOR = 5;

	public const int VOMITS_PER_CYCLE_EXTREME = 10;

	public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.FloatParameter radiationExposure;

	public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.FloatParameter currentExposurePerCycle;

	public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.BoolParameter isSick;

	public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.FloatParameter timeUntilNextExposureReact;

	public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.FloatParameter timeUntilNextSickReact;

	public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State idle;

	public RadiationMonitor.SickStates sick;

	protected static StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_LT_MINOR = (RadiationMonitor.Instance smi, float p) => p < 100f;

	protected static StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_GTE_MINOR = (RadiationMonitor.Instance smi, float p) => p >= 100f;

	protected static StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_GTE_MAJOR = (RadiationMonitor.Instance smi, float p) => p >= 300f;

	protected static StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_GTE_EXTREME = (RadiationMonitor.Instance smi, float p) => p >= 600f;

	protected static StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_GTE_DEADLY = (RadiationMonitor.Instance smi, float p) => p >= 900f;

	public class SickStates : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
	{
		public RadiationMonitor.SickStates.MinorStates minor;

		public RadiationMonitor.SickStates.MajorStates major;

		public RadiationMonitor.SickStates.ExtremeStates extreme;

		public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State deadly;

		public class MinorStates : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
		{
			public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State waiting;

			public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State reacting;
		}

		public class MajorStates : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
		{
			public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State waiting;

			public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State vomiting;
		}

		public class ExtremeStates : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
		{
			public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State waiting;

			public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State vomiting;
		}
	}

	public new class Instance : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.effects = base.GetComponent<Effects>();
		}

		public Reactable GetReactable()
		{
			EmoteReactable emoteReactable = new SelfEmoteReactable(base.master.gameObject, "RadiationSicknessReact", Db.Get().ChoreTypes.RadiationPain, "anim_react_radiation_kanim", 0f, 0f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react_radiation_itch"
			});
			emoteReactable.preventChoreInterruption = false;
			return emoteReactable;
		}

		public Effects effects;
	}
}
