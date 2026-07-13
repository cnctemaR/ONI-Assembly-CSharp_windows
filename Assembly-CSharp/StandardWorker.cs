using System;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Worker")]
public class StandardWorker : WorkerBase
{
	public override WorkerBase.State GetState()
	{
		return this.state;
	}

	public override WorkerBase.StartWorkInfo GetStartWorkInfo()
	{
		return this.startWorkInfo;
	}

	public override Workable GetWorkable()
	{
		if (this.startWorkInfo != null)
		{
			return this.startWorkInfo.workable;
		}
		return null;
	}

	public override KBatchedAnimController GetAnimController()
	{
		return base.GetComponent<KBatchedAnimController>();
	}

	public override Attributes GetAttributes()
	{
		return base.gameObject.GetAttributes();
	}

	public override AttributeConverterInstance GetAttributeConverter(string id)
	{
		return base.GetComponent<AttributeConverters>().GetConverter(id);
	}

	public override Guid OfferStatusItem(StatusItem item, object data = null)
	{
		return base.GetComponent<KSelectable>().AddStatusItem(item, data);
	}

	public override void RevokeStatusItem(Guid id)
	{
		base.GetComponent<KSelectable>().RemoveStatusItem(id, false);
	}

	public override void SetWorkCompleteData(object data)
	{
		this.workCompleteData = data;
	}

	public override bool UsesMultiTool()
	{
		return this.usesMultiTool;
	}

	public override bool IsFetchDrone()
	{
		return this.isFetchDrone;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.state = WorkerBase.State.Idle;
		base.Subscribe<StandardWorker>(1485595942, StandardWorker.OnChoreInterruptDelegate);
	}

	private string GetWorkableDebugString()
	{
		if (this.GetWorkable() == null)
		{
			return "Null";
		}
		return this.GetWorkable().name;
	}

	public void CompleteWork()
	{
		this.successFullyCompleted = false;
		this.state = WorkerBase.State.Idle;
		Workable workable = this.GetWorkable();
		if (workable != null)
		{
			if (workable.triggerWorkReactions && workable.GetWorkTime() > 30f)
			{
				string conversationTopic = workable.GetConversationTopic();
				if (!conversationTopic.IsNullOrWhiteSpace())
				{
					this.CreateCompletionReactable(conversationTopic);
				}
			}
			this.DetachAnimOverrides();
			workable.CompleteWork(this);
			if (workable.worker != null && !(workable is Constructable) && !(workable is Deconstructable) && !(workable is Repairable) && !(workable is Disinfectable))
			{
				BonusEvent.GameplayEventData gameplayEventData = new BonusEvent.GameplayEventData();
				gameplayEventData.workable = workable;
				gameplayEventData.worker = workable.worker;
				gameplayEventData.building = workable.GetComponent<BuildingComplete>();
				gameplayEventData.eventTrigger = GameHashes.UseBuilding;
				GameplayEventManager.Instance.Trigger(1175726587, gameplayEventData);
			}
		}
		this.InternalStopWork(workable, false);
	}

	protected virtual void TryPlayingIdle()
	{
		Navigator component = base.GetComponent<Navigator>();
		if (component != null)
		{
			NavGrid.NavTypeData navTypeData = component.NavGrid.GetNavTypeData(component.CurrentNavType);
			if (navTypeData.idleAnim.IsValid)
			{
				base.GetComponent<KAnimControllerBase>().Play(navTypeData.idleAnim, KAnim.PlayMode.Once, 1f, 0f);
			}
		}
	}

