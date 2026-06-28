using System;

public class RationalAi : GameStateMachine<RationalAi, RationalAi.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.alive;
		base.serializable = true;
		this.root.ToggleStateMachine((RationalAi.Instance smi) => new DeathMonitor.Instance(smi.master));
		this.alive.EventTransition(GameHashes.Died, this.dead, (RationalAi.Instance smi) => smi.IsDead()).ToggleStateMachine((RationalAi.Instance smi) => new ThoughtGraph.Instance(smi.master)).ToggleStateMachine((RationalAi.Instance smi) => new StaminaMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new StressMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new EmoteMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new SneezeMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new IdleMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new RationMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new CalorieMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new DoctorMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new ToxicantMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new DiseaseMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new BreathMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new TemperatureMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new ExternalTemperatureMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new SuffocationMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new BladderMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new SteppedInMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new RedAlertMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new SafeCellMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new CringeMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new HygieneMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new FallMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new ThreatMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new WoundMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new IncapacitationMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new SuitRegionMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new TiredMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new MoveToLocationMonitor.Instance(smi.master))
			.ToggleStateMachine((RationalAi.Instance smi) => new ReactionMonitor.Instance(smi.master));
		this.dead.ToggleStateMachine((RationalAi.Instance smi) => new DecompositionMonitor.Instance(smi.master, Db.Get().Diseases.SawCorpsosis, 0.00083333335f, true)).ToggleStateMachine((RationalAi.Instance smi) => new FallWhenDeadMonitor.Instance(smi.master)).ToggleBrain("dead")
			.Enter("RefreshUserMenu", delegate(RationalAi.Instance smi)
			{
				smi.RefreshUserMenu();
			})
			.Enter("DropStorage", delegate(RationalAi.Instance smi)
			{
				smi.GetComponent<Storage>().DropAll();
			});
	}

	public GameStateMachine<RationalAi, RationalAi.Instance, IStateMachineTarget, object>.State alive;

	public GameStateMachine<RationalAi, RationalAi.Instance, IStateMachineTarget, object>.State dead;

	public new class Instance : GameStateMachine<RationalAi, RationalAi.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			ChoreConsumer component = base.GetComponent<ChoreConsumer>();
			component.AddUrge(Db.Get().Urges.EmoteHighPriority);
		}

		public bool IsDead()
		{
			return base.GetComponent<Health>().IsDead();
		}

		public void RefreshUserMenu()
		{
			UserMenu component = base.GetComponent<UserMenu>();
			if (component != null)
			{
				component.Refresh();
			}
		}
	}
}
