using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using STRINGS;
using UnityEngine;

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

	public bool HasTransitionUp
	{
		get
		{
			return this.highTempTransitionTarget != (SimHashes)0 && this.highTempTransitionTarget != SimHashes.Unobtanium && this.highTempTransition != null && this.highTempTransition != this;
		}
	}

	public string name
	{
		get
		{
			return Strings.Get("STRINGS.ELEMENTS." + this.id.ToString().ToUpper() + ".NAME");
		}
	}

	public string FullDescription(bool addHardnessColor = true)
	{
		string text = this.Description();
		if (this.IsSolid)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCSOLID, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(this.highTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute), GameUtil.GetHardnessString(this, addHardnessColor));
		}
		else if (this.IsLiquid)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCLIQUID, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(this.lowTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute), GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(this.highTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
		}
		else if (this.IsVacuum)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCVACUUM, this.GetMaterialCategoryTag().ProperName());
		}
		else
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCGAS, this.GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(this.lowTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
		}
		if (this.oreTags.Length > 0)
		{
			text += "\n\n";
			string text2 = string.Empty;
			for (int i = 0; i < this.oreTags.Length; i++)
			{
				Tag tag = new Tag(this.oreTags[i]);
				text2 += tag.ProperName();
				if (i < this.oreTags.Length - 1)
				{
					text2 += ", ";
				}
			}
			text += string.Format(ELEMENTS.ELEMENTPROPERTIES, text2);
		}
		if (this.attributeModifiers.Count > 0)
		{
			foreach (AttributeModifier attributeModifier in this.attributeModifiers)
			{
				Klei.AI.Attribute attribute = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
				string text3 = text;
				text = string.Concat(new object[]
				{
					text3,
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

	public bool HasTag(Tag tag)
	{
		for (int i = 0; i < this.oreTags.Length; i++)
		{
			if (this.oreTags[i] == tag)
			{
				return true;
			}
		}
		return false;
	}

	public static List<Tag> GetMaterialCategoryTags()
	{
		return new List<Tag>(Element.materialCategoryTags);
	}

	public Tag GetMaterialCategoryTag()
	{
		return this.materialCategory;
	}

	public int CompareTo(Element other)
	{
		return this.id - other.id;
	}

	public const byte StateMask = 3;

	public SimHashes id;

	public Tag tag;

	public float specificHeatCapacity;

	public float thermalConductivity = 1f;

	public float electricalConductivity;

	public float molarMass = 1f;

	public float strength;

	public float flow;

	public float maxCompression;

	public float viscosity;

	public float minHorizontalLiquidFlow = float.PositiveInfinity;

	public float minVerticalLiquidFlow = float.PositiveInfinity;

	public float maxMass = 10000f;

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

	public Sim.PhysicsData defaultValues;

	public float emitDistance;

	public int emitIntensity;

	public float transparency;

	public float toxicity;

	public Substance substance;

	public Tag materialCategory;

	public Tag[] oreTags = new Tag[0];

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public string keywordStyle;

	private static Tag[] materialCategoryTags = new Tag[]
	{
		GameTags.Alloy,
		GameTags.Metal,
		GameTags.RefinedMetal,
		GameTags.BuildableRaw,
		GameTags.BuildableProcessed,
		GameTags.Liquifiable,
		GameTags.Liquid,
		GameTags.Farmable
	};

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
