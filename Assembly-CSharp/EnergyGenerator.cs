using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EnergyGenerator : Generator, IEffectDescriptor, ISingleSliderControl
{
	public float SingleSliderPercent
	{
		get
		{
			return this.batteryRefillPercent;
		}
		set
		{
			this.batteryRefillPercent = value;
		}
	}

	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TITLE";
		}
	}

	public string SliderTooltipKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TOOLTIP";
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EnergyGenerator.EnsureStatusItemAvailable();
		this.Subscribe(824508782, new Action<object>(this.OnActiveChanged));
	}

	protected void OnActiveChanged(object data)
	{
		bool flag = (bool)data;
		StatusItem statusItem = ((!flag) ? Db.Get().BuildingStatusItems.GeneratorOffline : Db.Get().BuildingStatusItems.Wattage);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.hasMeter)
		{
			this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", this.meterOffset, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		}
	}

	private bool IsConvertible(float dt)
	{
		bool flag = true;
		foreach (EnergyGenerator.InputItem inputItem in this.formula.inputs)
		{
			GameObject gameObject = this.storage.FindFirst(inputItem.tag);
			if (gameObject != null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				float num = inputItem.consumptionRate * dt;
				flag = flag && component.Mass >= num;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}

	protected override void SimUpdate(float dt)
	{
		base.SimUpdate(dt);
		if (this.hasMeter)
		{
			EnergyGenerator.InputItem inputItem = this.formula.inputs[0];
			float num = 0f;
			GameObject gameObject = this.storage.FindFirst(inputItem.tag);
			if (gameObject != null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				num = component.Mass / inputItem.maxStoredMass;
			}
			this.meter.SetPositionPercent(num);
		}
		ushort circuitID = base.CircuitID;
		this.operational.SetFlag(EnergyGenerator.wireConnectedFlag, circuitID != ushort.MaxValue);
		bool flag = false;
		if (this.operational.IsOperational)
		{
			bool flag2 = false;
			ReadOnlyCollection<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);
			if (!this.ignoreBatteryRefillPercent && batteriesOnCircuit.Count > 0)
			{
				foreach (Battery battery in batteriesOnCircuit)
				{
					if (battery.PercentFull < this.batteryRefillPercent)
					{
						flag2 = true;
						break;
					}
				}
			}
			else
			{
				flag2 = true;
			}
			if (!this.ignoreBatteryRefillPercent)
			{
				this.selectable.ToggleStatusItem(EnergyGenerator.batteriesSufficientlyFull, !flag2, null);
			}
			if (this.delivery != null)
			{
				this.delivery.Pause(!flag2, "Circuit has sufficient energy");
			}
			if (this.formula.inputs != null)
			{
				bool flag3 = this.IsConvertible(dt);
				this.selectable.ToggleStatusItem(EnergyGenerator.insufficientConversionMass, !flag3, null);
				if (flag3)
				{
					foreach (EnergyGenerator.InputItem inputItem2 in this.formula.inputs)
					{
						float num2 = inputItem2.consumptionRate * dt;
						this.storage.ConsumeIgnoringDisease(inputItem2.tag, num2);
					}
					PrimaryElement component2 = base.GetComponent<PrimaryElement>();
					foreach (EnergyGenerator.OutputItem outputItem in this.formula.outputs)
					{
						this.Emit(outputItem, dt, component2);
					}
					base.GenerateJoules(base.WattageRating * dt, false);
					this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this);
					flag = true;
				}
			}
		}
		this.operational.SetActive(flag, false);
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		if (this.formula.inputs == null || this.formula.inputs.Length == 0)
		{
			return null;
		}
		List<Descriptor> list = new List<Descriptor>();
		for (int i = 0; i < this.formula.inputs.Length; i++)
		{
			EnergyGenerator.InputItem inputItem = this.formula.inputs[i];
			Element element = ElementLoader.GetElement(inputItem.tag);
			string text = element.tag.ProperName();
			string keywordStyle = GameUtil.GetKeywordStyle(element);
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, keywordStyle, text, GameUtil.GetFormattedMass(inputItem.consumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, keywordStyle, text, GameUtil.GetFormattedMass(inputItem.consumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
			list.Add(descriptor);
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.formula.outputs == null || this.formula.outputs.Length == 0)
		{
			return list;
		}
		for (int i = 0; i < this.formula.outputs.Length; i++)
		{
			EnergyGenerator.OutputItem outputItem = this.formula.outputs[i];
			Element element = ElementLoader.FindElementByHash(outputItem.element);
			string text = element.tag.ProperName();
			string keywordStyle = GameUtil.GetKeywordStyle(element);
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, keywordStyle, text, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, keywordStyle, text, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
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

	public static StatusItem BatteriesSufficientlyFull
	{
		get
		{
			return EnergyGenerator.batteriesSufficientlyFull;
		}
	}

	public static void EnsureStatusItemAvailable()
	{
		if (EnergyGenerator.batteriesSufficientlyFull == null)
		{
			EnergyGenerator.batteriesSufficientlyFull = new StatusItem("BatteriesSufficientlyFull", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 14334);
		}
		if (EnergyGenerator.insufficientConversionMass == null)
		{
			EnergyGenerator.insufficientConversionMass = new StatusItem("INSUFFICIENT_CONVERSION_MASS", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 14334);
		}
	}

	public static EnergyGenerator.Formula CreateSimpleFormula(SimHashes input_element, float input_mass_rate, float max_stored_input_mass, SimHashes output_element = SimHashes.Void, float output_mass_rate = 0f, bool store_output_mass = true)
	{
		EnergyGenerator.Formula formula = default(EnergyGenerator.Formula);
		formula.inputs = new EnergyGenerator.InputItem[]
		{
			new EnergyGenerator.InputItem(GameTagExtensions.Create(input_element), input_mass_rate, max_stored_input_mass)
		};
		if (output_element != SimHashes.Void)
		{
			formula.outputs = new EnergyGenerator.OutputItem[]
			{
				new EnergyGenerator.OutputItem(output_element, output_mass_rate, store_output_mass)
			};
		}
		else
		{
			formula.outputs = null;
		}
		return formula;
	}

	private void Emit(EnergyGenerator.OutputItem output, float dt, PrimaryElement root_pe)
	{
		Element element = ElementLoader.FindElementByHash(output.element);
		float num = output.creationRate * dt;
		if (output.store)
		{
			if (element.IsGas)
			{
				this.storage.AddGasChunk(output.element, num, root_pe.Temperature, byte.MaxValue, 0, true, true);
			}
			else if (element.IsLiquid)
			{
				this.storage.AddLiquid(output.element, num, root_pe.Temperature, byte.MaxValue, 0, true, true);
			}
			else
			{
				GameObject gameObject = element.substance.SpawnResource(this.transform.position, num, root_pe.Temperature, byte.MaxValue, 0, false, false);
				this.storage.Store(gameObject, true, false, true);
			}
		}
		else
		{
			int num2 = Grid.PosToCell(this.transform.position);
			int num3 = Grid.OffsetCell(num2, output.emitOffset);
			if (element.IsGas)
			{
				SimMessages.ModifyMass(num3, num, byte.MaxValue, 0, CellEventLogger.Instance.EnergyGeneratorModifyMass, root_pe.Temperature, output.element);
			}
			else if (element.IsLiquid)
			{
				int elementIndex = ElementLoader.GetElementIndex(output.element);
				FallingWater.instance.AddParticle(num3, (byte)elementIndex, num, root_pe.Temperature, byte.MaxValue, 0, false, false, false, false);
			}
			else
			{
				element.substance.SpawnResource(Grid.CellToPosCCC(num3, Grid.SceneLayer.Front), num, root_pe.Temperature, byte.MaxValue, 0, true, false);
			}
		}
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ManualDeliveryKG delivery;

	[Serialize]
	[SerializeField]
	private float batteryRefillPercent = 0.5f;

	public bool ignoreBatteryRefillPercent;

	public bool hasMeter = true;

	private static StatusItem batteriesSufficientlyFull;

	private static StatusItem insufficientConversionMass;

	public Meter.Offset meterOffset;

	[SerializeField]
	public EnergyGenerator.Formula formula;

	private MeterController meter;

	private static Operational.Flag wireConnectedFlag = new Operational.Flag("generatorWireConnected", Operational.Flag.Type.Requirement);

	[DebuggerDisplay("{tag} -{consumptionRate} kg/s")]
	[Serializable]
	public struct InputItem
	{
		public InputItem(Tag tag, float consumption_rate, float max_stored_mass)
		{
			this.tag = tag;
			this.consumptionRate = consumption_rate;
			this.maxStoredMass = max_stored_mass;
		}

		public Tag tag;

		public float consumptionRate;

		public float maxStoredMass;
	}

	[DebuggerDisplay("{element} {creationRate} kg/s")]
	[Serializable]
	public struct OutputItem
	{
		public OutputItem(SimHashes element, float creation_rate, bool store)
		{
			this = new EnergyGenerator.OutputItem(element, creation_rate, store, CellOffset.none);
		}

		public OutputItem(SimHashes element, float creation_rate, bool store, CellOffset emit_offset)
		{
			this.element = element;
			this.creationRate = creation_rate;
			this.store = store;
			this.emitOffset = emit_offset;
		}

		public SimHashes element;

		public float creationRate;

		public bool store;

		public CellOffset emitOffset;
	}

	[Serializable]
	public struct Formula
	{
		public EnergyGenerator.InputItem[] inputs;

		public EnergyGenerator.OutputItem[] outputs;

		public Tag meterTag;
	}
}
