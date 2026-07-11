using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/BeachChairWorkable")]
public class BeachChairWorkable : Workable, IWorkerPrioritizable
{
	private BeachChairWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_beach_chair_kanim") };
		this.workAnims = null;
		this.workingPstComplete = null;
		this.workingPstFailed = null;
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = false;
		this.lightEfficiencyBonus = false;
		base.SetWorkTime(150f);
		this.beachChair = base.GetComponent<BeachChair>();
	}

	protected override void OnStartWork(Worker worker)
	{
		this.timeLit = 0f;
		this.beachChair.SetWorker(worker);
		this.operational.SetActive(true, false);
		worker.GetComponent<Effects>().Add("BeachChairRelaxing", false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		int num = Grid.PosToCell(base.gameObject);
		bool flag = (float)Grid.LightIntensity[num] >= 9999f;
		this.beachChair.SetLit(flag);
		if (flag)
		{
			base.GetComponent<LoopingSounds>().SetParameter(this.soundPath, this.BEACH_CHAIR_LIT_PARAMETER, 1f);
			this.timeLit += dt;
		}
		else
		{
			base.GetComponent<LoopingSounds>().SetParameter(this.soundPath, this.BEACH_CHAIR_LIT_PARAMETER, 0f);
		}
		return false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (this.timeLit / this.workTime >= 0.75f)
		{
			component.Add(this.beachChair.specificEffectLit, true);
			component.Remove(this.beachChair.specificEffectUnlit);
		}
		else
		{
			component.Add(this.beachChair.specificEffectUnlit, true);
			component.Remove(this.beachChair.specificEffectLit);
		}
		component.Add(this.beachChair.trackingEffect, true);
	}

	protected override void OnStopWork(Worker worker)
	{
		this.operational.SetActive(false, false);
		worker.GetComponent<Effects>().Remove("BeachChairRelaxing");
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (component.HasEffect(this.beachChair.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (component.HasEffect(this.beachChair.specificEffectLit) || component.HasEffect(this.beachChair.specificEffectUnlit))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	[MyCmpReq]
	private Operational operational;

	private float timeLit;

	public string soundPath = GlobalAssets.GetSound("BeachChair_music_lp", false);

	public HashedString BEACH_CHAIR_LIT_PARAMETER = "beachChair_lit";

	public int basePriority;

	private BeachChair beachChair;
}
