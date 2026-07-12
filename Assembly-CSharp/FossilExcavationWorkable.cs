using System;
using TUNING;

public abstract class FossilExcavationWorkable : Workable
{
	protected abstract bool IsMarkedForExcavation();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workingStatusItem = Db.Get().BuildingStatusItems.FossilHuntExcavationInProgress;
		base.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.FossilHunt_WorkerExcavating);
		this.requiredSkillPerk = Db.Get().SkillPerks.CanArtGreat.Id;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_fossils_small_kanim") };
		this.attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		this.lightEfficiencyBonus = true;
		this.synchronizeAnims = false;
		this.shouldShowSkillPerkStatusItem = false;
	}

	protected override void UpdateStatusItem(object data = null)
	{
		base.UpdateStatusItem(data);
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.waitingWorkStatusItemHandle != default(Guid))
		{
			component.RemoveStatusItem(this.waitingWorkStatusItemHandle, false);
		}
		if (base.worker == null && this.IsMarkedForExcavation())
		{
			this.waitingWorkStatusItemHandle = component.AddStatusItem(this.waitingForExcavationWorkStatusItem, null);
		}
	}

	protected Guid waitingWorkStatusItemHandle;

	protected StatusItem waitingForExcavationWorkStatusItem = Db.Get().BuildingStatusItems.FossilHuntExcavationOrdered;
}
