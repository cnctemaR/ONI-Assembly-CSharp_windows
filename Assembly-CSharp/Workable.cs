using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Workable")]
public class Workable : KMonoBehaviour, ISaveLoadable, IApproachable
{
	public WorkerBase worker { get; protected set; }

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

	public WorkerBase GetWorker()
	{
		return this.worker;
	}

	public virtual float GetPercentComplete()
	{
		if (this.workTimeRemaining > this.workTime)
		{
			return -1f;
		}
		return 1f - this.workTimeRemaining / this.workTime;
	}

	public void ConfigureMultitoolContext(HashedString context, Tag hitEffectTag)
	{
		this.multitoolContext = context;
		this.multitoolHitEffectTag = hitEffectTag;
	}

	public virtual Workable.AnimInfo GetAnim(WorkerBase worker)
	{
		Workable.AnimInfo animInfo = default(Workable.AnimInfo);
		if (this.overrideAnims != null && this.overrideAnims.Length != 0)
		{
			BuildingFacade buildingFacade = this.GetBuildingFacade();
			bool flag = false;
			if (buildingFacade != null && !buildingFacade.IsOriginal)
			{
				flag = buildingFacade.interactAnims.TryGetValue(base.name, out animInfo.overrideAnims);
			}
			if (!flag)
			{
				animInfo.overrideAnims = this.overrideAnims;
			}
		}
		if (this.multitoolContext.IsValid && this.multitoolHitEffectTag.IsValid)
		{
			animInfo.smi = new MultitoolController.Instance(this, worker, this.multitoolContext, Assets.GetPrefab(this.multitoolHitEffectTag));
		}
		return animInfo;
	}

	public virtual HashedString[] GetWorkAnims(WorkerBase worker)
	{
		return this.workAnims;
	}

	public virtual KAnim.PlayMode GetWorkAnimPlayMode()
	{
		return this.workAnimPlayMode;
	}

