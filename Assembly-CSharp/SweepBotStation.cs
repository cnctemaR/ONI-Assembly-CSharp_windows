using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SweepBotStation")]
public class SweepBotStation : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Initialize(false);
		base.Subscribe<SweepBotStation>(-592767678, SweepBotStation.OnOperationalChangedDelegate);
	}

	protected void Initialize(bool use_logic_meter)
	{
		base.OnPrefabInit();
		base.GetComponent<Operational>().SetFlag(this.hasRobot, false);
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
			smi.sm.sweepLocker.Set(base.GetComponent<Storage>(), smi);
			this.RefreshSweepBotSubscription();
		}
		this.UpdateMeter();
		this.UpdateNameDisplay();
	}

	private void RequestNewSweepBot(object data = null)
	{
		if (this.storage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS) == null)
		{
			FetchList2 fetchList = new FetchList2(this.storage, Db.Get().ChoreTypes.Fetch);
			fetchList.Add(GameTags.RefinedMetal, null, null, SweepBotConfig.MASS, FetchOrder2.OperationalRequirement.None);
			fetchList.Submit(null, true);
			return;
		}
		this.MakeNewSweepBot(null);
	}

	private void MakeNewSweepBot(object data = null)
	{
		if (this.storage.GetAmountAvailable(GameTags.RefinedMetal) < SweepBotConfig.MASS)
		{
			return;
		}
		PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS);
		if (primaryElement == null)
		{
			return;
		}
		SimHashes sweepBotMaterial = primaryElement.ElementID;
		primaryElement.Mass -= SweepBotConfig.MASS;
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
			smi.sm.sweepLocker.Set(this.GetComponent<Storage>(), smi);
			this.sweepBot.Get().GetComponent<PrimaryElement>().ElementID = sweepBotMaterial;
			this.RefreshSweepBotSubscription();
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

	public void StartCharging()
	{
		base.GetComponent<Operational>().SetFlag(this.hasRobot, true);
		base.GetComponent<KBatchedAnimController>().Queue("sleep_pre", KAnim.PlayMode.Once, 1f, 0f);
		base.GetComponent<KBatchedAnimController>().Queue("sleep_idle", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void StopCharging()
	{
		base.GetComponent<Operational>().SetFlag(this.hasRobot, false);
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
		return this.storage.Capacity() - this.storage.storageFullMargin;
	}

	private float GetAmountStored()
	{
		return this.storage.MassStored();
	}

	[Serialize]
	public Ref<KSelectable> sweepBot;

	[Serialize]
	public string storedName;

	private Operational.Flag hasRobot = new Operational.Flag("hasRobot", Operational.Flag.Type.Functional);

	private MeterController meter;

	[MyCmpGet]
	private Storage storage;

	private SchedulerHandle newSweepyHandle;

	private static readonly EventSystem.IntraObjectHandler<SweepBotStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SweepBotStation>(delegate(SweepBotStation component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private int refreshSweepbotHandle = -1;

	private int sweepBotNameChangeHandle = -1;
}
