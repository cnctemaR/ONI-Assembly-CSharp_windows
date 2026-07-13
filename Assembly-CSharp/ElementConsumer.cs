using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[SerializationConfig(MemberSerialization.OptIn)]
public class ElementConsumer : SimComponent, ISaveLoadable, IGameObjectEffectDescriptor
{
	public event Action<Sim.ConsumedMassInfo> OnElementConsumed;

	public event Action<Sim.ConsumedMassInfo> OnElementConsumedStored;

	public float AverageConsumeRate
	{
		get
		{
			return Game.Instance.accumulators.GetAverageRate(this.accumulator);
		}
	}

	public static void ClearInstanceMap()
	{
		ElementConsumer.handleInstanceMap.Clear();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.accumulator = Game.Instance.accumulators.Add("Element", this);
		if (this.elementToConsume == SimHashes.Void)
		{
			throw new ArgumentException("No consumable elements specified");
		}
		if (!this.ignoreActiveChanged)
		{
			base.Subscribe<ElementConsumer>(824508782, ElementConsumer.OnActiveChangedDelegate);
		}
		if (this.capacityKG != float.PositiveInfinity)
		{
			this.hasAvailableCapacity = !this.IsStorageFull();
			base.Subscribe<ElementConsumer>(-1697596308, ElementConsumer.OnStorageChangeDelegate);
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(this.accumulator);
		base.OnCleanUp();
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
		global::Debug.Assert(Sim.IsValidHandle(this.simHandle));
		int sampleCell = this.GetSampleCell();
		float num = ((this.consumptionEnabled && this.hasAvailableCapacity) ? this.consumptionRate : 0f);
		SimMessages.SetElementConsumerData(this.simHandle, sampleCell, num);
		this.UpdateStatusItem();
	}

	public static void AddMass(Sim.ConsumedMassInfo consumed_info)
	{
		if (!Sim.IsValidHandle(consumed_info.simHandle))
		{
			return;
		}
		ElementConsumer elementConsumer;
		if (ElementConsumer.handleInstanceMap.TryGetValue(consumed_info.simHandle, out elementConsumer))
		{
			elementConsumer.AddMassInternal(consumed_info);
		}
	}

	private int GetSampleCell()
	{
		return Grid.PosToCell(base.transform.GetPosition() + this.sampleCellOffset);
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
						this.storage.AddLiquid(element.id, consumed_info.mass, consumed_info.temperature, consumed_info.diseaseIdx, consumed_info.diseaseCount, true, true);
					}
					else if (element.IsGas)
					{
						this.storage.AddGasChunk(element.id, consumed_info.mass, consumed_info.temperature, consumed_info.diseaseIdx, consumed_info.diseaseCount, true, true);
					}
					if (this.OnElementConsumedStored != null)
					{
						this.OnElementConsumedStored(consumed_info);
					}
				}
			}
			else
			{
				this.consumedTemperature = GameUtil.GetFinalTemperature(consumed_info.temperature, consumed_info.mass, this.consumedTemperature, this.consumedMass);
				this.consumedMass += consumed_info.mass;
				if (this.OnElementConsumed != null)
				{
					this.OnElementConsumed(consumed_info);
				}
			}
		}
		Game.Instance.accumulators.Accumulate(this.accumulator, consumed_info.mass);
	}

	public bool IsElementAvailable
	{
		get
		{
			int sampleCell = this.GetSampleCell();
			SimHashes id = Grid.Element[sampleCell].id;
			return this.elementToConsume == id && Grid.Mass[sampleCell] >= this.minimumMass;
		}
	}

	private void UpdateStatusItem()
	{
		if (this.showInStatusPanel)
		{
			if (this.statusHandle == Guid.Empty && this.IsActive() && this.consumptionEnabled)
			{
				this.statusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.ElementConsumer, this);
				return;
			}
			if (this.statusHandle != Guid.Empty && !this.consumptionEnabled)
			{
				base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, false);
				this.statusHandle = Guid.Empty;
				return;
			}
		}
		else if (this.statusHandle != Guid.Empty)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, false);
			this.statusHandle = Guid.Empty;
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

	public List<Descriptor> RequirementDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.isRequired && this.showDescriptor)
		{
			Element element = ElementLoader.FindElementByHash(this.elementToConsume);
			string text = element.tag.ProperName();
			if (element.IsVacuum)
			{
				if (this.configuration == ElementConsumer.Configuration.AllGas)
				{
					text = ELEMENTS.STATE.GAS;
				}
				else if (this.configuration == ElementConsumer.Configuration.AllLiquid)
				{
					text = ELEMENTS.STATE.LIQUID;
				}
				else
				{
					text = UI.BUILDINGEFFECTS.CONSUMESANYELEMENT;
				}
			}
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.REQUIRESELEMENT, text), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESELEMENT, text), Descriptor.DescriptorType.Requirement);
			list.Add(descriptor);
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.showDescriptor)
		{
			Element element = ElementLoader.FindElementByHash(this.elementToConsume);
			string text = element.tag.ProperName();
			if (element.IsVacuum)
			{
				if (this.configuration == ElementConsumer.Configuration.AllGas)
				{
					text = ELEMENTS.STATE.GAS;
				}
				else if (this.configuration == ElementConsumer.Configuration.AllLiquid)
				{
					text = ELEMENTS.STATE.LIQUID;
				}
				else
				{
					text = UI.BUILDINGEFFECTS.CONSUMESANYELEMENT;
				}
			}
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, text, GameUtil.GetFormattedMass(this.consumptionRate / 100f * 100f, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, text, GameUtil.GetFormattedMass(this.consumptionRate / 100f * 100f, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in this.RequirementDescriptors())
		{
			list.Add(descriptor);
		}
		foreach (Descriptor descriptor2 in this.EffectDescriptors())
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

	protected override void OnSimUnregister()
	{
		global::Debug.Assert(Sim.IsValidHandle(this.simHandle));
		ElementConsumer.handleInstanceMap.Remove(this.simHandle);
		ElementConsumer.StaticUnregister(this.simHandle);
	}

	protected override void OnSimRegister(HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
	{
		SimMessages.AddElementConsumer(this.GetSampleCell(), this.configuration, this.elementToConsume, this.consumptionRadius, cb_handle.index);
	}

	protected override Action<int> GetStaticUnregister()
	{
		return new Action<int>(ElementConsumer.StaticUnregister);
	}

	private static void StaticUnregister(int sim_handle)
	{
		global::Debug.Assert(Sim.IsValidHandle(sim_handle));
		SimMessages.RemoveElementConsumer(-1, sim_handle);
	}

	protected override void OnSimRegistered()
	{
		if (this.consumptionEnabled)
		{
			this.UpdateSimData();
		}
		ElementConsumer.handleInstanceMap[this.simHandle] = this;
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

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	public bool ignoreActiveChanged;

	private Guid statusHandle;

	public bool showDescriptor = true;

	public bool isRequired = true;

	private bool consumptionEnabled;

	private bool hasAvailableCapacity = true;

	private static Dictionary<int, ElementConsumer> handleInstanceMap = new Dictionary<int, ElementConsumer>();

	private static readonly EventSystem.IntraObjectHandler<ElementConsumer> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ElementConsumer>(delegate(ElementConsumer component, object data)
	{
		component.OnActiveChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ElementConsumer> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<ElementConsumer>(delegate(ElementConsumer component, object data)
	{
		component.OnStorageChange(data);
	});

	public enum Configuration
	{
		Element,
		AllLiquid,
		AllGas
	}
}
