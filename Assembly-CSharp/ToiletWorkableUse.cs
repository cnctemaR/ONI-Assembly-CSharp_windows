using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ToiletWorkableUse")]
public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
	private ToiletWorkableUse()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		base.SetWorkTime(8.5f);
	}

	protected override void OnStartWork(WorkerBase worker)
	{
		base.OnStartWork(worker);
		if (Sim.IsRadiationEnabled() && worker.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).value > 0f)
		{
			worker.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads, null);
		}
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			roomOfGameObject.roomType.TriggerRoomEffects(base.GetComponent<KPrefabID>(), worker.GetComponent<Effects>());
		}
		if (worker != null)
		{
			this.last_user_id = worker.gameObject.PrefabID();
		}
	}

	public override HashedString[] GetWorkPstAnims(WorkerBase worker, bool successfully_completed)
	{
		HashedString[] array = null;
		if (this.workerTypePstAnims.TryGetValue(worker.PrefabID(), out array))
		{
			this.workingPstComplete = array;
			this.workingPstFailed = array;
		}
		return base.GetWorkPstAnims(worker, successfully_completed);
	}

	public override Workable.AnimInfo GetAnim(WorkerBase worker)
	{
		KAnimFile[] array = null;
		if (this.workerTypeOverrideAnims.TryGetValue(worker.PrefabID(), out array))
		{
			this.overrideAnims = array;
		}
		return base.GetAnim(worker);
	}

	protected override void OnStopWork(WorkerBase worker)
	{
		if (Sim.IsRadiationEnabled())
		{
			worker.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads, false);
		}
		base.OnStopWork(worker);
	}

	protected override void OnAbortWork(WorkerBase worker)
	{
		if (Sim.IsRadiationEnabled())
		{
			worker.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads, false);
		}
		base.OnAbortWork(worker);
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(worker);
		if (amountInstance != null)
		{
			this.lastAmountOfWasteMassRemovedFromDupe = DUPLICANTSTATS.STANDARD.Secretions.PEE_PER_TOILET_PEE;
			this.lastElementRemovedFromDupe = SimHashes.DirtyWater;
			amountInstance.SetValue(0f);
		}
		else
		{
			GunkMonitor.Instance smi = worker.GetSMI<GunkMonitor.Instance>();
			if (smi != null)
			{
				this.lastAmountOfWasteMassRemovedFromDupe = smi.CurrentGunkMass;
				this.lastElementRemovedFromDupe = GunkMonitor.GunkElement;
				smi.SetGunkMassValue(0f);
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_GunkedToilet, true);
			}
		}
		if (Sim.IsRadiationEnabled())
		{
			worker.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads, false);
			AmountInstance amountInstance2 = Db.Get().Amounts.RadiationBalance.Lookup(worker);
			RadiationMonitor.Instance smi2 = worker.GetSMI<RadiationMonitor.Instance>();
			float num = Math.Min(amountInstance2.value, 100f * smi2.difficultySettingMod);
			if (num >= 1f)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, Math.Floor((double)num).ToString() + UI.UNITSUFFIXES.RADIATION.RADS, worker.transform, Vector3.up * 2f, 1.5f, false, false);
			}
			amountInstance2.ApplyDelta(-num);
		}
		this.timesUsed++;
		if (amountInstance != null)
		{
			base.Trigger(-350347868, worker);
		}
		else
		{
			base.Trigger(1234642927, worker);
		}
		base.OnCompleteWork(worker);
	}

	public override StatusItem GetWorkerStatusItem()
	{
		if (base.worker != null && base.worker.gameObject.HasTag(GameTags.Minions.Models.Bionic))
		{
			return Db.Get().DuplicantStatusItems.CloggingToilet;
		}
		return base.GetWorkerStatusItem();
	}

	public Dictionary<Tag, KAnimFile[]> workerTypeOverrideAnims = new Dictionary<Tag, KAnimFile[]>();

	public Dictionary<Tag, HashedString[]> workerTypePstAnims = new Dictionary<Tag, HashedString[]>();

	[Serialize]
	public int timesUsed;

	[Serialize]
	public Tag last_user_id;

	[Serialize]
	public SimHashes lastElementRemovedFromDupe = SimHashes.DirtyWater;

	[Serialize]
	public float lastAmountOfWasteMassRemovedFromDupe;
}
