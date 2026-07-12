using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RocketControlStationLaunchWorkable")]
public class RocketControlStationLaunchWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_rocket_control_station_kanim") };
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = true;
		this.attributeConverter = Db.Get().AttributeConverters.PilotingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Rocketry.Id;
		this.skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
		base.SetWorkTime(30f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		RocketControlStation.StatesInstance smi = this.GetSMI<RocketControlStation.StatesInstance>();
		if (smi != null)
		{
			smi.SetPilotSpeedMult(worker);
			smi.LaunchRocket();
		}
	}

	[MyCmpReq]
	private Operational operational;
}
