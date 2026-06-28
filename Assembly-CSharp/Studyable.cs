using System;
using KSerialization;
using STRINGS;
using TUNING;

public class Studyable : Workable, ISidescreenButtonControl
{
	public bool Studied
	{
		get
		{
			return this.studied;
		}
	}

	public string SidescreenTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.TITLE";
		}
	}

	public string SidescreenStatusMessage
	{
		get
		{
			if (this.studied)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.STUDIED_STATUS;
			}
			if (this.markedForStudy)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.PENDING_STATUS;
			}
			return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.SEND_STATUS;
		}
	}

	public string SidescreenButtonText
	{
		get
		{
			if (this.studied)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.STUDIED_BUTTON;
			}
			if (this.markedForStudy)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.PENDING_BUTTON;
			}
			return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.SEND_BUTTON;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_machine_kanim") };
		this.faceTargetWhenWorking = true;
		this.synchronizeAnims = false;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Studying;
		this.resetProgressOnStop = false;
		this.requiredRolePerk = RoleManager.rolePerks.CanStudyWorldObjects.id;
		this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		base.SetWorkTime(3600f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.studiedIndicator = new MeterController(base.GetComponent<KBatchedAnimController>(), this.meterTrackerSymbol, this.meterAnim, Meter.Offset.Infront, new string[] { this.meterTrackerSymbol });
		this.Refresh();
	}

	public void CancelChore()
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Studyable.CancelChore");
			this.chore = null;
		}
	}

	public void Refresh()
	{
		if (KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.studied)
		{
			this.statusItemGuid = component.ReplaceStatusItem(this.statusItemGuid, Db.Get().MiscStatusItems.Studied, null);
			this.studiedIndicator.gameObject.SetActive(true);
			this.studiedIndicator.meterController.Play(this.meterAnim, KAnim.PlayMode.Loop, 1f, 0f);
			this.requiredRolePerk = HashedString.Invalid;
			this.UpdateStatusItem(null);
		}
		else
		{
			if (this.markedForStudy)
			{
				if (this.chore == null)
				{
					this.chore = new WorkChore<Studyable>(Db.Get().ChoreTypes.Research, this, null, null, true, null, null, null, true, null, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
				}
				this.statusItemGuid = component.ReplaceStatusItem(this.statusItemGuid, Db.Get().MiscStatusItems.AwaitingStudy, null);
			}
			else
			{
				this.CancelChore();
				this.statusItemGuid = component.RemoveStatusItem(this.statusItemGuid, false);
			}
			this.studiedIndicator.gameObject.SetActive(false);
		}
	}

	private void ToggleStudyChore()
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.studied = true;
			if (this.chore != null)
			{
				this.chore.Cancel("debug");
				this.chore = null;
			}
		}
		else
		{
			this.markedForStudy = !this.markedForStudy;
		}
		this.Refresh();
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(JuniorResearcher.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole(Researcher.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole(SeniorResearcher.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.studied = true;
		this.chore = null;
		this.Refresh();
	}

	public void OnSidescreenButtonPressed()
	{
		this.ToggleStudyChore();
	}

	public string meterTrackerSymbol;

	public string meterAnim;

	private Chore chore;

	private const float STUDY_WORK_TIME = 3600f;

	[Serialize]
	private bool studied;

	[Serialize]
	private bool markedForStudy;

	private Guid statusItemGuid;

	private Guid additionalStatusItemGuid;

	private MeterController studiedIndicator;
}
