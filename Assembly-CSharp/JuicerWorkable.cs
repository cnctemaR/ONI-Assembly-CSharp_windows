using System;
using Klei;
using Klei.AI;
using TUNING;

public class JuicerWorkable : Workable, IWorkerPrioritizable
{
	private JuicerWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_juicer_kanim") };
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = false;
		base.SetWorkTime(30f);
		this.juicer = base.GetComponent<Juicer>();
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
		component.ConsumeAndGetDisease(GameTags.Water, this.juicer.waterMassPerUse, out diseaseInfo, out num);
		GermExposureMonitor.Instance smi = worker.GetSMI<GermExposureMonitor.Instance>();
		foreach (Tag tag in this.juicer.ingredient_tags)
		{
			SimUtil.DiseaseInfo diseaseInfo2;
			component.ConsumeAndGetDisease(tag, this.juicer.ingredientMassPerUse, out diseaseInfo2, out num);
			if (smi != null)
			{
				smi.TryInjectDisease(diseaseInfo2.idx, diseaseInfo2.count, tag, Sickness.InfectionVector.Digestion);
			}
		}
		if (smi != null)
		{
			smi.TryInjectDisease(diseaseInfo.idx, diseaseInfo.count, GameTags.Water, Sickness.InfectionVector.Digestion);
		}
		Effects component2 = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.juicer.specificEffect))
		{
			component2.Add(this.juicer.specificEffect, true);
		}
		if (!string.IsNullOrEmpty(this.juicer.trackingEffect))
		{
			component2.Add(this.juicer.trackingEffect, true);
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
		if (!string.IsNullOrEmpty(this.juicer.trackingEffect) && component.HasEffect(this.juicer.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.juicer.specificEffect) && component.HasEffect(this.juicer.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private Juicer juicer;
}
