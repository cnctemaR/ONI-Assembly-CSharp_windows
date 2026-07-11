using System;
using STRINGS;

internal class MoveToLureStates : GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.move;
		GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>.State state = this.root.Enter("SetLure", delegate(MoveToLureStates.Instance smi)
		{
			this.target.Set(smi.GetSMI<LureableMonitor.Instance>().GetTargetLure(), smi);
		});
		string text = CREATURES.STATUSITEMS.CONSIDERINGLURE.NAME;
		string text2 = CREATURES.STATUSITEMS.CONSIDERINGLURE.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
		this.move.MoveTo(new Func<MoveToLureStates.Instance, int>(MoveToLureStates.GetLureCell), new Func<MoveToLureStates.Instance, CellOffset[]>(MoveToLureStates.GetLureOffsets), this.behaviourcomplete, null, false);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.MoveToLure, false);
	}

	private static Lure.Instance GetTargetLure(MoveToLureStates.Instance smi)
	{
		return smi.GetSMI<LureableMonitor.Instance>().GetTargetLure().GetSMI<Lure.Instance>();
	}

	private static int GetLureCell(MoveToLureStates.Instance smi)
	{
		return Grid.PosToCell(MoveToLureStates.GetTargetLure(smi).transform.GetPosition());
	}

	private static CellOffset[] GetLureOffsets(MoveToLureStates.Instance smi)
	{
		return MoveToLureStates.GetTargetLure(smi).def.lurePoints;
	}

	public GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>.State move;

	public GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>.State behaviourcomplete;

	public StateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>.TargetParameter target;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>.GameInstance
	{
		public Instance(Chore<MoveToLureStates.Instance> chore, MoveToLureStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.MoveToLure);
		}
	}
}
