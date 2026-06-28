using System;
using STRINGS;

internal class MoveToLureStates : GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.approach;
		GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>.State state = this.root.Enter("SetLure", delegate(MoveToLureStates.Instance smi)
		{
			this.target.Set(smi.GetSMI<LureableMonitor.Instance>().GetTargetLure(), smi);
		});
		string text = CREATURES.STATUSITEMS.CONSIDERINGLURE.NAME;
		string text2 = CREATURES.STATUSITEMS.CONSIDERINGLURE.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main);
		this.approach.InitializeStates(this.masterTarget, this.target, this.behaviourcomplete, this.behaviourcomplete, new CellOffset[]
		{
			new CellOffset(0, 2)
		}, null);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.MoveToLure, false);
	}

	public GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>.ApproachSubState<IApproachable> approach;

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
