using System;
using KSerialization;
using TUNING;

public class ClamHarvestable : Harvestable
{
	public bool IsClosedAndReadyForHarvesting
	{
		get
		{
			return !this.hasBeenOpened && this.growing.IsGrown();
		}
	}

	protected override void OnPrefabInit()
	{
		this.standardCropPlant = base.GetComponent<StandardCropPlant>();
		this.growing = base.GetComponent<Growing>();
		Components.ClamHarvestables.Add(this);
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.skillsUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.skillsUpdateHandle);
		}
		this.skillsUpdateHandle = Game.Instance.Subscribe(-1523247426, Workable.UpdateStatusItemDispatcher, this);
		base.Subscribe(-266953818, new Action<object>(this.OnHarvestDesignationChanged));
		base.Subscribe(1272413801, new Action<object>(this.OnHarvested));
		this.SetupWorkable();
		this.UpdateHarvestReadyAnimations(true);
	}

	private void SetupWorkable()
	{
		if (this.hasBeenOpened)
		{
			this.SetupWorkableForHarvestClam();
			this.growing.shouldGrowOld = true;
			this.growing.smi.ModifyOldAgeGrowthRate(1f);
		}
		else
		{
			this.SetupWorkableForOpenClam();
			this.growing.shouldGrowOld = false;
			this.growing.smi.ModifyOldAgeGrowthRate(0f);
		}
		this.UpdateStatusItem(null);
	}

	public override void OnMarkedForHarvest()
	{
		base.OnMarkedForHarvest();
		this.SetupWorkable();
	}

	private void OnHarvestDesignationChanged(object data)
	{
		this.SetupWorkable();
	}

	private void OnHarvested(object data)
	{
		if (this.hasBeenOpened)
		{
			this.hasBeenOpened = false;
			this.UpdateHarvestReadyAnimations(false);
			this.SetupWorkable();
		}
	}

	private void SetupWorkableForOpenClam()
	{
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Harvesting;
		this.multitoolContext = default(HashedString);
		this.multitoolHitEffectTag = null;
		this.faceTargetWhenWorking = true;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_sculpture_kanim") };
		this.synchronizeAnims = false;
		this.attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.requiredSkillPerk = Db.Get().SkillPerks.CanFarmClams.Id;
		this.shouldShowSkillPerkStatusItem = base.CanBeHarvested && this.harvestDesignatable.HarvestWhenReady;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		if (this.offsetTracker != null)
		{
			this.offsetTracker.Clear();
			this.offsetTracker = null;
		}
		base.SetWorkTime(10f);
	}

	private void SetupWorkableForHarvestClam()
	{
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Harvesting;
		this.multitoolContext = "harvest";
		this.multitoolHitEffectTag = "fx_harvest_splash";
		this.faceTargetWhenWorking = true;
		this.overrideAnims = null;
		this.synchronizeAnims = false;
		this.attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.requiredSkillPerk = null;
		this.shouldShowSkillPerkStatusItem = false;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		base.SetWorkTime(10f);
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		if (!this.hasBeenOpened)
		{
			this.hasBeenOpened = true;
			this.UpdateHarvestReadyAnimations(true);
			this.chore = null;
			this.SetupWorkable();
			return;
		}
		base.OnCompleteWork(worker);
	}

	public void PunchOpen()
	{
		if (!this.hasBeenOpened)
		{
			this.hasBeenOpened = true;
			if (base.worker != null && this.chore != null)
			{
				this.chore.Cancel("Punched open while being worked on");
			}
			this.UpdateHarvestReadyAnimations(true);
			this.chore = null;
			this.SetupWorkable();
		}
	}

	private void UpdateHarvestReadyAnimations(bool refresh = true)
	{
		if (this.hasBeenOpened)
		{
			this.standardCropPlant.anims = ClamConfig.CROP_PLANT_DEFAULT_ANIM_SET;
		}
		else
		{
			this.standardCropPlant.anims = ClamConfig.CROP_PLANT_CLOSED_ANIM_SET;
		}
		if ((refresh && this.standardCropPlant.smi.IsInsideState(this.standardCropPlant.smi.sm.alive.fruiting)) || this.standardCropPlant.smi.IsInsideState(this.standardCropPlant.smi.sm.alive.pre_fruiting))
		{
			this.standardCropPlant.smi.GoTo(this.standardCropPlant.smi.sm.alive.idle);
		}
	}

	protected override void OnCleanUp()
	{
		Components.ClamHarvestables.Remove(this);
		base.OnCleanUp();
	}

	[Serialize]
	protected bool hasBeenOpened;

	private StandardCropPlant standardCropPlant;

	private Growing growing;
}
