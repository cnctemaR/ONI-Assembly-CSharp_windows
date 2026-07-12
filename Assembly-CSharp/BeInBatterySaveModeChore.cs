using System;
using UnityEngine;

public class BeInBatterySaveModeChore : Chore<BeInBatterySaveModeChore.StatesInstance>
{
	public BeInBatterySaveModeChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.BeBatterySaveMode, master, master.GetComponent<ChoreProvider>(), true, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new BeInBatterySaveModeChore.StatesInstance(this, master.gameObject);
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
	}

	public static bool IsBatteryMonitorWaitingForUsToExit(BeInBatterySaveModeChore.StatesInstance smi, float dt)
	{
		return smi.batteryMonitor.IsInsideState(smi.batteryMonitor.sm.online.batterySaveMode.idle.exit);
	}

	public static string GetEnterAnim(BeInBatterySaveModeChore.StatesInstance smi)
	{
		NavType currentNavType = smi.gameObject.GetComponent<Navigator>().CurrentNavType;
		if (currentNavType != NavType.Ladder)
		{
		}
		return "low_power_pre";
	}

	public static string GetIdleAnim(BeInBatterySaveModeChore.StatesInstance smi)
	{
		NavType currentNavType = smi.gameObject.GetComponent<Navigator>().CurrentNavType;
		if (currentNavType != NavType.Ladder)
		{
		}
		return "low_power_loop";
	}

	public static string GetExitAnim(BeInBatterySaveModeChore.StatesInstance smi)
	{
		NavType currentNavType = smi.gameObject.GetComponent<Navigator>().CurrentNavType;
		if (currentNavType != NavType.Ladder)
		{
		}
		return "low_power_pst";
	}

	public const string EFFECT_NAME = "BionicBatterySaveMode";

	public class States : GameStateMachine<BeInBatterySaveModeChore.States, BeInBatterySaveModeChore.StatesInstance, BeInBatterySaveModeChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.enter;
			this.root.ToggleTag(GameTags.BatterySaveMode).TriggerOnEnter(GameHashes.BionicBatterySaveModeChanged, (BeInBatterySaveModeChore.StatesInstance smi) => true).TriggerOnExit(GameHashes.BionicBatterySaveModeChanged, (BeInBatterySaveModeChore.StatesInstance smi) => false)
				.ToggleEffect("BionicBatterySaveMode");
			this.enter.ToggleAnims("anim_bionic_kanim", 0f).PlayAnim(new Func<BeInBatterySaveModeChore.StatesInstance, string>(BeInBatterySaveModeChore.GetEnterAnim), KAnim.PlayMode.Once).OnAnimQueueComplete(this.idle);
			this.idle.ToggleAnims("anim_bionic_kanim", 0f).PlayAnim(new Func<BeInBatterySaveModeChore.StatesInstance, string>(BeInBatterySaveModeChore.GetIdleAnim), KAnim.PlayMode.Loop).UpdateTransition(this.exit, new Func<BeInBatterySaveModeChore.StatesInstance, float, bool>(BeInBatterySaveModeChore.IsBatteryMonitorWaitingForUsToExit), UpdateRate.SIM_1000ms, false);
			this.exit.ToggleAnims("anim_bionic_kanim", 0f).PlayAnim(new Func<BeInBatterySaveModeChore.StatesInstance, string>(BeInBatterySaveModeChore.GetExitAnim), KAnim.PlayMode.Once).OnAnimQueueComplete(this.end);
			this.end.ReturnSuccess();
		}

		public GameStateMachine<BeInBatterySaveModeChore.States, BeInBatterySaveModeChore.StatesInstance, BeInBatterySaveModeChore, object>.State enter;

		public GameStateMachine<BeInBatterySaveModeChore.States, BeInBatterySaveModeChore.StatesInstance, BeInBatterySaveModeChore, object>.State idle;

		public GameStateMachine<BeInBatterySaveModeChore.States, BeInBatterySaveModeChore.StatesInstance, BeInBatterySaveModeChore, object>.State exit;

		public GameStateMachine<BeInBatterySaveModeChore.States, BeInBatterySaveModeChore.StatesInstance, BeInBatterySaveModeChore, object>.State end;
	}

	public class StatesInstance : GameStateMachine<BeInBatterySaveModeChore.States, BeInBatterySaveModeChore.StatesInstance, BeInBatterySaveModeChore, object>.GameInstance
	{
		public StatesInstance(BeInBatterySaveModeChore master, GameObject duplicant)
			: base(master)
		{
			this.batteryMonitor = duplicant.GetSMI<BionicBatteryMonitor.Instance>();
		}

		public BionicBatteryMonitor.Instance batteryMonitor;
	}
}
