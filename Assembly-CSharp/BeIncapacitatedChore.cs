using System;
using UnityEngine;

public class BeIncapacitatedChore : Chore<BeIncapacitatedChore.StatesInstance>
{
	public BeIncapacitatedChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.BeIncapacitated, master, master.GetComponent<ChoreProvider>(), true, null, null, null, PriorityScreen.PriorityClass.emergency, 5, false, true, 0, null)
	{
		this.smi = new BeIncapacitatedChore.StatesInstance(this);
	}

	public void FindAvailableMedicalBed(Navigator navigator)
	{
		Clinic clinic = null;
		AssignableSlot clinic2 = Db.Get().AssignableSlots.Clinic;
		Ownables soleOwner = this.gameObject.GetComponent<MinionIdentity>().GetSoleOwner();
		AssignableSlotInstance slot = soleOwner.GetSlot(clinic2);
		if (slot.assignable == null)
		{
			Assignable assignable = soleOwner.AutoAssignSlot(clinic2);
			if (assignable != null)
			{
				clinic = assignable.GetComponent<Clinic>();
			}
		}
		else
		{
			clinic = slot.assignable.GetComponent<Clinic>();
		}
		if (clinic != null && navigator.CanReach(clinic))
		{
			this.smi.sm.clinic.Set(clinic.gameObject, this.smi);
			this.smi.GoTo(this.smi.sm.incapacitation_root.rescue.waitingForPickup);
		}
	}

	public GameObject GetChosenClinic()
	{
		return this.smi.sm.clinic.Get(this.smi);
	}

	private static string IncapacitatedDuplicantAnim_pre = "incapacitate_pre";

	private static string IncapacitatedDuplicantAnim_loop = "incapacitate_loop";

	private static string IncapacitatedDuplicantAnim_death = "incapacitate_death";

	private static string IncapacitatedDuplicantAnim_carry = "carry_loop";

	private static string IncapacitatedDuplicantAnim_place = "place";

	public class StatesInstance : GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.GameInstance
	{
		public StatesInstance(BeIncapacitatedChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			this.root.ToggleAnims("anim_incapacitated_kanim", 0f).ToggleStatusItem(Db.Get().DuplicantStatusItems.Incapacitated, (BeIncapacitatedChore.StatesInstance smi) => smi.master.gameObject.GetSMI<IncapacitationMonitor.Instance>()).Enter(delegate(BeIncapacitatedChore.StatesInstance smi)
			{
				smi.SetStatus(StateMachine.Status.Failed);
				smi.GoTo(this.incapacitation_root.lookingForBed);
			});
			this.incapacitation_root.EventHandler(GameHashes.Died, delegate(BeIncapacitatedChore.StatesInstance smi)
			{
				smi.SetStatus(StateMachine.Status.Failed);
				smi.StopSM("died");
			});
			this.incapacitation_root.lookingForBed.Update("LookForAvailableClinic", delegate(BeIncapacitatedChore.StatesInstance smi, float dt)
			{
				smi.master.FindAvailableMedicalBed(smi.master.GetComponent<Navigator>());
			}, UpdateRate.SIM_1000ms, false).Enter("PlayAnim", delegate(BeIncapacitatedChore.StatesInstance smi)
			{
				smi.sm.clinic.Set(null, smi);
				smi.Play(BeIncapacitatedChore.IncapacitatedDuplicantAnim_pre, KAnim.PlayMode.Once);
				smi.Queue(BeIncapacitatedChore.IncapacitatedDuplicantAnim_loop, KAnim.PlayMode.Loop);
			});
			this.incapacitation_root.rescue.ToggleChore((BeIncapacitatedChore.StatesInstance smi) => new RescueIncapacitatedChore(smi.master, this.masterTarget.Get(smi)), this.incapacitation_root.recovering, this.incapacitation_root.lookingForBed);
			this.incapacitation_root.rescue.waitingForPickup.EventTransition(GameHashes.OnStore, this.incapacitation_root.rescue.carried, null).Update("LookForAvailableClinic", delegate(BeIncapacitatedChore.StatesInstance smi, float dt)
			{
				bool flag = false;
				if (smi.sm.clinic.Get(smi) == null)
				{
					flag = true;
				}
				else if (!smi.master.gameObject.GetComponent<Navigator>().CanReach(this.clinic.Get(smi).GetComponent<Clinic>()))
				{
					flag = true;
				}
				else if (!this.clinic.Get(smi).GetComponent<Assignable>().IsAssignedTo(smi.master.GetComponent<IAssignableIdentity>()))
				{
					flag = true;
				}
				if (flag)
				{
					smi.GoTo(this.incapacitation_root.lookingForBed);
				}
			}, UpdateRate.SIM_1000ms, false);
			this.incapacitation_root.rescue.carried.Update("LookForAvailableClinic", delegate(BeIncapacitatedChore.StatesInstance smi, float dt)
			{
				bool flag2 = false;
				if (smi.sm.clinic.Get(smi) == null)
				{
					flag2 = true;
				}
				else if (!this.clinic.Get(smi).GetComponent<Assignable>().IsAssignedTo(smi.master.GetComponent<IAssignableIdentity>()))
				{
					flag2 = true;
				}
				if (flag2)
				{
					smi.GoTo(this.incapacitation_root.lookingForBed);
				}
			}, UpdateRate.SIM_1000ms, false).Enter(delegate(BeIncapacitatedChore.StatesInstance smi)
			{
				smi.Queue(BeIncapacitatedChore.IncapacitatedDuplicantAnim_carry, KAnim.PlayMode.Loop);
			}).Exit(delegate(BeIncapacitatedChore.StatesInstance smi)
			{
				smi.Play(BeIncapacitatedChore.IncapacitatedDuplicantAnim_place, KAnim.PlayMode.Once);
			});
			this.incapacitation_root.death.PlayAnim(BeIncapacitatedChore.IncapacitatedDuplicantAnim_death).Enter(delegate(BeIncapacitatedChore.StatesInstance smi)
			{
				smi.SetStatus(StateMachine.Status.Failed);
				smi.StopSM("died");
			});
			this.incapacitation_root.recovering.ToggleUrge(Db.Get().Urges.HealCritical).Enter(delegate(BeIncapacitatedChore.StatesInstance smi)
			{
				smi.Trigger(-1256572400, null);
				smi.SetStatus(StateMachine.Status.Success);
				smi.StopSM("recovering");
			});
		}

		public BeIncapacitatedChore.States.IncapacitatedStates incapacitation_root;

		public StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.TargetParameter clinic;

		public class IncapacitatedStates : GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State
		{
			public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State lookingForBed;

			public BeIncapacitatedChore.States.BeingRescued rescue;

			public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State death;

			public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State recovering;
		}

		public class BeingRescued : GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State
		{
			public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State waitingForPickup;

			public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State carried;
		}
	}
}
