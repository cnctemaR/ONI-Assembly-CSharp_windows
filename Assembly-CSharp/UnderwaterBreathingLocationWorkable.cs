using System;
using Klei;
using Klei.AI;
using TUNING;

public class UnderwaterBreathingLocationWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		this.workTime = 150f;
		this.workAnims = new HashedString[] { "working_pre", "working_loop" };
		this.workingPstComplete = new HashedString[] { "working_pst" };
		this.workingPstFailed = new HashedString[] { "working_pst" };
		this.resetProgressOnStop = false;
		this.showProgressBar = false;
		this.faceTargetWhenWorking = true;
		this.workLayer = Grid.SceneLayer.BuildingUse;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_underwater_breathing_station_kanim") };
		base.OnPrefabInit();
	}

	protected override void OnStartWork(WorkerBase worker)
	{
		base.OnStartWork(worker);
		base.SetWorkTime(150f);
		worker.GetComponent<KPrefabID>().AddTag(GameTags.RecoveringBreath, false);
		worker.Trigger(961737054, null);
		this.breather = worker.GetComponent<OxygenBreather>();
		this.breath = Db.Get().Amounts.Breath.Lookup(worker);
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
	{
		if (this.breather == null || this.breath == null)
		{
			return true;
		}
		float num = this.breather.ConsumptionRate * dt * 50f;
		float num2;
		SimUtil.DiseaseInfo diseaseInfo;
		float num3;
		SimHashes simHashes;
		this.storage.ConsumeAndGetDisease(GameTags.Breathable, num, out num2, out diseaseInfo, out num3, out simHashes);
		if (num2 > 0f)
		{
			OxygenBreather.BreathableGasConsumed(this.breather, simHashes, num2, num3, diseaseInfo.idx, diseaseInfo.count);
			this.breath.ApplyDelta(num2 * DUPLICANTSTATS.STANDARD.BaseStats.RECOVER_BREATH_DELTA);
		}
		return this.storage.FindFirstWithMass(GameTags.Breathable, 0f) == null;
	}

	protected override void OnStopWork(WorkerBase worker)
	{
		worker.GetComponent<KPrefabID>().RemoveTag(GameTags.RecoveringBreath);
		worker.Trigger(-2037519664, null);
		base.OnStopWork(worker);
	}

	[MyCmpReq]
	private Storage storage;

	private OxygenBreather breather;

	private AmountInstance breath;
}
