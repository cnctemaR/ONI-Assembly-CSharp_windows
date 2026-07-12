using System;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SweepBotStation")]
public class SweepBotStation : KMonoBehaviour
{
	public void SetStorages(Storage botMaterialStorage, Storage sweepStorage)
	{
		this.botMaterialStorage = botMaterialStorage;
		this.sweepStorage = sweepStorage;
	}

	protected override void OnPrefabInit()
	{
		this.Initialize(false);
		base.Subscribe<SweepBotStation>(-592767678, SweepBotStation.OnOperationalChangedDelegate);
	}

	protected void Initialize(bool use_logic_meter)
	{
		base.OnPrefabInit();
		base.GetComponent<Operational>().SetFlag(SweepBotStation.dockedRobot, false);
	}

	protected override void OnSpawn()
	{
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		this.meter = new MeterController(base.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_frame", "meter_level" });
		if (this.sweepBot == null || this.sweepBot.Get() == null)
		{
			this.RequestNewSweepBot(null);
		}
		else
		{
			StorageUnloadMonitor.Instance smi = this.sweepBot.Get().GetSMI<StorageUnloadMonitor.Instance>();
			smi.sm.sweepLocker.Set(this.sweepStorage, smi, false);
			this.RefreshSweepBotSubscription();
		}
		this.UpdateMeter();
		this.UpdateNameDisplay();
	}

	private void RequestNewSweepBot(object data = null)
	{
		if (this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS) == null)
		{
			FetchList2 fetchList = new FetchList2(this.botMaterialStorage, Db.Get().ChoreTypes.Fetch);
			fetchList.Add(GameTags.RefinedMetal, null, SweepBotConfig.MASS, Operational.State.None);
			fetchList.Submit(null, true);
			return;
		}
		this.MakeNewSweepBot(null);
	}

