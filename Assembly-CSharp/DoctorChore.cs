using System;
using Klei.AI;
using UnityEngine;

public class DoctorChore : Chore<DoctorChore.StatesInstance>
{
	public DoctorChore(IStateMachineTarget target, GameObject patient)
		: base(Db.Get().ChoreTypes.Doctor, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true)
	{
		this.smi = new DoctorChore.StatesInstance(this);
		this.smi.sm.patient.Set(patient, this.smi);
		base.AddPrecondition(ChorePreconditions.IsChattable, target);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.doctor.Set(context.consumer.gameObject, this.smi);
		base.Begin(context);
	}

	public class StatesInstance : GameStateMachine<DoctorChore.States, DoctorChore.StatesInstance, DoctorChore>.GameInstance
	{
		public StatesInstance(DoctorChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<DoctorChore.States, DoctorChore.StatesInstance, DoctorChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approachpatient;
			this.approachpatient.InitializeStates(this.doctor, this.patient, this.heal, null, null, null);
			this.heal.Target(this.doctor).PlayAnim("dig_up_pst", KAnim.PlayMode.Loop, null).ScheduleGoTo(2.5f, this.success);
			this.success.Enter("HealEffect", delegate(DoctorChore.StatesInstance smi)
			{
				Transform transform = this.patient.Get(smi).transform;
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, "Cured", transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
				Diseases diseases = this.patient.Get(smi).GetDiseases();
				foreach (DiseaseInstance diseaseInstance in diseases)
				{
					diseaseInstance.Cure();
				}
			}).ReturnSuccess();
		}

		public StateMachine<DoctorChore.States, DoctorChore.StatesInstance, DoctorChore>.TargetParameter patient;

		public StateMachine<DoctorChore.States, DoctorChore.StatesInstance, DoctorChore>.TargetParameter doctor;

		public GameStateMachine<DoctorChore.States, DoctorChore.StatesInstance, DoctorChore>.ApproachSubState<Chattable> approachpatient;

		public GameStateMachine<DoctorChore.States, DoctorChore.StatesInstance, DoctorChore>.State heal;

		public GameStateMachine<DoctorChore.States, DoctorChore.StatesInstance, DoctorChore>.State success;
	}
}
