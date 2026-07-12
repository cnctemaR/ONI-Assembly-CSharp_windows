using System;
using STRINGS;
using UnityEngine;

public class ReloadElectrobankChore : Chore<ReloadElectrobankChore.Instance>
{
	public ReloadElectrobankChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.ReloadElectrobank, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new ReloadElectrobankChore.Instance(this, target.gameObject);
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		this.AddPrecondition(ReloadElectrobankChore.ElectrobankIsNotNull, null);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("ReloadElectrobankChore null context.consumer");
			return;
		}
		BionicBatteryMonitor.Instance smi = context.consumerState.consumer.GetSMI<BionicBatteryMonitor.Instance>();
		if (smi == null)
		{
			global::Debug.LogError("ReloadElectrobankChore null RationMonitor.Instance");
			return;
		}
		Electrobank closestElectrobank = smi.GetClosestElectrobank();
		if (closestElectrobank == null)
		{
			global::Debug.LogError("ReloadElectrobankChore null electrobank.gameObject");
			return;
		}
		base.smi.cachedElectrobankSourcePrefabRef = Assets.GetPrefab(closestElectrobank.PrefabID());
		base.smi.sm.electrobankSource.Set(closestElectrobank.gameObject, base.smi, false);
		base.smi.sm.dupe.Set(context.consumerState.consumer, base.smi);
		base.Begin(context);
	}

	public static bool HasAnyDepletedBattery(ReloadElectrobankChore.Instance smi)
	{
		return smi.batteryMonitor.DepletedElectrobankCount > 0;
	}

	public static GameObject GetAnyEmptyBattery(ReloadElectrobankChore.Instance smi)
	{
		return smi.batteryMonitor.storage.FindFirst(GameTags.EmptyPortableBattery);
	}

	public static void RemoveDepletedElectrobank(ReloadElectrobankChore.Instance smi)
	{
		GameObject anyEmptyBattery = ReloadElectrobankChore.GetAnyEmptyBattery(smi);
		if (anyEmptyBattery != null)
		{
			smi.batteryMonitor.storage.Drop(anyEmptyBattery, true);
		}
	}

	public static void InstallElectrobank(ReloadElectrobankChore.Instance smi)
	{
		Storage[] components = smi.gameObject.GetComponents<Storage>();
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] != smi.batteryMonitor.storage && components[i].FindFirst(GameTags.ChargedPortableBattery) != null)
			{
				components[i].Transfer(smi.batteryMonitor.storage, false, false);
				break;
			}
		}
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_BionicBattery, true);
	}

	private static void SetStoredItemVisibility(GameObject item, bool visible)
	{
		KBatchedAnimTracker component = item.GetComponent<KBatchedAnimTracker>();
		if (component != null)
		{
			component.enabled = visible;
		}
		Storage.MakeItemInvisible(item, !visible, false);
	}

	public static void SetOverrideAnimSymbol(ReloadElectrobankChore.Instance smi, bool overriding, bool hideFromBack, GameObject electrobank)
	{
		string text = "object";
		KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
		SymbolOverrideController component2 = smi.gameObject.GetComponent<SymbolOverrideController>();
		if (electrobank != null && hideFromBack)
		{
			ReloadElectrobankChore.SetStoredItemVisibility(electrobank, !overriding);
		}
		if (!overriding)
		{
			component2.RemoveSymbolOverride(text, 0);
			component.SetSymbolVisiblity(text, false);
			return;
		}
		KAnim.Build.Symbol symbolByIndex = ((electrobank != null) ? electrobank.GetComponent<KBatchedAnimController>() : smi.cachedElectrobankSourcePrefabRef.GetComponent<KBatchedAnimController>()).AnimFiles[0].GetData().build.GetSymbolByIndex(0U);
		component2.AddSymbolOverride(text, symbolByIndex, 0);
		component.SetSymbolVisiblity(text, true);
	}

	public const float LOOP_LENGTH = 4.333f;

	public static readonly Chore.Precondition ElectrobankIsNotNull = new Chore.Precondition
	{
		id = "ElectrobankIsNotNull",
		description = DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return null != context.consumerState.consumer.GetSMI<BionicBatteryMonitor.Instance>().GetClosestElectrobank();
		}
	};

	public class States : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			base.Target(this.dupe);
			this.fetch.InitializeStates(this.dupe, this.electrobankSource, this.pickedUpElectrobank, this.amountRequested, this.actualunits, this.fetchCompleted, null).OnTargetLost(this.electrobankSource, this.electrobankLost);
			this.fetchCompleted.EnterTransition(this.emptyDepleatedBatteries, new StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.Transition.ConditionCallback(ReloadElectrobankChore.HasAnyDepletedBattery)).GoTo(this.install);
			this.emptyDepleatedBatteries.DefaultState(this.emptyDepleatedBatteries.animate);
			this.emptyDepleatedBatteries.animate.ToggleAnims("anim_bionic_kanim", 0f).PlayAnim("discharge", KAnim.PlayMode.Once).Enter("Add Symbol Override", delegate(ReloadElectrobankChore.Instance smi)
			{
				ReloadElectrobankChore.SetOverrideAnimSymbol(smi, true, false, ReloadElectrobankChore.GetAnyEmptyBattery(smi));
			})
				.Exit("Revert Symbol Override", delegate(ReloadElectrobankChore.Instance smi)
				{
					ReloadElectrobankChore.SetOverrideAnimSymbol(smi, false, false, ReloadElectrobankChore.GetAnyEmptyBattery(smi));
				})
				.OnAnimQueueComplete(this.emptyDepleatedBatteries.end);
			this.emptyDepleatedBatteries.end.Enter(new StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State.Callback(ReloadElectrobankChore.RemoveDepletedElectrobank)).EnterTransition(this.emptyDepleatedBatteries.animate, new StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.Transition.ConditionCallback(ReloadElectrobankChore.HasAnyDepletedBattery)).GoTo(this.install);
			this.install.DefaultState(this.install.pre).ToggleAnims("anim_bionic_kanim", 0f).Enter("Add Symbol Override", delegate(ReloadElectrobankChore.Instance smi)
			{
				ReloadElectrobankChore.SetOverrideAnimSymbol(smi, true, true, this.pickedUpElectrobank.Get(smi));
			})
				.Exit("Revert Symbol Override", delegate(ReloadElectrobankChore.Instance smi)
				{
					ReloadElectrobankChore.SetOverrideAnimSymbol(smi, false, true, this.pickedUpElectrobank.Get(smi));
				});
			this.install.pre.PlayAnim("consume_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(this.install.loop).ScheduleGoTo(3f, this.install.loop);
			this.install.loop.PlayAnim("consume_loop", KAnim.PlayMode.Loop).ScheduleGoTo(4.333f, this.install.pst);
			this.install.pst.PlayAnim("consume_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.complete).ScheduleGoTo(3f, this.complete);
			this.complete.Enter(new StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State.Callback(ReloadElectrobankChore.InstallElectrobank)).ReturnSuccess();
			this.electrobankLost.Target(this.dupe).TriggerOnEnter(GameHashes.TargetElectrobankLost, null).ReturnFailure();
		}

		public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.FetchSubState fetch;

		public ReloadElectrobankChore.States.EmptyDepleatedStates emptyDepleatedBatteries;

		public ReloadElectrobankChore.States.InstallState install;

		public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State fetchCompleted;

		public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State complete;

		public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State electrobankLost;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter dupe;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter electrobankSource;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter lastDepleatedElectrobankFound;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter pickedUpElectrobank;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.TargetParameter messstation;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.FloatParameter actualunits;

		public StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.FloatParameter amountRequested = new StateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.FloatParameter(1f);

		public class EmptyDepleatedStates : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State
		{
			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State animate;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State end;
		}

		public class InstallState : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State
		{
			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State pre;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State loop;

			public GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.State pst;
		}
	}

	public class Instance : GameStateMachine<ReloadElectrobankChore.States, ReloadElectrobankChore.Instance, ReloadElectrobankChore, object>.GameInstance
	{
		public BionicBatteryMonitor.Instance batteryMonitor
		{
			get
			{
				return base.sm.dupe.Get(this).GetSMI<BionicBatteryMonitor.Instance>();
			}
		}

		public Instance(ReloadElectrobankChore master, GameObject duplicant)
			: base(master)
		{
		}

		public GameObject cachedElectrobankSourcePrefabRef;
	}
}
