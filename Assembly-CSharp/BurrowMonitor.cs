using System;
using UnityEngine;

public class BurrowMonitor : GameStateMachine<BurrowMonitor, BurrowMonitor.Instance, IStateMachineTarget, BurrowMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.openair;
		this.openair.ToggleBehaviour(GameTags.Creatures.WantsToEnterBurrow, (BurrowMonitor.Instance smi) => smi.ShouldBurrow() && smi.timeinstate > smi.def.minimumAwakeTime, delegate(BurrowMonitor.Instance smi)
		{
			smi.BurrowComplete();
		}).Transition(this.entombed, (BurrowMonitor.Instance smi) => smi.IsEntombed(), UpdateRate.SIM_200ms).Enter("SetFallAnim", delegate(BurrowMonitor.Instance smi)
		{
			smi.GetSMI<CreatureFallMonitor.Instance>().anim = "fall";
		})
			.Enter("SetCollider", delegate(BurrowMonitor.Instance smi)
			{
				smi.SetCollider(true);
			});
		this.entombed.Enter("SetFallAnim", delegate(BurrowMonitor.Instance smi)
		{
			smi.GetSMI<CreatureFallMonitor.Instance>().anim = "dormant_pre";
		}).Enter("SetCollider", delegate(BurrowMonitor.Instance smi)
		{
			smi.SetCollider(false);
		}).Transition(this.openair, (BurrowMonitor.Instance smi) => !smi.IsEntombed(), UpdateRate.SIM_200ms)
			.ToggleBehaviour(GameTags.Creatures.Burrowed, (BurrowMonitor.Instance smi) => smi.IsEntombed(), delegate(BurrowMonitor.Instance smi)
			{
				smi.GoTo(this.openair);
			})
			.ToggleBehaviour(GameTags.Creatures.WantsToExitBurrow, (BurrowMonitor.Instance smi) => smi.EmergeIsClear() && GameClock.Instance.IsNighttime(), delegate(BurrowMonitor.Instance smi)
			{
				smi.ExitBurrowComplete();
			});
	}

	public GameStateMachine<BurrowMonitor, BurrowMonitor.Instance, IStateMachineTarget, BurrowMonitor.Def>.State openair;

	public GameStateMachine<BurrowMonitor, BurrowMonitor.Instance, IStateMachineTarget, BurrowMonitor.Def>.State entombed;

	public GameStateMachine<BurrowMonitor, BurrowMonitor.Instance, IStateMachineTarget, BurrowMonitor.Def>.State burrowcomplete;

	public GameStateMachine<BurrowMonitor, BurrowMonitor.Instance, IStateMachineTarget, BurrowMonitor.Def>.State exitburrowcomplete;

	public class Def : StateMachine.BaseDef
	{
		public float burrowHardnessLimit = 20f;

		public float minimumAwakeTime = 24f;

		public Vector2 moundColliderSize = new Vector2f(1f, 1.5f);

		public Vector2 moundColliderOffset = new Vector2(0f, 0.75f);
	}

	public new class Instance : GameStateMachine<BurrowMonitor, BurrowMonitor.Instance, IStateMachineTarget, BurrowMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, BurrowMonitor.Def def)
			: base(master, def)
		{
			KBoxCollider2D component = master.GetComponent<KBoxCollider2D>();
			this.originalColliderSize = component.size;
			this.originalColliderOffset = component.offset;
		}

		public bool EmergeIsClear()
		{
			int num = Grid.PosToCell(base.gameObject);
			if (!Grid.IsValidCell(num) || !Grid.IsValidCell(Grid.CellAbove(num)))
			{
				return false;
			}
			int num2 = Grid.CellAbove(num);
			return !Grid.Solid[num2] && !Grid.IsSubstantialLiquid(Grid.CellAbove(num), 0.9f);
		}

		public bool ShouldBurrow()
		{
			return !GameClock.Instance.IsNighttime() && this.CanBurrowInto(Grid.CellBelow(Grid.PosToCell(base.gameObject)));
		}

		public bool CanBurrowInto(int cell)
		{
			return Grid.IsValidCell(cell) && Grid.Solid[cell] && !Grid.IsSubstantialLiquid(Grid.CellAbove(cell), 0.35f) && !(Grid.Objects[cell, 1] != null) && (float)Grid.Element[cell].hardness <= base.def.burrowHardnessLimit && !Grid.Foundation[cell];
		}

		public bool IsEntombed()
		{
			int num = Grid.PosToCell(base.smi);
			return Grid.IsValidCell(num) && Grid.Solid[num];
		}

		public void ExitBurrowComplete()
		{
			base.smi.GetComponent<KBatchedAnimController>().Play("idle_loop", KAnim.PlayMode.Once, 1f, 0f);
			this.GoTo(base.sm.openair);
		}

		public void BurrowComplete()
		{
			base.smi.transform.SetPosition(Grid.CellToPosCBC(Grid.CellBelow(Grid.PosToCell(base.transform.GetPosition())), Grid.SceneLayer.Creatures));
			base.smi.GetComponent<KBatchedAnimController>().Play("idle_mound", KAnim.PlayMode.Once, 1f, 0f);
			this.GoTo(base.sm.entombed);
		}

		public void SetCollider(bool original_size)
		{
			KBoxCollider2D component = base.master.GetComponent<KBoxCollider2D>();
			if (original_size)
			{
				component.size = this.originalColliderSize;
				component.offset = this.originalColliderOffset;
				return;
			}
			component.size = base.def.moundColliderSize;
			component.offset = base.def.moundColliderOffset;
		}

		private Vector2 originalColliderSize;

		private Vector2 originalColliderOffset;
	}
}
