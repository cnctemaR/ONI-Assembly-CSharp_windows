using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ElementData;
using Klei.AI;
using STRINGS;
using UnityEngine;

[DebuggerDisplay("{name}")]
[Serializable]
public class Element : IComparable<Element>
{
	public bool IsMoltenMetal
	{
		get
		{
			return this.IsLiquid && this.substance.Metalic && this.lowTemp > 373.15f && this.HasTag(GameTags.Metal);
		}
	}

	public float GetRelativeHeatLevel(float currentTemperature)
	{
		float num = this.lowTemp - 3f;
		float num2 = this.highTemp + 3f;
		return Mathf.Clamp01((currentTemperature - num) / (num2 - num));
	}

	public float PressureToMass(float pressure)
	{
		return pressure / this.defaultValues.pressure;
	}

	public bool IsSlippery
	{
		get
		{
			return this.HasTag(GameTags.Slippery);
		}
	}

	public bool IsUnstable
	{
		get
		{
			return this.HasTag(GameTags.Unstable);
		}
	}

	public bool IsLiquid
	{
		get
		{
			return (this.state & Element.State.Solid) == Element.State.Liquid;
		}
	}

	public bool IsGas
	{
		get
		{
			return (this.state & Element.State.Solid) == Element.State.Gas;
		}
	}

	public bool IsSolid
	{
		get
		{
			return (this.state & Element.State.Solid) == Element.State.Solid;
		}
	}

	public bool IsVacuum
	{
		get
		{
			return (this.state & Element.State.Solid) == Element.State.Vacuum;
		}
	}

	public bool IsTemperatureInsulated
	{
		get
		{
			return (this.state & Element.State.TemperatureInsulated) > Element.State.Vacuum;
		}
	}

	public bool IsState(Element.State expected_state)
	{
		return (this.state & Element.State.Solid) == expected_state;
	}

	public bool HasTransitionUp
	{
		get
		{
			return this.highTempTransitionTarget != (SimHashes)0 && this.highTempTransitionTarget != SimHashes.Unobtanium && this.highTempTransition != null && this.highTempTransition != this;
		}
	}

	public string name { get; set; }

	public string nameUpperCase { get; set; }

	public string description { get; set; }

	public string GetStateString()
	{
		return Element.GetStateString(this.state);
	}

	public static string GetStateString(Element.State state)
	{
		if ((state & Element.State.Solid) == Element.State.Solid)
		{
			return ELEMENTS.STATE.SOLID;
		}
		if ((state & Element.State.Solid) == Element.State.Liquid)
		{
			return ELEMENTS.STATE.LIQUID;
		}
		if ((state & Element.State.Solid) == Element.State.Gas)
		{
			return ELEMENTS.STATE.GAS;
		}
		return ELEMENTS.STATE.VACUUM;
	}

