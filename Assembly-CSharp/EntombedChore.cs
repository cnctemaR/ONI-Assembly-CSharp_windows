using System;
using UnityEngine;

public class EntombedChore : Chore<EntombedChore.StatesInstance>
{
	public EntombedChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Entombed, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		this.smi = new EntombedChore.StatesInstance(this, target.gameObject);
	}

	public class StatesInstance : GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.GameInstance
	{
		public StatesInstance(EntombedChore master, GameObject entombable)
			: base(master)
		{
			base.sm.entombable.Set(entombable, base.smi);
		}

		public void UpdateFaceEntombed()
		{
			int num = Grid.PosToCell(base.transform.position);
			int num2 = Grid.CellAbove(num);
			base.sm.isFaceEntombed.Set(Grid.Solid[num2], base.smi);
		}
	}

	public class States : GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.entombedbody;
			base.Target(this.entombable);
			this.root.Update("IsFaceEntombed", delegate(EntombedChore.StatesInstance smi)
			{
				smi.UpdateFaceEntombed();
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.EntombedChore, null);
			this.entombedface.PlayAnim("entombed_ceiling", KAnim.PlayMode.Loop).ParamTransition<bool>(this.isFaceEntombed, this.entombedbody, (EntombedChore.StatesInstance smi, bool p) => !p);
			this.entombedbody.PlayAnim("entombed_floor", KAnim.PlayMode.Loop).StopMoving().ParamTransition<bool>(this.isFaceEntombed, this.entombedface, (EntombedChore.StatesInstance smi, bool p) => p);
		}

		public StateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.BoolParameter isFaceEntombed;

		public StateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.TargetParameter entombable;

		public GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.State entombedface;

		public GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.State entombedbody;
	}
}
