using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementConsumer : KMonoBehaviour, ISaveLoadableJson, IEffectDescriptor
{
	public int DescriptionOrder { get; set; }

	public float AverageConsumeRate
	{
		get
		{
			return this.accumulator.AvgFlowRate;
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
		this.Subscribe(824508782, new EventSystem.EventHandler(this.OnActiveChanged));
		this.Subscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChanged));
		if (this.IsActive())
		{
			this.SimRegister();
		}
	}

	protected virtual bool IsActive()
	{
		return this.operational == null || this.operational.IsActive;
	}

	protected unsafe void SimUpdate(float dt)
	{
		if (!Sim.IsValidHandle(this.simHandle))
		{
			return;
		}
		bool flag = false;
		Sim.MassChangeInfo massChangeInfo = Game.Instance.simData.removedMassEntries[this.simHandle];
		if (massChangeInfo.mass > 0f)
		{
			if (this.storeOnConsume)
			{
				Element element = ElementLoader.elements[(int)massChangeInfo.removedElemIdx];
				if (this.elementToConsume == SimHashes.Vacuum || this.elementToConsume == element.id)
				{
					if (element.IsLiquid)
					{
						this.storage.AddLiquid(element.id, massChangeInfo.mass, massChangeInfo.temperature, true);
					}
					else if (element.IsGas)
					{
						this.storage.AddGasChunk(element.id, massChangeInfo.mass, massChangeInfo.temperature, true);
					}
				}
			}
			else
			{
				this.consumedTemperature = GameUtil.GetFinalTemperature(massChangeInfo.temperature, massChangeInfo.mass, this.consumedTemperature, this.consumedMass);
				this.consumedMass += massChangeInfo.mass;
			}
			flag = true;
		}
		this.TriggerEvents(flag);
		this.accumulator.Accumulate(this.consumedMass);
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
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.showInStatusPanel)
		{
			this.statusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.ElementConsumer, this);
		}
		this.SimRegister();
	}

	protected override void OnCmpDisable()
	{
		if (this.previouslyActive)
		{
			this.Trigger(-2065215890, this);
			this.previouslyActive = false;
		}
		if (this.showInStatusPanel)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle);
		}
		this.SimUnregister();
	}

	protected override void OnCleanUp()
	{
		this.SimUnregister();
		base.OnCleanUp();
	}

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.isRequired && this.showDescriptor)
		{
			Element element = ElementLoader.FindElementByHash(this.elementToConsume);
			string text = element.tag.ProperName();
			string text2 = GameUtil.GetKeywordStyle(this.elementToConsume);
			if (element.IsVacuum)
			{
				text2 = GameUtil.GetKeywordStyle(SimHashes.Oxygen);
				text = ELEMENTS.STATEGAS;
			}
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.REQUIRESELEMENT, text2, text)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESELEMENT, text2, text));
			list.Add(descriptor);
		}
		return list;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.showDescriptor)
		{
			Element element = ElementLoader.FindElementByHash(this.elementToConsume);
			string text = element.tag.ProperName();
			string text2 = element.keywordStyle;
			if (element.IsVacuum)
			{
				text2 = GameUtil.GetKeywordStyle(SimHashes.Oxygen);
				text = ELEMENTS.STATEGAS;
			}
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, text2, text, GameUtil.GetFormattedMass(this.consumptionRate, GameUtil.TimeSlice.PerSecond, true, "F1"))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, text2, text, GameUtil.GetFormattedMass(this.consumptionRate, GameUtil.TimeSlice.PerSecond, true, "F1")));
			list.Add(descriptor);
		}
		return list;
	}

	private void OnActiveChanged(object data)
	{
		bool isActive = this.operational.IsActive;
		base.enabled = isActive;
		if (base.enabled && isActive)
		{
			this.SimRegister();
		}
		else
		{
			this.SimUnregister();
		}
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1)
		{
			int num = Grid.PosToCell(this.transform.position + this.sampleCellOffset);
			this.simHandle = -2;
			HandleVector<Action<object>>.Handle handle = Game.Instance.complexCallbackManager.Add(delegate(object data)
			{
				ElementConsumer.OnSimRegistered(this, data);
			}, "ElementConsumer");
			SimMessages.AddElementConsumer(num, this.configuration, this.elementToConsume, this.consumptionRate, this.consumptionRadius, handle.index);
		}
	}

	private void SimUnregister()
	{
		if (this.simHandle != -1)
		{
			if (Sim.IsValidHandle(this.simHandle))
			{
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
		}
		else
		{
			SimMessages.RemoveElementConsumer(-1, num);
		}
	}

	private void OnStorageChanged(object data)
	{
		bool flag = true;
		if (this.operational != null)
		{
			flag = this.operational.IsOperational && !this.storage.IsFull();
			this.operational.SetActive(flag, false);
		}
		this.TriggerEvents(flag);
	}

	private void TriggerEvents(bool active)
	{
		if (!active)
		{
			if (this.previouslyActive)
			{
				this.previouslyActive = false;
				this.Trigger(-2065215890, this);
			}
		}
		else if (!this.previouslyActive)
		{
			this.previouslyActive = true;
			this.Trigger(-1995890641, this);
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
	private Operational operational;

	[MyCmpGet]
	private Storage storage;

	private Accumulator accumulator;

	private Guid statusHandle;

	private bool previouslyActive;

	public bool showDescriptor = true;

	public bool isRequired = true;

	private int simHandle = -1;

	public enum Configuration
	{
		Element,
		AllLiquid,
		AllGas
	}
}
