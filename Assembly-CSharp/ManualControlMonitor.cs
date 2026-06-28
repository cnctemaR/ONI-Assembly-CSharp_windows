using System;
using UnityEngine;

public class ManualControlMonitor : GameStateMachine<ManualControlMonitor, ManualControlMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.satisfied.ParamTransition<bool>(this.isControlled, this.controlled, (ManualControlMonitor.Instance smi, bool p) => p);
		this.controlled.DefaultState(this.controlled.idle).ToggleStatusItem(Db.Get().DuplicantStatusItems.ManuallyControlled, null).ParamTransition<bool>(this.isControlled, this.satisfied, (ManualControlMonitor.Instance smi, bool p) => !p);
		this.controlled.idle.ToggleChore((ManualControlMonitor.Instance smi) => new ManualControlIdleChore(smi.master), this.satisfied, false).OnSignal(this.moveSignal, this.controlled.moving);
		this.controlled.moving.ToggleChore((ManualControlMonitor.Instance smi) => new ManualControlGoToChore(smi.master, this.movePos.Get(smi)), this.controlled.idle, false).OnSignal(this.moveSignal, this.controlled.resetmoving);
		this.controlled.resetmoving.GoTo(this.controlled.moving);
	}

	public StateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.BoolParameter isControlled;

	public StateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.Vector3Parameter movePos;

	public StateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.Signal moveSignal;

	public GameStateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.State satisfied;

	public ManualControlMonitor.ControlledState controlled;

	public class ControlledState : GameStateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.State idle;

		public GameStateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.State moving;

		public GameStateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.State resetmoving;
	}

	public new class Instance : GameStateMachine<ManualControlMonitor, ManualControlMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			master.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		}

		public bool IsControlled()
		{
			return base.sm.isControlled.Get(this);
		}

		public void SetDestination(Vector3 pos)
		{
			int num = Grid.PosToCell(pos);
			if (Grid.IsValidCell(num))
			{
				if (!base.master.GetComponent<Navigator>().CanReach(num))
				{
					int num2 = Grid.CellBelow(num);
					if (base.master.GetComponent<Navigator>().CanReach(num2))
					{
						base.sm.movePos.Set(Grid.CellToPosCBC(num2, Grid.SceneLayer.Move), this);
					}
				}
				else
				{
					base.sm.movePos.Set(pos, this);
				}
				base.sm.moveSignal.Trigger(base.smi);
			}
		}

		public bool CanReachPosition(Vector3 pos)
		{
			int num = Grid.PosToCell(pos);
			return base.master.GetComponent<Navigator>().CanReach(num);
		}

		private void OnClickTakeControl()
		{
			base.master.GetComponent<KMonoBehaviour>().PlaySound3D(GlobalAssets.GetSound("HUD_Click_Open", false));
			base.sm.isControlled.Set(true, this);
			base.GetComponent<UserMenu>().Refresh();
		}

		private void OnClickReleaseControl()
		{
			base.master.GetComponent<KMonoBehaviour>().PlaySound3D(GlobalAssets.GetSound("HUD_Click_Close", false));
			base.sm.isControlled.Set(false, this);
			base.GetComponent<UserMenu>().Refresh();
		}

		private void OnRefreshUserMenu(object data)
		{
		}
	}
}
