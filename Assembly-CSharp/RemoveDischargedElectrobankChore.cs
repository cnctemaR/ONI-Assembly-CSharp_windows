using System;
using UnityEngine;

public class RemoveDischargedElectrobankChore : Chore<RemoveDischargedElectrobankChore.Instance>
{
	public RemoveDischargedElectrobankChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.UnloadElectrobank, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new RemoveDischargedElectrobankChore.Instance(this, target.gameObject);
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("RemoveDischargedElectrobankChore null context.consumer");
			return;
		}
		BionicBatteryMonitor.Instance smi = context.consumerState.consumer.GetSMI<BionicBatteryMonitor.Instance>();
		if (smi == null)
		{
			global::Debug.LogError("RemoveDischargedElectrobankChore null RationMonitor.Instance");
			return;
		}
		GameObject firstDischargedElectrobankInInventory = smi.GetFirstDischargedElectrobankInInventory();
		if (firstDischargedElectrobankInInventory == null)
		{
			global::Debug.LogError("RemoveDischargedElectrobankChore dischargedElectrobank is null");
			return;
		}
		base.smi.sm.dischargedElectrobank.Set(firstDischargedElectrobankInInventory, base.smi, false);
		base.smi.sm.dupe.Set(context.consumerState.consumer, base.smi);
		base.Begin(context);
	}

	private static string GetAnimName(RemoveDischargedElectrobankChore.Instance smi)
	{
		if (smi.GetComponent<Navigator>().CurrentNavType != NavType.Ladder)
		{
			return "discharge";
		}
		return "ladder_discharge";
	}

	public static void SetOverrideAnimSymbol(RemoveDischargedElectrobankChore.Instance smi, bool overriding)
	{
		string text = "object";
		KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
		SymbolOverrideController component2 = smi.gameObject.GetComponent<SymbolOverrideController>();
		GameObject gameObject = smi.sm.dischargedElectrobank.Get(smi);
		if (gameObject != null)
		{
			KBatchedAnimTracker component3 = gameObject.GetComponent<KBatchedAnimTracker>();
			if (component3 != null)
			{
				component3.enabled = !overriding;
				Storage.MakeItemInvisible(gameObject, overriding, false);
			}
		}
		if (!overriding)
		{
			component2.RemoveSymbolOverride(text, 0);
			component.SetSymbolVisiblity(text, false);
			return;
		}
		KAnim.Build.Symbol symbolByIndex = gameObject.GetComponent<KBatchedAnimController>().CurrentAnim.animFile.build.GetSymbolByIndex(0U);
		component2.AddSymbolOverride(text, symbolByIndex, 0);
		component.SetSymbolVisiblity(text, true);
	}

	public static void RemoveDepletedElectrobank(RemoveDischargedElectrobankChore.Instance smi)
	{
		GameObject gameObject = smi.sm.dischargedElectrobank.Get(smi);
		smi.batteryMonitor.storage.Drop(gameObject, true);
	}

	public class States : GameStateMachine<RemoveDischargedElectrobankChore.States, RemoveDischargedElectrobankChore.Instance, RemoveDischargedElectrobankChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.working;
			base.Target(this.dupe);
			this.working.ToggleAnims("anim_bionic_kanim", 0f).PlayAnim(new Func<RemoveDischargedElectrobankChore.Instance, string>(RemoveDischargedElectrobankChore.GetAnimName), KAnim.PlayMode.Once).Enter("Add Symbol Override", delegate(RemoveDischargedElectrobankChore.Instance smi)
			{
				RemoveDischargedElectrobankChore.SetOverrideAnimSymbol(smi, true);
			})
				.Exit("Revert Symbol Override", delegate(RemoveDischargedElectrobankChore.Instance smi)
				{
					RemoveDischargedElectrobankChore.SetOverrideAnimSymbol(smi, false);
				})
				.OnAnimQueueComplete(this.complete);
			this.complete.Enter(new StateMachine<RemoveDischargedElectrobankChore.States, RemoveDischargedElectrobankChore.Instance, RemoveDischargedElectrobankChore, object>.State.Callback(RemoveDischargedElectrobankChore.RemoveDepletedElectrobank)).ReturnSuccess();
		}

		public GameStateMachine<RemoveDischargedElectrobankChore.States, RemoveDischargedElectrobankChore.Instance, RemoveDischargedElectrobankChore, object>.State working;

		public GameStateMachine<RemoveDischargedElectrobankChore.States, RemoveDischargedElectrobankChore.Instance, RemoveDischargedElectrobankChore, object>.State complete;

		public StateMachine<RemoveDischargedElectrobankChore.States, RemoveDischargedElectrobankChore.Instance, RemoveDischargedElectrobankChore, object>.TargetParameter dupe;

		public StateMachine<RemoveDischargedElectrobankChore.States, RemoveDischargedElectrobankChore.Instance, RemoveDischargedElectrobankChore, object>.TargetParameter dischargedElectrobank;
	}

	public class Instance : GameStateMachine<RemoveDischargedElectrobankChore.States, RemoveDischargedElectrobankChore.Instance, RemoveDischargedElectrobankChore, object>.GameInstance
	{
		public BionicBatteryMonitor.Instance batteryMonitor
		{
			get
			{
				return base.sm.dupe.Get(this).GetSMI<BionicBatteryMonitor.Instance>();
			}
		}

		public Instance(RemoveDischargedElectrobankChore master, GameObject duplicant)
			: base(master)
		{
		}
	}
}
