using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementConverter : StateMachineComponent<ElementConverter.StatesInstance>, IEffectDescriptor
{
	public float OutputMultiplier
	{
		get
		{
			return this.outputMultiplier;
		}
		set
		{
			this.outputMultiplier = value;
		}
	}

	public Accumulator Accumulator
	{
		get
		{
			return this.outputElements[0].accumulator;
		}
	}

	public float AverageConvertRate
	{
		get
		{
			return this.outputElements[0].accumulator.AvgFlowRate;
		}
	}

	public int DescriptionOrder { get; set; }

	public bool HasEnoughMass(SimHashes elem_hash)
	{
		bool flag = false;
		Element element = ElementLoader.FindElementByHash(elem_hash);
		List<GameObject> items = this.storage.items;
		foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
		{
			if (element.tag == consumedElement.tag)
			{
				float num = 0f;
				foreach (GameObject gameObject in items)
				{
					if (gameObject.HasTag(element.tag))
					{
						num += gameObject.GetComponent<PrimaryElement>().Mass;
					}
				}
				flag = num >= consumedElement.amount * 0.1f / this.conversionInterval;
				break;
			}
		}
		return flag;
	}

	public bool HasEnoughMass()
	{
		return this.HasEnoughMass(0.1f);
	}

	private bool HasEnoughMass(float dt)
	{
		bool flag = true;
		List<GameObject> items = this.storage.items;
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			ElementConverter.ConsumedElement consumedElement = this.consumedElements[i];
			float num = 0f;
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (gameObject.HasTag(consumedElement.tag))
				{
					num += gameObject.GetComponent<PrimaryElement>().Mass;
				}
			}
			if (num < consumedElement.amount * dt / this.conversionInterval)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	private void ConvertMass(float dt)
	{
		if (!this.HasEnoughMass(dt))
		{
			return;
		}
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			ElementConverter.ConsumedElement consumedElement = this.consumedElements[i];
			float num = consumedElement.amount * dt / this.conversionInterval;
			consumedElement.accumulator.Accumulate(num);
			for (int j = 0; j < this.storage.items.Count; j++)
			{
				GameObject gameObject = this.storage.items[j];
				if (gameObject.HasTag(consumedElement.tag))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					float num2 = Mathf.Min(num, component.Mass);
					component.Mass -= num2;
					num -= num2;
					if (num <= 0f)
					{
						break;
					}
				}
			}
			Debug.Assert(num <= 0f);
			this.storage.Trigger(-1697596308, base.gameObject);
		}
		this.accumulatedTime += 0.1f;
		if (this.accumulatedTime > this.conversionInterval)
		{
			for (int k = 0; k < this.outputElements.Length; k++)
			{
				ElementConverter.OutputElement outputElement = this.outputElements[k];
				float num3 = outputElement.outputMass * this.OutputMultiplier;
				outputElement.accumulator.Accumulate(num3);
				float num4 = ((outputElement.outputTemperature != 0f) ? outputElement.outputTemperature : base.GetComponent<PrimaryElement>().Temperature);
				if (outputElement.prefab != null)
				{
					Vector3 vector = new Vector3(this.transform.position.x + outputElement.outputElementOffset.x, this.transform.position.y + outputElement.outputElementOffset.y, Grid.GetLayerZ(Grid.SceneLayer.Use));
					GameObject gameObject2 = Util.KInstantiate(outputElement.prefab, vector, Quaternion.identity, SceneOrganizer.Instance.GetFolder(Folder.Loot), null, true, 0);
					PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
					component2.Temperature = num4;
					component2.Mass = num3;
					gameObject2.SetActive(true);
					if (outputElement.storeOutput)
					{
						this.storage.Store(gameObject2, false, false);
					}
				}
				else
				{
					if (outputElement.storeOutput)
					{
						if (outputElement.element.IsGas)
						{
							this.storage.AddGasChunk(outputElement.elementHash, num3, num4, true);
						}
						else if (outputElement.element.IsLiquid)
						{
							this.storage.AddLiquid(outputElement.elementHash, num3, num4, true);
						}
						else
						{
							GameObject gameObject3 = outputElement.element.substance.SpawnResource(this.transform.position, num3, num4, false, false);
							this.storage.Store(gameObject3, false, false);
						}
					}
					else
					{
						Vector3 vector2 = new Vector3(this.transform.position.x + outputElement.outputElementOffset.x, this.transform.position.y + outputElement.outputElementOffset.y, 0f);
						int num5 = Grid.PosToCell(vector2);
						if (outputElement.element.IsSolid)
						{
							outputElement.element.substance.SpawnResource(vector2, num3, num4, false, false);
						}
						else
						{
							SimMessages.AddRemoveSubstance(num5, outputElement.elementHash, CellEventLogger.Instance.OxygenModifierSimUpdate, num3, num4, -1);
						}
					}
					if (outputElement.elementHash == SimHashes.Oxygen)
					{
						ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num3, null);
					}
				}
			}
			this.accumulatedTime -= this.conversionInterval;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			this.consumedElements[i].accumulator = new Accumulator("ElementsConsumed", this, 3f);
		}
		for (int j = 0; j < this.outputElements.Length; j++)
		{
			this.outputElements[j].accumulator = new Accumulator("OutputElements", this, 3f);
		}
		base.smi.StartSM();
	}

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
		{
			string keywordStyle = GameUtil.GetKeywordStyle(consumedElement.tag);
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, keywordStyle, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.amount / this.conversionInterval, GameUtil.TimeSlice.PerSecond, true, "F1"))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, keywordStyle, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.amount / this.conversionInterval, GameUtil.TimeSlice.PerSecond, true, "F1")));
			list.Add(descriptor);
		}
		return list;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (ElementConverter.OutputElement outputElement in this.outputElements)
		{
			string text;
			if (outputElement.prefab != null)
			{
				Element element = outputElement.prefab.GetComponent<PrimaryElement>().Element;
				text = GameUtil.GetKeywordStyle(element);
			}
			else
			{
				text = GameUtil.GetKeywordStyle(outputElement.elementHash);
			}
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, text, outputElement.Name, GameUtil.GetFormattedMass(outputElement.outputMass / this.conversionInterval, GameUtil.TimeSlice.PerSecond, true, "F1"))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, text, outputElement.Name, GameUtil.GetFormattedMass(outputElement.outputMass / this.conversionInterval, GameUtil.TimeSlice.PerSecond, true, "F1")));
			list.Add(descriptor);
		}
		return list;
	}

	private const float CONVERSION_INTERVAL = 0.1f;

	[MyCmpGet]
	private Operational operational;

	[MyCmpReq]
	private Storage storage;

	[SerializeField]
	public ElementConverter.ConsumedElement[] consumedElements;

	[SerializeField]
	public ElementConverter.OutputElement[] outputElements;

	[SerializeField]
	public float conversionInterval = 1f;

	private float accumulatedTime;

	private float outputMultiplier = 1f;

	[Serializable]
	public struct ConsumedElement
	{
		public ConsumedElement(Tag _tag, float _amount)
		{
			this.tag = _tag;
			this.amount = _amount;
			this.accumulator = null;
		}

		public string Name
		{
			get
			{
				return this.tag.ProperName();
			}
		}

		public float Rate
		{
			get
			{
				return this.accumulator.AvgFlowRate;
			}
		}

		public Tag tag;

		public float amount;

		public Accumulator accumulator;
	}

	[Serializable]
	public struct OutputElement
	{
		public OutputElement(GameObject prefab, float outputMass = 10f, SimHashes element = SimHashes.Vacuum, float outputTemperature = 0f, bool storeOutput = false, float outputElementOffsetx = 0f, float outputElementOffsety = 0f)
		{
			this.prefab = prefab;
			this.elementHash = element;
			this.outputTemperature = outputTemperature;
			this.storeOutput = storeOutput;
			this.outputMass = outputMass;
			this.outputElementOffset = new Vector2(outputElementOffsetx, outputElementOffsety);
			this.accumulator = null;
			this.element = ElementLoader.FindElementByHash(element);
		}

		public string Name
		{
			get
			{
				if (this.prefab != null)
				{
					KSelectable component = this.prefab.GetComponent<KSelectable>();
					return component.entityName;
				}
				return ElementLoader.FindElementByHash(this.elementHash).tag.ProperName();
			}
		}

		public float Rate
		{
			get
			{
				return this.accumulator.AvgFlowRate;
			}
		}

		public GameObject prefab;

		public SimHashes elementHash;

		public Element element;

		public float outputTemperature;

		public float outputMass;

		public bool storeOutput;

		public Vector2 outputElementOffset;

		public Accumulator accumulator;
	}

	public class StatesInstance : GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter>.GameInstance
	{
		public StatesInstance(ElementConverter smi)
			: base(smi)
		{
		}

		public void AddStatusItems()
		{
			foreach (ElementConverter.ConsumedElement consumedElement in base.master.consumedElements)
			{
				Guid guid = base.master.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ElementConverterInput, consumedElement);
				this.statusItemEntries.Add(guid);
			}
			foreach (ElementConverter.OutputElement outputElement in base.master.outputElements)
			{
				Guid guid2 = base.master.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ElementConverterOutput, outputElement);
				this.statusItemEntries.Add(guid2);
			}
		}

		public void RemoveStatusItems()
		{
			foreach (Guid guid in this.statusItemEntries)
			{
				base.master.GetComponent<KSelectable>().RemoveStatusItem(guid);
			}
			this.statusItemEntries.Clear();
		}

		private List<Guid> statusItemEntries = new List<Guid>();
	}

	public class States : GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disabled;
			this.disabled.EventTransition(GameHashes.ActiveChanged, this.converting, (ElementConverter.StatesInstance smi) => smi.master.operational.IsActive);
			this.converting.Enter("AddStatusItems", delegate(ElementConverter.StatesInstance smi)
			{
				smi.AddStatusItems();
			}).Exit("RemoveStatusItems", delegate(ElementConverter.StatesInstance smi)
			{
				smi.RemoveStatusItems();
			}).EventTransition(GameHashes.ActiveChanged, this.disabled, (ElementConverter.StatesInstance smi) => !smi.master.operational.IsActive)
				.ToggleSchedulePeriodic("ConvertMass", 0.1f, delegate(ElementConverter.StatesInstance smi)
				{
					smi.master.ConvertMass(0.1f);
				});
		}

		public GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter>.State disabled;

		public GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter>.State converting;
	}
}