	public virtual HashedString[] GetWorkPstAnims(WorkerBase worker, bool successfully_completed)
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
		this.readyForSkillWorkStatusItem = Db.Get().BuildingStatusItems.RequiresSkillPerk;
		this.workTime = this.GetWorkTime();
		this.workTimeRemaining = Mathf.Min(this.workTimeRemaining, this.workTime);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.shouldShowSkillPerkStatusItem && !string.IsNullOrEmpty(this.requiredSkillPerk))
		{
			if (this.skillsUpdateHandle != -1)
			{
				Game.Instance.Unsubscribe(this.skillsUpdateHandle);
			}
			this.skillsUpdateHandle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
		}
		if (this.requireMinionToWork && this.minionUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.minionUpdateHandle);
		}
		this.minionUpdateHandle = Game.Instance.Subscribe(586301400, new Action<object>(this.UpdateStatusItem));
		base.GetComponent<KPrefabID>().AddTag(GameTags.HasChores, false);
		if (base.gameObject.HasTag(this.laboratoryEfficiencyBonusTagRequired))
		{
			this.useLaboratoryEfficiencyBonus = true;
			base.Subscribe<Workable>(144050788, Workable.OnUpdateRoomDelegate);
		}
		this.ShowProgressBar(this.alwaysShowProgressBar && this.workTimeRemaining < this.GetWorkTime());
		this.UpdateStatusItem(null);
	}

	private void RefreshRoom()
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(base.gameObject));
		if (cavityForCell != null && cavityForCell.room != null)
		{
			this.OnUpdateRoom(cavityForCell.room);
			return;
		}
		this.OnUpdateRoom(null);
	}

	private void OnUpdateRoom(object data)
	{
		if (this.worker == null)
		{
			return;
		}
		Room room = (Room)data;
		if (room != null && room.roomType == Db.Get().RoomTypes.Laboratory)
		{
			this.currentlyInLaboratory = true;
			if (this.laboratoryEfficiencyBonusStatusItemHandle == Guid.Empty)
			{
				this.laboratoryEfficiencyBonusStatusItemHandle = this.worker.OfferStatusItem(Db.Get().DuplicantStatusItems.LaboratoryWorkEfficiencyBonus, this);
				return;
			}
		}
		else
		{
			this.currentlyInLaboratory = false;
			if (this.laboratoryEfficiencyBonusStatusItemHandle != Guid.Empty)
			{
				this.worker.RevokeStatusItem(this.laboratoryEfficiencyBonusStatusItemHandle);
				this.laboratoryEfficiencyBonusStatusItemHandle = Guid.Empty;
			}
		}
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
			if (this.requireMinionToWork && Components.LiveMinionIdentities.GetWorldItems(this.GetMyWorldId(), false).Count == 0)
			{
				this.workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.WorkRequiresMinion, null);
				return;
			}
			if (this.shouldShowSkillPerkStatusItem && !string.IsNullOrEmpty(this.requiredSkillPerk))
			{
				if (!MinionResume.AnyMinionHasPerk(this.requiredSkillPerk, this.GetMyWorldId()))
				{
					StatusItem statusItem = (DlcManager.FeatureClusterSpaceEnabled() ? Db.Get().BuildingStatusItems.ClusterColonyLacksRequiredSkillPerk : Db.Get().BuildingStatusItems.ColonyLacksRequiredSkillPerk);
					this.workStatusItemHandle = component.AddStatusItem(statusItem, this.requiredSkillPerk);
					return;
				}
				this.workStatusItemHandle = component.AddStatusItem(this.readyForSkillWorkStatusItem, this.requiredSkillPerk);
				return;
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

	public virtual int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public void StartWork(WorkerBase worker_to_start)
	{
		global::Debug.Assert(worker_to_start != null, "How did we get a null worker?");
		this.worker = worker_to_start;
		this.UpdateStatusItem(null);
		if (this.showProgressBar)
		{
			this.ShowProgressBar(true);
		}
		if (this.useLaboratoryEfficiencyBonus)
		{
			this.RefreshRoom();
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
			this.OnWorkableEventCB(this, Workable.WorkableEvent.WorkStarted);
		}
		this.numberOfUses++;
		if (this.worker != null)
		{
			if (base.gameObject.GetComponent<KSelectable>() != null && base.gameObject.GetComponent<KSelectable>().IsSelected && this.worker.gameObject.GetComponent<LoopingSounds>() != null)
			{
				this.worker.gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(true);
			}
			else if (this.worker.gameObject.GetComponent<KSelectable>() != null && this.worker.gameObject.GetComponent<KSelectable>().IsSelected && base.gameObject.GetComponent<LoopingSounds>() != null)
			{
				base.gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(true);
			}
		}
		base.gameObject.Trigger(853695848, this);
	}

	public bool WorkTick(WorkerBase worker, float dt)
	{
		bool flag = false;
		if (dt > 0f)
		{
			this.workTimeRemaining -= dt;
			flag = this.OnWorkTick(worker, dt);
		}
		return flag || this.workTimeRemaining < 0f;
	}

	public virtual float GetEfficiencyMultiplier(WorkerBase worker)
	{
		float num = 1f;
		if (this.attributeConverter != null)
		{
			AttributeConverterInstance attributeConverterInstance = worker.GetAttributeConverter(this.attributeConverter.Id);
			if (attributeConverterInstance != null)
			{
				num += attributeConverterInstance.Evaluate();
			}
		}
		if (this.lightEfficiencyBonus)
		{
			int num2 = Grid.PosToCell(worker.gameObject);
			if (Grid.IsValidCell(num2))
			{
				if (Grid.LightIntensity[num2] > DUPLICANTSTATS.STANDARD.Light.NO_LIGHT)
				{
					this.currentlyLit = true;
					num += DUPLICANTSTATS.STANDARD.Light.LIGHT_WORK_EFFICIENCY_BONUS;
					if (this.lightEfficiencyBonusStatusItemHandle == Guid.Empty)
					{
						this.lightEfficiencyBonusStatusItemHandle = worker.OfferStatusItem(Db.Get().DuplicantStatusItems.LightWorkEfficiencyBonus, this);
					}
				}
				else
				{
					this.currentlyLit = false;
					if (this.lightEfficiencyBonusStatusItemHandle != Guid.Empty)
					{
						worker.RevokeStatusItem(this.lightEfficiencyBonusStatusItemHandle);
					}
				}
			}
		}
		if (this.useLaboratoryEfficiencyBonus && this.currentlyInLaboratory)
		{
			num += 0.1f;
		}
		return Mathf.Max(num, this.minimumAttributeMultiplier);
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
		if (!component.HasTag(GameTags.NotConversationTopic))
		{
			return component.PrefabTag.Name;
		}
		return null;
	}

	public float GetAttributeExperienceMultiplier()
	{
		return this.attributeExperienceMultiplier;
	}

	public string GetSkillExperienceSkillGroup()
	{
		return this.skillExperienceSkillGroup;
	}

	public float GetSkillExperienceMultiplier()
	{
		return this.skillExperienceMultiplier;
	}

	protected virtual bool OnWorkTick(WorkerBase worker, float dt)
	{
		return false;
	}

	public void StopWork(WorkerBase workerToStop, bool aborted)
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
			this.OnWorkableEventCB(this, Workable.WorkableEvent.WorkStopped);
		}
		this.OnStopWork(workerToStop);
		if (this.resetProgressOnStop)
		{
			this.workTimeRemaining = this.GetWorkTime();
		}
		this.ShowProgressBar(this.alwaysShowProgressBar && this.workTimeRemaining < this.GetWorkTime());
		if (this.lightEfficiencyBonusStatusItemHandle != Guid.Empty)
		{
			workerToStop.RevokeStatusItem(this.lightEfficiencyBonusStatusItemHandle);
			this.lightEfficiencyBonusStatusItemHandle = Guid.Empty;
		}
		if (this.laboratoryEfficiencyBonusStatusItemHandle != Guid.Empty)
		{
			this.worker.RevokeStatusItem(this.laboratoryEfficiencyBonusStatusItemHandle);
			this.laboratoryEfficiencyBonusStatusItemHandle = Guid.Empty;
		}
		if (base.gameObject.GetComponent<KSelectable>() != null && !base.gameObject.GetComponent<KSelectable>().IsSelected && base.gameObject.GetComponent<LoopingSounds>() != null)
		{
			base.gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(false);
		}
		else if (workerToStop.gameObject.GetComponent<KSelectable>() != null && !workerToStop.gameObject.GetComponent<KSelectable>().IsSelected && workerToStop.gameObject.GetComponent<LoopingSounds>() != null)
		{
			workerToStop.gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(false);
		}
		this.worker = null;
		base.gameObject.Trigger(679550494, this);
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

	public void CompleteWork(WorkerBase worker)
	{
		if (this.shouldTransferDiseaseWithWorker)
		{
			this.TransferDiseaseWithWorker(worker);
		}
		this.OnCompleteWork(worker);
		if (this.OnWorkableEventCB != null)
		{
			this.OnWorkableEventCB(this, Workable.WorkableEvent.WorkCompleted);
		}
		this.workTimeRemaining = this.GetWorkTime();
		this.ShowProgressBar(false);
		base.gameObject.Trigger(-2011693419, this);
	}

	public void SetReportType(ReportManager.ReportType report_type)
	{
		this.reportType = report_type;
	}

	public ReportManager.ReportType GetReportType()
	{
		return this.reportType;
	}

	protected virtual void OnStartWork(WorkerBase worker)
	{
	}

	protected virtual void OnStopWork(WorkerBase worker)
	{
	}

	protected virtual void OnCompleteWork(WorkerBase worker)
	{
	}

	protected virtual void OnAbortWork(WorkerBase worker)
	{
	}

	public virtual void OnPendingCompleteWork(WorkerBase worker)
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
			this.offsetTracker = new StandardOffsetTracker(new CellOffset[1]);
		}
		return this.offsetTracker.GetOffsets(cell);
	}

	public virtual bool ValidateOffsets(int cell)
	{
		if (this.offsetTracker == null)
		{
			this.offsetTracker = new StandardOffsetTracker(new CellOffset[1]);
		}
		return this.offsetTracker.ValidateOffsets(cell);
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
			if (this.progressBar == null)
			{
				this.progressBar = ProgressBar.CreateProgressBar(base.gameObject, new Func<float>(this.GetPercentComplete));
			}
			this.progressBar.SetVisibility(true);
			return;
		}
		if (this.progressBar != null)
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
		if (this.skillsUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.skillsUpdateHandle);
		}
		if (this.minionUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.minionUpdateHandle);
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

	private void TransferDiseaseWithWorker(WorkerBase worker)
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

	public void SetShouldShowSkillPerkStatusItem(bool shouldItBeShown)
	{
		this.shouldShowSkillPerkStatusItem = shouldItBeShown;
		if (this.skillsUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.skillsUpdateHandle);
			this.skillsUpdateHandle = -1;
		}
		if (this.shouldShowSkillPerkStatusItem && !string.IsNullOrEmpty(this.requiredSkillPerk))
		{
			this.skillsUpdateHandle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
		}
		this.UpdateStatusItem(null);
	}

	public virtual bool InstantlyFinish(WorkerBase worker)
	{
		float num = worker.GetWorkable().WorkTimeRemaining;
		if (!float.IsInfinity(num))
		{
			worker.Work(num);
			return true;
		}
		DebugUtil.DevAssert(false, this.ToString() + " was asked to instantly finish but it has infinite work time! Override InstantlyFinish in your workable!", null);
		return false;
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

	public virtual BuildingFacade GetBuildingFacade()
	{
		return base.GetComponent<BuildingFacade>();
	}

	public virtual KAnimControllerBase GetAnimController()
	{
		return base.GetComponent<KAnimControllerBase>();
	}

	[ContextMenu("Refresh Reachability")]
	public void RefreshReachability()
	{
		if (this.offsetTracker != null)
		{
			this.offsetTracker.ForceRefresh();
		}
	}

	public float workTime;

	protected bool showProgressBar = true;

	public bool alwaysShowProgressBar;

	public bool surpressWorkerForceSync;

	protected bool lightEfficiencyBonus = true;

	protected Guid lightEfficiencyBonusStatusItemHandle;

	public bool currentlyLit;

	public Tag laboratoryEfficiencyBonusTagRequired = RoomConstraints.ConstraintTags.ScienceBuilding;

	private bool useLaboratoryEfficiencyBonus;

	protected Guid laboratoryEfficiencyBonusStatusItemHandle;

	private bool currentlyInLaboratory;

	protected StatusItem workerStatusItem;

	protected StatusItem workingStatusItem;

	protected Guid workStatusItemHandle;

	protected OffsetTracker offsetTracker;

	[SerializeField]
	protected string attributeConverterId;

	protected AttributeConverter attributeConverter;

	protected float minimumAttributeMultiplier = 0.5f;

	public bool resetProgressOnStop;

	protected bool shouldTransferDiseaseWithWorker = true;

	[SerializeField]
	protected float attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;

	[SerializeField]
	protected string skillExperienceSkillGroup;

	[SerializeField]
	protected float skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;

	public bool triggerWorkReactions = true;

	public ReportManager.ReportType reportType = ReportManager.ReportType.WorkTime;

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

	public Action<Workable, Workable.WorkableEvent> OnWorkableEventCB;

	protected int skillsUpdateHandle = -1;

	private int minionUpdateHandle = -1;

	public string requiredSkillPerk;

	[SerializeField]
	protected bool shouldShowSkillPerkStatusItem = true;

	[SerializeField]
	public bool requireMinionToWork;

	protected StatusItem readyForSkillWorkStatusItem;

	public HashedString[] workAnims = new HashedString[] { "working_pre", "working_loop" };

	public HashedString[] workingPstComplete = new HashedString[] { "working_pst" };

	public HashedString[] workingPstFailed = new HashedString[] { "working_pst" };

	public KAnim.PlayMode workAnimPlayMode;

	public bool faceTargetWhenWorking;

	private static readonly EventSystem.IntraObjectHandler<Workable> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<Workable>(delegate(Workable component, object data)
	{
		component.OnUpdateRoom(data);
	});

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