	private void MakeNewSweepBot(object data = null)
	{
		if (this.newSweepyHandle.IsValid)
		{
			return;
		}
		if (this.botMaterialStorage.GetAmountAvailable(GameTags.RefinedMetal) < SweepBotConfig.MASS)
		{
			return;
		}
		PrimaryElement primaryElement = this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS);
		if (primaryElement == null)
		{
			return;
		}
		SimHashes sweepBotMaterial = primaryElement.ElementID;
		float temperature;
		SimUtil.DiseaseInfo disease;
		float num;
		this.botMaterialStorage.ConsumeAndGetDisease(sweepBotMaterial.CreateTag(), SweepBotConfig.MASS, out num, out disease, out temperature);
		this.UpdateMeter();
		this.newSweepyHandle = GameScheduler.Instance.Schedule("MakeSweepy", 2f, delegate(object obj)
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("SweepBot"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures, null, 0);
			gameObject.SetActive(true);
			this.sweepBot = new Ref<KSelectable>(gameObject.GetComponent<KSelectable>());
			if (!string.IsNullOrEmpty(this.storedName))
			{
				this.sweepBot.Get().GetComponent<UserNameable>().SetName(this.storedName);
			}
			this.UpdateNameDisplay();
			StorageUnloadMonitor.Instance smi = gameObject.GetSMI<StorageUnloadMonitor.Instance>();
			smi.sm.sweepLocker.Set(this.sweepStorage, smi, false);
			PrimaryElement component = this.sweepBot.Get().GetComponent<PrimaryElement>();
			component.ElementID = sweepBotMaterial;
			component.Temperature = temperature;
			if (disease.idx != 255)
			{
				component.AddDisease(disease.idx, disease.count, "Inherited from the material used for its creation");
			}
			this.RefreshSweepBotSubscription();
			this.newSweepyHandle.ClearScheduler();
		}, null, null);
		base.GetComponent<KBatchedAnimController>().Play("newsweepy", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void RefreshSweepBotSubscription()
	{
		if (this.refreshSweepbotHandle != -1)
		{
			this.sweepBot.Get().Unsubscribe(this.refreshSweepbotHandle);
			this.sweepBot.Get().Unsubscribe(this.sweepBotNameChangeHandle);
		}
		this.refreshSweepbotHandle = this.sweepBot.Get().Subscribe(1969584890, new Action<object>(this.RequestNewSweepBot));
		this.sweepBotNameChangeHandle = this.sweepBot.Get().Subscribe(1102426921, new Action<object>(this.UpdateStoredName));
	}

	private void UpdateStoredName(object data)
	{
		this.storedName = (string)data;
		this.UpdateNameDisplay();
	}

	private void UpdateNameDisplay()
	{
		if (string.IsNullOrEmpty(this.storedName))
		{
			base.GetComponent<KSelectable>().SetName(string.Format(BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, ROBOTS.MODELS.SWEEPBOT.NAME));
		}
		else
		{
			base.GetComponent<KSelectable>().SetName(string.Format(BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, this.storedName));
		}
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public void DockRobot(bool docked)
	{
		base.GetComponent<Operational>().SetFlag(SweepBotStation.dockedRobot, docked);
	}

	public void StartCharging()
	{
		base.GetComponent<KBatchedAnimController>().Queue("sleep_pre", KAnim.PlayMode.Once, 1f, 0f);
		base.GetComponent<KBatchedAnimController>().Queue("sleep_idle", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void StopCharging()
	{
		base.GetComponent<KBatchedAnimController>().Play("sleep_pst", KAnim.PlayMode.Once, 1f, 0f);
		this.UpdateNameDisplay();
	}

	protected override void OnCleanUp()
	{
		if (this.newSweepyHandle.IsValid)
		{
			this.newSweepyHandle.ClearScheduler();
		}
		if (this.refreshSweepbotHandle != -1 && this.sweepBot.Get() != null)
		{
			this.sweepBot.Get().Unsubscribe(this.refreshSweepbotHandle);
		}
	}

	private void UpdateMeter()
	{
		float maxCapacityMinusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
		float num = Mathf.Clamp01(this.GetAmountStored() / maxCapacityMinusStorageMargin);
		if (this.meter != null)
		{
			this.meter.SetPositionPercent(num);
		}
	}

	private void OnStorageChanged(object data)
	{
		this.UpdateMeter();
		if (this.sweepBot == null || this.sweepBot.Get() == null)
		{
			this.RequestNewSweepBot(null);
		}
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (component.currentFrame >= component.GetCurrentNumFrames())
		{
			base.GetComponent<KBatchedAnimController>().Play("remove", KAnim.PlayMode.Once, 1f, 0f);
		}
		for (int i = 0; i < this.sweepStorage.Count; i++)
		{
			this.sweepStorage[i].GetComponent<Clearable>().MarkForClear(false, true);
		}
	}

	private void OnOperationalChanged(object data)
	{
		Operational component = base.GetComponent<Operational>();
		if (component.Flags.ContainsValue(false))
		{
			component.SetActive(false, false);
		}
		else
		{
			component.SetActive(true, false);
		}
		if (this.sweepBot == null || this.sweepBot.Get() == null)
		{
			this.RequestNewSweepBot(null);
		}
	}

	private float GetMaxCapacityMinusStorageMargin()
	{
		return this.sweepStorage.Capacity() - this.sweepStorage.storageFullMargin;
	}

	private float GetAmountStored()
	{
		return this.sweepStorage.MassStored();
	}

	[Serialize]
	public Ref<KSelectable> sweepBot;

	[Serialize]
	public string storedName;

	private static readonly Operational.Flag dockedRobot = new Operational.Flag("dockedRobot", Operational.Flag.Type.Functional);

	private MeterController meter;

	[SerializeField]
	private Storage botMaterialStorage;

	[SerializeField]
	private Storage sweepStorage;

	private SchedulerHandle newSweepyHandle;

	private static readonly EventSystem.IntraObjectHandler<SweepBotStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SweepBotStation>(delegate(SweepBotStation component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private int refreshSweepbotHandle = -1;

	private int sweepBotNameChangeHandle = -1;
}
