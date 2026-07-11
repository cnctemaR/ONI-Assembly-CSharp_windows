using System;
using Klei;
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
		SimUtil.DiseaseInfo diseaseInfo;
		float num;
		component.ConsumeAndGetDisease(GameTags.Water, 1f, out diseaseInfo, out num);
		SimUtil.DiseaseInfo diseaseInfo2;
		component.ConsumeAndGetDisease(EspressoMachine.INGREDIENT_TAG, 1f, out diseaseInfo2, out num);
		ImmuneSystemMonitor.Instance smi = worker.GetSMI<ImmuneSystemMonitor.Instance>();
		if (smi != null)
		{
			smi.TryInjectDisease(diseaseInfo.idx, diseaseInfo.count, GameTags.Water, Disease.InfectionVector.Digestion);
			smi.TryInjectDisease(diseaseInfo2.idx, diseaseInfo2.count, EspressoMachine.INGREDIENT_TAG, Disease.InfectionVector.Digestion);
		}
		Effects component2 = worker.GetComponent<Effects>();
		component2.Add("TookABreak", true);
		if (!string.IsNullOrEmpty(this.specificEffect))
		{
			component2.Add(this.specificEffect, true);
		}
		if (!string.IsNullOrEmpty(this.trackingEffect))
		{
			component2.Add(this.trackingEffect, true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		this.operational.SetActive(false, false);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.trackingEffect) && component.HasEffect(this.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.specificEffect) && component.HasEffect(this.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	[MyCmpReq]
	private Operational operational;

	public int basePriority = RELAXATION.PRIORITY.TIER5;

	public string specificEffect = "Espresso";

	public string trackingEffect = "RecentlyEspresso";
}
