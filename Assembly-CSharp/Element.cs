using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using STRINGS;

[DebuggerDisplay("{id}")]
[Serializable]
public class Element : IComparable<Element>
{
	public float PressureToMass(float pressure)
	{
		return pressure / this.defaultValues.pressure;
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
			return (byte)(this.state & Element.State.Solid) == 2;
		}
	}

	public bool IsGas
	{
		get
		{
			return (byte)(this.state & Element.State.Solid) == 1;
		}
	}

	public bool IsSolid
	{
		get
		{
			return (byte)(this.state & Element.State.Solid) == 3;
		}
	}

	public bool IsVacuum
	{
		get
		{
			return (byte)(this.state & Element.State.Solid) == 0;
		}
	}

	public bool IsTemperatureInsulated
	{
		get
		{
			return (byte)(this.state & Element.State.TemperatureInsulated) != 0;
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

	public string GetStateString()
	{
		return Element.GetStateString(this.state);
	}

	public static string GetStateString(Element.State state)
	{
		if ((byte)(state & Element.State.Solid) == 3)
		{
			return ELEMENTS.STATE.SOLID;
		}
		if ((byte)(state & Element.State.Solid) == 2)
		{
			return ELEMENTS.STATE.LIQUID;
		}
		if ((byte)(state & Element.State.Solid) == 1)
		{
			return ELEMENTS.STATE.GAS;
		}
		return ELEMENTS.STATE.VACUUM;
	}

	public string FullDescription(bool addHardnessColor = true)
	{
		string text = this.Description();
		if (this.IsSolid)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCSOLID, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(this.highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetHardnessString(this, addHardnessColor));
		}
		else if (this.IsLiquid)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCLIQUID, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(this.lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(this.highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
		}
		else if (!this.IsVacuum)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCGAS, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(this.lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
		}
		string text2 = ELEMENTS.THERMALPROPERTIES;
		text2 = text2.Replace("{SPECIFIC_HEAT_CAPACITY}", GameUtil.GetFormattedSHC(this.specificHeatCapacity));
		text2 = text2.Replace("{THERMAL_CONDUCTIVITY}", GameUtil.GetFormattedThermalConductivity(this.thermalConductivity));
		text = text + "\n" + text2;
		if (this.oreTags.Length > 0 && !this.IsVacuum)
		{
			text += "\n\n";
			string text3 = string.Empty;
			for (int i = 0; i < this.oreTags.Length; i++)
			{
				Tag tag = new Tag(this.oreTags[i]);
				text3 += tag.ProperName();
				if (i < this.oreTags.Length - 1)
				{
					text3 += ", ";
				}
			}
			text += string.Format(ELEMENTS.ELEMENTPROPERTIES, text3);
		}
		if (this.attributeModifiers.Count > 0)
		{
			foreach (AttributeModifier attributeModifier in this.attributeModifiers)
			{
				Klei.AI.Attribute attribute = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
				string text4 = text;
				text = string.Concat(new object[]
				{
					text4,
					"\n",
					attribute.Name,
					": +",
					attributeModifier.Value * 100f,
					"%"
				});
			}
		}
		return text;
	}

	public string Description()
	{
		return Strings.Get("STRINGS.ELEMENTS." + this.id.ToString().ToUpper() + ".DESC");
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

	public SimHashes id;

	public Tag tag;

	public byte idx;

	public float specificHeatCapacity;

	public float thermalConductivity = 1f;

	public float molarMass = 1f;

	public float strength;

	public float flow;

	public float maxCompression;

	public float viscosity;

	public float minHorizontalLiquidFlow = float.PositiveInfinity;

	public float minVerticalLiquidFlow = float.PositiveInfinity;

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

	public float lightAbsorptionFactor;

	public Sim.PhysicsData defaultValues;

	public float toxicity;

	public Substance substance;

	public Tag materialCategory;

	public Tag[] oreTags = new Tag[0];

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public bool disabled;

	public const byte StateMask = 3;

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
