using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BionicMassOxygenAbsorbChore : Chore<BionicMassOxygenAbsorbChore.Instance>
{
	public BionicMassOxygenAbsorbChore(IStateMachineTarget target, bool critical)
		: base(critical ? Db.Get().ChoreTypes.BionicAbsorbOxygen_Critical : Db.Get().ChoreTypes.BionicAbsorbOxygen, target, target.GetComponent<ChoreProvider>(), false, null, null, null, critical ? PriorityScreen.PriorityClass.compulsory : PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new BionicMassOxygenAbsorbChore.Instance(this, target.gameObject);
		Func<int> func = new Func<int>(base.smi.UpdateTargetCell);
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		this.AddPrecondition(ChorePreconditions.instance.CanMoveToDynamicCellUntilBegun, func);
	}

	public override string ResolveString(string str)
	{
		float num = ((base.smi == null) ? 0f : base.smi.GetAverageMassConsumedPerSecond());
		return string.Format(base.ResolveString(str), GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("BionicMassAbsorbOxygenChore null context.consumer");
			return;
		}
		if (context.consumerState.consumer.GetSMI<BionicOxygenTankMonitor.Instance>() == null)
		{
			global::Debug.LogError("BionicMassAbsorbOxygenChore null BionicOxygenTankMonitor.Instance");
			return;
		}
		base.smi.ResetMassTrackHistory();
		base.smi.sm.dupe.Set(context.consumerState.consumer, base.smi);
		base.Begin(context);
	}

	public static bool IsNotAllowedByScheduleAndChoreIsNotCritical(BionicMassOxygenAbsorbChore.Instance smi)
	{
		return !BionicMassOxygenAbsorbChore.IsCriticalChore(smi) && !BionicMassOxygenAbsorbChore.IsAllowedBySchedule(smi);
	}

	public static bool IsAllowedBySchedule(BionicMassOxygenAbsorbChore.Instance smi)
	{
		return BionicOxygenTankMonitor.IsAllowedToSeekOxygenBySchedule(smi.oxygenTankMonitor);
	}

	public static bool IsCriticalChore(BionicMassOxygenAbsorbChore.Instance smi)
	{
		return smi.master.choreType == Db.Get().ChoreTypes.BionicAbsorbOxygen_Critical;
	}

	public static void ResetOxygenTimer(BionicMassOxygenAbsorbChore.Instance smi)
	{
		smi.sm.SecondsPassedWithoutOxygen.Set(0f, smi, false);
	}

	public static void RefreshTargetSafeCell(BionicMassOxygenAbsorbChore.Instance smi)
	{
		smi.UpdateTargetCell();
	}

	public static void UpdateTargetSafeCell(BionicMassOxygenAbsorbChore.Instance smi, float dt)
	{
		BionicMassOxygenAbsorbChore.RefreshTargetSafeCell(smi);
	}

	public static bool HasSpaceInOxygenTank(BionicMassOxygenAbsorbChore.Instance smi)
	{
		return smi.oxygenTankMonitor.SpaceAvailableInTank > 0f;
	}

	public static bool ChoreIsCriticalModeAndGiveUpOxygenLevelReached(BionicMassOxygenAbsorbChore.Instance smi)
	{
		return BionicMassOxygenAbsorbChore.IsCriticalChore(smi) && smi.oxygenTankMonitor.OxygenPercentage >= 0.25f;
	}

	public static bool BreathIsFull(BionicMassOxygenAbsorbChore.Instance smi)
	{
		AmountInstance amountInstance = smi.gameObject.GetAmounts().Get(Db.Get().Amounts.Breath);
		return amountInstance.value >= amountInstance.GetMax();
	}

	public static void UpdateTargetSafeCellOnlyInCriticalMode(BionicMassOxygenAbsorbChore.Instance smi, float dt)
	{
		if (BionicMassOxygenAbsorbChore.IsCriticalChore(smi))
		{
			BionicMassOxygenAbsorbChore.RefreshTargetSafeCell(smi);
		}
	}

	public static void AbsorbUpdate(BionicMassOxygenAbsorbChore.Instance smi, float dt)
	{
		float num = Mathf.Min(dt * BionicMassOxygenAbsorbChore.ABSORB_RATE, smi.oxygenTankMonitor.SpaceAvailableInTank);
		BionicMassOxygenAbsorbChore.AbsorbUpdateData absorbUpdateData = new BionicMassOxygenAbsorbChore.AbsorbUpdateData(smi, dt);
		int num2;
		SimHashes nearBreathableElement = BionicMassOxygenAbsorbChore.GetNearBreathableElement(num2 = Grid.PosToCell(smi.sm.dupe.Get(smi)), BionicMassOxygenAbsorbChore.ABSORB_RANGE, out num2);
		HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(new Action<Sim.MassConsumedCallback, object>(BionicMassOxygenAbsorbChore.OnSimConsumeCallback), absorbUpdateData, "BionicMassOxygenAbsorbChore");
		SimMessages.ConsumeMass(num2, nearBreathableElement, num, 6, handle.index);
	}

	private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		BionicMassOxygenAbsorbChore.AbsorbUpdateData absorbUpdateData = (BionicMassOxygenAbsorbChore.AbsorbUpdateData)data;
		absorbUpdateData.smi.OnSimConsume(mass_cb_info, absorbUpdateData.dt);
	}

	private static void ShowOxygenBar(BionicMassOxygenAbsorbChore.Instance smi)
	{
		if (NameDisplayScreen.Instance != null)
		{
			NameDisplayScreen.Instance.SetBionicOxygenTankDisplay(smi.gameObject, new Func<float>(smi.GetOxygen), true);
		}
	}

	private static void HideOxygenBar(BionicMassOxygenAbsorbChore.Instance smi)
	{
		if (NameDisplayScreen.Instance != null)
		{
			NameDisplayScreen.Instance.SetBionicOxygenTankDisplay(smi.gameObject, null, false);
		}
	}

	public static SimHashes GetNearBreathableElement(int centralCell, CellOffset[] range, out int elementCell)
	{
		float num = 0f;
		int num2 = centralCell;
		SimHashes simHashes = SimHashes.Vacuum;
		foreach (CellOffset cellOffset in range)
		{
			int num3 = Grid.OffsetCell(centralCell, cellOffset);
			SimHashes simHashes2 = SimHashes.Vacuum;
			float breathableMassInCell = BionicMassOxygenAbsorbChore.GetBreathableMassInCell(num3, out simHashes2);
			if (breathableMassInCell > Mathf.Epsilon && (simHashes == SimHashes.Vacuum || breathableMassInCell > num))
			{
				simHashes = simHashes2;
				num = breathableMassInCell;
				num2 = num3;
			}
		}
		elementCell = num2;
		return simHashes;
	}

	private static float GetBreathableMassInCell(int cell, out SimHashes elementID)
	{
		if (Grid.IsValidCell(cell))
		{
			Element element = Grid.Element[cell];
			if (element.HasTag(GameTags.Breathable))
			{
				elementID = element.id;
				return Grid.Mass[cell];
			}
		}
		elementID = SimHashes.Vacuum;
		return 0f;
	}

	public static CellOffset[] ABSORB_RANGE = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(0, 1),
		new CellOffset(1, 1),
		new CellOffset(-1, 1),
		new CellOffset(1, 0),
		new CellOffset(-1, 0)
	};

	public const float ABSORB_RATE_IDEAL_CHORE_DURATION = 30f;

	public static readonly float ABSORB_RATE = BionicOxygenTankMonitor.OXYGEN_TANK_CAPACITY_KG / 30f;

	public const int HISTORY_ROW_COUNT = 15;

	public const float LOW_OXYGEN_TRESHOLD = 2f;

	public const float GIVE_UP_DURATION_CRTICIAL_MODE = 2f;

	public const float GIVE_UP_DURATION_LOW_OXYGEN_MODE = 4f;

	public const float CRITICAL_CHORE_GIVE_UP_OXYGEN_LEVEL_TRESHOLD = 0.25f;

	public const string ABSORB_ANIM_FILE = "anim_bionic_absorb_kanim";

	public const string ABSORB_PRE_ANIM_NAME = "absorb_pre";

	public const string ABSORB_LOOP_ANIM_NAME = "absorb_loop";

	public const string ABSORB_PST_ANIM_NAME = "absorb_pst";

	public static CellOffset MouthCellOffset = new CellOffset(0, 1);

	public class States : GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.move;
			base.Target(this.dupe);
			this.root.Exit(delegate(BionicMassOxygenAbsorbChore.Instance smi)
			{
				smi.ChangeCellReservation(Grid.InvalidCell);
			});
			this.move.DefaultState(this.move.onGoing).ScheduleChange(this.fail, new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.Transition.ConditionCallback(BionicMassOxygenAbsorbChore.IsNotAllowedByScheduleAndChoreIsNotCritical));
			this.move.onGoing.Enter(new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State.Callback(BionicMassOxygenAbsorbChore.RefreshTargetSafeCell)).Update(new Action<BionicMassOxygenAbsorbChore.Instance, float>(BionicMassOxygenAbsorbChore.UpdateTargetSafeCellOnlyInCriticalMode), UpdateRate.RENDER_1000ms, false).MoveTo((BionicMassOxygenAbsorbChore.Instance smi) => smi.targetCell, this.absorb, this.move.fail, true);
			this.move.fail.ReturnFailure();
			this.absorb.ScheduleChange(this.fail, new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.Transition.ConditionCallback(BionicMassOxygenAbsorbChore.IsNotAllowedByScheduleAndChoreIsNotCritical)).ToggleTag(GameTags.RecoveringBreath).ToggleAnims("anim_bionic_absorb_kanim", 0f)
				.Enter(new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State.Callback(BionicMassOxygenAbsorbChore.ShowOxygenBar))
				.Exit(new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State.Callback(BionicMassOxygenAbsorbChore.HideOxygenBar))
				.DefaultState(this.absorb.pre);
			this.absorb.pre.PlayAnim("absorb_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.absorb.loop).ScheduleGoTo(3f, this.absorb.loop)
				.Exit(new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State.Callback(BionicMassOxygenAbsorbChore.ResetOxygenTimer));
			this.absorb.loop.Enter(new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State.Callback(BionicMassOxygenAbsorbChore.ResetOxygenTimer)).ParamTransition<float>(this.SecondsPassedWithoutOxygen, this.absorb.pst, (BionicMassOxygenAbsorbChore.Instance smi, float secondsPassed) => secondsPassed > smi.GetGiveupTimerTimeout()).OnSignal(this.TankFilledSignal, this.absorb.pst)
				.PlayAnim("absorb_loop", KAnim.PlayMode.Loop)
				.Update(new Action<BionicMassOxygenAbsorbChore.Instance, float>(BionicMassOxygenAbsorbChore.AbsorbUpdate), UpdateRate.SIM_200ms, false)
				.Transition(this.absorb.pst, new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.Transition.ConditionCallback(BionicMassOxygenAbsorbChore.ChoreIsCriticalModeAndGiveUpOxygenLevelReached), UpdateRate.SIM_200ms);
			this.absorb.pst.Transition(this.absorb.criticalRecoverBreath.pre, new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.Transition.ConditionCallback(BionicMassOxygenAbsorbChore.IsCriticalChore), UpdateRate.SIM_200ms).PlayAnim("absorb_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.complete)
				.ScheduleGoTo(3f, this.complete);
			this.absorb.criticalRecoverBreath.ToggleAnims("anim_emotes_default_kanim", 0f).DefaultState(this.absorb.criticalRecoverBreath.pre);
			this.absorb.criticalRecoverBreath.pre.PlayAnim("breathe_pre").QueueAnim("breathe_loop", false, null).OnAnimQueueComplete(this.absorb.criticalRecoverBreath.loop);
			this.absorb.criticalRecoverBreath.loop.PlayAnim("breathe_loop", KAnim.PlayMode.Loop).ToggleAttributeModifier("Recovering Breath", (BionicMassOxygenAbsorbChore.Instance smi) => smi.recoveringbreath, null).Transition(this.absorb.criticalRecoverBreath.pst, new StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.Transition.ConditionCallback(BionicMassOxygenAbsorbChore.BreathIsFull), UpdateRate.SIM_200ms)
				.Transition(this.absorb.criticalRecoverBreath.pst, (BionicMassOxygenAbsorbChore.Instance smi) => smi.UpdateTargetCell() == Grid.InvalidCell, UpdateRate.SIM_200ms);
			this.absorb.criticalRecoverBreath.pst.PlayAnim("breathe_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.complete).ScheduleGoTo(3f, this.complete);
			this.fail.ReturnFailure();
			this.complete.ReturnSuccess();
		}

		public BionicMassOxygenAbsorbChore.States.MoveStates move;

		public BionicMassOxygenAbsorbChore.States.MassAbsorbStates absorb;

		public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State fail;

		public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State complete;

		public StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.FloatParameter SecondsPassedWithoutOxygen;

		public StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.TargetParameter dupe;

		public StateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.Signal TankFilledSignal;

		public class MoveStates : GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State
		{
			public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State onGoing;

			public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State fail;
		}

		public class MassAbsorbStates : GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State
		{
			public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State pre;

			public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State loop;

			public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State pst;

			public BionicMassOxygenAbsorbChore.States.MassAbsorbStates.CriticalRecover criticalRecoverBreath;

			public class CriticalRecover : GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State
			{
				public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State pre;

				public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State loop;

				public GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.State pst;
			}
		}
	}

	public struct AbsorbUpdateData
	{
		public AbsorbUpdateData(BionicMassOxygenAbsorbChore.Instance smi, float dt)
		{
			this.smi = smi;
			this.dt = dt;
		}

		public BionicMassOxygenAbsorbChore.Instance smi;

		public float dt;
	}

	public class Instance : GameStateMachine<BionicMassOxygenAbsorbChore.States, BionicMassOxygenAbsorbChore.Instance, BionicMassOxygenAbsorbChore, object>.GameInstance
	{
		public float CRITICAL_OXYGEN_MASS_GIVE_UP_TRESHOLD
		{
			get
			{
				return this.oxygenBreather.ConsumptionRate * 8f;
			}
		}

		public float GetGiveupTimerTimeout()
		{
			if (this.oxygenTankMonitor == null)
			{
				return 2f;
			}
			if (!BionicOxygenTankMonitor.AreOxygenLevelsCritical(this.oxygenTankMonitor))
			{
				return 4f;
			}
			return 2f;
		}

		public OxygenBreather oxygenBreather { get; private set; }

		public Instance(BionicMassOxygenAbsorbChore master, GameObject duplicant)
			: base(master)
		{
			base.sm.dupe.Set(duplicant, base.smi, false);
			this.oxygenTankMonitor = duplicant.GetSMI<BionicOxygenTankMonitor.Instance>();
			this.oxygenBreather = duplicant.GetComponent<OxygenBreather>();
			Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float recover_BREATH_DELTA = DUPLICANTSTATS.STANDARD.BaseStats.RECOVER_BREATH_DELTA;
			this.recoveringbreath = new AttributeModifier(deltaAttribute.Id, recover_BREATH_DELTA, DUPLICANTS.MODIFIERS.RECOVERINGBREATH.NAME, false, false, true);
		}

		public void ChangeCellReservation(int newCell)
		{
			if (this.targetCell != Grid.InvalidCell && Grid.Reserved[this.targetCell])
			{
				Grid.Reserved[this.targetCell] = false;
			}
			if (newCell != Grid.InvalidCell && !Grid.Reserved[newCell])
			{
				Grid.Reserved[newCell] = true;
			}
		}

		public override void StopSM(string reason)
		{
			this.ChangeCellReservation(Grid.InvalidCell);
			base.StopSM(reason);
		}

		public int UpdateTargetCell()
		{
			this.oxygenTankMonitor.UpdatePotentialCellToAbsorbOxygen(this.targetCell);
			int absorbOxygenCell = this.oxygenTankMonitor.AbsorbOxygenCell;
			this.ChangeCellReservation(absorbOxygenCell);
			this.targetCell = absorbOxygenCell;
			return absorbOxygenCell;
		}

		public void ResetMassTrackHistory()
		{
			this.massAbsorbedHistory.Clear();
			for (int i = 0; i < 15; i++)
			{
				this.massAbsorbedHistory.Enqueue(0f);
			}
		}

		public void AddMassToHistory(float mass_rate_this_tick)
		{
			if (this.massAbsorbedHistory.Count == 15)
			{
				this.massAbsorbedHistory.Dequeue();
			}
			this.massAbsorbedHistory.Enqueue(mass_rate_this_tick);
		}

		public float GetAverageMassConsumedPerSecond()
		{
			float num = 0f;
			int num2 = 0;
			foreach (float num3 in this.massAbsorbedHistory)
			{
				num += num3;
				num2++;
			}
			if (num2 <= 0)
			{
				return 0f;
			}
			num /= (float)num2;
			return num;
		}

		public void OnSimConsume(Sim.MassConsumedCallback mass_cb_info, float dt)
		{
			if (this.oxygenBreather == null || this.oxygenTankMonitor == null || this.oxygenBreather.prefabID.HasTag(GameTags.Dead))
			{
				return;
			}
			this.AddMassToHistory(mass_cb_info.mass / dt);
			GameObject gameObject = this.oxygenBreather.gameObject;
			bool flag = BionicOxygenTankMonitor.AreOxygenLevelsCritical(this.oxygenTankMonitor);
			float num = (flag ? this.CRITICAL_OXYGEN_MASS_GIVE_UP_TRESHOLD : 2f);
			if (this.GetAverageMassConsumedPerSecond() <= num)
			{
				base.sm.SecondsPassedWithoutOxygen.Set(base.sm.SecondsPassedWithoutOxygen.Get(base.smi) + dt, base.smi, false);
			}
			else
			{
				BionicMassOxygenAbsorbChore.ResetOxygenTimer(base.smi);
			}
			if (flag)
			{
				float num2 = DUPLICANTSTATS.STANDARD.Breath.BREATH_RATE * DUPLICANTSTATS.STANDARD.BaseStats.OXYGEN_USED_PER_SECOND;
				if (mass_cb_info.mass == 0f)
				{
					mass_cb_info.temperature = DUPLICANTSTATS.BIONICS.Temperature.Internal.IDEAL;
				}
				mass_cb_info.mass += DUPLICANTSTATS.STANDARD.BaseStats.RECOVER_BREATH_DELTA * num2 * dt + DUPLICANTSTATS.STANDARD.BaseStats.OXYGEN_USED_PER_SECOND * dt;
			}
			float num3 = this.oxygenTankMonitor.AddGas(mass_cb_info);
			if (num3 > Mathf.Epsilon)
			{
				SimMessages.EmitMass(Grid.PosToCell(gameObject), mass_cb_info.elemIdx, num3, mass_cb_info.temperature, byte.MaxValue, 0, -1);
			}
			if (!BionicMassOxygenAbsorbChore.HasSpaceInOxygenTank(this))
			{
				base.sm.TankFilledSignal.Trigger(this);
			}
		}

		public float GetOxygen()
		{
			if (this.oxygenTankMonitor != null)
			{
				return this.oxygenTankMonitor.OxygenPercentage;
			}
			return 0f;
		}

		public AttributeModifier recoveringbreath;

		public Queue<float> massAbsorbedHistory = new Queue<float>();

		public int targetCell = Grid.InvalidCell;

		public BionicOxygenTankMonitor.Instance oxygenTankMonitor;
	}
}
