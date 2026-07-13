using System;
using UnityEngine;

public class PokeMonitor : StateMachineComponent<PokeMonitor.Instance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	private static void ClearTarget(PokeMonitor.Instance smi)
	{
		smi.AbortPoke();
	}

	public class States : GameStateMachine<PokeMonitor.States, PokeMonitor.Instance, PokeMonitor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.Never;
			default_state = this.noTarget;
			this.noTarget.ParamTransition<GameObject>(this.target, this.hasTarget, GameStateMachine<PokeMonitor.States, PokeMonitor.Instance, PokeMonitor, object>.IsNotNull);
			this.hasTarget.ParamTransition<GameObject>(this.target, this.noTarget, GameStateMachine<PokeMonitor.States, PokeMonitor.Instance, PokeMonitor, object>.IsNull).ToggleBehaviour(GameTags.Creatures.UrgeToPoke, (PokeMonitor.Instance smi) => true, new Action<PokeMonitor.Instance>(PokeMonitor.ClearTarget));
		}

		public StateMachine<PokeMonitor.States, PokeMonitor.Instance, PokeMonitor, object>.TargetParameter target;

		public GameStateMachine<PokeMonitor.States, PokeMonitor.Instance, PokeMonitor, object>.State noTarget;

		public GameStateMachine<PokeMonitor.States, PokeMonitor.Instance, PokeMonitor, object>.State hasTarget;
	}

	public class Instance : GameStateMachine<PokeMonitor.States, PokeMonitor.Instance, PokeMonitor, object>.GameInstance
	{
		public GameObject Target
		{
			get
			{
				return base.sm.target.Get(this);
			}
		}

		public Instance(PokeMonitor master)
			: base(master)
		{
		}

		public void InitiatePoke(GameObject target)
		{
			this.InitiatePoke(target, new CellOffset[]
			{
				new CellOffset(0, 0)
			});
		}

		public void InitiatePoke(GameObject target, CellOffset[] pokeOffesets)
		{
			base.sm.target.Set(target, this, false);
			this.TargetOffsets = pokeOffesets;
		}

		public void AbortPoke()
		{
			base.sm.target.Set(null, this);
			this.TargetOffsets = new CellOffset[]
			{
				new CellOffset(0, 0)
			};
		}

		public CellOffset[] TargetOffsets = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
	}
}
