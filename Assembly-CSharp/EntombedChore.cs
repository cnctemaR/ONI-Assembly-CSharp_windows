using System;
using UnityEngine;

public class EntombedChore : Chore<EntombedChore.StatesInstance>
{
	public EntombedChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Entombed, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new EntombedChore.StatesInstance(this, target.gameObject);
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
			int num = Grid.CellAbove(Grid.PosToCell(base.transform.GetPosition()));
			base.sm.isFaceEntombed.Set(Grid.IsValidCell(num) && Grid.Solid[num], base.smi);
		}
	}

	public class States : GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.entombedbody;
			base.Target(this.entombable);
			this.root.ToggleAnims("anim_emotes_default_kanim", 0f).Update("IsFaceEntombed", delegate(EntombedChore.StatesInstance smi, float dt)
			{
				smi.UpdateFaceEntombed();
			}, UpdateRate.SIM_200ms, false).ToggleStatusItem(Db.Get().DuplicantStatusItems.EntombedChore, null);
			this.entombedface.PlayAnim("entombed_ceiling", KAnim.PlayMode.Loop).ParamTransition<bool>(this.isFaceEntombed, this.entombedbody, GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.IsFalse);
			this.entombedbody.PlayAnim("entombed_floor", KAnim.PlayMode.Loop).StopMoving().ParamTransition<bool>(this.isFaceEntombed, this.entombedface, GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.IsTrue);
		}

		public StateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.BoolParameter isFaceEntombed;

		public StateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.TargetParameter entombable;

		public GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.State entombedface;

		public GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.State entombedbody;
	}
}
