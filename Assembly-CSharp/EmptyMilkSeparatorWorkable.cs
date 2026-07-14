using System;
using TUNING;
using UnityEngine;

public class EmptyMilkSeparatorWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workLayer = Grid.SceneLayer.BuildingFront;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_milk_separator_kanim") };
		this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		base.SetWorkTime(15f);
		this.synchronizeAnims = true;
		this.SetupDroppedItemSymbol();
	}

	private void SetupDroppedItemSymbol()
	{
		KBatchedAnimController component = base.gameObject.GetComponent<KBatchedAnimController>();
		GameObject gameObject = Util.NewGameObject(base.gameObject, base.gameObject.name + ".dropped_item_symbol");
		gameObject.SetActive(false);
		bool flag;
		Vector3 vector = component.GetSymbolTransform(EmptyMilkSeparatorWorkable.DROPPED_SYMBOL_HASH, out flag).GetColumn(3);
		vector.z = component.transform.GetPosition().z - 0.05f;
		gameObject.transform.SetPosition(vector);
		this.droppedItemController = gameObject.AddComponent<KBatchedAnimController>();
		this.droppedItemController.AnimFiles = new KAnimFile[] { Assets.GetAnim("milkfat_kanim") };
		this.droppedItemController.initialAnim = "idle1";
		component.SetSymbolVisiblity(EmptyMilkSeparatorWorkable.DROPPED_SYMBOL_HASH, false);
		KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.symbol = EmptyMilkSeparatorWorkable.DROPPED_SYMBOL_HASH;
		kbatchedAnimTracker.offset = Vector3.zero;
	}

	public override void OnPendingCompleteWork(WorkerBase worker)
	{
		global::System.Action onWork_PST_Begins = this.OnWork_PST_Begins;
		if (onWork_PST_Begins != null)
		{
			onWork_PST_Begins();
		}
		this.ShowDroppedItemSymbol();
		this.TintDupeFatInHandSymbol(worker);
		base.OnPendingCompleteWork(worker);
	}

	protected override void OnStopWork(WorkerBase worker)
	{
		this.HideDroppedItemSymbol();
		this.ClearDupeFatInHandColor(worker);
	}

	private void ShowDroppedItemSymbol()
	{
		MilkSeparator.Instance smi = base.gameObject.GetSMI<MilkSeparator.Instance>();
		if (smi == null)
		{
			return;
		}
		bool flag = smi.MilkFatStored >= smi.CaviarStored;
		HashedString hashedString = (flag ? "milkfat_kanim" : "caviar_kanim");
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim(hashedString) };
		this.droppedItemController.SwapAnims(array);
		this.droppedItemController.gameObject.SetActive(true);
		this.droppedItemController.Play(flag ? "idle2" : "object", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void HideDroppedItemSymbol()
	{
		this.droppedItemController.gameObject.SetActive(false);
	}

	private void TintDupeFatInHandSymbol(WorkerBase worker)
	{
		if (worker == null)
		{
			return;
		}
		MilkSeparator.Instance smi = base.gameObject.GetSMI<MilkSeparator.Instance>();
		if (smi == null)
		{
			return;
		}
		worker.GetComponent<KBatchedAnimController>().SetSymbolTint("fat_goop", smi.GetFatColor());
	}

	private void ClearDupeFatInHandColor(WorkerBase worker)
	{
		if (worker == null)
		{
			return;
		}
		KBatchedAnimController component = worker.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.SetSymbolTint("fat_goop", Color.white);
		}
	}

	public global::System.Action OnWork_PST_Begins;

	private static readonly HashedString DROPPED_SYMBOL_HASH = "object";

	private const string DROPPED_SYMBOL_NAME = "object";

	private const string FAT_ON_HAND_SYMBOL_NAME = "fat_goop";

	private KBatchedAnimController droppedItemController;
}
