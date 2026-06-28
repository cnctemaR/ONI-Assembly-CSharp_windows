using System;
using STRINGS;

internal class AttackStates : GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.approach;
		this.root.Enter("SetTarget", delegate(AttackStates.Instance smi)
		{
			this.target.Set(smi.GetSMI<ThreatMonitor.Instance>().GetMainThreat, smi);
		});
		GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State state = this.approach.InitializeStates(this.masterTarget, this.target, this.attack, null, new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1)
		}, null);
		string text = CREATURES.STATUSITEMS.ATTACK_APPROACH.NAME;
		string text2 = CREATURES.STATUSITEMS.ATTACK_APPROACH.TOOLTIP;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, statusItemCategory);
		GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State state2 = this.attack.Enter(delegate(AttackStates.Instance smi)
		{
			smi.Play("eat_pre", KAnim.PlayMode.Once);
			smi.Queue("eat_pst", KAnim.PlayMode.Once);
			smi.Schedule(0.5f, delegate
			{
				smi.GetComponent<Weapon>().AttackTarget(this.target.Get(smi));
			}, null);
		});
		text2 = CREATURES.STATUSITEMS.ATTACK.NAME;
		text = CREATURES.STATUSITEMS.ATTACK.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, statusItemCategory).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Attack, false);
	}

	public StateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.TargetParameter target;

	public GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.ApproachSubState<AttackableBase> approach;

	public GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State attack;

	public GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.GameInstance
	{
		public Instance(Chore<AttackStates.Instance> chore, AttackStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Attack);
		}
	}
}
