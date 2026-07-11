using System;
using Klei.AI;
using UnityEngine;

public class Worker : KMonoBehaviour
{
	public Worker.State state { get; private set; }

	public Worker.StartWorkInfo startWorkInfo { get; private set; }

	public Workable workable
	{
		get
		{
			if (this.startWorkInfo != null)
			{
				return this.startWorkInfo.workable;
			}
			return null;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.state = Worker.State.Idle;
		base.Subscribe<Worker>(1485595942, Worker.OnChoreInterruptDelegate);
	}

	private string GetWorkableDebugString()
	{
		if (this.workable == null)
		{
			return "Null";
		}
		return this.workable.name;
	}

	public void CompleteWork()
	{
		this.successFullyCompleted = false;
		this.state = Worker.State.Idle;
		if (this.workable != null)
		{
			if (this.workable.triggerWorkReactions && this.workable.GetWorkTime() > 30f)
			{
				string conversationTopic = this.workable.GetConversationTopic();
				if (!conversationTopic.IsNullOrWhiteSpace())
				{
					this.CreateCompletionReactable(conversationTopic);
				}
			}
			this.DetachAnimOverrides();
			this.workable.CompleteWork(this);
		}
		this.InternalStopWork(this.workable, false);
	}

	public Worker.WorkResult Work(float dt)
	{
		if (this.state != Worker.State.PendingCompletion)
		{
			if (this.workable != null)
			{
				if (this.facing)
				{
					if (this.workable.ShouldFaceTargetWhenWorking())
					{
						this.facing.Face(this.workable.GetFacingTarget());
					}
					else
					{
						Rotatable component = this.workable.GetComponent<Rotatable>();
						bool flag = component != null && component.GetOrientation() == Orientation.FlipH;
						Vector3 vector = this.facing.transform.GetPosition();
						vector += ((!flag) ? Vector3.right : Vector3.left);
						this.facing.Face(vector);
					}
				}
				Klei.AI.Attribute workAttribute = this.workable.GetWorkAttribute();
				if (workAttribute != null && workAttribute.IsTrainable)
				{
					float attributeExperienceMultiplier = this.workable.GetAttributeExperienceMultiplier();
					base.GetComponent<AttributeLevels>().AddExperience(workAttribute.Id, dt, attributeExperienceMultiplier);
				}
				float efficiencyMultiplier = this.workable.GetEfficiencyMultiplier(this);
				float num = dt * efficiencyMultiplier * 1f;
				if (this.resume != null)
				{
					this.workable.AwardExperience(num, this.resume);
				}
				if (this.workable.WorkTick(this, num) && this.state == Worker.State.Working)
				{
					this.successFullyCompleted = true;
					this.StartPlayingPostAnim();
				}
			}
			return Worker.WorkResult.InProgress;
		}
		if (!base.GetComponent<KAnimControllerBase>().IsStopped() && Time.time - this.workPendingCompletionTime <= 4f / Mathf.Max(Time.timeScale, 1f))
		{
			return Worker.WorkResult.InProgress;
		}
		Navigator component2 = base.GetComponent<Navigator>();
		if (component2 != null)
		{
			NavGrid.NavTypeData navTypeData = component2.NavGrid.GetNavTypeData(component2.CurrentNavType);
			if (navTypeData.idleAnim.IsValid)
			{
				base.GetComponent<KAnimControllerBase>().Play(navTypeData.idleAnim, KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		if (this.successFullyCompleted)
		{
			this.CompleteWork();
			return Worker.WorkResult.Success;
		}
		this.state = Worker.State.Working;
		this.StopWork();
		return Worker.WorkResult.Failed;
	}

	private void StartPlayingPostAnim()
	{
		if (this.workable != null)
		{
			this.workable.ShowProgressBar(false);
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption);
		this.state = Worker.State.PendingCompletion;
		this.workPendingCompletionTime = Time.time;
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		HashedString workPstAnim = this.workable.GetWorkPstAnim(this, this.successFullyCompleted);
		if (workPstAnim.IsValid)
		{
			if (this.workable != null && this.workable.synchronizeAnims)
			{
				KAnimControllerBase component2 = this.workable.GetComponent<KAnimControllerBase>();
				if (component2 != null && component2.HasAnimation(workPstAnim))
				{
					component2.Play(workPstAnim, KAnim.PlayMode.Once, 1f, 0f);
				}
			}
			else
			{
				component.Play(workPstAnim, KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		base.Trigger(-1142962013, this);
	}

	private void InternalStopWork(Workable target_workable, bool is_aborted)
	{
		this.state = Worker.State.Idle;
		base.gameObject.RemoveTag(GameTags.PerformingWorkRequest);
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Offset -= this.workAnimOffset;
		this.workAnimOffset = Vector3.zero;
		base.GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
		this.DetachAnimOverrides();
		this.ClearPasserbyReactable();
		AnimEventHandler component2 = base.GetComponent<AnimEventHandler>();
		if (component2)
		{
			component2.ClearContext();
		}
		if (this.previousStatusItem.item != null)
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, this.previousStatusItem.item, this.previousStatusItem.data);
		}
		if (target_workable != null)
		{
			target_workable.Unsubscribe(this.onWorkChoreDisabledHandle);
			target_workable.StopWork(this, is_aborted);
		}
		if (this.smi != null)
		{
			this.smi.StopSM("stopping work");
			this.smi = null;
		}
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		base.transform.SetPosition(position);
		this.startWorkInfo = null;
	}

	private void OnChoreInterrupt(object data)
	{
		if (this.state == Worker.State.Working)
		{
			this.successFullyCompleted = false;
			this.StartPlayingPostAnim();
		}
	}

	private void OnWorkChoreDisabled(object data)
	{
		string text = data as string;
		ChoreConsumer component = base.GetComponent<ChoreConsumer>();
		if (component != null && component.choreDriver != null)
		{
			Chore currentChore = component.choreDriver.GetCurrentChore();
			currentChore.Fail((text == null) ? "WorkChoreDisabled" : text);
		}
	}

	public void StopWork()
	{
		if (this.state == Worker.State.PendingCompletion)
		{
			this.state = Worker.State.Idle;
			if (this.successFullyCompleted)
			{
				this.CompleteWork();
			}
			else
			{
				this.InternalStopWork(this.workable, true);
			}
		}
		else if (this.state == Worker.State.Working)
		{
			if (this.workable != null && this.workable.synchronizeAnims)
			{
				KBatchedAnimController component = this.workable.GetComponent<KBatchedAnimController>();
				if (component != null)
				{
					HashedString workPstAnim = this.workable.GetWorkPstAnim(this, false);
					if (workPstAnim.IsValid)
					{
						component.Play(workPstAnim, KAnim.PlayMode.Once, 1f, 0f);
						component.SetPositionPercent(1f);
					}
				}
			}
			this.InternalStopWork(this.workable, true);
		}
	}

	public void StartWork(Worker.StartWorkInfo start_work_info)
	{
		this.startWorkInfo = start_work_info;
		Game.Instance.StartedWork();
		if (this.state != Worker.State.Idle)
		{
			string text = string.Empty;
			if (this.workable != null)
			{
				text = this.workable.name;
			}
			global::Debug.LogError(string.Concat(new string[]
			{
				base.name,
				".",
				text,
				".state should be idle but instead it's:",
				this.state.ToString()
			}), null);
		}
		string name = this.workable.GetType().Name;
		try
		{
			base.gameObject.AddTag(GameTags.PerformingWorkRequest);
			this.state = Worker.State.Working;
			if (this.workable != null)
			{
				this.animInfo = this.workable.GetAnim(this);
				if (this.animInfo.smi != null)
				{
					this.smi = this.animInfo.smi;
					this.smi.StartSM();
				}
				Vector3 position = base.transform.GetPosition();
				position.z = Grid.GetLayerZ(this.workable.workLayer);
				base.transform.SetPosition(position);
				KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
				if (this.animInfo.smi == null)
				{
					this.AttachOverrideAnims(component);
				}
				HashedString[] workAnims = this.workable.GetWorkAnims(this);
				KAnim.PlayMode workAnimPlayMode = this.workable.GetWorkAnimPlayMode();
				Vector3 workOffset = this.workable.GetWorkOffset();
				this.workAnimOffset = workOffset;
				component.Offset += workOffset;
				if (this.usesMultiTool && this.animInfo.smi == null && workAnims != null)
				{
					if (this.workable.synchronizeAnims)
					{
						KAnimControllerBase component2 = this.workable.GetComponent<KAnimControllerBase>();
						if (component2 != null)
						{
							this.kanimSynchronizer = component2.GetSynchronizer();
							if (this.kanimSynchronizer != null)
							{
								this.kanimSynchronizer.Add(component);
							}
						}
						component2.Play(workAnims, workAnimPlayMode);
					}
					else
					{
						component.Play(workAnims, workAnimPlayMode);
					}
				}
			}
			this.workable.StartWork(this);
			if (this.workable == null)
			{
				global::Debug.LogWarning("Stopped work as soon as I started. This is usually a sign that a chore is open when it shouldn't be or that it's preconditions are wrong.", null);
			}
			else
			{
				this.onWorkChoreDisabledHandle = this.workable.Subscribe(2108245096, new Action<object>(this.OnWorkChoreDisabled));
				if (this.workable.triggerWorkReactions && this.workable.WorkTimeRemaining > 10f)
				{
					this.CreatePasserbyReactable();
				}
				KSelectable component3 = base.GetComponent<KSelectable>();
				this.previousStatusItem = component3.GetStatusItem(Db.Get().StatusItemCategories.Main);
				component3.SetStatusItem(Db.Get().StatusItemCategories.Main, this.workable.GetWorkerStatusItem(), this.workable);
			}
		}
		catch (Exception ex)
		{
			string text2 = "Exception in: Worker.StartWork(" + name + ")";
			Output.LogErrorWithObj(this, new object[] { text2 + "\n" + ex.ToString() });
			throw;
		}
	}

	private void AttachOverrideAnims(KAnimControllerBase worker_controller)
	{
		if (this.animInfo.overrideAnims != null && this.animInfo.overrideAnims.Length > 0)
		{
			for (int i = 0; i < this.animInfo.overrideAnims.Length; i++)
			{
				worker_controller.AddAnimOverrides(this.animInfo.overrideAnims[i], 0f);
			}
		}
	}

	private void DetachAnimOverrides()
	{
		if (this.animInfo.overrideAnims != null)
		{
			KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
			if (this.kanimSynchronizer != null)
			{
				this.kanimSynchronizer.Remove(component);
				this.kanimSynchronizer = null;
			}
			for (int i = 0; i < this.animInfo.overrideAnims.Length; i++)
			{
				component.RemoveAnimOverrides(this.animInfo.overrideAnims[i]);
			}
			this.animInfo.overrideAnims = null;
		}
	}

	private void CreateCompletionReactable(string topic)
	{
		if (GameClock.Instance.GetTime() / 600f < 1f)
		{
			return;
		}
		EmoteReactable emoteReactable = OneshotReactableLocator.CreateOneshotReactable(base.gameObject, 3f, "WorkCompleteAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_clapcheer_kanim", 9, 5, 100f);
		emoteReactable.AddStep(new EmoteReactable.EmoteStep
		{
			anim = "clapcheer_pre",
			startcb = new Action<GameObject>(this.GetReactionEffect)
		}).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "clapcheer_loop"
		}).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "clapcheer_pst",
			finishcb = delegate(GameObject r)
			{
				r.Trigger(937885943, topic);
			}
		})
			.AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsOnFloor));
		Tuple<Sprite, Color> uisprite = Def.GetUISprite(topic, "ui", true);
		if (uisprite != null)
		{
			Thought thought = new Thought("Completion_" + topic, null, uisprite.first, "mode_satisfaction", "conversation_short", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, string.Empty, true, 4f);
			emoteReactable.AddThought(thought);
		}
	}

	public void CreatePasserbyReactable()
	{
		if (GameClock.Instance.GetTime() / 600f < 1f)
		{
			return;
		}
		if (this.passerbyReactable == null)
		{
			this.passerbyReactable = new EmoteReactable(base.gameObject, "WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_react_thumbsup_kanim", 5, 5, 30f, 720f * TuningData<DupeGreetingManager.Tuning>.Get().greetingDelayMultiplier, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react",
				startcb = new Action<GameObject>(this.GetReactionEffect)
			}).AddThought(Db.Get().Thoughts.Encourage).AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsOnFloor))
				.AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsFacingMe));
		}
	}

	private void GetReactionEffect(GameObject reactor)
	{
		Effects component = base.GetComponent<Effects>();
		component.Add("WorkEncouraged", true);
	}

	private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition)
	{
		return transition.end == NavType.Floor;
	}

	private bool ReactorIsFacingMe(GameObject reactor, Navigator.ActiveTransition transition)
	{
		Facing component = reactor.GetComponent<Facing>();
		return base.transform.GetPosition().x < reactor.transform.GetPosition().x == component.GetFacing();
	}

	public void ClearPasserbyReactable()
	{
		if (this.passerbyReactable != null)
		{
			this.passerbyReactable.Cleanup();
			this.passerbyReactable = null;
		}
	}

	private const float EARLIEST_REACT_TIME = 1f;

	[MyCmpGet]
	private Facing facing;

	[MyCmpGet]
	private MinionResume resume;

	private float workPendingCompletionTime;

	private int onWorkChoreDisabledHandle;

	public object workCompleteData;

	private Workable.AnimInfo animInfo;

	private KAnimSynchronizer kanimSynchronizer;

	private StatusItemGroup.Entry previousStatusItem;

	private StateMachine.Instance smi;

	private bool successFullyCompleted;

	private Vector3 workAnimOffset = Vector3.zero;

	public bool usesMultiTool = true;

	private static readonly EventSystem.IntraObjectHandler<Worker> OnChoreInterruptDelegate = new EventSystem.IntraObjectHandler<Worker>(delegate(Worker worker, object data)
	{
		worker.OnChoreInterrupt(data);
	});

	private Reactable passerbyReactable;

	public enum State
	{
		Idle,
		Working,
		PendingCompletion
	}

	public class StartWorkInfo
	{
		public StartWorkInfo(Workable workable)
		{
			this.workable = workable;
		}

		public Workable workable { get; set; }
	}

	public enum WorkResult
	{
		Success,
		InProgress,
		Failed
	}
}
