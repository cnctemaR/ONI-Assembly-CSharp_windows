using System;
using Klei;
using Klei.AI;
using TUNING;

public class SodaFountainWorkable : Workable, IWorkerPrioritizable
{
	private SodaFountainWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_sodamaker_kanim") };
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = false;
		base.SetWorkTime(30f);
		this.sodaFountain = base.GetComponent<SodaFountain>();
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
		component.ConsumeAndGetDisease(GameTags.Water, this.sodaFountain.waterMassPerUse, out diseaseInfo, out num);
		SimUtil.DiseaseInfo diseaseInfo2;
		component.ConsumeAndGetDisease(this.sodaFountain.ingredientTag, this.sodaFountain.ingredientMassPerUse, out diseaseInfo2, out num);
		GermExposureMonitor.Instance smi = worker.GetSMI<GermExposureMonitor.Instance>();
		if (smi != null)
		{
			smi.TryInjectDisease(diseaseInfo.idx, diseaseInfo.count, GameTags.Water, Sickness.InfectionVector.Digestion);
			smi.TryInjectDisease(diseaseInfo2.idx, diseaseInfo2.count, this.sodaFountain.ingredientTag, Sickness.InfectionVector.Digestion);
		}
		Effects component2 = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.sodaFountain.specificEffect))
		{
			component2.Add(this.sodaFountain.specificEffect, true);
		}
		if (!string.IsNullOrEmpty(this.sodaFountain.trackingEffect))
		{
			component2.Add(this.sodaFountain.trackingEffect, true);
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
		if (!string.IsNullOrEmpty(this.sodaFountain.trackingEffect) && component.HasEffect(this.sodaFountain.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.sodaFountain.specificEffect) && component.HasEffect(this.sodaFountain.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private SodaFountain sodaFountain;
}
