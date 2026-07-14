using System;
using Klei.AI;
using STRINGS;

public class SurfaceAirConsumerStates : GameStateMachine<SurfaceAirConsumerStates, SurfaceAirConsumerStates.Instance, IStateMachineTarget, SurfaceAirConsumerStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.goingToSurface;
		GameStateMachine<SurfaceAirConsumerStates, SurfaceAirConsumerStates.Instance, IStateMachineTarget, SurfaceAirConsumerStates.Def>.State state = this.goingToSurface.MoveTo((SurfaceAirConsumerStates.Instance smi) => smi.monitor.targetCell, this.consuming, this.behaviourComplete, false);
		string text = CREATURES.STATUSITEMS.SURFACE_AIR_CONSUMER_MOVING.NAME;
		string text2 = CREATURES.STATUSITEMS.SURFACE_AIR_CONSUMER_MOVING.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, statusItemCategory);
		GameStateMachine<SurfaceAirConsumerStates, SurfaceAirConsumerStates.Instance, IStateMachineTarget, SurfaceAirConsumerStates.Def>.State state2 = this.consuming.PlayAnim("breathe_pre").QueueAnim("breathe_loop", true, null);
		string text4 = CREATURES.STATUSITEMS.SURFACE_AIR_CONSUMER_CONSUMING.NAME;
		string text5 = CREATURES.STATUSITEMS.SURFACE_AIR_CONSUMER_CONSUMING.TOOLTIP;
		string text6 = "";
		StatusItem.IconType iconType2 = StatusItem.IconType.Info;
		NotificationType notificationType2 = NotificationType.Neutral;
		bool flag2 = false;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text4, text5, text6, iconType2, notificationType2, flag2, default(HashedString), 129022, null, null, statusItemCategory).Update("ConsumeOxygen", delegate(SurfaceAirConsumerStates.Instance smi, float dt)
		{
			smi.ConsumeOxygen(dt);
		}, UpdateRate.SIM_1000ms, false).ScheduleGoTo((SurfaceAirConsumerStates.Instance smi) => smi.def.consumeDuration, this.consuming_pst);
		this.consuming_pst.QueueAnim("breathe_pst", false, null).OnAnimQueueComplete(this.behaviourComplete);
		this.behaviourComplete.Enter(delegate(SurfaceAirConsumerStates.Instance smi)
		{
			smi.ApplyEffect();
		}).BehaviourComplete(GameTags.Creatures.WantsToConsumeAir, false);
	}

	public GameStateMachine<SurfaceAirConsumerStates, SurfaceAirConsumerStates.Instance, IStateMachineTarget, SurfaceAirConsumerStates.Def>.State goingToSurface;

	public GameStateMachine<SurfaceAirConsumerStates, SurfaceAirConsumerStates.Instance, IStateMachineTarget, SurfaceAirConsumerStates.Def>.State consuming;

	public GameStateMachine<SurfaceAirConsumerStates, SurfaceAirConsumerStates.Instance, IStateMachineTarget, SurfaceAirConsumerStates.Def>.State consuming_pst;

	public GameStateMachine<SurfaceAirConsumerStates, SurfaceAirConsumerStates.Instance, IStateMachineTarget, SurfaceAirConsumerStates.Def>.State behaviourComplete;

	public class Def : StateMachine.BaseDef
	{
		public string effectId;

		public float consumptionRate;

		public float consumeDuration;
	}

	public new class Instance : GameStateMachine<SurfaceAirConsumerStates, SurfaceAirConsumerStates.Instance, IStateMachineTarget, SurfaceAirConsumerStates.Def>.GameInstance
	{
		public Instance(Chore<SurfaceAirConsumerStates.Instance> chore, SurfaceAirConsumerStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToConsumeAir);
		}

		public void ConsumeOxygen(float dt)
		{
			int num = Grid.CellAbove(Grid.PosToCell(this));
			if (!Grid.IsValidCell(num))
			{
				return;
			}
			SimHashes element = this.monitor.def.element;
			SimMessages.ConsumeMass(num, element, base.def.consumptionRate * dt, 3, -1);
		}

		public void ApplyEffect()
		{
			Effects component = base.GetComponent<Effects>();
			if (component != null && !string.IsNullOrEmpty(base.def.effectId))
			{
				component.Add(base.def.effectId, true);
			}
		}

		[MySmiGet]
		public SurfaceAirConsumerMonitor.Instance monitor;
	}
}
