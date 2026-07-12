using System;
using Klei.AI;
using UnityEngine;

public class ColdImmunityMonitor : GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.idle.DefaultState(this.idle.feelingFine).TagTransition(GameTags.FeelingCold, this.cold, false).ParamTransition<float>(this.coldCountdown, this.cold, GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.IsGTZero);
		this.idle.feelingFine.DoNothing();
		this.idle.leftWithDesireToWarmupAfterBeingCold.Enter(new StateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State.Callback(ColdImmunityMonitor.UpdateWarmUpCell)).Update(new Action<ColdImmunityMonitor.Instance, float>(ColdImmunityMonitor.UpdateWarmUpCell), UpdateRate.RENDER_1000ms, false).ToggleChore(new Func<ColdImmunityMonitor.Instance, Chore>(ColdImmunityMonitor.CreateRecoverFromChillyBonesChore), this.idle.feelingFine, this.idle.feelingFine);
		this.cold.DefaultState(this.cold.exiting).TagTransition(GameTags.FeelingWarm, this.idle, false).ToggleAnims("anim_idle_cold_kanim", 0f)
			.ToggleAnims("anim_loco_run_cold_kanim", 0f)
			.ToggleAnims("anim_loco_walk_cold_kanim", 0f)
			.ToggleExpression(Db.Get().Expressions.Cold, null)
			.ToggleThought(Db.Get().Thoughts.Cold, null)
			.ToggleEffect("ColdAir")
			.Enter(new StateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State.Callback(ColdImmunityMonitor.UpdateWarmUpCell))
			.Update(new Action<ColdImmunityMonitor.Instance, float>(ColdImmunityMonitor.UpdateWarmUpCell), UpdateRate.RENDER_1000ms, false)
			.ToggleChore(new Func<ColdImmunityMonitor.Instance, Chore>(ColdImmunityMonitor.CreateRecoverFromChillyBonesChore), this.idle, this.cold);
		this.cold.exiting.EventHandlerTransition(GameHashes.EffectAdded, this.idle, new Func<ColdImmunityMonitor.Instance, object, bool>(ColdImmunityMonitor.HasImmunityEffect)).TagTransition(GameTags.FeelingCold, this.cold.idle, false).ToggleStatusItem(Db.Get().DuplicantStatusItems.ExitingCold, null)
			.ParamTransition<float>(this.coldCountdown, this.idle.leftWithDesireToWarmupAfterBeingCold, GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.IsZero)
			.Update(new Action<ColdImmunityMonitor.Instance, float>(ColdImmunityMonitor.ColdTimerUpdate), UpdateRate.SIM_200ms, false)
			.Exit(new StateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State.Callback(ColdImmunityMonitor.ClearTimer));
		this.cold.idle.Enter(new StateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State.Callback(ColdImmunityMonitor.ResetColdTimer)).ToggleStatusItem(Db.Get().DuplicantStatusItems.Cold, (ColdImmunityMonitor.Instance smi) => smi).TagTransition(GameTags.FeelingCold, this.cold.exiting, true);
	}

	public static bool OnEffectAdded(ColdImmunityMonitor.Instance smi, object data)
	{
		return true;
	}

	public static void ClearTimer(ColdImmunityMonitor.Instance smi)
	{
		smi.sm.coldCountdown.Set(0f, smi, false);
	}

	public static void ResetColdTimer(ColdImmunityMonitor.Instance smi)
	{
		smi.sm.coldCountdown.Set(5f, smi, false);
	}

	public static void ColdTimerUpdate(ColdImmunityMonitor.Instance smi, float dt)
	{
		float num = Mathf.Clamp(smi.ColdCountdown - dt, 0f, 5f);
		smi.sm.coldCountdown.Set(num, smi, false);
	}

	private static void UpdateWarmUpCell(ColdImmunityMonitor.Instance smi, float dt)
	{
		smi.UpdateWarmUpCell();
	}

	private static void UpdateWarmUpCell(ColdImmunityMonitor.Instance smi)
	{
		smi.UpdateWarmUpCell();
	}

	public static bool HasImmunityEffect(ColdImmunityMonitor.Instance smi, object data)
	{
		Effects component = smi.GetComponent<Effects>();
		return component != null && component.HasEffect("WarmTouch");
	}

	private static Chore CreateRecoverFromChillyBonesChore(ColdImmunityMonitor.Instance smi)
	{
		return new RecoverFromColdChore(smi.master);
	}

	private const float EFFECT_DURATION = 5f;

	public ColdImmunityMonitor.IdleStates idle;

	public ColdImmunityMonitor.ColdStates cold;

	public StateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.FloatParameter coldCountdown;

	public class ColdStates : GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State idle;

		public GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State exiting;

		public GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State resetChore;
	}

	public class IdleStates : GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State feelingFine;

		public GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.State leftWithDesireToWarmupAfterBeingCold;
	}

	public new class Instance : GameStateMachine<ColdImmunityMonitor, ColdImmunityMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public ColdImmunityProvider.Instance NearestImmunityProvider { get; private set; }

		public int WarmUpCell { get; private set; }

		public float ColdCountdown
		{
			get
			{
				return base.smi.sm.coldCountdown.Get(this);
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

		public void UpdateWarmUpCell()
		{
			int myWorldId = this.navigator.GetMyWorldId();
			int num = Grid.InvalidCell;
			int num2 = int.MaxValue;
			ColdImmunityProvider.Instance instance = null;
			foreach (StateMachine.Instance instance2 in Components.EffectImmunityProviderStations.Items.FindAll((StateMachine.Instance t) => t is ColdImmunityProvider.Instance))
			{
				ColdImmunityProvider.Instance instance3 = instance2 as ColdImmunityProvider.Instance;
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
			this.WarmUpCell = num;
		}

		private Navigator navigator;
	}
}
