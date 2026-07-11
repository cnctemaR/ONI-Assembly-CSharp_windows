using System;
using STRINGS;

public class AstronautTrainingCenter : Workable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.chore = this.CreateChore();
	}

	private Chore CreateChore()
	{
		return new WorkChore<AstronautTrainingCenter>(Db.Get().ChoreTypes.Train, this, null, true, null, null, null, false, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		base.GetComponent<Operational>().SetActive(true, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		worker == null;
		return true;
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
		base.worker == null;
		return 0f;
	}

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

	public float daysToMasterRole;

	private Chore chore;

	public Chore.Precondition IsNotMarkedForDeconstruction;
}
