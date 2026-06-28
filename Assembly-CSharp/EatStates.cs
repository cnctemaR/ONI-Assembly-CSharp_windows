using System;
using STRINGS;

internal class EatStates : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.goingtoeat;
		this.root.Enter("SetTarget", delegate(EatStates.Instance smi)
		{
			this.target.Set(smi.GetSMI<SolidConsumerMonitor.Instance>().targetEdible, smi);
		});
		GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State state = this.goingtoeat.InitializeStates(this.masterTarget, this.target, this.eating, null, Grid.DefaultOffset, null);
		string text = CREATURES.STATUSITEMS.LOOKINGFORFOOD.NAME;
		string text2 = CREATURES.STATUSITEMS.LOOKINGFORFOOD.TOOLTIP;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State state2 = this.eating.DefaultState(this.eating.pre);
		text2 = CREATURES.STATUSITEMS.EATING.NAME;
		text = CREATURES.STATUSITEMS.EATING.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		this.eating.pre.QueueAnim("eat_pre", false, null).OnAnimQueueComplete(this.eating.loop);
		this.eating.loop.Enter("EatingComplete", delegate(EatStates.Instance smi)
		{
			smi.Trigger(1386391852, this.target.Get<KPrefabID>(smi));
		}).QueueAnim("eat_loop", true, null).ScheduleGoTo(3f, this.eating.pst);
		this.eating.pst.QueueAnim("eat_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToEat, false);
	}

	public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.ApproachSubState<Pickupable> goingtoeat;

	public EatStates.EatingState eating;

	public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State behaviourcomplete;

	public StateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.TargetParameter target;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.GameInstance
	{
		public Instance(Chore<EatStates.Instance> chore, EatStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
		}

		public Element GetLatestMealElement()
		{
			PrimaryElement primaryElement = base.sm.target.Get<PrimaryElement>(this);
			if (primaryElement != null)
			{
				return primaryElement.Element;
			}
			return null;
		}
	}

	public class EatingState : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State
	{
		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State pre;

		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State loop;

		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State pst;
	}
}
