using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class PartyPointWorkable : Workable, IWorkerPrioritizable
{
	private PartyPointWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_generic_convo_kanim") };
		this.workAnimPlayMode = KAnim.PlayMode.Loop;
		this.faceTargetWhenWorking = true;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Socializing;
		this.synchronizeAnims = false;
		this.showProgressBar = false;
		this.resetProgressOnStop = true;
		this.lightEfficiencyBonus = false;
		if (global::UnityEngine.Random.Range(0f, 100f) > 80f)
		{
			this.activity = PartyPointWorkable.ActivityType.Dance;
		}
		else
		{
			this.activity = PartyPointWorkable.ActivityType.Talk;
		}
		PartyPointWorkable.ActivityType activityType = this.activity;
		if (activityType == PartyPointWorkable.ActivityType.Talk)
		{
			this.workAnims = new HashedString[] { "idle" };
			this.workerOverrideAnims = new KAnimFile[][] { new KAnimFile[] { Assets.GetAnim("anim_generic_convo_kanim") } };
			return;
		}
		if (activityType != PartyPointWorkable.ActivityType.Dance)
		{
			return;
		}
		this.workAnims = new HashedString[] { "working_loop" };
		this.workerOverrideAnims = new KAnimFile[][]
		{
			new KAnimFile[] { Assets.GetAnim("anim_interacts_phonobox_danceone_kanim") },
			new KAnimFile[] { Assets.GetAnim("anim_interacts_phonobox_dancetwo_kanim") },
			new KAnimFile[] { Assets.GetAnim("anim_interacts_phonobox_dancethree_kanim") }
		};
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		int num = global::UnityEngine.Random.Range(0, this.workerOverrideAnims.Length);
		this.overrideAnims = this.workerOverrideAnims[num];
		return base.GetAnim(worker);
	}

	public override Vector3 GetFacingTarget()
	{
		if (this.lastTalker != null)
		{
			return this.lastTalker.transform.GetPosition();
		}
		return base.GetFacingTarget();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse, false);
		worker.Subscribe(-594200555, new Action<object>(this.OnStartedTalking));
		worker.Subscribe(25860745, new Action<object>(this.OnStoppedTalking));
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		worker.GetComponent<KPrefabID>().RemoveTag(GameTags.AlwaysConverse);
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
		ConversationManager.StartedTalkingEvent startedTalkingEvent = data as ConversationManager.StartedTalkingEvent;
		if (startedTalkingEvent == null)
		{
			return;
		}
		GameObject talker = startedTalkingEvent.talker;
		if (talker == base.worker.gameObject)
		{
			if (this.activity == PartyPointWorkable.ActivityType.Talk)
			{
				KBatchedAnimController component = base.worker.GetComponent<KBatchedAnimController>();
				string text = startedTalkingEvent.anim;
				text += global::UnityEngine.Random.Range(1, 9).ToString();
				component.Play(text, KAnim.PlayMode.Once, 1f, 0f);
				component.Queue("idle", KAnim.PlayMode.Loop, 1f, 0f);
				return;
			}
		}
		else
		{
			if (this.activity == PartyPointWorkable.ActivityType.Talk)
			{
				base.worker.GetComponent<Facing>().Face(talker.transform.GetPosition());
			}
			this.lastTalker = talker;
		}
	}

	private void OnStoppedTalking(object data)
	{
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		if (!string.IsNullOrEmpty(this.specificEffect) && worker.GetComponent<Effects>().HasEffect(this.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	private GameObject lastTalker;

	public int basePriority;

	public string specificEffect;

	public KAnimFile[][] workerOverrideAnims;

	private PartyPointWorkable.ActivityType activity;

	private enum ActivityType
	{
		Talk,
		Dance,
		LENGTH
	}
}
