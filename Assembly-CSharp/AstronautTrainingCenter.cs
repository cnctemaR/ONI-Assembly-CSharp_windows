using System;
using STRINGS;
using UnityEngine;

public class AstronautTrainingCenter : Workable
{
	public AstronautTrainingCenter()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsNotMarkedForDeconstruction";
		precondition.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION;
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Deconstructable deconstructable = data as Deconstructable;
			return deconstructable == null || !deconstructable.IsMarkedForDeconstruction();
		};
		this.IsNotMarkedForDeconstruction = precondition;
		base..ctor();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.chore = this.CreateChore();
	}

	private Chore CreateChore()
	{
		WorkChore<AstronautTrainingCenter> workChore = new WorkChore<AstronautTrainingCenter>(Db.Get().ChoreTypes.Train, this, null, null, true, null, null, null, false, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		workChore.AddPrecondition(ChorePreconditions.instance.IsRole, AstronautTrainee.ID);
		workChore.AddPrecondition(ChorePreconditions.instance.HasNotMasteredRole, AstronautTrainee.ID);
		workChore.AddPrecondition(ResearchCenter.IsBuildingReady, this);
		return workChore;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		RoleConfig role = Game.Instance.roleManager.GetRole(AstronautTrainee.ID);
		float experienceRequired = role.experienceRequired;
		float num = experienceRequired / (this.daysToMasterRole * 525f);
		resume.AddExperienceIfRole(AstronautTrainee.ID, work_dt * num);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		base.GetComponent<Operational>().SetActive(true, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (worker == null)
		{
			return true;
		}
		MinionResume component = worker.GetComponent<MinionResume>();
		return component.HasMasteredRole(AstronautTrainee.ID);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		if (this.chore != null && !this.chore.isComplete)
		{
			this.chore.Cancel("completed but not complete??");
		}
		this.chore = this.CreateChore();
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		base.GetComponent<Operational>().SetActive(false, false);
	}

	public override float GetPercentComplete()
	{
		if (base.worker == null)
		{
			return 0f;
		}
		MinionResume component = base.worker.GetComponent<MinionResume>();
		float num = component.ExperienceByRoleID[AstronautTrainee.ID];
		RoleConfig role = Game.Instance.roleManager.GetRole(AstronautTrainee.ID);
		return Mathf.Clamp01(num / role.experienceRequired);
	}

	public float daysToMasterRole;

	private Chore chore;

	public Chore.Precondition IsNotMarkedForDeconstruction;
}
