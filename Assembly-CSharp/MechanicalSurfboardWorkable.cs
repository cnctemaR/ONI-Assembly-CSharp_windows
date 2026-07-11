using System;
using Klei;
using Klei.AI;
using TUNING;
using UnityEngine;

public class MechanicalSurfboardWorkable : Workable, IWorkerPrioritizable
{
	private MechanicalSurfboardWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = true;
		base.SetWorkTime(30f);
		this.surfboard = base.GetComponent<MechanicalSurfboard>();
	}

	protected override void OnStartWork(Worker worker)
	{
		this.operational.SetActive(true, false);
		worker.GetComponent<Effects>().Add("MechanicalSurfing", false);
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo animInfo = default(Workable.AnimInfo);
		Attributes attributes = worker.GetAttributes();
		AttributeInstance attributeInstance = attributes.Get(Db.Get().Attributes.Athletics);
		if (attributeInstance.GetTotalValue() <= 7f)
		{
			animInfo.overrideAnims = new KAnimFile[] { Assets.GetAnim(this.surfboard.interactAnims[0]) };
		}
		else if (attributeInstance.GetTotalValue() <= 15f)
		{
			animInfo.overrideAnims = new KAnimFile[] { Assets.GetAnim(this.surfboard.interactAnims[1]) };
		}
		else
		{
			animInfo.overrideAnims = new KAnimFile[] { Assets.GetAnim(this.surfboard.interactAnims[2]) };
		}
		return animInfo;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		Building component = base.GetComponent<Building>();
		MechanicalSurfboard component2 = base.GetComponent<MechanicalSurfboard>();
		int widthInCells = component.Def.WidthInCells;
		int num = -(widthInCells - 1) / 2;
		int num2 = widthInCells / 2;
		int num3 = global::UnityEngine.Random.Range(num, num2);
		float num4 = component2.waterSpillRateKG * dt;
		SimUtil.DiseaseInfo diseaseInfo;
		float num5;
		base.GetComponent<Storage>().ConsumeAndGetDisease(SimHashes.Water.CreateTag(), num4, out diseaseInfo, out num5);
		int num6 = Grid.OffsetCell(Grid.PosToCell(base.gameObject), new CellOffset(num3, 0));
		int elementIndex = ElementLoader.GetElementIndex(SimHashes.Water);
		FallingWater.instance.AddParticle(num6, (byte)elementIndex, num4, num5, diseaseInfo.idx, diseaseInfo.count, true, false, false, false);
		return false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.surfboard.specificEffect))
		{
			component.Add(this.surfboard.specificEffect, true);
		}
		if (!string.IsNullOrEmpty(this.surfboard.trackingEffect))
		{
			component.Add(this.surfboard.trackingEffect, true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		this.operational.SetActive(false, false);
		worker.GetComponent<Effects>().Remove("MechanicalSurfing");
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.surfboard.trackingEffect) && component.HasEffect(this.surfboard.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.surfboard.specificEffect) && component.HasEffect(this.surfboard.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private MechanicalSurfboard surfboard;
}
