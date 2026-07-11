using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class PhonoboxWorkable : Workable, IWorkerPrioritizable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = false;
		this.forcePlayPst = true;
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		base.SetWorkTime(15f);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		component.Add("TookABreak", true);
		if (!string.IsNullOrEmpty(this.specificEffect))
		{
			component.Add(this.specificEffect, true);
		}
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

	protected override void OnStartWork(Worker worker)
	{
		this.owner.AddWorker(worker);
	}

	protected override void OnStopWork(Worker worker)
	{
		this.owner.RemoveWorker(worker);
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		int num = global::UnityEngine.Random.Range(0, this.workerOverrideAnims.Length);
		this.overrideAnims = this.workerOverrideAnims[num];
		return base.GetAnim(worker);
	}

	public ISharedWorkable owner;

	public int basePriority;

	public string specificEffect;

	public KAnimFile[][] workerOverrideAnims = new KAnimFile[][]
	{
		new KAnimFile[] { Assets.GetAnim("anim_interacts_phonobox_danceone_kanim") },
		new KAnimFile[] { Assets.GetAnim("anim_interacts_phonobox_dancetwo_kanim") },
		new KAnimFile[] { Assets.GetAnim("anim_interacts_phonobox_dancethree_kanim") }
	};
}
