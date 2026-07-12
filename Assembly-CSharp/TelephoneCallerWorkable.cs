using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/TelephoneWorkable")]
public class TelephoneCallerWorkable : Workable, IWorkerPrioritizable
{
	private TelephoneCallerWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
		this.workingPstComplete = new HashedString[] { "on_pst" };
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_telephone_kanim") };
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = true;
		base.SetWorkTime(40f);
		this.telephone = base.GetComponent<Telephone>();
	}

	protected override void OnStartWork(Worker worker)
	{
		this.operational.SetActive(true, false);
		this.telephone.isInUse = true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (this.telephone.HasTag(GameTags.LongDistanceCall))
		{
			if (!string.IsNullOrEmpty(this.telephone.longDistanceEffect))
			{
				component.Add(this.telephone.longDistanceEffect, true);
			}
		}
		else if (this.telephone.wasAnswered)
		{
			if (!string.IsNullOrEmpty(this.telephone.chatEffect))
			{
				component.Add(this.telephone.chatEffect, true);
			}
		}
		else if (!string.IsNullOrEmpty(this.telephone.babbleEffect))
		{
			component.Add(this.telephone.babbleEffect, true);
		}
		if (!string.IsNullOrEmpty(this.telephone.trackingEffect))
		{
			component.Add(this.telephone.trackingEffect, true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		this.telephone.HangUp();
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.telephone.trackingEffect) && component.HasEffect(this.telephone.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.telephone.chatEffect) && component.HasEffect(this.telephone.chatEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		if (!string.IsNullOrEmpty(this.telephone.babbleEffect) && component.HasEffect(this.telephone.babbleEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private Telephone telephone;
}
