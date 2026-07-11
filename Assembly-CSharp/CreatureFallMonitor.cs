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
			this.navigator = master.GetComponent<Navigator>();
		}

		public void SnapToGround()
		{
			Vector3 position = base.smi.transform.GetPosition();
			Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(position), Grid.SceneLayer.Creatures);
			vector.x = position.x;
			base.smi.transform.SetPosition(vector);
			if (this.navigator.IsValidNavType(NavType.Floor))
			{
				this.navigator.SetCurrentNavType(NavType.Floor);
			}
		}

		public bool ShouldFall()
		{
			if (base.gameObject.HasTag(GameTags.Stored))
			{
				return false;
			}
			Vector3 position = base.smi.transform.GetPosition();
			int num = Grid.PosToCell(position);
			bool flag = Grid.Solid[num];
			if (flag)
			{
				return false;
			}
			if (this.navigator.IsMoving())
			{
				return false;
			}
			if (this.CanSwimAtCurrentLocation(false))
			{
				return false;
			}
			if (this.navigator.CurrentNavType != NavType.Swim)
			{
				bool flag2 = this.navigator.NavGrid.NavTable.IsValid(num, this.navigator.CurrentNavType);
				if (flag2)
				{
					return false;
				}
				if (this.navigator.CurrentNavType != NavType.Floor)
				{
					return true;
				}
			}
			Vector3 vector = position;
			vector.y += CreatureFallMonitor.FLOOR_DISTANCE;
			int num2 = Grid.PosToCell(vector);
			bool flag3 = Grid.Solid[num2];
			return !flag3;
		}

		public bool CanSwimAtCurrentLocation(bool check_head)
		{
			if (base.def.canSwim)
			{
				Vector3 position = base.transform.GetPosition();
				float num = 1f;
				if (!check_head)
				{
					num = 0.5f;
				}
				position.y += base.transform.GetComponent<KBoxCollider2D>().size.y * num;
				int num2 = Grid.PosToCell(position);
				if (Grid.IsSubstantialLiquid(num2, 0.35f))
				{
					if (!GameComps.Gravities.Has(base.gameObject))
					{
						return true;
					}
					if (GameComps.Gravities.GetData(GameComps.Gravities.GetHandle(base.gameObject)).velocity.magnitude < 2f)
					{
						return true;
					}
				}
			}
			return false;
		}

		public string anim = "fall";

		private Navigator navigator;
	}
}
