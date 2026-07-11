using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AlgaeHabitatEmpty")]
public class AlgaeHabitatEmpty : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.workAnims = AlgaeHabitatEmpty.CLEAN_ANIMS;
		this.workingPstComplete = new HashedString[] { AlgaeHabitatEmpty.PST_ANIM };
		this.workingPstFailed = new HashedString[] { AlgaeHabitatEmpty.PST_ANIM };
		this.synchronizeAnims = false;
	}

	private static readonly HashedString[] CLEAN_ANIMS = new HashedString[] { "sponge_pre", "sponge_loop" };

	private static readonly HashedString PST_ANIM = new HashedString("sponge_pst");
}
