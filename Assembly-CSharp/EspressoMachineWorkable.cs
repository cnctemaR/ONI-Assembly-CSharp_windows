using System;
using Klei.AI;
using TUNING;

public class EspressoMachineWorkable : Workable, IGameObjectEffectDescriptor, IWorkerPrioritizable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_espresso_machine_kanim") };
		this.forcePlayPst = true;
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = false;
		base.SetWorkTime(30f);
	}

	protected override void OnStartWork(Worker worker)
	{
		this.operational.SetActive(true, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = base.GetComponent<Storage>();
		component.ConsumeIgnoringDisease(GameTags.Water, 1f);
		component.ConsumeIgnoringDisease(new Tag("SpiceNut"), 1f);
		Effects component2 = worker.GetComponent<Effects>();
		component2.Add("TookABreak", true);
		if (!string.IsNullOrEmpty(this.specificEffect))
		{
			component2.Add(this.specificEffect, true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		this.operational.SetActive(false, false);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		if (!string.IsNullOrEmpty(this.specificEffect))
		{
			Effects component = worker.GetComponent<Effects>();
			if (component.HasEffect(this.specificEffect))
			{
				priority = RELAXATION.PRIORITY.RECENTLY_USED;
			}
		}
		return true;
	}

	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	public string specificEffect;
}
