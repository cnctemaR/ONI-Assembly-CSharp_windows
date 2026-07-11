using System;

internal class DiggerStates : GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>
{
	private static int MoveToNewCell(DiggerStates.Instance smi)
	{
		int num = Grid.OffsetCell(Grid.PosToCell(smi.master.gameObject), 0, smi.def.depthToDig);
		if (Grid.IsValidCell(num))
		{
			return num;
		}
		return Grid.PosToCell(smi.master.gameObject);
	}

	private static float GetHideDuration()
	{
		if (SaveGame.Instance != null && SaveGame.Instance.GetComponent<SeasonManager>() != null)
		{
			return SaveGame.Instance.GetComponent<SeasonManager>().GetBombardmentDuration();
		}
		return 0f;
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.move;
		this.move.MoveTo(new Func<DiggerStates.Instance, int>(DiggerStates.MoveToNewCell), this.hide, this.behaviourcomplete, false);
		this.hide.ScheduleGoTo(DiggerStates.GetHideDuration(), this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Tunnel, false);
	}

	public GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.State surface;

	public GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.State move;

	public GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.State hide;

	public GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
		public Def(int depth)
		{
			this.depthToDig = depth;
		}

		public int depthToDig { get; private set; }
	}

	public new class Instance : GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.GameInstance
	{
		public Instance(Chore<DiggerStates.Instance> chore, DiggerStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Tunnel);
		}
	}
}
