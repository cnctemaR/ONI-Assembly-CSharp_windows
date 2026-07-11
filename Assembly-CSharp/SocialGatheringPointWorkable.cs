using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class SocialGatheringPointWorkable : Workable, IWorkerPrioritizable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_generic_convo_kanim") };
		this.workAnims = new HashedString[] { "idle" };
		this.faceTargetWhenWorking = true;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Socializing;
		this.synchronizeAnims = false;
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
	}

	public override Vector3 GetFacingTarget()
	{
		if (this.lastTalker != null)
		{
			return this.lastTalker.transform.GetPosition();
		}
		return base.GetFacingTarget();
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.AddTag(GameTags.AlwaysConverse);
		worker.Subscribe(-594200555, new Action<object>(this.OnStartedTalking));
		worker.Subscribe(25860745, new Action<object>(this.OnStoppedTalking));
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.RemoveTag(GameTags.AlwaysConverse);
		worker.Unsubscribe(-594200555, new Action<object>(this.OnStartedTalking));
		worker.Unsubscribe(25860745, new Action<object>(this.OnStoppedTalking));
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.specificEffect))
		{
			component.Add(this.specificEffect, true);
		}
	}

	private void OnStartedTalking(object data)
	{
		ConversationManager.StartedTalkingEvent startedTalkingEvent = (ConversationManager.StartedTalkingEvent)data;
		GameObject talker = startedTalkingEvent.talker;
		if (talker == base.worker.gameObject)
		{
			KBatchedAnimController component = base.worker.GetComponent<KBatchedAnimController>();
			string text = startedTalkingEvent.anim;
			text += global::UnityEngine.Random.Range(1, 9).ToString();
			component.Play(text, KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("idle", KAnim.PlayMode.Loop, 1f, 0f);
		}
		else
		{
			Facing component2 = base.worker.GetComponent<Facing>();
			component2.Face(talker.transform.GetPosition());
			this.lastTalker = talker;
		}
	}

	private void OnStoppedTalking(object data)
	{
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

	private GameObject lastTalker;

	public int basePriority;

	public string specificEffect;
}
