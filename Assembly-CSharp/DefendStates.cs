using System;
using STRINGS;
using UnityEngine;

public class DefendStates : GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.protectEntity.moveToThreat;
		GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State state = this.root.Enter("SetTarget", delegate(DefendStates.Instance smi)
		{
			this.target.Set(smi.GetSMI<ThreatMonitor.Instance>().MainThreat, smi, false);
		});
		string text = CREATURES.STATUSITEMS.ATTACKINGENTITY.NAME;
		string text2 = CREATURES.STATUSITEMS.ATTACKINGENTITY.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, main);
		this.protectEntity.moveToThreat.InitializeStates(this.masterTarget, this.target, this.protectEntity.attackThreat, null, CrabTuning.DEFEND_OFFSETS, null);
		this.protectEntity.attackThreat.OnTargetLost(this.target, this.behaviourcomplete).DefaultState(this.protectEntity.attackThreat.pre).Face(this.target, 0f);
		this.protectEntity.attackThreat.pre.PlayAnim((DefendStates.Instance smi) => smi.def.preAnim, KAnim.PlayMode.Once).OnAnimQueueComplete(this.protectEntity.attackThreat.loop);
		this.protectEntity.attackThreat.loop.PlayAnim((DefendStates.Instance smi) => smi.def.attackAnim, KAnim.PlayMode.Once).OnAnimQueueComplete(this.protectEntity.attackThreat.pst);
		this.protectEntity.attackThreat.pst.Enter(new StateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State.Callback(DefendStates.UseWeapon)).PlayAnim((DefendStates.Instance smi) => smi.def.pstAnim, KAnim.PlayMode.Once).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Defend, false);
	}

	private static void UseWeapon(DefendStates.Instance smi)
	{
		smi.GetComponent<Weapon>().AttackTarget(smi.sm.target.Get(smi));
		Action<GameObject, GameObject> specialAttackAction = smi.def.specialAttackAction;
		if (specialAttackAction == null)
		{
			return;
		}
		specialAttackAction(smi.gameObject, smi.sm.target.Get(smi));
	}

	public StateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.TargetParameter target;

	public DefendStates.ProtectStates protectEntity;

	public GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
		public string preAnim = "slap_pre";

		public string attackAnim = "slap";

		public string pstAnim = "slap_pst";

		public Action<GameObject, GameObject> specialAttackAction;
	}

	public new class Instance : GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.GameInstance
	{
		public Instance(Chore<DefendStates.Instance> chore, DefendStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Defend);
		}

		[MyCmpGet]
		public KBatchedAnimController animcontroller;
	}

	public class ProtectStates : GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State
	{
		public GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.ApproachSubState<AttackableBase> moveToThreat;

		public GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.PreLoopPostState attackThreat;
	}
}
