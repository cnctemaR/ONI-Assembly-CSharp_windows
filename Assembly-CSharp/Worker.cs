using System;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Worker")]
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
			if (this.workable.worker != null && !(this.workable is Constructable) && !(this.workable is Deconstructable) && !(this.workable is Repairable) && !(this.workable is Disinfectable))
			{
				BonusEvent.GameplayEventData gameplayEventData = new BonusEvent.GameplayEventData();
				gameplayEventData.workable = this.workable;
				gameplayEventData.worker = this.workable.worker;
				gameplayEventData.building = this.workable.GetComponent<BuildingComplete>();
				gameplayEventData.eventTrigger = GameHashes.UseBuilding;
				GameplayEventManager.Instance.Trigger(1175726587, gameplayEventData);
			}
		}
		this.InternalStopWork(this.workable, false);
	}

	public Worker.WorkResult Work(float dt)
	{
		if (this.state == Worker.State.PendingCompletion)
		{
			bool flag = Time.time - this.workPendingCompletionTime > 10f;
			if (!base.GetComponent<KAnimControllerBase>().IsStopped() && !flag)
			{
				return Worker.WorkResult.InProgress;
			}
			Navigator component = base.GetComponent<Navigator>();
			if (component != null)
			{
				NavGrid.NavTypeData navTypeData = component.NavGrid.GetNavTypeData(component.CurrentNavType);
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
			this.StopWork();
			return Worker.WorkResult.Failed;
		}
		else
		{
			if (this.state != Worker.State.Completing)
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
							Rotatable component2 = this.workable.GetComponent<Rotatable>();
							bool flag2 = component2 != null && component2.GetOrientation() == Orientation.FlipH;
							Vector3 vector = this.facing.transform.GetPosition();
							vector += (flag2 ? Vector3.left : Vector3.right);
							this.facing.Face(vector);
						}
					}
					if (dt > 0f && Game.Instance.FastWorkersModeActive)
					{
						dt = Mathf.Min(this.workable.WorkTimeRemaining + 0.01f, 5f);
					}
					Klei.AI.Attribute workAttribute = this.workable.GetWorkAttribute();
					AttributeLevels component3 = base.GetComponent<AttributeLevels>();
					if (workAttribute != null && workAttribute.IsTrainable && component3 != null)
					{
						float attributeExperienceMultiplier = this.workable.GetAttributeExperienceMultiplier();
						component3.AddExperience(workAttribute.Id, dt, attributeExperienceMultiplier);
					}
					string skillExperienceSkillGroup = this.workable.GetSkillExperienceSkillGroup();
					if (this.resume != null && skillExperienceSkillGroup != null)
					{
						float skillExperienceMultiplier = this.workable.GetSkillExperienceMultiplier();
						this.resume.AddExperienceWithAptitude(skillExperienceSkillGroup, dt, skillExperienceMultiplier);
					}
					float efficiencyMultiplier = this.workable.GetEfficiencyMultiplier(this);
					float num = dt * efficiencyMultiplier * 1f;
					if (this.workable.WorkTick(this, num) && this.state == Worker.State.Working)
					{
						this.successFullyCompleted = true;
						this.StartPlayingPostAnim();
						this.workable.OnPendingCompleteWork(this);
					}
				}
				return Worker.WorkResult.InProgress;
			}
			if (this.successFullyCompleted)
			{
				this.CompleteWork();
				return Worker.WorkResult.Success;
			}
			this.StopWork();
			return Worker.WorkResult.Failed;
		}
	}

	private void StartPlayingPostAnim()
	{
		if (this.workable != null && !this.workable.alwaysShowProgressBar)
		{
			this.workable.ShowProgressBar(false);
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption, false);
		this.state = Worker.State.PendingCompletion;
		this.workPendingCompletionTime = Time.time;
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		HashedString[] workPstAnims = this.workable.GetWorkPstAnims(this, this.successFullyCompleted);
		if (this.smi == null)
		{
			if (workPstAnims != null && workPstAnims.Length != 0)
			{
				if (this.workable != null && this.workable.synchronizeAnims)
				{
					KAnimControllerBase component2 = this.workable.GetComponent<KAnimControllerBase>();
					if (component2 != null)
					{
						component2.Play(workPstAnims, KAnim.PlayMode.Once);
					}
				}
				else
				{
					component.Play(workPstAnims, KAnim.PlayMode.Once);
				}
			}
			else
			{
				this.state = Worker.State.Completing;
			}
		}
		base.Trigger(-1142962013, this);
	}

	private void InternalStopWork(Workable target_workable, bool is_aborted)
	{
		this.state = Worker.State.Idle;
		base.gameObject.RemoveTag(GameTags.PerformingWorkRequest);
		base.GetComponent<KAnimControllerBase>().Offset -= this.workAnimOffset;
		this.workAnimOffset = Vector3.zero;
		base.GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
		this.DetachAnimOverrides();
		this.ClearPasserbyReactable();
		AnimEventHandler component = base.GetComponent<AnimEventHandler>();
		if (component)
		{
			component.ClearContext();
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
			component.choreDriver.GetCurrentChore().Fail((text != null) ? text : "WorkChoreDisabled");
		}
	}

	public void StopWork()
	{
		if (this.state != Worker.State.PendingCompletion && this.state != Worker.State.Completing)
		{
			if (this.state == Worker.State.Working)
			{
				if (this.workable != null && this.workable.synchronizeAnims)
				{
					KBatchedAnimController component = this.workable.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						HashedString[] workPstAnims = this.workable.GetWorkPstAnims(this, false);
						if (workPstAnims != null && workPstAnims.Length != 0)
						{
							component.Play(workPstAnims, KAnim.PlayMode.Once);
							component.SetPositionPercent(1f);
						}
					}
				}
				this.InternalStopWork(this.workable, true);
			}
			return;
		}
		this.state = Worker.State.Idle;
		if (this.successFullyCompleted)
		{
			this.CompleteWork();
			return;
		}
		this.InternalStopWork(this.workable, true);
	}

	public void StartWork(Worker.StartWorkInfo start_work_info)
	{
		this.startWorkInfo = start_work_info;
		Game.Instance.StartedWork();
		if (this.state != Worker.State.Idle)
		{
			string text = "";
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
			}));
		}
		string name = this.workable.GetType().Name;
		try
		{
			base.gameObject.AddTag(GameTags.PerformingWorkRequest);
			this.state = Worker.State.Working;
			base.gameObject.Trigger(1568504979, this);
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
				if (this.usesMultiTool && this.animInfo.smi == null && workAnims != null && this.resume != null)
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
				global::Debug.LogWarning("Stopped work as soon as I started. This is usually a sign that a chore is open when it shouldn't be or that it's preconditions are wrong.");
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
			DebugUtil.LogErrorArgs(this, new object[] { text2 + "\n" + ex.ToString() });
			throw;
		}
	}

	private void Update()
	{
		if (this.state == Worker.State.Working)
		{
			this.ForceSyncAnims();
		}
	}

	private void ForceSyncAnims()
	{
		if (Time.deltaTime > 0f && this.kanimSynchronizer != null)
		{
			this.kanimSynchronizer.SyncTime();
		}
	}

	public bool InstantlyFinish()
	{
		return this.workable != null && this.workable.InstantlyFinish(this);
	}

	private void AttachOverrideAnims(KAnimControllerBase worker_controller)
	{
		if (this.animInfo.overrideAnims != null && this.animInfo.overrideAnims.Length != 0)
		{
			for (int i = 0; i < this.animInfo.overrideAnims.Length; i++)
			{
				worker_controller.AddAnimOverrides(this.animInfo.overrideAnims[i], 0f);
			}
		}
	}

	private void DetachAnimOverrides()
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		if (this.kanimSynchronizer != null)
		{
			this.kanimSynchronizer.Remove(component);
			this.kanimSynchronizer = null;
		}
		if (this.animInfo.overrideAnims != null)
		{
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
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(topic, "ui", true);
		if (uisprite != null)
		{
			Thought thought = new Thought("Completion_" + topic, null, uisprite.first, "mode_satisfaction", "conversation_short", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, "", true, 4f);
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
				.AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsFacingMe))
				.AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsntPartying));
		}
	}

	private void GetReactionEffect(GameObject reactor)
	{
		base.GetComponent<Effects>().Add("WorkEncouraged", true);
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

	private bool ReactorIsntPartying(GameObject reactor, Navigator.ActiveTransition transition)
	{
		ChoreConsumer component = reactor.GetComponent<ChoreConsumer>();
		return component.choreDriver.HasChore() && component.choreDriver.GetCurrentChore().choreType != Db.Get().ChoreTypes.Party;
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

	private static readonly EventSystem.IntraObjectHandler<Worker> OnChoreInterruptDelegate = new EventSystem.IntraObjectHandler<Worker>(delegate(Worker component, object data)
	{
		component.OnChoreInterrupt(data);
	});

	private Reactable passerbyReactable;

	public enum State
	{
		Idle,
		Working,
		PendingCompletion,
		Completing
	}

	public class StartWorkInfo
	{
		public Workable workable { get; set; }

		public StartWorkInfo(Workable workable)
		{
			this.workable = workable;
		}
	}

	public enum WorkResult
	{
		Success,
		InProgress,
		Failed
	}
}
