using System;

public class SetNavOrientationOnSpawnMonitor : GameStateMachine<SetNavOrientationOnSpawnMonitor, SetNavOrientationOnSpawnMonitor.Instance, IStateMachineTarget, SetNavOrientationOnSpawnMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.root;
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<SetNavOrientationOnSpawnMonitor, SetNavOrientationOnSpawnMonitor.Instance, IStateMachineTarget, SetNavOrientationOnSpawnMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SetNavOrientationOnSpawnMonitor.Def def)
			: base(master, def)
		{
			base.Subscribe(1119167081, new Action<object>(this.SetSpawnOrientation));
		}

		public void SetSpawnOrientation(object o)
		{
			int num = Grid.PosToCell(this);
			if (!Grid.IsValidCell(num))
			{
				return;
			}
			int num2 = Grid.CellAbove(num);
			int num3 = Grid.CellBelow(num);
			if (Grid.IsValidCell(num2) && Grid.Solid[num2] && (!Grid.IsValidCell(num3) || !Grid.Solid[num3]))
			{
				base.gameObject.GetComponent<Navigator>().CurrentNavType = NavType.Ceiling;
			}
		}

		protected override void OnCleanUp()
		{
			base.Unsubscribe(1119167081, new Action<object>(this.SetSpawnOrientation));
			base.OnCleanUp();
		}
	}
}
