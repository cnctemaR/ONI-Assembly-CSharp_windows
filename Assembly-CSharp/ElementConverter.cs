using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementConverter : StateMachineComponent<ElementConverter.StatesInstance>, IEffectDescriptor
{
	public void SetWorkSpeedMultiplier(float speed)
	{
		this.workSpeedMultiplier = speed;
	}

	public void SetStorage(Storage storage)
	{
		this.storage = storage;
	}

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

	public float AverageConvertRate
	{
		get
		{
			return Game.Instance.accumulators.GetAverageRate(this.outputElements[0].accumulator);
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
				flag = num >= consumedElement.massConsumptionRate;
				break;
			}
		}
		return flag;
	}

	public bool HasEnoughMassToStartConverting()
	{
		return this.HasEnoughMass();
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

	private float GetSpeedMultiplier()
	{
		return this.machinerySpeedAttribute.GetTotalValue() * this.workSpeedMultiplier;
	}

	private bool HasEnoughMass()
	{
		float speedMultiplier = this.GetSpeedMultiplier();
		float num = 1f * speedMultiplier;
		bool flag = true;
		List<GameObject> items = this.storage.items;
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			ElementConverter.ConsumedElement consumedElement = this.consumedElements[i];
			float num2 = 0f;
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (gameObject.HasTag(consumedElement.tag))
				{
					num2 += gameObject.GetComponent<PrimaryElement>().Mass;
				}
			}
			if (num2 < consumedElement.massConsumptionRate * num)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	private void ConvertMass()
	{
		float speedMultiplier = this.GetSpeedMultiplier();
		float num = 1f * speedMultiplier;
		float num2 = 0f;
		float num3 = 1f;
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			ElementConverter.ConsumedElement consumedElement = this.consumedElements[i];
			float num4 = consumedElement.massConsumptionRate * num * num3;
			if (num4 <= 0f)
			{
				num3 = 0f;
				break;
			}
			float num5 = 0f;
			for (int j = 0; j < this.storage.items.Count; j++)
			{
				GameObject gameObject = this.storage.items[j];
				if (!(gameObject == null))
				{
					if (gameObject.HasTag(consumedElement.tag))
					{
						PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
						float num6 = Mathf.Min(num4, component.Mass);
						num5 += num6 / num4;
					}
				}
			}
			num3 = Mathf.Min(num3, num5);
		}
		if (num3 <= 0f)
		{
			return;
		}
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
		diseaseInfo.idx = byte.MaxValue;
		diseaseInfo.count = 0;
		float num7 = 0f;
		for (int k = 0; k < this.consumedElements.Length; k++)
		{
			ElementConverter.ConsumedElement consumedElement2 = this.consumedElements[k];
			float num8 = consumedElement2.massConsumptionRate * num * num3;
			Game.Instance.accumulators.Accumulate(consumedElement2.accumulator, num8);
			for (int l = 0; l < this.storage.items.Count; l++)
			{
				GameObject gameObject2 = this.storage.items[l];
				if (gameObject2.HasTag(consumedElement2.tag))
				{
					PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
					component2.KeepZeroMassObject = true;
					float num9 = Mathf.Min(num8, component2.Mass);
					float num10 = num9 / component2.Mass;
					int num11 = (int)(num10 * (float)component2.DiseaseCount);
					component2.Mass -= num9;
					component2.ModifyDiseaseCount(-num11, "ElementConverter.ConvertMass");
					num7 += num9;
					diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo.idx, diseaseInfo.count, component2.DiseaseIdx, num11);
					num2 = component2.Temperature;
					num8 -= num9;
					if (num8 <= 0f)
					{
						break;
					}
				}
				if (num8 <= 0f)
				{
				}
			}
		}
		if (this.onConvertMass != null && num7 > 0f)
		{
			this.onConvertMass(num7);
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
					float num12 = outputElement.diseaseWeight / this.totalDiseaseWeight;
					diseaseInfo2.count = (int)((float)diseaseInfo2.count * num12);
				}
				if (outputElement.addedDiseaseIdx != 255)
				{
					diseaseInfo2 = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo2, new SimUtil.DiseaseInfo
					{
						idx = outputElement.addedDiseaseIdx,
						count = outputElement.addedDiseaseCount
					});
				}
				float num13 = outputElement.massGenerationRate * this.OutputMultiplier * num * num3;
				Game.Instance.accumulators.Accumulate(outputElement.accumulator, num13);
				float num14 = ((outputElement.outputTemperature != 0f) ? outputElement.outputTemperature : base.GetComponent<PrimaryElement>().Temperature);
				if (outputElement.applyInputTemperature)
				{
					num14 = num2;
				}
				Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
				if (outputElement.storeOutput)
				{
					PrimaryElement primaryElement = this.storage.AddToPrimaryElement(outputElement.elementHash, num13, num14);
					if (primaryElement == null)
					{
						if (element.IsGas)
						{
							this.storage.AddGasChunk(outputElement.elementHash, num13, num14, diseaseInfo2.idx, diseaseInfo2.count, true, true);
						}
						else if (element.IsLiquid)
						{
							this.storage.AddLiquid(outputElement.elementHash, num13, num14, diseaseInfo2.idx, diseaseInfo2.count, true, true);
						}
						else
						{
							GameObject gameObject3 = element.substance.SpawnResource(base.transform.GetPosition(), num13, num14, diseaseInfo2.idx, diseaseInfo2.count, true, false);
							this.storage.Store(gameObject3, true, false, true, false);
						}
					}
					else
					{
						primaryElement.AddDisease(diseaseInfo2.idx, diseaseInfo2.count, "ElementConverter.ConvertMass");
					}
				}
				else
				{
					Vector3 vector = new Vector3(base.transform.GetPosition().x + outputElement.outputElementOffset.x, base.transform.GetPosition().y + outputElement.outputElementOffset.y, 0f);
					int num15 = Grid.PosToCell(vector);
					if (element.IsLiquid)
					{
						int idx = (int)element.idx;
						FallingWater.instance.AddParticle(num15, (byte)idx, num13, num14, diseaseInfo2.idx, diseaseInfo2.count, true, false, false, false);
					}
					else if (element.IsSolid)
					{
						element.substance.SpawnResource(vector, num13, num14, diseaseInfo2.idx, diseaseInfo2.count, false, false);
					}
					else
					{
						SimMessages.AddRemoveSubstance(num15, outputElement.elementHash, CellEventLogger.Instance.OxygenModifierSimUpdate, num13, num14, diseaseInfo2.idx, diseaseInfo2.count, true, -1);
					}
				}
				if (outputElement.elementHash == SimHashes.Oxygen)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num13, base.gameObject.GetProperName(), null);
				}
			}
		}
		this.storage.Trigger(-1697596308, base.gameObject);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Attributes attributes = base.gameObject.GetAttributes();
		this.machinerySpeedAttribute = attributes.Add(Db.Get().Attributes.MachinerySpeed);
		if (ElementConverter.ElementConverterInput == null)
		{
			ElementConverter.ElementConverterInput = new StatusItem("ElementConverterInput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 63486).SetResolveStringCallback(delegate(string str, object data)
			{
				ElementConverter.ConsumedElement consumedElement = (ElementConverter.ConsumedElement)data;
				str = str.Replace("{ElementTypes}", consumedElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedByTag(consumedElement.tag, consumedElement.Rate, GameUtil.TimeSlice.PerSecond));
				return str;
			});
		}
		if (ElementConverter.ElementConverterOutput == null)
		{
			ElementConverter.ElementConverterOutput = new StatusItem("ElementConverterOutput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 63486).SetResolveStringCallback(delegate(string str, object data)
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
			this.consumedElements[i].accumulator = Game.Instance.accumulators.Add("ElementsConsumed", this);
		}
		this.totalDiseaseWeight = 0f;
		for (int j = 0; j < this.outputElements.Length; j++)
		{
			this.outputElements[j].accumulator = Game.Instance.accumulators.Add("OutputElements", this);
			this.totalDiseaseWeight += this.outputElements[j].diseaseWeight;
		}
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		for (int i = 0; i < this.consumedElements.Length; i++)
		{
			Game.Instance.accumulators.Remove(this.consumedElements[i].accumulator);
		}
		for (int j = 0; j < this.outputElements.Length; j++)
		{
			Game.Instance.accumulators.Remove(this.outputElements[j].accumulator);
		}
		base.OnCleanUp();
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (!this.showDescriptors)
		{
			return list;
		}
		if (this.consumedElements != null)
		{
			foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
			{
				Descriptor descriptor = default(Descriptor);
				descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.massConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.massConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
				list.Add(descriptor);
			}
		}
		if (this.outputElements != null)
		{
			foreach (ElementConverter.OutputElement outputElement in this.outputElements)
			{
				Descriptor descriptor2 = default(Descriptor);
				descriptor2.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect);
				list.Add(descriptor2);
			}
		}
		return list;
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpReq]
	private Storage storage;

	public Action<float> onConvertMass;

	private float totalDiseaseWeight = float.MaxValue;

	private AttributeInstance machinerySpeedAttribute;

	private float workSpeedMultiplier = 1f;

	public bool showDescriptors = true;

	private const float BASE_INTERVAL = 1f;

	[SerializeField]
	public ElementConverter.ConsumedElement[] consumedElements;

	[SerializeField]
	public ElementConverter.OutputElement[] outputElements;

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
			this.accumulator = HandleVector<int>.InvalidHandle;
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
				return Game.Instance.accumulators.GetAverageRate(this.accumulator);
			}
		}

		public Tag tag;

		public float massConsumptionRate;

		public HandleVector<int>.Handle accumulator;
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
			this.accumulator = HandleVector<int>.InvalidHandle;
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
				return Game.Instance.accumulators.GetAverageRate(this.accumulator);
			}
		}

		public SimHashes elementHash;

		public float outputTemperature;

		public float massGenerationRate;

		public bool storeOutput;

		public bool applyInputTemperature;

		public Vector2 outputElementOffset;

		public HandleVector<int>.Handle accumulator;

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
			this.disabled.EventTransition(GameHashes.ActiveChanged, this.converting, (ElementConverter.StatesInstance smi) => smi.master.operational == null || smi.master.operational.IsActive);
			this.converting.Enter("AddStatusItems", delegate(ElementConverter.StatesInstance smi)
			{
				smi.AddStatusItems();
			}).Exit("RemoveStatusItems", delegate(ElementConverter.StatesInstance smi)
			{
				smi.RemoveStatusItems();
			}).EventTransition(GameHashes.ActiveChanged, this.disabled, (ElementConverter.StatesInstance smi) => smi.master.operational != null && !smi.master.operational.IsActive)
				.Update("ConvertMass", delegate(ElementConverter.StatesInstance smi, float dt)
				{
					smi.master.ConvertMass();
				}, UpdateRate.SIM_1000ms, true);
		}

		public GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State disabled;

		public GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State converting;
	}
}
