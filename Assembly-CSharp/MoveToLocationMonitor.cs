using System;
using STRINGS;

public class MoveToLocationMonitor : GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance, IStateMachineTarget, MoveToLocationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.DoNothing();
		this.moving.ToggleChore((MoveToLocationMonitor.Instance smi) => new MoveChore(smi.master, Db.Get().ChoreTypes.MoveTo, (MoveChore.StatesInstance smii) => smi.targetCell, false), this.satisfied);
	}

	public GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance, IStateMachineTarget, MoveToLocationMonitor.Def>.State satisfied;

	public GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance, IStateMachineTarget, MoveToLocationMonitor.Def>.State moving;

	public class Def : StateMachine.BaseDef
	{
		public Tag[] invalidTagsForMoveTo = new Tag[0];
	}

	public new class Instance : GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance, IStateMachineTarget, MoveToLocationMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, MoveToLocationMonitor.Def def)
			: base(master, def)
		{
			master.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
			this.kPrefabID = base.GetComponent<KPrefabID>();
		}

		private void OnRefreshUserMenu(object data)
		{
			if (this.kPrefabID.HasAnyTags(base.def.invalidTagsForMoveTo))
			{
				return;
			}
			Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.MOVETOLOCATION.NAME, new global::System.Action(this.OnClickMoveToLocation), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.MOVETOLOCATION.TOOLTIP, true), 0.2f);
		}

		private void OnClickMoveToLocation()
		{
			MoveToLocationTool.Instance.Activate(base.GetComponent<Navigator>());
		}

		public void MoveToLocation(int cell)
		{
			this.targetCell = cell;
			base.smi.GoTo(base.smi.sm.satisfied);
			base.smi.GoTo(base.smi.sm.moving);
		}

		public override void StopSM(string reason)
		{
			base.master.Unsubscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
			base.StopSM(reason);
		}

		public int targetCell;

		private KPrefabID kPrefabID;
	}
}
