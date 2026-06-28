using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei;
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
			return this.outputElements[0].accumulator.AvgRate;
		}
	}

	public bool HasEnoughMass(Tag tag)
	{
		bool flag = false;
		List<GameObject> items = this.storage.items;
		foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
		{
			if (tag == consumedElement.tag)
			{
				float num = 0f;
				for (int j = 0; j < items.Count; j++)
				{
					GameObject gameObject = items[j];
					if (gameObject.HasTag(tag))
					{
						num += gameObject.GetComponent<PrimaryElement>().Mass;
					}
				}
				flag = num >= consumedElement.massConsumptionRate * this.conversionInterval;
				break;
			}
		}
		return flag;
	}

	public bool HasEnoughMassToStartConverting()
	{
		return this.HasEnoughMass(this.conversionInterval);
	}

	public bool CanConvertAtAll()
	{
		bool flag = true;
		List<GameObject> items = this.storage.items;
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			ElementConverter.ConsumedElement consumedElement = this.consumedElements[i];
			bool flag2 = false;
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (gameObject.HasTag(consumedElement.tag) && gameObject.GetComponent<PrimaryElement>().Mass > 0f)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = false;
				break;
			}
		}
		return flag;
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
			if (num < consumedElement.massConsumptionRate * dt)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	private void ConvertMass(float dt)
	{
		float num = 0f;
		float num2 = 1f;
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			ElementConverter.ConsumedElement consumedElement = this.consumedElements[i];
			float num3 = consumedElement.massConsumptionRate * dt * num2;
			if (num3 <= 0f)
			{
				num2 = 0f;
				break;
			}
			float num4 = 0f;
			for (int j = 0; j < this.storage.items.Count; j++)
			{
				GameObject gameObject = this.storage.items[j];
				if (gameObject.HasTag(consumedElement.tag))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					float num5 = Mathf.Min(num3, component.Mass);
					num4 += num5 / num3;
				}
			}
			num2 = Mathf.Min(num2, num4);
		}
		if (num2 <= 0f)
		{
			return;
		}
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
		diseaseInfo.idx = byte.MaxValue;
		diseaseInfo.count = 0;
		float num6 = 0f;
		for (int k = 0; k < this.consumedElements.Length; k++)
		{
			ElementConverter.ConsumedElement consumedElement2 = this.consumedElements[k];
			float num7 = consumedElement2.massConsumptionRate * dt * num2;
			consumedElement2.accumulator.Accumulate(num7);
			for (int l = 0; l < this.storage.items.Count; l++)
			{
				GameObject gameObject2 = this.storage.items[l];
				if (gameObject2.HasTag(consumedElement2.tag))
				{
					PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
					component2.KeepZeroMassObject = true;
					float num8 = Mathf.Min(num7, component2.Mass);
					float num9 = num8 / component2.Mass;
					int num10 = (int)(num9 * (float)component2.DiseaseCount);
					component2.Mass -= num8;
					component2.ModifyDiseaseCount(-num10, "ElementConverter.ConvertMass");
					num6 += num8;
					diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo.idx, diseaseInfo.count, component2.DiseaseIdx, num10);
					num = component2.Temperature;
					if (num7 <= 0f)
					{
						break;
					}
				}
				if (num7 <= 0f)
				{
				}
			}
		}
		if (this.onConvertMass != null && num6 > 0f)
		{
			this.onConvertMass(num6);
		}
		if (this.outputElements != null && this.outputElements.Length > 0)
		{
			for (int m = 0; m < this.outputElements.Length; m++)
			{
				ElementConverter.OutputElement outputElement = this.outputElements[m];
				SimUtil.DiseaseInfo diseaseInfo2 = diseaseInfo;
				if (this.totalDiseaseWeight <= 0f)
				{
					diseaseInfo2.idx = byte.MaxValue;
					diseaseInfo2.count = 0;
				}
				else
				{
					float num11 = outputElement.diseaseWeight / this.totalDiseaseWeight;
					diseaseInfo2.count = (int)((float)diseaseInfo2.count * num11);
				}
				if (outputElement.addedDiseaseIdx != 255)
				{
					diseaseInfo2 = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo2, new SimUtil.DiseaseInfo
					{
						idx = outputElement.addedDiseaseIdx,
						count = outputElement.addedDiseaseCount
					});
				}
				float num12 = outputElement.massGenerationRate * this.OutputMultiplier * dt * num2;
				outputElement.accumulator.Accumulate(num12);
				float num13 = ((outputElement.outputTemperature != 0f) ? outputElement.outputTemperature : base.GetComponent<PrimaryElement>().Temperature);
				if (outputElement.applyInputTemperature)
				{
					num13 = num;
				}
				if (outputElement.storeOutput)
				{
					PrimaryElement primaryElement = this.storage.AddToPrimaryElement(outputElement.element.id, num12, num13);
					if (primaryElement == null)
					{
						if (outputElement.element.IsGas)
						{
							this.storage.AddGasChunk(outputElement.elementHash, num12, num13, diseaseInfo2.idx, diseaseInfo2.count, true, true);
						}
						else if (outputElement.element.IsLiquid)
						{
							this.storage.AddLiquid(outputElement.elementHash, num12, num13, diseaseInfo2.idx, diseaseInfo2.count, true, true);
						}
						else
						{
							GameObject gameObject3 = outputElement.element.substance.SpawnResource(this.transform.position, num12, num13, diseaseInfo2.idx, diseaseInfo2.count, true, false);
							this.storage.Store(gameObject3, true, false, true);
						}
					}
					else
					{
						primaryElement.AddDisease(diseaseInfo2.idx, diseaseInfo2.count, "ElementCovnerter.ConvertMass");
					}
				}
				else
				{
					Vector3 vector = new Vector3(this.transform.position.x + outputElement.outputElementOffset.x, this.transform.position.y + outputElement.outputElementOffset.y, 0f);
					int num14 = Grid.PosToCell(vector);
					if (outputElement.element.IsSolid)
					{
						outputElement.element.substance.SpawnResource(vector, num12, num13, diseaseInfo2.idx, diseaseInfo2.count, false, false);
					}
					else
					{
						SimMessages.AddRemoveSubstance(num14, outputElement.elementHash, CellEventLogger.Instance.OxygenModifierSimUpdate, num12, num13, diseaseInfo2.idx, diseaseInfo2.count, -1);
					}
				}
				if (outputElement.elementHash == SimHashes.Oxygen)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num12, null);
				}
			}
		}
		this.storage.Trigger(-1697596308, base.gameObject);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (ElementConverter.ElementConverterInput == null)
		{
			ElementConverter.ElementConverterInput = new StatusItem("ElementConverterInput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None, true, 2046).SetResolveStringCallback(delegate(string str, object data)
			{
				ElementConverter.ConsumedElement consumedElement = (ElementConverter.ConsumedElement)data;
				str = str.Replace("{ElementTypes}", consumedElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(consumedElement.Rate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			});
		}
		if (ElementConverter.ElementConverterOutput == null)
		{
			ElementConverter.ElementConverterOutput = new StatusItem("ElementConverterOutput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None, true, 2046).SetResolveStringCallback(delegate(string str, object data)
			{
				ElementConverter.OutputElement outputElement = (ElementConverter.OutputElement)data;
				str = str.Replace("{ElementTypes}", outputElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(outputElement.Rate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			});
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			this.consumedElements[i].accumulator = new Accumulator("ElementsConsumed", this, 3f);
		}
		this.totalDiseaseWeight = 0f;
		for (int j = 0; j < this.outputElements.Length; j++)
		{
			this.outputElements[j].accumulator = new Accumulator("OutputElements", this, 3f);
			this.totalDiseaseWeight += this.outputElements[j].diseaseWeight;
		}
		base.smi.StartSM();
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
		{
			string keywordStyle = GameUtil.GetKeywordStyle(consumedElement.tag);
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, keywordStyle, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.massConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, keywordStyle, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.massConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
			list.Add(descriptor);
		}
		foreach (ElementConverter.OutputElement outputElement in this.outputElements)
		{
			string keywordStyle2 = GameUtil.GetKeywordStyle(outputElement.elementHash);
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, keywordStyle2, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, keywordStyle2, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect);
			list.Add(descriptor2);
		}
		return list;
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpReq]
	private Storage storage;

	public Action<float> onConvertMass;

	private float totalDiseaseWeight = float.MaxValue;

	[SerializeField]
	public ElementConverter.ConsumedElement[] consumedElements;

	[SerializeField]
	public ElementConverter.OutputElement[] outputElements;

	[SerializeField]
	public float conversionInterval = 1f;

	private float outputMultiplier = 1f;

	private static StatusItem ElementConverterInput;

	private static StatusItem ElementConverterOutput;

	[DebuggerDisplay("{tag} {massConsumptionRate}")]
	[Serializable]
	public struct ConsumedElement
	{
		public ConsumedElement(Tag tag, float kgPerSecond)
		{
			this.tag = tag;
			this.massConsumptionRate = kgPerSecond;
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
				return this.accumulator.AvgRate;
			}
		}

		public Tag tag;

		public float massConsumptionRate;

		public Accumulator accumulator;
	}

	[Serializable]
	public struct OutputElement
	{
		public OutputElement(float kgPerSecond, SimHashes element, float outputTemperature = 0f, bool storeOutput = false, float outputElementOffsetx = 0f, float outputElementOffsety = 0.5f, bool apply_input_temperature = false, float diseaseWeight = 1f, byte addedDiseaseIdx = 255, int addedDiseaseCount = 0)
		{
			this.elementHash = element;
			this.outputTemperature = ((outputTemperature <= 0f) ? ElementLoader.FindElementByHash(element).defaultValues.temperature : outputTemperature);
			this.storeOutput = storeOutput;
			this.massGenerationRate = kgPerSecond;
			this.outputElementOffset = new Vector2(outputElementOffsetx, outputElementOffsety);
			this.accumulator = null;
			this.element = ElementLoader.FindElementByHash(element);
			this.applyInputTemperature = apply_input_temperature;
			this.diseaseWeight = diseaseWeight;
			this.addedDiseaseIdx = addedDiseaseIdx;
			this.addedDiseaseCount = addedDiseaseCount;
		}

		public string Name
		{
			get
			{
				return ElementLoader.FindElementByHash(this.elementHash).tag.ProperName();
			}
		}

		public float Rate
		{
			get
			{
				return this.accumulator.AvgRate;
			}
		}

		public SimHashes elementHash;

		public Element element;

		public float outputTemperature;

		public float massGenerationRate;

		public bool storeOutput;

		public bool applyInputTemperature;

		public Vector2 outputElementOffset;

		public Accumulator accumulator;

		public float diseaseWeight;

		public byte addedDiseaseIdx;

		public int addedDiseaseCount;
	}

	public class StatesInstance : GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.GameInstance
	{
		public StatesInstance(ElementConverter smi)
			: base(smi)
		{
		}

		public void AddStatusItems()
		{
			foreach (ElementConverter.ConsumedElement consumedElement in base.master.consumedElements)
			{
				Guid guid = base.master.GetComponent<KSelectable>().AddStatusItem(ElementConverter.ElementConverterInput, consumedElement);
				this.statusItemEntries.Add(guid);
			}
			foreach (ElementConverter.OutputElement outputElement in base.master.outputElements)
			{
				Guid guid2 = base.master.GetComponent<KSelectable>().AddStatusItem(ElementConverter.ElementConverterOutput, outputElement);
				this.statusItemEntries.Add(guid2);
			}
		}

		public void RemoveStatusItems()
		{
			foreach (Guid guid in this.statusItemEntries)
			{
				base.master.GetComponent<KSelectable>().RemoveStatusItem(guid, false);
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
				.ToggleSchedulePeriodic("ConvertMass", (ElementConverter.StatesInstance smi) => smi.master.conversionInterval, delegate(ElementConverter.StatesInstance smi)
				{
					smi.master.ConvertMass(smi.master.conversionInterval);
				}, (ElementConverter.StatesInstance smi) => smi.master.gameObject);
		}

		public GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State disabled;

		public GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State converting;
	}
}
