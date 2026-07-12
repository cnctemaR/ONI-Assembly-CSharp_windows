using System;
using KSerialization;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ToiletWorkableClean")]
public class ToiletWorkableClean : Workable
{
	public bool IsCloggedByGunk { get; private set; }

	public void SetIsCloggedByGunk(bool isIt)
	{
		this.IsCloggedByGunk = isIt;
		this.workAnims = (this.IsCloggedByGunk ? ToiletWorkableClean.CLEAN_GUNK_ANIMS : ToiletWorkableClean.CLEAN_ANIMS);
		this.workingPstComplete = (this.IsCloggedByGunk ? ToiletWorkableClean.PST_GUNK_ANIM : ToiletWorkableClean.PST_ANIM);
		this.workingPstFailed = (this.IsCloggedByGunk ? ToiletWorkableClean.PST_GUNK_ANIM : ToiletWorkableClean.PST_ANIM);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		ToiletWorkableUse component = base.gameObject.GetComponent<ToiletWorkableUse>();
		if (component != null && this.IsCloggedByGunk && base.gameObject.GetComponent<FlushToilet>() == null)
		{
			LiquidSourceManager.Instance.CreateChunk(SimHashes.LiquidGunk, component.lastAmountOfWasteMassRemovedFromDupe, DUPLICANTSTATS.STANDARD.Temperature.Internal.IDEAL, byte.MaxValue, 0, Grid.CellToPos(Grid.PosToCell(base.gameObject), CellAlignment.Top, Grid.SceneLayer.Ore));
		}
		this.timesCleaned++;
		base.OnCompleteWork(worker);
	}

	[Serialize]
	public int timesCleaned;

	private static readonly HashedString[] CLEAN_GUNK_ANIMS = new HashedString[] { "degunk_pre", "degunk_loop" };

	private static readonly HashedString[] CLEAN_ANIMS = new HashedString[] { "unclog_pre", "unclog_loop" };

	private static readonly HashedString[] PST_ANIM = new HashedString[]
	{
		new HashedString("unclog_pst")
	};

	private static readonly HashedString[] PST_GUNK_ANIM = new HashedString[]
	{
		new HashedString("degunk_pst")
	};
}
