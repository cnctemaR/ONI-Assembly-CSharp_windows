using System;
using Klei.AI;
using UnityEngine;

public class HeatImmunityMonitor : GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.idle.DefaultState(this.idle.feelingFine).TagTransition(GameTags.FeelingWarm, this.warm, false).ParamTransition<float>(this.heatCountdown, this.warm, GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.IsGTZero);
		this.idle.feelingFine.DoNothing();
		this.idle.leftWithDesireToCooldownAfterBeingWarm.Enter(new StateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State.Callback(HeatImmunityMonitor.UpdateShelterCell)).Update(new Action<HeatImmunityMonitor.Instance, float>(HeatImmunityMonitor.UpdateShelterCell), UpdateRate.RENDER_1000ms, false).ToggleChore(new Func<HeatImmunityMonitor.Instance, Chore>(HeatImmunityMonitor.CreateRecoverFromOverheatChore), this.idle.feelingFine, this.idle.feelingFine);
		this.warm.DefaultState(this.warm.exiting).TagTransition(GameTags.FeelingCold, this.idle, false).ToggleAnims("anim_idle_hot_kanim", 0f)
			.ToggleAnims("anim_loco_run_hot_kanim", 0f)
			.ToggleAnims("anim_loco_walk_hot_kanim", 0f)
			.ToggleExpression(Db.Get().Expressions.Hot, null)
			.ToggleThought(Db.Get().Thoughts.Hot, null)
			.ToggleEffect("WarmAir")
			.Enter(new StateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State.Callback(HeatImmunityMonitor.UpdateShelterCell))
			.Update(new Action<HeatImmunityMonitor.Instance, float>(HeatImmunityMonitor.UpdateShelterCell), UpdateRate.RENDER_1000ms, false)
			.ToggleChore(new Func<HeatImmunityMonitor.Instance, Chore>(HeatImmunityMonitor.CreateRecoverFromOverheatChore), this.idle, this.warm);
		this.warm.exiting.EventHandlerTransition(GameHashes.EffectAdded, this.idle, new Func<HeatImmunityMonitor.Instance, object, bool>(HeatImmunityMonitor.HasImmunityEffect)).TagTransition(GameTags.FeelingWarm, this.warm.idle, false).ToggleStatusItem(Db.Get().DuplicantStatusItems.ExitingHot, null)
			.ParamTransition<float>(this.heatCountdown, this.idle.leftWithDesireToCooldownAfterBeingWarm, GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.IsZero)
			.Update(new Action<HeatImmunityMonitor.Instance, float>(HeatImmunityMonitor.HeatTimerUpdate), UpdateRate.SIM_200ms, false)
			.Exit(new StateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State.Callback(HeatImmunityMonitor.ClearTimer));
		this.warm.idle.Enter(new StateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State.Callback(HeatImmunityMonitor.ResetHeatTimer)).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hot, (HeatImmunityMonitor.Instance smi) => smi).TagTransition(GameTags.FeelingWarm, this.warm.exiting, true);
	}

	public static bool OnEffectAdded(HeatImmunityMonitor.Instance smi, object data)
	{
		return true;
	}

	public static void ClearTimer(HeatImmunityMonitor.Instance smi)
	{
		smi.sm.heatCountdown.Set(0f, smi, false);
	}

	public static void ResetHeatTimer(HeatImmunityMonitor.Instance smi)
	{
		smi.sm.heatCountdown.Set(5f, smi, false);
	}

	public static void HeatTimerUpdate(HeatImmunityMonitor.Instance smi, float dt)
	{
		float num = Mathf.Clamp(smi.HeatCountdown - dt, 0f, 5f);
		smi.sm.heatCountdown.Set(num, smi, false);
	}

	private static void UpdateShelterCell(HeatImmunityMonitor.Instance smi, float dt)
	{
		smi.UpdateShelterCell();
	}

	private static void UpdateShelterCell(HeatImmunityMonitor.Instance smi)
	{
		smi.UpdateShelterCell();
	}

	public static bool HasImmunityEffect(HeatImmunityMonitor.Instance smi, object data)
	{
		Effects component = smi.GetComponent<Effects>();
		return component != null && component.HasEffect("RefreshingTouch");
	}

	private static Chore CreateRecoverFromOverheatChore(HeatImmunityMonitor.Instance smi)
	{
		return new RecoverFromHeatChore(smi.master);
	}

	private const float EFFECT_DURATION = 5f;

	public HeatImmunityMonitor.IdleStates idle;

	public HeatImmunityMonitor.WarmStates warm;

	public StateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.FloatParameter heatCountdown;

	public class WarmStates : GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State idle;

		public GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State exiting;

		public GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State resetChore;
	}

	public class IdleStates : GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State feelingFine;

		public GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.State leftWithDesireToCooldownAfterBeingWarm;
	}

	public new class Instance : GameStateMachine<HeatImmunityMonitor, HeatImmunityMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public HeatImmunityProvider.Instance NearestImmunityProvider { get; private set; }

		public int ShelterCell { get; private set; }

		public float HeatCountdown
		{
			get
			{
				return base.smi.sm.heatCountdown.Get(this);
			}
		}

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public override void StartSM()
		{
			this.navigator = base.gameObject.GetComponent<Navigator>();
			base.StartSM();
		}

		public void UpdateShelterCell()
		{
			int myWorldId = this.navigator.GetMyWorldId();
			int num = Grid.InvalidCell;
			int num2 = int.MaxValue;
			HeatImmunityProvider.Instance instance = null;
			foreach (StateMachine.Instance instance2 in Components.EffectImmunityProviderStations.Items.FindAll((StateMachine.Instance t) => t is HeatImmunityProvider.Instance))
			{
				HeatImmunityProvider.Instance instance3 = instance2 as HeatImmunityProvider.Instance;
				if (instance3.GetMyWorldId() == myWorldId)
				{
					int maxValue = int.MaxValue;
					int bestAvailableCell = instance3.GetBestAvailableCell(this.navigator, out maxValue);
					if (maxValue < num2)
					{
						num2 = maxValue;
						instance = instance3;
						num = bestAvailableCell;
					}
				}
			}
			this.NearestImmunityProvider = instance;
			this.ShelterCell = num;
		}

		private Navigator navigator;
	}
}
