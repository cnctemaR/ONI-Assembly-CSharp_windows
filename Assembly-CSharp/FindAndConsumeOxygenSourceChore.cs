using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FindAndConsumeOxygenSourceChore : Chore<FindAndConsumeOxygenSourceChore.Instance>
{
	public FindAndConsumeOxygenSourceChore(IStateMachineTarget target, bool critical)
		: base(critical ? Db.Get().ChoreTypes.FindOxygenSourceItem_Critical : Db.Get().ChoreTypes.FindOxygenSourceItem, target, target.GetComponent<ChoreProvider>(), false, null, null, null, critical ? PriorityScreen.PriorityClass.compulsory : PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new FindAndConsumeOxygenSourceChore.Instance(this, target.gameObject);
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		this.AddPrecondition(FindAndConsumeOxygenSourceChore.OxygenSourceItemIsNotNull, null);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("FindAndConsumeOxygenSourceChore null context.consumer");
			return;
		}
		BionicOxygenTankMonitor.Instance smi = context.consumerState.consumer.GetSMI<BionicOxygenTankMonitor.Instance>();
		if (smi == null)
		{
			global::Debug.LogError("FindAndConsumeOxygenSourceChore null BionicOxygenTankMonitor.Instance");
			return;
		}
		Pickupable closestOxygenSource = smi.GetClosestOxygenSource();
		if (closestOxygenSource == null)
		{
			global::Debug.LogError("FindAndConsumeOxygenSourceChore null oxygenSourceItem.gameObject");
			return;
		}
		base.smi.sm.oxygenSourceItem.Set(closestOxygenSource.gameObject, base.smi, false);
		base.smi.sm.amountRequested.Set(Mathf.Min(smi.SpaceAvailableInTank, closestOxygenSource.UnreservedAmount), base.smi, false);
		base.smi.sm.dupe.Set(context.consumerState.consumer, base.smi);
		base.Begin(context);
	}

	public static bool IsNotAllowedByScheduleAndChoreIsNotCritical(FindAndConsumeOxygenSourceChore.Instance smi)
	{
		return !FindAndConsumeOxygenSourceChore.IsCriticalChore(smi) && !FindAndConsumeOxygenSourceChore.IsAllowedBySchedule(smi);
	}

	public static bool IsAllowedBySchedule(FindAndConsumeOxygenSourceChore.Instance smi)
	{
		return BionicOxygenTankMonitor.IsAllowedToSeekOxygenBySchedule(smi.oxygenTankMonitor);
	}

	public static bool IsCriticalChore(FindAndConsumeOxygenSourceChore.Instance smi)
	{
		return smi.master.choreType == Db.Get().ChoreTypes.FindOxygenSourceItem_Critical;
	}

	public static void ExtractOxygenFromItem(FindAndConsumeOxygenSourceChore.Instance smi)
	{
		GameObject gameObject = smi.sm.pickedUpItem.Get(smi);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		if (component.Element.IsGas)
		{
			Storage[] components = smi.gameObject.GetComponents<Storage>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i] != smi.oxygenTankMonitor.storage)
				{
					List<GameObject> list = new List<GameObject>();
					components[i].Find(GameTags.Breathable, list);
					foreach (GameObject gameObject2 in list)
					{
						if (gameObject2 != null)
						{
							components[i].Transfer(gameObject2, smi.oxygenTankMonitor.storage, false, false);
							break;
						}
					}
				}
			}
			return;
		}
		SimHashes simHashes = SimHashes.Oxygen;
		if (ElementLoader.GetElement(component.Element.sublimateId.CreateTag()).HasTag(GameTags.Breathable))
		{
			simHashes = component.Element.sublimateId;
		}
		smi.oxygenTankMonitor.storage.AddGasChunk(simHashes, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, false, true);
		Util.KDestroyGameObject(gameObject);
	}

	public static void SetOverrideAnimSymbol(FindAndConsumeOxygenSourceChore.Instance smi, bool overriding)
	{
		GameObject gameObject = smi.sm.pickedUpItem.Get(smi);
		if (gameObject != null)
		{
			KBatchedAnimTracker component = gameObject.GetComponent<KBatchedAnimTracker>();
			if (component != null)
			{
				component.enabled = !overriding;
			}
			Storage.MakeItemInvisible(gameObject, overriding, false);
		}
		if (!overriding)
		{
			smi.RemoveSymbolOverrideObject();
			return;
		}
		if (gameObject != null)
		{
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			smi.ShowBottleSymbolOverrideObject(component2.Element);
		}
	}

	public static void TriggerOxygenItemLostSignal(FindAndConsumeOxygenSourceChore.Instance smi)
	{
		if (smi.oxygenTankMonitor != null)
		{
			smi.oxygenTankMonitor.sm.OxygenSourceItemLostSignal.Trigger(smi.oxygenTankMonitor);
		}
	}

	public static float GetConsumeDuration(FindAndConsumeOxygenSourceChore.Instance smi)
	{
		float num = smi.sm.actualunits.Get(smi) / BionicOxygenTankMonitor.OXYGEN_TANK_CAPACITY_KG;
		return Mathf.Max(24f * num, 4.333f);
	}

	public const string CANISTER_BODY_SYMBOL_NAME = "canister";

	public const string CANISTER_CAP_SYMBOL_NAME = "cap";

	public const string CANISTER_CAP_COLOR_SYMBOL_NAME = "substance_tinter_cap";

	public const string CANISTER_BODY_COLOR_SYMBOL_NAME = "substance_tinter";

	public const float MAX_LOOP_DURATION = 24f;

	public const float MIN_LOOP_DURATION = 4.333f;

	public static readonly Chore.Precondition OxygenSourceItemIsNotNull = new Chore.Precondition
	{
		id = "OxygenSourceIsNotNull",
		description = DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Pickupable closestOxygenSource = context.consumerState.consumer.GetSMI<BionicOxygenTankMonitor.Instance>().GetClosestOxygenSource();
			return closestOxygenSource != null && closestOxygenSource.UnreservedAmount > 0f;
		}
	};

	public class States : GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			base.Target(this.dupe);
			this.fetch.InitializeStates(this.dupe, this.oxygenSourceItem, this.pickedUpItem, this.amountRequested, this.actualunits, this.consume, null).OnTargetLost(this.oxygenSourceItem, this.oxygenSourceLost).ScheduleChange(this.scheduleFailure, new StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.Transition.ConditionCallback(FindAndConsumeOxygenSourceChore.IsNotAllowedByScheduleAndChoreIsNotCritical));
			this.consume.Target(this.pickedUpItem).OnTargetLost(this.pickedUpItem, this.oxygenSourceLost).Target(this.dupe)
				.ScheduleChange(this.scheduleFailure, new StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.Transition.ConditionCallback(FindAndConsumeOxygenSourceChore.IsNotAllowedByScheduleAndChoreIsNotCritical))
				.DefaultState(this.consume.pre)
				.ToggleAnims("anim_bionic_kanim", 0f)
				.Enter("Add Symbol Override", delegate(FindAndConsumeOxygenSourceChore.Instance smi)
				{
					FindAndConsumeOxygenSourceChore.SetOverrideAnimSymbol(smi, true);
				})
				.Exit("Revert Symbol Override", delegate(FindAndConsumeOxygenSourceChore.Instance smi)
				{
					FindAndConsumeOxygenSourceChore.SetOverrideAnimSymbol(smi, false);
				});
			this.consume.pre.PlayAnim("consume_canister_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.consume.loop).ScheduleGoTo(3f, this.consume.loop);
			this.consume.loop.PlayAnim("consume_canister_loop", KAnim.PlayMode.Loop).ScheduleGoTo(new Func<FindAndConsumeOxygenSourceChore.Instance, float>(FindAndConsumeOxygenSourceChore.GetConsumeDuration), this.consume.pst);
			this.consume.pst.PlayAnim("consume_canister_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.complete).ScheduleGoTo(3f, this.complete);
			this.complete.Enter(new StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State.Callback(FindAndConsumeOxygenSourceChore.ExtractOxygenFromItem)).ReturnSuccess();
			this.scheduleFailure.Target(this.dupe).ReturnFailure();
			this.oxygenSourceLost.Target(this.dupe).Enter(new StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State.Callback(FindAndConsumeOxygenSourceChore.TriggerOxygenItemLostSignal)).ReturnFailure();
		}

		public GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.FetchSubState fetch;

		public FindAndConsumeOxygenSourceChore.States.InstallState consume;

		public GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State complete;

		public GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State oxygenSourceLost;

		public GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State scheduleFailure;

		public StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.TargetParameter dupe;

		public StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.TargetParameter oxygenSourceItem;

		public StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.TargetParameter pickedUpItem;

		public StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.FloatParameter actualunits;

		public StateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.FloatParameter amountRequested;

		public class InstallState : GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State
		{
			public GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State pre;

			public GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State loop;

			public GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.State pst;
		}
	}

	public class Instance : GameStateMachine<FindAndConsumeOxygenSourceChore.States, FindAndConsumeOxygenSourceChore.Instance, FindAndConsumeOxygenSourceChore, object>.GameInstance
	{
		public BionicOxygenTankMonitor.Instance oxygenTankMonitor
		{
			get
			{
				return base.sm.dupe.Get(this).GetSMI<BionicOxygenTankMonitor.Instance>();
			}
		}

		public Instance(FindAndConsumeOxygenSourceChore master, GameObject duplicant)
			: base(master)
		{
		}

		public void ShowBottleSymbolOverrideObject(Element elementOfCanister)
		{
			if (this.canisterBodySymbolOverrideObject == null)
			{
				KAnimFile[] anims = elementOfCanister.substance.anims;
				GameObject gameObject = Util.NewGameObject(base.gameObject, "canister_symbol");
				gameObject.transform.SetParent(base.gameObject.transform, false);
				gameObject.SetActive(false);
				this.canisterBodySymbolOverrideObject = gameObject.AddComponent<KBatchedAnimController>();
				this.canisterBodySymbolOverrideObject.AnimFiles = anims;
				this.canisterBodySymbolOverrideObject.initialAnim = "idle1";
				this.canisterBodySymbolOverrideObject.SetSymbolVisiblity("cap", false);
				this.canisterBodySymbolOverrideObject.SetSymbolVisiblity("substance_tinter_cap", false);
				KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
				kbatchedAnimTracker.symbol = new HashedString("canister");
				kbatchedAnimTracker.offset = Vector3.zero;
				kbatchedAnimTracker.matchParentOffset = true;
				kbatchedAnimTracker.forceAlwaysAlive = true;
				kbatchedAnimTracker.forceAlwaysVisible = true;
				gameObject.SetActive(true);
				Color32 colour = elementOfCanister.substance.colour;
				colour.a = byte.MaxValue;
				this.canisterBodySymbolOverrideObject.SetSymbolTint(new KAnimHashedString("substance_tinter"), colour);
			}
			if (this.canisterCapSymbolOverrideObject == null)
			{
				KAnimFile[] anims2 = elementOfCanister.substance.anims;
				GameObject gameObject2 = Util.NewGameObject(base.gameObject, "canister_cap_symbol");
				gameObject2.transform.SetParent(base.gameObject.transform, false);
				gameObject2.SetActive(false);
				this.canisterCapSymbolOverrideObject = gameObject2.AddComponent<KBatchedAnimController>();
				this.canisterCapSymbolOverrideObject.AnimFiles = anims2;
				this.canisterCapSymbolOverrideObject.initialAnim = "cap";
				KBatchedAnimTracker kbatchedAnimTracker2 = gameObject2.AddComponent<KBatchedAnimTracker>();
				kbatchedAnimTracker2.symbol = new HashedString("cap");
				kbatchedAnimTracker2.offset = Vector3.zero;
				kbatchedAnimTracker2.matchParentOffset = true;
				kbatchedAnimTracker2.forceAlwaysAlive = true;
				kbatchedAnimTracker2.forceAlwaysVisible = true;
				gameObject2.SetActive(true);
				Color32 colour2 = elementOfCanister.substance.colour;
				colour2.a = byte.MaxValue;
				this.canisterCapSymbolOverrideObject.SetSymbolTint(new KAnimHashedString("substance_tinter_cap"), colour2);
			}
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			bool flag;
			Vector3 vector = component.GetSymbolTransform("canister", out flag).GetColumn(3);
			vector.z = this.canisterBodySymbolOverrideObject.transform.parent.position.z - 0.01f;
			this.canisterBodySymbolOverrideObject.transform.position = vector;
			bool flag2;
			Vector3 vector2 = component.GetSymbolTransform("canister", out flag2).GetColumn(3);
			vector2.z = vector.z - 0.01f;
			this.canisterCapSymbolOverrideObject.transform.position = vector2;
			component.SetSymbolVisiblity("canister", false);
			component.SetSymbolVisiblity("cap", false);
		}

		public void RemoveSymbolOverrideObject()
		{
			if (this.canisterBodySymbolOverrideObject != null)
			{
				this.canisterBodySymbolOverrideObject.gameObject.DeleteObject();
				this.canisterBodySymbolOverrideObject = null;
			}
			if (this.canisterCapSymbolOverrideObject != null)
			{
				this.canisterCapSymbolOverrideObject.gameObject.DeleteObject();
				this.canisterCapSymbolOverrideObject = null;
			}
		}

		protected override void OnCleanUp()
		{
			this.RemoveSymbolOverrideObject();
			base.OnCleanUp();
		}

		public KBatchedAnimController canisterBodySymbolOverrideObject;

		public KBatchedAnimController canisterCapSymbolOverrideObject;
	}
}
