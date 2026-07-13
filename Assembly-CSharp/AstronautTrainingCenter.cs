using System;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AstronautTrainingCenter")]
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

	protected override void OnStartWork(WorkerBase worker)
	{
		base.OnStartWork(worker);
		base.GetComponent<Operational>().SetActive(true, false);
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
	{
		worker == null;
		return true;
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		base.OnCompleteWork(worker);
		if (this.chore != null && !this.chore.isComplete)
		{
			this.chore.Cancel("completed but not complete??");
		}
		this.chore = this.CreateChore();
	}

	protected override void OnStopWork(WorkerBase worker)
	{
		base.OnStopWork(worker);
		base.GetComponent<Operational>().SetActive(false, false);
	}

	public override float GetPercentComplete()
	{
		base.worker == null;
		return 0f;
	}

	public float daysToMasterRole;

	private Chore chore;

	public static Chore.Precondition IsNotMarkedForDeconstruction = new Chore.Precondition
	{
		id = "IsNotMarkedForDeconstruction",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Deconstructable deconstructable = data as Deconstructable;
			return deconstructable == null || !deconstructable.IsMarkedForDeconstruction();
		}
	};
}
