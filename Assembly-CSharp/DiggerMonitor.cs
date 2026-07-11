using System;
using ProcGen;

public class DiggerMonitor : GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		this.loop.EventTransition(GameHashes.BeginMeteorBombardment, (DiggerMonitor.Instance smi) => Game.Instance, this.dig, (DiggerMonitor.Instance smi) => smi.IsOnSurface());
		this.dig.ToggleBehaviour(GameTags.Creatures.Tunnel, (DiggerMonitor.Instance smi) => true, null).GoTo(this.loop);
	}

	public GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>.State loop;

	public GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>.State dig;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, DiggerMonitor.Def def)
			: base(master, def)
		{
			global::World instance = global::World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
		}

		private void CheckInSolid(int cell)
		{
			int num = Grid.PosToCell(base.gameObject);
			if (cell == num && Grid.IsSolidCell(num))
			{
				Navigator component = base.gameObject.GetComponent<Navigator>();
				if (component != null && component.CurrentNavType != NavType.Solid)
				{
					component.SetCurrentNavType(NavType.Solid);
				}
			}
		}

		private void OnSolidChanged(int cell)
		{
			this.CheckInSolid(cell);
		}

		public bool IsOnSurface()
		{
			int num = Grid.PosToCell(this);
			SubWorld.ZoneType subWorldZoneType = global::World.Instance.zoneRenderData.GetSubWorldZoneType(num);
			if (subWorldZoneType == SubWorld.ZoneType.Space)
			{
				int num2 = Grid.CellAbove(num);
				while (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					num2 = Grid.CellAbove(num2);
				}
				return !Grid.IsValidCell(num2);
			}
			return false;
		}
	}
}
