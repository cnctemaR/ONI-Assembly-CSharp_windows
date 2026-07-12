using System;
using Klei;
using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/SaunaWorkable")]
public class SaunaWorkable : Workable, IWorkerPrioritizable
{
	private SaunaWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_sauna_kanim") };
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = true;
		this.workLayer = Grid.SceneLayer.BuildingUse;
		base.SetWorkTime(30f);
		this.sauna = base.GetComponent<Sauna>();
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.operational.SetActive(true, false);
		worker.GetComponent<Effects>().Add("SaunaRelaxing", false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.sauna.specificEffect))
		{
			component.Add(this.sauna.specificEffect, true);
		}
		if (!string.IsNullOrEmpty(this.sauna.trackingEffect))
		{
			component.Add(this.sauna.trackingEffect, true);
		}
		component.Add("WarmTouch", true).timeRemaining = 1800f;
		this.operational.SetActive(false, false);
	}

	protected override void OnStopWork(Worker worker)
	{
		this.operational.SetActive(false, false);
		worker.GetComponent<Effects>().Remove("SaunaRelaxing");
		Storage component = base.GetComponent<Storage>();
		float num;
		SimUtil.DiseaseInfo diseaseInfo;
		float num2;
		component.ConsumeAndGetDisease(SimHashes.Steam.CreateTag(), this.sauna.steamPerUseKG, out num, out diseaseInfo, out num2);
		component.AddLiquid(SimHashes.Water, this.sauna.steamPerUseKG, this.sauna.waterOutputTemp, diseaseInfo.idx, diseaseInfo.count, true, false);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.sauna.trackingEffect) && component.HasEffect(this.sauna.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.sauna.specificEffect) && component.HasEffect(this.sauna.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private Sauna sauna;
}
