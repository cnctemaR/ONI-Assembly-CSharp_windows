using System;
using Klei.AI;

public class PatientMonitor : GameStateMachine<PatientMonitor, PatientMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.satisfied.EventTransition(GameHashes.DiseaseAdded, this.infected, (PatientMonitor.Instance smi) => smi.IsInfected());
		this.infected.DefaultState(this.infected.needdoctor).EventTransition(GameHashes.DiseaseCured, this.satisfied, (PatientMonitor.Instance smi) => !smi.IsInfected());
		this.infected.needdoctor.ToggleChore((PatientMonitor.Instance smi) => new DoctorChore(smi.master, this.masterTarget.Get(smi)), this.infected.recovering, false);
		this.infected.recovering.ScheduleGoTo(600f, this.infected.needdoctor);
	}

	public GameStateMachine<PatientMonitor, PatientMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public PatientMonitor.InfectedState infected;

	public class InfectedState : GameStateMachine<PatientMonitor, PatientMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<PatientMonitor, PatientMonitor.Instance, IStateMachineTarget, object>.State needdoctor;

		public GameStateMachine<PatientMonitor, PatientMonitor.Instance, IStateMachineTarget, object>.State recovering;
	}

	public new class Instance : GameStateMachine<PatientMonitor, PatientMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool IsInfected()
		{
			Diseases diseases = base.master.gameObject.GetDiseases();
			return diseases.IsInfected();
		}
	}
}
