using System;
using TUNING;

public class MissionControlClusterWorkable : Workable
{
	public Clustercraft TargetClustercraft
	{
		get
		{
			return this.targetClustercraft;
		}
		set
		{
			base.WorkTimeRemaining = this.GetWorkTime();
			this.targetClustercraft = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.requiredSkillPerk = Db.Get().SkillPerks.CanMissionControl.Id;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.MissionControlling;
		this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_mission_control_station_kanim") };
		base.SetWorkTime(90f);
		this.showProgressBar = true;
		this.lightEfficiencyBonus = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.MissionControlClusterWorkables.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.MissionControlClusterWorkables.Remove(this);
		base.OnCleanUp();
	}

	public static bool IsRocketInRange(AxialI worldLocation, AxialI rocketLocation)
	{
		return AxialUtil.GetDistance(worldLocation, rocketLocation) <= 2;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.workStatusItem = base.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.MissionControlAssistingRocket, this.TargetClustercraft);
		this.operational.SetActive(true, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.TargetClustercraft == null || !MissionControlClusterWorkable.IsRocketInRange(base.gameObject.GetMyWorldLocation(), this.TargetClustercraft.Location))
		{
			worker.StopWork();
			return true;
		}
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Debug.Assert(this.TargetClustercraft != null);
		base.gameObject.GetSMI<MissionControlCluster.Instance>().ApplyEffect(this.TargetClustercraft);
		base.OnCompleteWork(worker);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		base.gameObject.GetComponent<KSelectable>().RemoveStatusItem(this.workStatusItem, false);
		this.TargetClustercraft = null;
		this.operational.SetActive(false, false);
	}

	private Clustercraft targetClustercraft;

	[MyCmpReq]
	private Operational operational;

	private Guid workStatusItem = Guid.Empty;
}