	public string FullDescription(bool addHardnessColor = true)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		stringBuilder.Clear();
		stringBuilder.Append(this.Description());
		if (this.IsSolid)
		{
			stringBuilder.Append("\n\n");
			stringBuilder.AppendFormat(ELEMENTS.ELEMENTDESCSOLID, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(this.highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetHardnessString(this, addHardnessColor));
		}
		else if (this.IsLiquid)
		{
			stringBuilder.Append("\n\n");
			stringBuilder.AppendFormat(ELEMENTS.ELEMENTDESCLIQUID, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(this.lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(this.highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
		}
		else if (!this.IsVacuum)
		{
			stringBuilder.Append("\n\n");
			stringBuilder.AppendFormat(ELEMENTS.ELEMENTDESCGAS, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(this.lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
		}
		StringBuilder stringBuilder2 = GlobalStringBuilderPool.Alloc();
		stringBuilder2.Append(ELEMENTS.THERMALPROPERTIES);
		stringBuilder2.Replace("{SPECIFIC_HEAT_CAPACITY}", GameUtil.GetFormattedSHC(this.specificHeatCapacity));
		stringBuilder2.Replace("{THERMAL_CONDUCTIVITY}", GameUtil.GetFormattedThermalConductivity(this.thermalConductivity));
		stringBuilder.Append("\n");
		stringBuilder.Append(stringBuilder2.ToString());
		GlobalStringBuilderPool.Free(stringBuilder2);
		if (DlcManager.FeatureRadiationEnabled())
		{
			stringBuilder.Append("\n");
			stringBuilder.AppendFormat(ELEMENTS.RADIATIONPROPERTIES, this.radiationAbsorptionFactor, GameUtil.GetFormattedRads(this.radiationPer1000Mass * 1.1f / 600f, GameUtil.TimeSlice.PerCycle));
		}
		if (this.oreTags.Length != 0 && !this.IsVacuum)
		{
			stringBuilder.Append("\n\n");
			StringBuilder stringBuilder3 = GlobalStringBuilderPool.Alloc();
			for (int i = 0; i < this.oreTags.Length; i++)
			{
				Tag tag = new Tag(this.oreTags[i]);
				if (!GameTags.HiddenElementTags.Contains(tag))
				{
					stringBuilder3.Append(tag.ProperName());
					if (i < this.oreTags.Length - 1)
					{
						stringBuilder3.Append(", ");
					}
				}
			}
			stringBuilder.AppendFormat(ELEMENTS.ELEMENTPROPERTIES, GlobalStringBuilderPool.ReturnAndFree(stringBuilder3));
		}
		if (this.attributeModifiers.Count > 0)
		{
			foreach (AttributeModifier attributeModifier in this.attributeModifiers)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name, attributeModifier.GetFormattedString());
			}
		}
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public string Description()
	{
		return this.description;
	}

	public bool HasTag(Tag search_tag)
	{
		return this.tag == search_tag || Array.IndexOf<Tag>(this.oreTags, search_tag) != -1;
	}

	public Tag GetMaterialCategoryTag()
	{
		return this.materialCategory;
	}

	public int CompareTo(Element other)
	{
		return this.id - other.id;
	}

	public const int INVALID_ID = 0;

	public SimHashes id;

	public Tag tag;

	public ushort idx;

	public float specificHeatCapacity;

	public float thermalConductivity = 1f;

	public float molarMass = 1f;

	public float strength;

	public float flow;

	public float maxCompression;

	public float viscosity;

	public float minHorizontalFlow = float.PositiveInfinity;

	public float minVerticalFlow = float.PositiveInfinity;

	public float maxMass = 10000f;

	public float solidSurfaceAreaMultiplier;

	public float liquidSurfaceAreaMultiplier;

	public float gasSurfaceAreaMultiplier;

	public Element.State state;

	public byte hardness;

	public float lowTemp;

	public SimHashes lowTempTransitionTarget;

	public Element lowTempTransition;

	public float highTemp;

	public SimHashes highTempTransitionTarget;

	public Element highTempTransition;

	public SimHashes highTempTransitionOreID = SimHashes.Vacuum;

	public float highTempTransitionOreMassConversion;

	public SimHashes lowTempTransitionOreID = SimHashes.Vacuum;

	public float lowTempTransitionOreMassConversion;

	public SimHashes sublimateId;

	public SimHashes convertId;

	public SpawnFXHashes sublimateFX;

	public float sublimateRate;

	public float sublimateEfficiency;

	public float sublimateProbability;

	public float offGasPercentage;

	public float lightAbsorptionFactor;

	public float radiationAbsorptionFactor;

	public float radiationPer1000Mass;

	public Sim.PhysicsData defaultValues;

	public SimHashes refinedMetalTarget;

	public float toxicity;

	public Substance substance;

	public Tag materialCategory;

	public int buildMenuSort;

	public ElementComposition[] elementComposition;

	public Tag[] oreTags = new Tag[0];

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public bool disabled;

	public string dlcId;

	private const float MOLTEN_LIQUID_MIN_TEMP = 373.15f;

	public const byte StateMask = 3;

	[Serializable]
	public enum State : byte
	{
		Vacuum,
		Gas,
		Liquid,
		Solid,
		Unbreakable,
		Unstable = 8,
		TemperatureInsulated = 16
	}
}
