using System;
using STRINGS;

internal class InhaleStates : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inhaling;
		GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State state = this.inhaling.DefaultState(this.inhaling.pre);
		string text = CREATURES.STATUSITEMS.INHALING.NAME;
		string text2 = CREATURES.STATUSITEMS.INHALING.TOOLTIP;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		this.inhaling.pre.PlayAnim("inhale_pre").QueueAnim("inhale_loop", true, null).Update("Consume", delegate(InhaleStates.Instance smi, float dt)
		{
			smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().Consume(dt);
		}, UpdateRate.SIM_200ms, false)
			.EventTransition(GameHashes.ElementNoLongerAvailable, this.behaviourcomplete, null)
			.TagTransition(GameTags.Creatures.Hungry, this.inhaling.full, true)
			.Enter("StartInhaleSound", delegate(InhaleStates.Instance smi)
			{
				smi.StartInhaleSound();
			})
			.Exit("StopInhaleSound", delegate(InhaleStates.Instance smi)
			{
				smi.StopInhaleSound();
			})
			.ScheduleGoTo((InhaleStates.Instance smi) => smi.def.maximumInhaleTime, this.inhaling.full);
		this.inhaling.full.QueueAnim("inhale_pst", false, null).QueueAnim("idle_loop_full", true, null).ScheduleGoTo(3f, this.exhaling);
		GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State state2 = this.exhaling;
		text2 = CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME;
		text = CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory).DefaultState(this.exhaling.pre);
		this.exhaling.pre.PlayAnim("poop").OnAnimQueueComplete(this.exhaling.poop);
		this.exhaling.poop.Enter("Poop", delegate(InhaleStates.Instance smi)
		{
			smi.GetSMI<CreatureCalorieMonitor.Instance>().Poop();
		}).GoTo(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToEat, false);
	}

	public InhaleStates.InhalingStates inhaling;

	public InhaleStates.ExhalingStates exhaling;

	public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
		public string inhaleSound;

		public float maximumInhaleTime = 2.5f;
	}

	public new class Instance : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.GameInstance
	{
		public Instance(Chore<InhaleStates.Instance> chore, InhaleStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
			this.inhaleSound = GlobalAssets.GetSound(def.inhaleSound, false);
		}

		public void StartInhaleSound()
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.AddLoopingSoundUpdater();
				component.StartSound(base.smi.inhaleSound, base.transform.GetPosition());
			}
		}

		public void StopInhaleSound()
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StopSound(base.smi.inhaleSound);
				component.RemoveLoopingSoundUpdater();
			}
		}

		public string inhaleSound;
	}

	public class InhalingStates : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State
	{
		public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State pre;

		public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State full;
	}

	public class ExhalingStates : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State
	{
		public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State pre;

		public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State poop;
	}
}