	public override WorkerBase.WorkResult Work(float dt)
	{
		if (this.state == WorkerBase.State.PendingCompletion)
		{
			bool flag = Time.time - this.workPendingCompletionTime > 10f;
			if (!base.GetComponent<KAnimControllerBase>().IsStopped() && !flag)
			{
				return WorkerBase.WorkResult.InProgress;
			}
			this.TryPlayingIdle();
			if (this.successFullyCompleted)
			{
				this.CompleteWork();
				return WorkerBase.WorkResult.Success;
			}
			this.StopWork();
			return WorkerBase.WorkResult.Failed;
		}
		else
		{
			if (this.state != WorkerBase.State.Completing)
			{
				Workable workable = this.GetWorkable();
				if (workable != null)
				{
					if (this.facing)
					{
						if (workable.ShouldFaceTargetWhenWorking())
						{
							this.facing.Face(workable.GetFacingTarget());
						}
						else
						{
							Rotatable component = workable.GetComponent<Rotatable>();
							bool flag2 = component != null && component.GetOrientation() == Orientation.FlipH;
							Vector3 vector = this.facing.transform.GetPosition();
							vector += (flag2 ? Vector3.left : Vector3.right);
							this.facing.Face(vector);
						}
					}
					if (dt > 0f && Game.Instance.FastWorkersModeActive)
					{
						dt = Mathf.Min(workable.WorkTimeRemaining + 0.01f, 5f);
					}
					Klei.AI.Attribute workAttribute = workable.GetWorkAttribute();
					AttributeLevels component2 = base.GetComponent<AttributeLevels>();
					if (workAttribute != null && workAttribute.IsTrainable && component2 != null)
					{
						float attributeExperienceMultiplier = workable.GetAttributeExperienceMultiplier();
						component2.AddExperience(workAttribute.Id, dt, attributeExperienceMultiplier);
					}
					string skillExperienceSkillGroup = workable.GetSkillExperienceSkillGroup();
					if (this.experienceRecipient != null && skillExperienceSkillGroup != null)
					{
						float skillExperienceMultiplier = workable.GetSkillExperienceMultiplier();
						this.experienceRecipient.AddExperienceWithAptitude(skillExperienceSkillGroup, dt, skillExperienceMultiplier);
					}
					float efficiencyMultiplier = workable.GetEfficiencyMultiplier(this);
					float num = dt * efficiencyMultiplier * 1f;
					if (workable.WorkTick(this, num) && this.state == WorkerBase.State.Working)
					{
						this.successFullyCompleted = true;
						this.StartPlayingPostAnim();
						workable.OnPendingCompleteWork(this);
					}
				}
				return WorkerBase.WorkResult.InProgress;
			}
			if (this.successFullyCompleted)
			{
				this.CompleteWork();
				return WorkerBase.WorkResult.Success;
			}
			this.StopWork();
			return WorkerBase.WorkResult.Failed;
		}
	}

	private void StartPlayingPostAnim()
	{
		Workable workable = this.GetWorkable();
		if (workable != null && !workable.alwaysShowProgressBar)
		{
			workable.ShowProgressBar(false);
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption, false);
		this.state = WorkerBase.State.PendingCompletion;
		this.workPendingCompletionTime = Time.time;
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		HashedString[] workPstAnims = workable.GetWorkPstAnims(this, this.successFullyCompleted);
		if (this.smi == null)
		{
			if (workPstAnims != null && workPstAnims.Length != 0)
			{
				if (workable != null && workable.synchronizeAnims)
				{
					KAnimControllerBase animController = workable.GetAnimController();
					if (animController != null)
					{
						animController.Play(workPstAnims, KAnim.PlayMode.Once);
					}
				}
				else
				{
					component.Play(workPstAnims, KAnim.PlayMode.Once);
				}
			}
			else
			{
				this.state = WorkerBase.State.Completing;
			}
		}
		base.Trigger(-1142962013, this);
	}

