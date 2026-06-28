using System;
using STRINGS;

internal class LayEggStates : GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.LAYINGANEGG.NAME;
		string text2 = CREATURES.STATUSITEMS.LAYINGANEGG.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main);
		this.idle.PlayAnim("idle_loop", KAnim.PlayMode.Loop).ScheduleGoTo(3f, this.layeggpre);
		this.layeggpre.PlayAnim("lay_egg_pre").OnAnimQueueComplete(this.layeggpst);
		this.layeggpst.TriggerOnEnter(GameHashes.LayEgg, null).PlayAnim("lay_egg_pst").OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.QueueAnim("idle_loop", true, null).BehaviourComplete(GameTags.Creatures.Fertile, false);
	}

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State idle;

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State layeggpre;

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State layeggpst;

	public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.GameInstance
	{
		public Instance(Chore<LayEggStates.Instance> chore, LayEggStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Fertile);
		}
	}
}
