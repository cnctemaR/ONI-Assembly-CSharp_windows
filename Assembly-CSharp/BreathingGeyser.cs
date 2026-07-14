using System;
using Klei;
using UnityEngine;

public class BreathingGeyser : GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.inactive;
		this.root.Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.RestrictElementConsumer));
		this.inactive.EventTransition(GameHashes.SubmergedStateChanged, this.active, new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.Transition.ConditionCallback(BreathingGeyser.IsSubmerged)).EventTransition(GameHashes.OnStorageChange, this.active.exhale, new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.Transition.ConditionCallback(BreathingGeyser.HasStoredMass)).PlayAnim("inactive")
			.Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.DisableElementConsumer))
			.Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.RestrictElementConsumer));
		this.active.Transition(this.inactive, new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.Transition.ConditionCallback(BreathingGeyser.ShouldGoInactive), UpdateRate.SIM_1000ms).DefaultState(this.active.inhale);
		this.active.inhale.ParamTransition<bool>(this.exhaleFlowDirection, this.active.exhale, GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.IsTrue).DefaultState(this.active.inhale.pre);
		this.active.inhale.pre.PlayAnim("inhale_pre").OnAnimQueueComplete(this.active.inhale.loop);
		this.active.inhale.loop.Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.UnRestrictElementConsumer)).Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.EnableElementConsumer)).PlayAnim("inhale_loop", KAnim.PlayMode.Loop)
			.EventTransition(GameHashes.OnStorageChange, this.active.inhale.pst, new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.Transition.ConditionCallback(BreathingGeyser.IsStorageFull))
			.EventTransition(GameHashes.SubmergedStateChanged, this.active.inhale.pst, new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.Transition.ConditionCallback(BreathingGeyser.IsNotSubmergedWithStorage));
		this.active.inhale.pst.PlayAnim("inhale_pst").OnAnimQueueComplete(this.active.inhale.done);
		this.active.inhale.done.Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.StopInhaling));
		this.active.exhale.ParamTransition<bool>(this.exhaleFlowDirection, this.active.inhale, GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.IsFalse).DefaultState(this.active.exhale.pre).ToggleMainStatusItem(Db.Get().BuildingStatusItems.GeyserExpelling, (BreathingGeyser.Instance smi) => new global::Tuple<Element, float>(smi.GetStoredElementTag(), smi.def.exhaleRate))
			.ToggleTag(GameTags.GeyserExhaling)
			.Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.DisableElementConsumer))
			.Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.RestrictElementConsumer));
		this.active.exhale.pre.PlayAnim("exhale_pre").OnAnimQueueComplete(this.active.exhale.loop);
		this.active.exhale.loop.EventTransition(GameHashes.OnStorageChange, this.active.exhale.pst, new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.Transition.ConditionCallback(BreathingGeyser.IsStorageEmpty)).PlayAnim("exhale_loop", KAnim.PlayMode.Loop).Update(new Action<BreathingGeyser.Instance, float>(BreathingGeyser.ExhaleUpdate), UpdateRate.SIM_1000ms, false);
		this.active.exhale.pst.PlayAnim("exhale_pst").OnAnimQueueComplete(this.active.exhale.done);
		this.active.exhale.done.Enter(new StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State.Callback(BreathingGeyser.StopExhaling));
	}

	public static void StopInhaling(BreathingGeyser.Instance smi)
	{
		smi.sm.exhaleFlowDirection.Set(true, smi, false);
	}

	public static void StopExhaling(BreathingGeyser.Instance smi)
	{
		smi.sm.exhaleFlowDirection.Set(false, smi, false);
	}

	public static void RestrictElementConsumer(BreathingGeyser.Instance smi)
	{
		smi.RestrictElementConsumerState();
	}

	public static void UnRestrictElementConsumer(BreathingGeyser.Instance smi)
	{
		smi.UnrestrictElementConsumerState();
	}

	public static void EnableElementConsumer(BreathingGeyser.Instance smi)
	{
		smi.SetElementConsumerState(true);
	}

	public static void DisableElementConsumer(BreathingGeyser.Instance smi)
	{
		smi.SetElementConsumerState(false);
	}

	public static bool IsSubmerged(BreathingGeyser.Instance smi)
	{
		return smi.IsSubmerged;
	}

	public static bool IsStorageFull(BreathingGeyser.Instance smi)
	{
		return smi.IsStorageFull;
	}

	public static bool IsStorageEmpty(BreathingGeyser.Instance smi)
	{
		return smi.IsStorageEmpty;
	}

	public static bool HasStoredMass(BreathingGeyser.Instance smi)
	{
		return !smi.IsStorageEmpty;
	}

	public static bool ShouldGoInactive(BreathingGeyser.Instance smi)
	{
		return !smi.IsSubmerged && smi.IsStorageEmpty;
	}

	public static bool IsNotSubmergedWithStorage(BreathingGeyser.Instance smi)
	{
		return !smi.IsSubmerged && !smi.IsStorageEmpty;
	}

	public static void ExhaleUpdate(BreathingGeyser.Instance smi, float dt)
	{
		smi.ExhaleUpdate(dt);
	}

	public StateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.BoolParameter exhaleFlowDirection;

	public GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State inactive;

	public BreathingGeyser.ActiveStates active;

	public class Def : StateMachine.BaseDef
	{
		public float inhaleRate;

		public float exhaleRate;

		public byte diseaseIdx;

		public float germsPerKg;
	}

	public class ActiveStates : GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State
	{
		public BreathingGeyser.ActiveStates.AnimStates inhale;

		public BreathingGeyser.ActiveStates.AnimStates exhale;

		public class AnimStates : GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State
		{
			public GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State pre;

			public GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State loop;

			public GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State pst;

			public GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.State done;
		}
	}

	public new class Instance : GameStateMachine<BreathingGeyser, BreathingGeyser.Instance, IStateMachineTarget, BreathingGeyser.Def>.GameInstance
	{
		public bool IsSubmerged
		{
			get
			{
				return this.submergable.IsSubmerged;
			}
		}

		public Instance(IStateMachineTarget master, BreathingGeyser.Def def)
			: base(master, def)
		{
			this.elementConsumer = base.GetComponent<ElementConsumer>();
			this.storage = base.GetComponent<Storage>();
			this.submergable = base.GetComponent<Submergable>();
			this.exhaleCell = Grid.PosToCell(base.transform.GetPosition() + this.elementConsumer.sampleCellOffset);
			this.prefabID = base.GetComponent<KPrefabID>();
		}

		public bool IsExhaling
		{
			get
			{
				return this.prefabID.HasTag(GameTags.GeyserExhaling);
			}
		}

		public void ExhaleUpdate(float dt)
		{
			if (dt == 0f)
			{
				return;
			}
			PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Liquid, 0f);
			if (primaryElement == null || primaryElement.Mass == 0f)
			{
				return;
			}
			float num = base.smi.def.exhaleRate * dt;
			float num2 = Mathf.Min(primaryElement.Mass, num);
			Pickupable pickupable = primaryElement.GetComponent<Pickupable>().Take(num2);
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(pickupable.PrimaryElement.DiseaseIdx, pickupable.PrimaryElement.DiseaseCount, base.smi.def.diseaseIdx, (int)(base.smi.def.germsPerKg * num2));
			SimMessages.AddRemoveSubstance(this.exhaleCell, primaryElement.ElementID, CellEventLogger.Instance.BreathingGeyser, num2, primaryElement.Temperature, diseaseInfo.idx, diseaseInfo.count, true, -1);
			Util.KDestroyGameObject(pickupable);
		}

		public Element GetStoredElementTag()
		{
			PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Liquid, 0f);
			if (primaryElement == null)
			{
				return null;
			}
			return primaryElement.Element;
		}

		public bool IsStorageFull
		{
			get
			{
				return this.storage.RemainingCapacity() <= 0f;
			}
		}

		public bool IsStorageEmpty
		{
			get
			{
				return this.storage.ExactMassStored() == 0f;
			}
		}

		public float GetStoredMass()
		{
			return this.storage.MassStored();
		}

		public string GetExpellingElementName()
		{
			PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Liquid, 0f);
			if (primaryElement != null && primaryElement.Mass > 0f)
			{
				return primaryElement.Element.name;
			}
			return Strings.Get("STRINGS.ELEMENTS.STATE.LIQUID");
		}

		public void RestrictElementConsumerState()
		{
			this.elementConsumer.consumptionRate = 0f;
		}

		public void UnrestrictElementConsumerState()
		{
			this.elementConsumer.consumptionRate = base.def.inhaleRate;
		}

		public void SetElementConsumerState(bool enabled)
		{
			this.elementConsumer.EnableConsumption(enabled);
		}

		private ElementConsumer elementConsumer;

		private Submergable submergable;

		private Storage storage;

		private int exhaleCell;

		private KPrefabID prefabID;
	}
}
