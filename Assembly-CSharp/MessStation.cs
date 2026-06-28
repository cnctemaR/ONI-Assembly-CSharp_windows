using System;

public class MessStation : BuildingWorkable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_machine_kanim") };
		Components.MessStations.Add(this);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		worker.workable.GetComponent<Edible>().CompleteWork(worker);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.MessStations.Remove(this);
	}
}
