using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[SkipSaveFileSerialization]
public class ElementConsumer : KMonoBehaviour, ISaveLoadable, IEffectDescriptor
{
	public float AverageConsumeRate
	{
		get
		{
			return this.accumulator.AvgRate;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.accumulator = new Accumulator("Element", this, 3f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.elementToConsume == SimHashes.Void)
		{
			throw new ArgumentException("No consumable elements specified");
		}
		this.SimRegister();
		this.Subscribe(824508782, new Action<object>(this.OnActiveChanged));
		if (this.capacityKG != float.PositiveInfinity)
		{
			this.hasAvailableCapacity = this.IsStorageFull();
			this.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		}
	}

	protected override void OnCleanUp()
	{
		this.SimUnregister();
		base.OnForcedCleanUp();
	}

	protected virtual bool IsActive()
	{
		return this.operational == null || this.operational.IsActive;
	}

	public void EnableConsumption(bool enabled)
	{
		bool flag = this.consumptionEnabled;
		this.consumptionEnabled = enabled;
		if (!Sim.IsValidHandle(this.simHandle))
		{
			return;
		}
		if (enabled != flag)
		{
			this.UpdateSimData();
			this.UpdateStatusItem();
		}
	}

	private bool IsStorageFull()
	{
		PrimaryElement primaryElement = this.storage.FindPrimaryElement(this.elementToConsume);
		return primaryElement != null && primaryElement.Mass >= this.capacityKG;
	}

	public void RefreshConsumptionRate()
	{
		if (!Sim.IsValidHandle(this.simHandle))
		{
			return;
		}
		this.UpdateSimData();
	}

	private void UpdateSimData()
	{
		SimMessages.SetElementConsumerData(this.simHandle, (!this.consumptionEnabled || !this.hasAvailableCapacity) ? 0f : this.consumptionRate);
	}

	public static void AddMass(Sim.ConsumedMassInfo consumed_info)
	{
		if (!Sim.IsValidHandle(consumed_info.simHandle))
		{
			return;
		}
		ElementConsumer elementConsumer = null;
		if (ElementConsumer.handleInstanceMap.TryGetValue(consumed_info.simHandle, out elementConsumer))
		{
			elementConsumer.AddMassInternal(consumed_info);
		}
	}

	private void AddMassInternal(Sim.ConsumedMassInfo consumed_info)
	{
		if (consumed_info.mass > 0f)
		{
			if (this.storeOnConsume)
			{
				Element element = ElementLoader.elements[(int)consumed_info.removedElemIdx];
				if (this.elementToConsume == SimHashes.Vacuum || this.elementToConsume == element.id)
				{
					if (element.IsLiquid)
					{
						this.storage.AddLiquid(element.id, consumed_info.mass, consumed_info.temperature, true);
					}
					else if (element.IsGas)
					{
						this.storage.AddGasChunk(element.id, consumed_info.mass, consumed_info.temperature, true);
					}
				}
			}
			else
			{
				this.consumedTemperature = GameUtil.GetFinalTemperature(consumed_info.temperature, consumed_info.mass, this.consumedTemperature, this.consumedMass);
				this.consumedMass += consumed_info.mass;
			}
		}
		this.accumulator.Accumulate(consumed_info.mass);
	}

	public bool IsElementAvailable
	{
		get
		{
			int num = Grid.PosToCell(this.transform.position + this.sampleCellOffset);
			SimHashes id = Grid.Element[num].id;
			return this.elementToConsume == id && Grid.Cell[num].mass >= this.minimumMass;
		}
	}

	private void UpdateStatusItem()
	{
		if (this.showInStatusPanel)
		{
			if (this.statusHandle == Guid.Empty && this.IsActive() && this.consumptionEnabled)
			{
				this.statusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.ElementConsumer, this);
			}
			else if (this.statusHandle != Guid.Empty)
			{
				base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, false);
			}
		}
	}

	private void OnStorageChange(object data)
	{
		bool flag = !this.IsStorageFull();
		if (flag != this.hasAvailableCapacity)
		{
			this.hasAvailableCapacity = flag;
			this.RefreshConsumptionRate();
		}
	}

	protected override void OnCmpEnable()
	{
		if (!base.isSpawned)
		{
			return;
		}
		if (!this.IsActive())
		{
			return;
		}
		this.UpdateStatusItem();
	}

	protected override void OnCmpDisable()
	{
		this.UpdateStatusItem();
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.isRequired && this.showDescriptor)
		{
			Element element = ElementLoader.FindElementByHash(this.elementToConsume);
			string text = element.tag.ProperName();
			string text2 = GameUtil.GetKeywordStyle(this.elementToConsume);
			if (element.IsVacuum)
			{
				if (this.configuration == ElementConsumer.Configuration.AllGas)
				{
					text2 = "gas";
					text = ELEMENTS.STATEGAS;
				}
				else if (this.configuration == ElementConsumer.Configuration.AllLiquid)
				{
					text2 = "liquid";
					text = ELEMENTS.STATELIQUID;
				}
				else
				{
					text2 = "anyElement";
					text = UI.BUILDINGEFFECTS.CONSUMESANYELEMENT;
				}
			}
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.REQUIRESELEMENT, text2, text), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESELEMENT, text2, text), Descriptor.DescriptorType.Requirement);
			list.Add(descriptor);
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.showDescriptor)
		{
			Element element = ElementLoader.FindElementByHash(this.elementToConsume);
			string text = element.tag.ProperName();
			string text2 = GameUtil.GetKeywordStyle(element);
			if (element.IsVacuum)
			{
				if (this.configuration == ElementConsumer.Configuration.AllGas)
				{
					text2 = "gas";
					text = ELEMENTS.STATEGAS;
				}
				else if (this.configuration == ElementConsumer.Configuration.AllLiquid)
				{
					text2 = "liquid";
					text = ELEMENTS.STATELIQUID;
				}
				else
				{
					text2 = "anyElement";
					text = UI.BUILDINGEFFECTS.CONSUMESANYELEMENT;
				}
			}
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, text2, text, GameUtil.GetFormattedMass(this.consumptionRate / 100f * 100f, GameUtil.TimeSlice.PerSecond, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, text2, text, GameUtil.GetFormattedMass(this.consumptionRate / 100f * 100f, GameUtil.TimeSlice.PerSecond, true, "{0:0.##}")), Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in this.RequirementDescriptors(def))
		{
			list.Add(descriptor);
		}
		foreach (Descriptor descriptor2 in this.EffectDescriptors(def))
		{
			list.Add(descriptor2);
		}
		return list;
	}

	private void OnActiveChanged(object data)
	{
		bool isActive = this.operational.IsActive;
		this.EnableConsumption(isActive);
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1)
		{
			int num = Grid.PosToCell(this.transform.position + this.sampleCellOffset);
			this.simHandle = -2;
			HandleVector<Game.ComplexCallbackInfo>.Handle handle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(delegate(object data)
			{
				ElementConsumer.OnSimRegistered(this, data);
			}), "ElementConsumer");
			SimMessages.AddElementConsumer(num, this.configuration, this.elementToConsume, this.consumptionRadius, handle.index);
		}
	}

	private void SimUnregister()
	{
		if (this.simHandle != -1)
		{
			if (Sim.IsValidHandle(this.simHandle))
			{
				ElementConsumer.handleInstanceMap.Remove(this.simHandle);
				SimMessages.RemoveElementConsumer(-1, this.simHandle);
			}
			this.simHandle = -1;
		}
	}

	private static void OnSimRegistered(ElementConsumer instance, object data)
	{
		int num = (int)data;
		if (instance != null && instance.simHandle == -2)
		{
			instance.simHandle = num;
			if (instance.consumptionEnabled)
			{
				instance.UpdateSimData();
			}
			ElementConsumer.handleInstanceMap[num] = instance;
		}
		else
		{
			SimMessages.RemoveElementConsumer(-1, num);
		}
	}

	[HashedEnum]
	[SerializeField]
	public SimHashes elementToConsume = SimHashes.Vacuum;

	[SerializeField]
	public float consumptionRate;

	[SerializeField]
	public byte consumptionRadius = 1;

	[SerializeField]
	public float minimumMass;

	[SerializeField]
	public bool showInStatusPanel = true;

	[SerializeField]
	public Vector3 sampleCellOffset;

	[SerializeField]
	public float capacityKG = float.PositiveInfinity;

	[SerializeField]
	public ElementConsumer.Configuration configuration;

	[Serialize]
	[NonSerialized]
	public float consumedMass;

	[Serialize]
	[NonSerialized]
	public float consumedTemperature;

	[SerializeField]
	public bool storeOnConsume;

	[MyCmpGet]
	public Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	private Accumulator accumulator;

	private Guid statusHandle;

	public bool showDescriptor = true;

	public bool isRequired = true;

	private bool consumptionEnabled;

	private bool hasAvailableCapacity = true;

	[SerializeField]
	private int simHandle = -1;

	private static Dictionary<int, ElementConsumer> handleInstanceMap = new Dictionary<int, ElementConsumer>();

	public enum Configuration
	{
		Element,
		AllLiquid,
		AllGas
	}
}
