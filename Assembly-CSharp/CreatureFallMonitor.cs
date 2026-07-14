using System;
using UnityEngine;

public class CreatureFallMonitor : GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.grounded;
		this.grounded.ToggleBehaviour(GameTags.Creatures.Falling, (CreatureFallMonitor.Instance smi) => smi.ShouldFall(), null);
	}

	public static float FLOOR_DISTANCE = -0.065f;

	private const float SWIM_SETTLE_EPSILON = 0.1f;

	private const float SWIM_MAX_FALL_SPEED = 2f;

	private static readonly NavType[] SNAP_NAV_TYPES = new NavType[]
	{
		NavType.Floor,
		NavType.Hover,
		NavType.Swim
	};

	private static readonly NavType[] WALL_CRAWLER_NAV_TYPES = new NavType[]
	{
		NavType.Ceiling,
		NavType.LeftWall,
		NavType.RightWall
	};

	public GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.State grounded;

	public GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.State falling;

	public class Def : StateMachine.BaseDef
	{
		public bool canSwim;
	}

	public new class Instance : GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CreatureFallMonitor.Def def)
			: base(master, def)
		{
		}

		private Vector3 GetNavAnchor(Vector3 pos)
		{
			Vector3 vector = this.navigator.NavGrid.GetNavTypeData(this.navigator.CurrentNavType).animControllerOffset;
			return pos - vector;
		}

		public void SnapToGround()
		{
			Vector3 navAnchor = this.GetNavAnchor(base.smi.transform.GetPosition());
			Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(navAnchor), Grid.SceneLayer.Creatures);
			vector.x = navAnchor.x;
			base.smi.transform.SetPosition(vector);
			foreach (NavType navType in CreatureFallMonitor.SNAP_NAV_TYPES)
			{
				if (this.navigator.IsValidNavType(navType))
				{
					this.navigator.SetCurrentNavType(navType);
					return;
				}
			}
		}

		public bool ShouldFall()
		{
			if (this.kprefabId.HasTag(GameTags.Stored))
			{
				return false;
			}
			Vector3 position = base.smi.transform.GetPosition();
			int num = Grid.PosToCell(position);
			if (Grid.IsValidCell(num) && Grid.Solid[num])
			{
				return false;
			}
			if (this.navigator.IsMoving())
			{
				return false;
			}
			if (this.ShouldSettleIntoSwim(position))
			{
				return false;
			}
			if (this.navigator.CurrentNavType != NavType.Swim)
			{
				if (this.navigator.NavGrid.NavTable.IsValid(num, this.navigator.CurrentNavType))
				{
					return false;
				}
				foreach (NavType navType in CreatureFallMonitor.WALL_CRAWLER_NAV_TYPES)
				{
					if (this.navigator.CurrentNavType == navType)
					{
						return true;
					}
				}
			}
			Vector3 vector = position;
			vector.y += CreatureFallMonitor.FLOOR_DISTANCE;
			int num2 = Grid.PosToCell(vector);
			return !Grid.IsValidCell(num2) || !Grid.Solid[num2];
		}

		public bool CanSwimAtCurrentLocation()
		{
			return this.CanSwimAtCell(Grid.PosToCell(base.transform.GetPosition()));
		}

		private bool CanSwimAtCell(int cell)
		{
			return base.def.canSwim && this.navigator.NavGrid.NavTable.IsValid(cell, NavType.Swim) && (!GameComps.Gravities.Has(base.gameObject) || GameComps.Gravities.GetData(GameComps.Gravities.GetHandle(base.gameObject)).velocity.magnitude < 2f);
		}

		public bool ShouldSettleIntoSwim()
		{
			return this.ShouldSettleIntoSwim(base.transform.GetPosition());
		}

		private bool ShouldSettleIntoSwim(Vector3 pos)
		{
			Vector3 navAnchor = this.GetNavAnchor(pos);
			int num = Grid.PosToCell(navAnchor);
			if (!this.CanSwimAtCell(num))
			{
				return false;
			}
			float y = Grid.CellToPosCBC(num, Grid.SceneLayer.Creatures).y;
			return navAnchor.y <= y + 0.1f;
		}

		public string anim = "fall";

		[MyCmpReq]
		private KPrefabID kprefabId;

		[MyCmpReq]
		private Navigator navigator;
	}
}