	protected virtual void InternalStopWork(Workable target_workable, bool is_aborted)
	{
		this.state = WorkerBase.State.Idle;
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
		else
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
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
		if (this.state == WorkerBase.State.Working)
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
			if (currentChore != null)
			{
				currentChore.Fail((text != null) ? text : "WorkChoreDisabled");
			}
		}
	}

	public override void StopWork()
	{
		Workable workable = this.GetWorkable();
		if (this.state == WorkerBase.State.PendingCompletion || this.state == WorkerBase.State.Completing)
		{
			this.state = WorkerBase.State.Idle;
			if (this.successFullyCompleted)
			{
				this.CompleteWork();
				base.Trigger(1705586602, this);
			}
			else
			{
				base.Trigger(-993481695, this);
				this.InternalStopWork(workable, true);
			}
		}
		else if (this.state == WorkerBase.State.Working)
		{
			if (workable != null && workable.synchronizeAnims)
			{
				KAnimControllerBase animController = workable.GetAnimController();
				if (animController != null)
				{
					HashedString[] workPstAnims = workable.GetWorkPstAnims(this, false);
					if (workPstAnims != null && workPstAnims.Length != 0)
					{
						animController.Play(workPstAnims, KAnim.PlayMode.Once);
						animController.SetPositionPercent(1f);
					}
				}
			}
			base.Trigger(-993481695, this);
			this.InternalStopWork(workable, true);
		}
		base.Trigger(2027193395, this);
	}

	public override void StartWork(WorkerBase.StartWorkInfo start_work_info)
	{
		this.startWorkInfo = start_work_info;
		Game.Instance.StartedWork();
		Workable workable = this.GetWorkable();
		this.surpressForceSyncOnUpdate = false;
		if (this.state != WorkerBase.State.Idle)
		{
			string text = "";
			if (workable != null)
			{
				text = workable.name;
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
		string name = workable.GetType().Name;
		try
		{
			base.gameObject.AddTag(GameTags.PerformingWorkRequest);
			this.state = WorkerBase.State.Working;
			base.gameObject.Trigger(1568504979, this);
			if (workable != null)
			{
				this.animInfo = workable.GetAnim(this);
				if (this.animInfo.smi != null)
				{
					this.smi = this.animInfo.smi;
					this.smi.StartSM();
				}
				Vector3 position = base.transform.GetPosition();
				position.z = Grid.GetLayerZ(workable.workLayer);
				base.transform.SetPosition(position);
				KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
				if (this.animInfo.smi == null)
				{
					this.AttachOverrideAnims(component);
				}
				this.surpressForceSyncOnUpdate = workable.surpressWorkerForceSync;
				HashedString[] workAnims = workable.GetWorkAnims(this);
				KAnim.PlayMode workAnimPlayMode = workable.GetWorkAnimPlayMode();
				Vector3 workOffset = workable.GetWorkOffset();
				this.workAnimOffset = workOffset;
				component.Offset += workOffset;
				if (this.usesMultiTool && this.animInfo.smi == null && workAnims != null && workAnims.Length != 0 && this.experienceRecipient != null)
				{
					if (workable.synchronizeAnims)
					{
						KAnimControllerBase animController = workable.GetAnimController();
						if (animController != null)
						{
							this.kanimSynchronizer = animController.GetSynchronizer();
							if (this.kanimSynchronizer != null)
							{
								this.kanimSynchronizer.Add(component);
							}
						}
						animController.Play(workAnims, workAnimPlayMode);
					}
					else
					{
						component.Play(workAnims, workAnimPlayMode);
					}
				}
			}
			workable.StartWork(this);
			if (workable == null)
			{
				global::Debug.LogWarning("Stopped work as soon as I started. This is usually a sign that a chore is open when it shouldn't be or that it's preconditions are wrong.");
			}
			else
			{
				this.onWorkChoreDisabledHandle = workable.Subscribe(2108245096, new Action<object>(this.OnWorkChoreDisabled));
				if (workable.triggerWorkReactions && workable.WorkTimeRemaining > 10f)
				{
					this.CreatePasserbyReactable();
				}
				KSelectable component2 = base.GetComponent<KSelectable>();
				this.previousStatusItem = component2.GetStatusItem(Db.Get().StatusItemCategories.Main);
				component2.SetStatusItem(Db.Get().StatusItemCategories.Main, workable.GetWorkerStatusItem(), workable);
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
		if (this.state == WorkerBase.State.Working && !this.surpressForceSyncOnUpdate)
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

	public override bool InstantlyFinish()
	{
		Workable workable = this.GetWorkable();
		return workable != null && workable.InstantlyFinish(this);
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
			this.kanimSynchronizer.RemoveWithoutIdleAnim(component);
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
		EmoteReactable emoteReactable = OneshotReactableLocator.CreateOneshotReactable(base.gameObject, 3f, "WorkCompleteAcknowledgement", Db.Get().ChoreTypes.Emote, 9, 5, 100f);
		Emote clapCheer = Db.Get().Emotes.Minion.ClapCheer;
		emoteReactable.SetEmote(clapCheer);
		emoteReactable.RegisterEmoteStepCallbacks("clapcheer_pre", new Action<GameObject>(this.GetReactionEffect), null).RegisterEmoteStepCallbacks("clapcheer_pst", null, delegate(GameObject r)
		{
			r.Trigger(937885943, topic);
		});
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(topic, "ui", true);
		if (uisprite != null)
		{
			Thought thought = new Thought("Completion_" + topic, null, uisprite.first, "mode_satisfaction", "conversation_short", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, "", true, 4f);
			emoteReactable.SetThought(thought);
		}
	}

	private void CreatePasserbyReactable()
	{
		if (GameClock.Instance.GetTime() / 600f < 1f)
		{
			return;
		}
		if (this.passerbyReactable == null)
		{
			EmoteReactable emoteReactable = new EmoteReactable(base.gameObject, "WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, 5, 5, 30f, 720f * TuningData<DupeGreetingManager.Tuning>.Get().greetingDelayMultiplier, float.PositiveInfinity, 0f);
			Emote thumbsUp = Db.Get().Emotes.Minion.ThumbsUp;
			emoteReactable.SetEmote(thumbsUp).SetThought(Db.Get().Thoughts.Encourage).AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsOnFloor))
				.AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsFacingMe))
				.AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsntPartying));
			emoteReactable.RegisterEmoteStepCallbacks("react", new Action<GameObject>(this.GetReactionEffect), null);
			this.passerbyReactable = emoteReactable;
		}
	}

	private void GetReactionEffect(GameObject reactor)
	{
		Effects component = base.GetComponent<Effects>();
		if (component != null)
		{
			component.Add("WorkEncouraged", true);
		}
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

	private void ClearPasserbyReactable()
	{
		if (this.passerbyReactable != null)
		{
			this.passerbyReactable.Cleanup();
			this.passerbyReactable = null;
		}
	}

	private WorkerBase.State state;

	private WorkerBase.StartWorkInfo startWorkInfo;

	private const float EARLIEST_REACT_TIME = 1f;

	[MyCmpGet]
	private Facing facing;

	[MyCmpGet]
	private IExperienceRecipient experienceRecipient;

	private float workPendingCompletionTime;

	private int onWorkChoreDisabledHandle;

	public object workCompleteData;

	private Workable.AnimInfo animInfo;

	private KAnimSynchronizer kanimSynchronizer;

	private StatusItemGroup.Entry previousStatusItem;

	private StateMachine.Instance smi;

	private bool successFullyCompleted;

	private bool surpressForceSyncOnUpdate;

	private Vector3 workAnimOffset = Vector3.zero;

	public bool usesMultiTool = true;

	public bool isFetchDrone;

	private static readonly EventSystem.IntraObjectHandler<StandardWorker> OnChoreInterruptDelegate = new EventSystem.IntraObjectHandler<StandardWorker>(delegate(StandardWorker component, object data)
	{
		component.OnChoreInterrupt(data);
	});

	private Reactable passerbyReactable;
}
