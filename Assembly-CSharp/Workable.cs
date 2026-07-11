using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Workable : KMonoBehaviour, ISaveLoadable, IApproachable
{
	public Worker worker { get; protected set; }

	public float WorkTimeRemaining
	{
		get
		{
			return this.workTimeRemaining;
		}
		set
		{
			this.workTimeRemaining = value;
		}
	}

	public bool preferUnreservedCell { get; set; }

	public virtual float GetWorkTime()
	{
		return this.workTime;
	}

	public Worker GetWorker()
	{
		return this.worker;
	}

	public virtual float GetPercentComplete()
	{
		return (this.workTimeRemaining > this.workTime) ? (-1f) : (1f - this.workTimeRemaining / this.workTime);
	}

	public virtual Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo animInfo = default(Workable.AnimInfo);
		if (this.overrideAnims != null && this.overrideAnims.Length > 0)
		{
			animInfo.overrideAnims = this.overrideAnims;
		}
		if (this.multitoolContext.IsValid && this.multitoolHitEffectTag.IsValid)
		{
			animInfo.smi = new MultitoolController.Instance(this, worker, this.multitoolContext, Assets.GetPrefab(this.multitoolHitEffectTag));
		}
		return animInfo;
	}

	public virtual HashedString[] GetWorkAnims(Worker worker)
	{
		return this.workAnims;
	}

	public virtual KAnim.PlayMode GetWorkAnimPlayMode()
	{
		return this.workAnimPlayMode;
	}

	public virtual HashedString GetWorkPstAnim(Worker worker, bool successfully_completed)
	{
		if (successfully_completed)
		{
			return this.workingPstComplete;
		}
		return this.workingPstFailed;
	}

	public virtual Vector3 GetWorkOffset()
	{
		return Vector3.zero;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().MiscStatusItems.Using;
		this.workingStatusItem = Db.Get().MiscStatusItems.Operating;
		this.readyForRoleWorkStatusItem = Db.Get().BuildingStatusItems.RequiresRolePerk;
		this.workTime = this.GetWorkTime();
		this.workTimeRemaining = Mathf.Min(this.workTimeRemaining, this.workTime);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.shouldShowRolePerkStatusItem && this.requiredRolePerk.IsValid)
		{
			if (this.roleUpdateHandle != -1)
			{
				Game.Instance.Unsubscribe(this.roleUpdateHandle);
			}
			this.roleUpdateHandle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
		}
		this.UpdateStatusItem(null);
	}

	protected virtual void UpdateStatusItem(object data = null)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		if (component == null)
		{
			return;
		}
		component.RemoveStatusItem(this.workStatusItemHandle, false);
		if (this.worker == null)
		{
			if (this.shouldShowRolePerkStatusItem && this.requiredRolePerk.IsValid)
			{
				if (Game.Instance.roleManager.GetRoleAssigneesWithPerk(this.requiredRolePerk).Count == 0)
				{
					this.workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.ColonyLacksRequiredRolePerk, this.requiredRolePerk);
				}
				else
				{
					this.workStatusItemHandle = component.AddStatusItem(this.readyForRoleWorkStatusItem, this.requiredRolePerk);
				}
			}
		}
		else if (this.workingStatusItem != null)
		{
			this.workStatusItemHandle = component.AddStatusItem(this.workingStatusItem, this);
		}
	}

	protected override void OnLoadLevel()
	{
		this.overrideAnims = null;
		base.OnLoadLevel();
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public void StartWork(Worker worker_to_start)
	{
		this.worker = worker_to_start;
		this.UpdateStatusItem(null);
		if (this.showProgressBar)
		{
			this.ShowProgressBar(true);
		}
		this.OnStartWork(this.worker);
		if (this.worker != null)
		{
			string conversationTopic = this.GetConversationTopic();
			if (conversationTopic != null)
			{
				this.worker.Trigger(937885943, conversationTopic);
			}
		}
		if (this.OnWorkableEventCB != null)
		{
			this.OnWorkableEventCB(Workable.WorkableEvent.WorkStarted);
		}
		this.numberOfUses++;
	}

	public bool WorkTick(Worker worker, float dt)
	{
		bool flag = false;
		if (dt > 0f)
		{
			this.workTimeRemaining -= dt;
			flag = this.OnWorkTick(worker, dt);
		}
		return flag || this.workTimeRemaining < 0f;
	}

	public virtual float GetEfficiencyMultiplier(Worker worker)
	{
		if (this.attributeConverter != null)
		{
			AttributeConverterInstance converter = worker.GetComponent<AttributeConverters>().GetConverter(this.attributeConverter.Id);
			return Mathf.Max(1f + converter.Evaluate(), 0.1f);
		}
		return 1f;
	}

	public virtual global::Klei.AI.Attribute GetWorkAttribute()
	{
		if (this.attributeConverter != null)
		{
			return this.attributeConverter.attribute;
		}
		return null;
	}

	public virtual string GetConversationTopic()
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		return (!component.HasTag(GameTags.NotAPrefab)) ? component.PrefabTag.Name : null;
	}

	public virtual void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	public void SetAttributeConverter(AttributeConverter attributeConverter)
	{
		this.attributeConverter = attributeConverter;
	}

	public float GetAttributeExperienceMultiplier()
	{
		return this.attributeExperienceMultiplier;
	}

	protected virtual bool OnWorkTick(Worker worker, float dt)
	{
		return false;
	}

	public void StopWork(Worker workerToStop, bool aborted)
	{
		if (this.worker == workerToStop && aborted)
		{
			this.OnAbortWork(workerToStop);
		}
		if (this.shouldTransferDiseaseWithWorker)
		{
			this.TransferDiseaseWithWorker(workerToStop);
		}
		if (this.OnWorkableEventCB != null)
		{
			this.OnWorkableEventCB(Workable.WorkableEvent.WorkStopped);
		}
		this.OnStopWork(workerToStop);
		if (this.resetProgressOnStop)
		{
			this.workTimeRemaining = this.GetWorkTime();
		}
		this.ShowProgressBar(false);
		this.worker = null;
		this.UpdateStatusItem(null);
	}

	public virtual StatusItem GetWorkerStatusItem()
	{
		return this.workerStatusItem;
	}

	public void SetWorkerStatusItem(StatusItem item)
	{
		this.workerStatusItem = item;
	}

	public void CompleteWork(Worker worker)
	{
		if (this.shouldTransferDiseaseWithWorker)
		{
			this.TransferDiseaseWithWorker(worker);
		}
		this.OnCompleteWork(worker);
		if (this.OnWorkableEventCB != null)
		{
			this.OnWorkableEventCB(Workable.WorkableEvent.WorkCompleted);
		}
		if (this.OnWorkableEventCB != null)
		{
			this.OnWorkableEventCB(Workable.WorkableEvent.WorkStopped);
		}
		this.workTimeRemaining = this.GetWorkTime();
		this.ShowProgressBar(false);
	}

	protected virtual void OnStartWork(Worker worker)
	{
	}

	protected virtual void OnStopWork(Worker worker)
	{
	}

	protected virtual void OnCompleteWork(Worker worker)
	{
	}

	protected virtual void OnAbortWork(Worker worker)
	{
	}

	public void SetOffsets(CellOffset[] offsets)
	{
		if (this.offsetTracker != null)
		{
			this.offsetTracker.Clear();
		}
		this.offsetTracker = new StandardOffsetTracker(offsets);
	}

	public void SetOffsetTable(CellOffset[][] offset_table)
	{
		if (this.offsetTracker != null)
		{
			this.offsetTracker.Clear();
		}
		this.offsetTracker = new OffsetTableTracker(offset_table, this);
	}

	public virtual CellOffset[] GetOffsets(int cell)
	{
		if (this.offsetTracker == null)
		{
			this.offsetTracker = new StandardOffsetTracker(new CellOffset[] { default(CellOffset) });
		}
		return this.offsetTracker.GetOffsets(cell);
	}

	public CellOffset[] GetOffsets()
	{
		return this.GetOffsets(Grid.PosToCell(this));
	}

	public void SetWorkTime(float work_time)
	{
		this.workTime = work_time;
		this.workTimeRemaining = work_time;
	}

	public bool ShouldFaceTargetWhenWorking()
	{
		return this.faceTargetWhenWorking;
	}

	public virtual Vector3 GetFacingTarget()
	{
		return base.transform.GetPosition();
	}

	public void ShowProgressBar(bool show)
	{
		if (show)
		{
			this.progressBar = ProgressBar.CreateProgressBar(this, new Func<float>(this.GetPercentComplete));
			this.progressBar.gameObject.SetActive(true);
		}
		else if (this.progressBar != null)
		{
			this.progressBar.gameObject.DeleteObject();
			this.progressBar = null;
		}
	}

	protected override void OnCleanUp()
	{
		this.ShowProgressBar(false);
		if (this.offsetTracker != null)
		{
			this.offsetTracker.Clear();
		}
		if (this.roleUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.roleUpdateHandle);
		}
		base.OnCleanUp();
		this.OnWorkableEventCB = null;
	}

	public virtual Vector3 GetTargetPoint()
	{
		Vector3 vector = base.transform.GetPosition();
		float num = vector.y + 0.65f;
		KBoxCollider2D component = base.GetComponent<KBoxCollider2D>();
		if (component != null)
		{
			vector = component.bounds.center;
		}
		vector.y = num;
		vector.z = 0f;
		return vector;
	}

	public int GetNavigationCost(Navigator navigator, int cell)
	{
		return navigator.GetNavigationCost(cell, this.GetOffsets(cell));
	}

	public int GetNavigationCost(Navigator navigator)
	{
		return this.GetNavigationCost(navigator, Grid.PosToCell(this));
	}

	private void TransferDiseaseWithWorker(Worker worker)
	{
		if (this == null || worker == null)
		{
			return;
		}
		Workable.TransferDiseaseWithWorker(base.gameObject, worker.gameObject);
	}

	public static void TransferDiseaseWithWorker(GameObject workable, GameObject worker)
	{
		if (workable == null || worker == null)
		{
			return;
		}
		PrimaryElement component = workable.GetComponent<PrimaryElement>();
		if (component == null)
		{
			return;
		}
		PrimaryElement component2 = worker.GetComponent<PrimaryElement>();
		if (component2 == null)
		{
			return;
		}
		SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
		invalid.idx = component2.DiseaseIdx;
		invalid.count = (int)((float)component2.DiseaseCount * 0.33f);
		SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
		invalid2.idx = component.DiseaseIdx;
		invalid2.count = (int)((float)component.DiseaseCount * 0.33f);
		component2.ModifyDiseaseCount(-invalid.count, "Workable.TransferDiseaseWithWorker");
		component.ModifyDiseaseCount(-invalid2.count, "Workable.TransferDiseaseWithWorker");
		if (invalid.count > 0)
		{
			component.AddDisease(invalid.idx, invalid.count, "Workable.TransferDiseaseWithWorker");
		}
		if (invalid2.count > 0)
		{
			component2.AddDisease(invalid2.idx, invalid2.count, "Workable.TransferDiseaseWithWorker");
		}
	}

	public virtual List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.trackUses)
		{
			Descriptor descriptor = new Descriptor(string.Format(BUILDING.DETAILS.USE_COUNT, this.numberOfUses), string.Format(BUILDING.DETAILS.USE_COUNT_TOOLTIP, this.numberOfUses), Descriptor.DescriptorType.Detail, false);
			list.Add(descriptor);
		}
		return list;
	}

	[ContextMenu("Refresh Reachability")]
	public void RefreshReachability()
	{
		if (this.offsetTracker != null)
		{
			this.offsetTracker.ForceRefresh();
		}
	}

	Transform IApproachable.get_transform()
	{
		return base.transform;
	}

	public float workTime;

	public Vector3 AnimOffset = Vector3.zero;

	protected bool showProgressBar = true;

	protected StatusItem workerStatusItem;

	protected StatusItem workingStatusItem;

	protected Guid workStatusItemHandle;

	protected OffsetTracker offsetTracker;

	protected AttributeConverter attributeConverter;

	public bool resetProgressOnStop;

	protected bool shouldTransferDiseaseWithWorker = true;

	protected float attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;

	public bool triggerWorkReactions = true;

	[SerializeField]
	[Tooltip("What layer does the dupe switch to when interacting with the building")]
	public Grid.SceneLayer workLayer = Grid.SceneLayer.Move;

	[SerializeField]
	[Serialize]
	protected float workTimeRemaining = float.PositiveInfinity;

	[SerializeField]
	public KAnimFile[] overrideAnims;

	[SerializeField]
	protected HashedString multitoolContext;

	[SerializeField]
	protected Tag multitoolHitEffectTag;

	[SerializeField]
	[Tooltip("Whether to user the KAnimSynchronizer or not")]
	public bool synchronizeAnims = true;

	[SerializeField]
	[Tooltip("Whether to display number of uses in the details panel")]
	public bool trackUses;

	[Serialize]
	protected int numberOfUses;

	public Action<Workable.WorkableEvent> OnWorkableEventCB;

	private int roleUpdateHandle = -1;

	public HashedString requiredRolePerk;

	[SerializeField]
	protected bool shouldShowRolePerkStatusItem = true;

	protected StatusItem readyForRoleWorkStatusItem;

	public HashedString[] workAnims = new HashedString[] { "working_pre", "working_loop" };

	public HashedString workingPstComplete = "working_pst";

	public HashedString workingPstFailed = "working_pst";

	public KAnim.PlayMode workAnimPlayMode;

	protected bool faceTargetWhenWorking;

	protected ProgressBar progressBar;

	public enum WorkableEvent
	{
		WorkStarted,
		WorkCompleted,
		WorkStopped
	}

	public struct AnimInfo
	{
		public KAnimFile[] overrideAnims;

		public StateMachine.Instance smi;
	}
}
