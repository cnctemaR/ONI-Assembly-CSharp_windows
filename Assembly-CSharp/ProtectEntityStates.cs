using System;
using STRINGS;

public class ProtectEntityStates : GameStateMachine<ProtectEntityStates, ProtectEntityStates.Instance, IStateMachineTarget, ProtectEntityStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.protectEntity.moveToThreat;
		this.root.Enter("SetTarget", delegate(ProtectEntityStates.Instance smi)
		{
			this.target.Set(smi.GetSMI<EntityThreatMonitor.Instance>().MainThreat, smi);
		}).ToggleStatusItem(CREATURES.STATUSITEMS.PROTECTINGENTITY.NAME, CREATURES.STATUSITEMS.PROTECTINGENTITY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.protectEntity.DoNothing();
		this.protectEntity.moveToThreat.InitializeStates(this.masterTarget, this.target, this.protectEntity.attackThreat, null, new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1)
		}, null);
		this.protectEntity.attackThreat.Enter(delegate(ProtectEntityStates.Instance smi)
		{
			smi.Play("slap_pre", KAnim.PlayMode.Once);
			smi.Queue("slap", KAnim.PlayMode.Once);
			smi.Queue("slap_pst", KAnim.PlayMode.Once);
			smi.Schedule(0.5f, delegate
			{
				smi.GetComponent<Weapon>().AttackTarget(this.target.Get(smi));
			}, null);
		}).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Defend, false);
	}

	public StateMachine<ProtectEntityStates, ProtectEntityStates.Instance, IStateMachineTarget, ProtectEntityStates.Def>.TargetParameter target;

	public ProtectEntityStates.ProtectStates protectEntity;

	public GameStateMachine<ProtectEntityStates, ProtectEntityStates.Instance, IStateMachineTarget, ProtectEntityStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<ProtectEntityStates, ProtectEntityStates.Instance, IStateMachineTarget, ProtectEntityStates.Def>.GameInstance
	{
		public Instance(Chore<ProtectEntityStates.Instance> chore, ProtectEntityStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Defend);
		}
	}

	public class ProtectStates : GameStateMachine<ProtectEntityStates, ProtectEntityStates.Instance, IStateMachineTarget, ProtectEntityStates.Def>.State
	{
		public GameStateMachine<ProtectEntityStates, ProtectEntityStates.Instance, IStateMachineTarget, ProtectEntityStates.Def>.ApproachSubState<AttackableBase> moveToThreat;

		public GameStateMachine<ProtectEntityStates, ProtectEntityStates.Instance, IStateMachineTarget, ProtectEntityStates.Def>.State attackThreat;
	}
}
