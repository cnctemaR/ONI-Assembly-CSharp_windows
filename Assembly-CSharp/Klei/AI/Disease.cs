using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public abstract class Disease : Resource
	{
		public Disease(string id, Disease.DiseaseType type, Disease.Severity severity, float immune_attack_strength, List<Disease.InfectionVector> infection_vectors, float sickness_duration, byte strength, Disease.RangeInfo temperature_range, Disease.RangeInfo temperature_half_lives, Disease.RangeInfo pressure_range, Disease.RangeInfo pressure_half_lives)
			: base(id, null, null)
		{
			this.name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
			this.id = id;
			this.diseaseType = type;
			this.severity = severity;
			this.immuneAttackStrength = immune_attack_strength;
			this.infectionVectors = infection_vectors;
			this.sicknessDuration = sickness_duration;
			this.overlayColour = Assets.instance.DiseaseVisualization.GetInfo(id).overlayColour;
			this.temperatureRange = temperature_range;
			this.temperatureHalfLives = temperature_half_lives;
			this.pressureRange = pressure_range;
			this.pressureHalfLives = pressure_half_lives;
			this.descriptiveSymptoms = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".DESCRIPTIVE_SYMPTOMS");
			this.PopulateElemGrowthInfo();
			this.ApplyRules();
			string text = Strings.Get("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".LEGEND_HOVERTEXT").ToString();
			foreach (Descriptor descriptor in this.GetQualitativeDescriptors())
			{
				text = text + string.Empty + descriptor.IndentedText() + "\n";
			}
			this.overlayLegendHovertext = text + DUPLICANTS.DISEASES.LEGEND_POSTAMBLE;
			Attribute attribute = new Attribute(id + "Min", "Minimum" + id.ToString(), string.Empty, string.Empty, 0f, Attribute.Display.Never, false);
			Attribute attribute2 = new Attribute(id + "Max", "Maximum" + id.ToString(), string.Empty, string.Empty, 10000000f, Attribute.Display.Never, false);
			this.amountDeltaAttribute = new Attribute(id + "Delta", id.ToString(), string.Empty, string.Empty, 0f, Attribute.Display.Never, false);
			this.amount = new Amount(id, id + " " + DUPLICANTS.DISEASES.GERMS, id + " " + DUPLICANTS.DISEASES.GERMS, 0f, 10000000f, attribute, attribute2, this.amountDeltaAttribute, false, Units.Flat, 0.01f, true);
			Db.Get().Attributes.Add(attribute);
			Db.Get().Attributes.Add(attribute2);
			Db.Get().Attributes.Add(this.amountDeltaAttribute);
		}

		public new string Name
		{
			get
			{
				return Strings.Get(this.name);
			}
		}

		public float SicknessDuration
		{
			get
			{
				return this.sicknessDuration;
			}
		}

		protected virtual void PopulateElemGrowthInfo()
		{
			this.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			this.AddGrowthRule(new Disease.GrowthRule
			{
				underPopulationDeathRate = new float?(0f),
				minCount = new int?(100),
				populationHalfLife = new float?(float.PositiveInfinity),
				maxCount = new int?(1000000),
				overPopulationHalfLife = new float?(float.PositiveInfinity),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = 1
			});
			this.InitializeElemGrowthArray(ref this.elemExposureInfo, Disease.DEFAULT_GROWTH_INFO);
			this.AddExposureRule(new Disease.GrowthRule
			{
				underPopulationDeathRate = new float?(0f),
				minCount = new int?(100),
				populationHalfLife = new float?(float.PositiveInfinity),
				maxCount = new int?(1000000),
				overPopulationHalfLife = new float?(float.PositiveInfinity)
			});
		}

		protected void AddGrowthRule(Disease.GrowthRule g)
		{
			if (this.growthRules == null)
			{
				this.growthRules = new List<Disease.GrowthRule>();
			}
			this.growthRules.Add(g);
		}

		protected void AddExposureRule(Disease.GrowthRule g)
		{
			if (this.exposureRules == null)
			{
				this.exposureRules = new List<Disease.GrowthRule>();
			}
			this.exposureRules.Add(g);
		}

		public Disease.CompositeGrowthRule GetGrowthRuleForElement(Element e)
		{
			Disease.CompositeGrowthRule compositeGrowthRule = new Disease.CompositeGrowthRule();
			if (this.growthRules != null)
			{
				for (int i = 0; i < this.growthRules.Count; i++)
				{
					if (this.growthRules[i].Test(e))
					{
						compositeGrowthRule.Overlay(this.growthRules[i]);
					}
				}
			}
			return compositeGrowthRule;
		}

		public Disease.CompositeGrowthRule GetExposureRuleForElement(Element e)
		{
			Disease.CompositeGrowthRule compositeGrowthRule = new Disease.CompositeGrowthRule();
			if (this.exposureRules != null)
			{
				for (int i = 0; i < this.exposureRules.Count; i++)
				{
					if (this.exposureRules[i].Test(e))
					{
						compositeGrowthRule.Overlay(this.exposureRules[i]);
					}
				}
			}
			return compositeGrowthRule;
		}

		public Disease.TagGrowthRule GetGrowthRuleForTag(Tag t)
		{
			if (this.growthRules != null)
			{
				for (int i = 0; i < this.growthRules.Count; i++)
				{
					Disease.TagGrowthRule tagGrowthRule = this.growthRules[i] as Disease.TagGrowthRule;
					if (tagGrowthRule != null && tagGrowthRule.tag == t)
					{
						return tagGrowthRule;
					}
				}
			}
			return null;
		}

		protected void ApplyRules()
		{
			if (this.growthRules != null)
			{
				for (int i = 0; i < this.growthRules.Count; i++)
				{
					this.growthRules[i].Apply(this.elemGrowthInfo);
				}
			}
			if (this.exposureRules != null)
			{
				for (int j = 0; j < this.exposureRules.Count; j++)
				{
					this.exposureRules[j].Apply(this.elemExposureInfo);
				}
			}
		}

		protected void InitializeElemGrowthArray(ref Disease.ElemGrowthInfo[] infoArray, Disease.ElemGrowthInfo default_value)
		{
			List<Element> elements = ElementLoader.elements;
			infoArray = new Disease.ElemGrowthInfo[elements.Count];
			for (int i = 0; i < elements.Count; i++)
			{
				infoArray[i] = default_value;
			}
		}

		public float GetGrowthRateForTags(ICollection<Tag> tags, bool overpopulated)
		{
			float num = 1f;
			if (this.growthRules != null)
			{
				for (int i = 0; i < this.growthRules.Count; i++)
				{
					Disease.TagGrowthRule tagGrowthRule = this.growthRules[i] as Disease.TagGrowthRule;
					if (tagGrowthRule != null && tags.Contains(tagGrowthRule.tag))
					{
						num *= Disease.HalfLifeToGrowthRate(((!overpopulated) ? tagGrowthRule.populationHalfLife : tagGrowthRule.overPopulationHalfLife).Value, 1f);
					}
				}
			}
			return num;
		}

		public object[] Infect(GameObject go, DiseaseInstance diseaseInstance, DiseaseExposureInfo exposure_info)
		{
			object[] array = new object[this.components.Count];
			for (int i = 0; i < this.components.Count; i++)
			{
				array[i] = this.components[i].OnInfect(go, diseaseInstance);
			}
			return array;
		}

		public void Cure(GameObject go, object[] componentData)
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				this.components[i].OnCure(go, componentData[i]);
			}
		}

		public List<Descriptor> GetSymptoms()
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(Strings.Get("STRINGS.DUPLICANTS.DISEASES." + this.Id.ToUpper() + ".DESCRIPTION"), Strings.Get("STRINGS.DUPLICANTS.DISEASES." + this.Id.ToUpper() + ".DESCRIPTION"), Descriptor.DescriptorType.Information, false));
			for (int i = 0; i < this.components.Count; i++)
			{
				List<Descriptor> symptoms = this.components[i].GetSymptoms();
				if (symptoms != null)
				{
					list.AddRange(symptoms);
				}
			}
			if (this.fatalityDuration > 0f)
			{
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DEATH_SYMPTOM, GameUtil.GetFormattedCycles(this.fatalityDuration, "F1")), string.Format(DUPLICANTS.DISEASES.DEATH_SYMPTOM_TOOLTIP, GameUtil.GetFormattedCycles(this.fatalityDuration, "F1")), Descriptor.DescriptorType.SymptomAidable, false));
			}
			return list;
		}

		public static float HalfLifeToGrowthRate(float half_life_in_seconds, float dt)
		{
			float num;
			if (half_life_in_seconds == 0f)
			{
				num = 0f;
			}
			else if (half_life_in_seconds == float.PositiveInfinity)
			{
				num = 1f;
			}
			else
			{
				float num2 = half_life_in_seconds / dt;
				num = Mathf.Pow(2f, -1f / num2);
			}
			return num;
		}

		public static float GrowthRateToHalfLife(float growth_rate)
		{
			float num;
			if (growth_rate == 0f)
			{
				num = 0f;
			}
			else if (growth_rate == 1f)
			{
				num = float.PositiveInfinity;
			}
			else
			{
				num = Mathf.Log(2f, growth_rate);
			}
			return num;
		}

		public float CalculateTemperatureHalfLife(float temperature)
		{
			return Disease.CalculateRangeHalfLife(temperature, ref this.temperatureRange, ref this.temperatureHalfLives);
		}

		public static float CalculateRangeHalfLife(float range_value, ref Disease.RangeInfo range, ref Disease.RangeInfo half_lives)
		{
			int num = 3;
			int num2 = 3;
			for (int i = 0; i < 4; i++)
			{
				if (range_value <= range.GetValue(i))
				{
					num = i - 1;
					num2 = i;
					break;
				}
			}
			if (num < 0)
			{
				num = num2;
			}
			float value = half_lives.GetValue(num);
			float value2 = half_lives.GetValue(num2);
			if (num == 1 && num2 == 2)
			{
				return float.PositiveInfinity;
			}
			if (float.IsInfinity(value) || float.IsInfinity(value2))
			{
				return float.PositiveInfinity;
			}
			float value3 = range.GetValue(num);
			float value4 = range.GetValue(num2);
			float num3 = 0f;
			float num4 = value4 - value3;
			if (num4 > 0f)
			{
				num3 = (range_value - value3) / num4;
			}
			return Mathf.Lerp(value, value2, num3);
		}

		protected void AddDiseaseComponent(Disease.DiseaseComponent cmp)
		{
			this.components.Add(cmp);
		}

		public T GetDiseaseComponent<T>() where T : Disease.DiseaseComponent
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i] is T)
				{
					return this.components[i] as T;
				}
			}
			return (T)((object)null);
		}

		public virtual List<Descriptor> GetDiseaseSourceDescriptors()
		{
			return new List<Descriptor>();
		}

		public List<Descriptor> GetQualitativeDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			using (List<Disease.InfectionVector>.Enumerator enumerator = this.infectionVectors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case Disease.InfectionVector.Contact:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case Disease.InfectionVector.Digestion:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case Disease.InfectionVector.Inhalation:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					}
				}
			}
			list.Add(new Descriptor(Strings.Get(this.descriptiveSymptoms), string.Empty, Descriptor.DescriptorType.Information, false));
			return list;
		}

		public List<Descriptor> GetQuantitativeDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE, GameUtil.GetFormattedTemperature(this.temperatureRange.minViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(this.temperatureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE_TOOLTIP, new object[]
			{
				GameUtil.GetFormattedTemperature(this.temperatureRange.minViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true),
				GameUtil.GetFormattedTemperature(this.temperatureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true),
				GameUtil.GetFormattedTemperature(this.temperatureRange.minGrowth, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true),
				GameUtil.GetFormattedTemperature(this.temperatureRange.maxGrowth, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)
			}), Descriptor.DescriptorType.Information, false));
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.PRESSURE_RANGE, GameUtil.GetFormattedMass(this.pressureRange.minViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedMass(this.pressureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.PRESSURE_RANGE_TOOLTIP, new object[]
			{
				GameUtil.GetFormattedMass(this.pressureRange.minViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"),
				GameUtil.GetFormattedMass(this.pressureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"),
				GameUtil.GetFormattedMass(this.pressureRange.minGrowth, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"),
				GameUtil.GetFormattedMass(this.pressureRange.maxGrowth, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")
			}), Descriptor.DescriptorType.Information, false));
			List<Disease.GrowthRule> list2 = new List<Disease.GrowthRule>();
			List<Disease.GrowthRule> list3 = new List<Disease.GrowthRule>();
			List<Disease.GrowthRule> list4 = new List<Disease.GrowthRule>();
			List<Disease.GrowthRule> list5 = new List<Disease.GrowthRule>();
			List<Disease.GrowthRule> list6 = new List<Disease.GrowthRule>();
			foreach (Disease.GrowthRule growthRule in this.growthRules)
			{
				float? populationHalfLife = growthRule.populationHalfLife;
				if (populationHalfLife != null)
				{
					if (growthRule.Name() != null)
					{
						float? populationHalfLife2 = growthRule.populationHalfLife;
						if (populationHalfLife2.Value < 0f)
						{
							list2.Add(growthRule);
						}
						else
						{
							float? populationHalfLife3 = growthRule.populationHalfLife;
							if (populationHalfLife3.Value == float.PositiveInfinity)
							{
								list3.Add(growthRule);
							}
							else
							{
								float? populationHalfLife4 = growthRule.populationHalfLife;
								if (populationHalfLife4.Value >= 12000f)
								{
									list4.Add(growthRule);
								}
								else
								{
									float? populationHalfLife5 = growthRule.populationHalfLife;
									if (populationHalfLife5.Value >= 1200f)
									{
										list5.Add(growthRule);
									}
									else
									{
										list6.Add(growthRule);
									}
								}
							}
						}
					}
				}
			}
			list.AddRange(this.BuildGrowthInfoDescriptors(list2, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_TOOLTIP));
			list.AddRange(this.BuildGrowthInfoDescriptors(list3, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_TOOLTIP));
			list.AddRange(this.BuildGrowthInfoDescriptors(list4, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_TOOLTIP));
			list.AddRange(this.BuildGrowthInfoDescriptors(list5, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_TOOLTIP));
			list.AddRange(this.BuildGrowthInfoDescriptors(list6, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_TOOLTIP));
			return list;
		}

		private List<Descriptor> BuildGrowthInfoDescriptors(List<Disease.GrowthRule> rules, string section_text, string section_tooltip, string item_tooltip)
		{
			List<Descriptor> list = new List<Descriptor>();
			if (rules.Count > 0)
			{
				list.Add(new Descriptor(section_text, section_tooltip, Descriptor.DescriptorType.Information, false));
				for (int i = 0; i < rules.Count; i++)
				{
					List<Descriptor> list2 = list;
					string text = string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWTH_FORMAT, rules[i].Name());
					float? populationHalfLife = rules[i].populationHalfLife;
					list2.Add(new Descriptor(text, string.Format(item_tooltip, GameUtil.GetFormattedCycles(Mathf.Abs(populationHalfLife.Value), "F1")), Descriptor.DescriptorType.Information, false));
				}
			}
			return list;
		}

		private StringKey name;

		private StringKey descriptiveSymptoms;

		private float sicknessDuration = 600f;

		public bool doctorRequired;

		public float fatalityDuration;

		public HashedString id;

		public Disease.DiseaseType diseaseType;

		public Disease.Severity severity;

		public float strength;

		public float immuneAttackStrength;

		public Disease.RangeInfo temperatureRange;

		public Disease.RangeInfo temperatureHalfLives;

		public Disease.RangeInfo pressureRange;

		public Disease.RangeInfo pressureHalfLives;

		public List<Disease.GrowthRule> growthRules;

		public List<Disease.GrowthRule> exposureRules;

		public Disease.ElemGrowthInfo[] elemGrowthInfo;

		public Disease.ElemGrowthInfo[] elemExposureInfo;

		public List<Disease.InfectionVector> infectionVectors;

		public Color32 overlayColour = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

		public string overlayLegendHovertext;

		public bool isPathogen;

		private List<Disease.DiseaseComponent> components = new List<Disease.DiseaseComponent>();

		public Amount amount;

		public Attribute amountDeltaAttribute;

		public static Disease.ElemGrowthInfo DEFAULT_GROWTH_INFO = new Disease.ElemGrowthInfo
		{
			maxCount = int.MaxValue,
			underPopulationDeathRate = 0f,
			overPopulationHalfLife = float.PositiveInfinity,
			minDiffusionCount = 0,
			diffusionScale = 1f
		};

		public struct RangeInfo
		{
			public RangeInfo(float min_viable, float min_growth, float max_growth, float max_viable)
			{
				this.minViable = min_viable;
				this.minGrowth = min_growth;
				this.maxGrowth = max_growth;
				this.maxViable = max_viable;
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(this.minViable);
				writer.Write(this.minGrowth);
				writer.Write(this.maxGrowth);
				writer.Write(this.maxViable);
			}

			public float GetValue(int idx)
			{
				switch (idx)
				{
				case 0:
					return this.minViable;
				case 1:
					return this.minGrowth;
				case 2:
					return this.maxGrowth;
				case 3:
					return this.maxViable;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}

			public static Disease.RangeInfo Idempotent()
			{
				return new Disease.RangeInfo(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			}

			public float minViable;

			public float minGrowth;

			public float maxGrowth;

			public float maxViable;
		}

		public class GrowthRule
		{
			public void Apply(Disease.ElemGrowthInfo[] infoList)
			{
				List<Element> elements = ElementLoader.elements;
				for (int i = 0; i < elements.Count; i++)
				{
					if (this.Test(elements[i]))
					{
						Disease.ElemGrowthInfo elemGrowthInfo = infoList[i];
						float? num = this.underPopulationDeathRate;
						if (num != null)
						{
							float? num2 = this.underPopulationDeathRate;
							elemGrowthInfo.underPopulationDeathRate = num2.Value;
						}
						float? num3 = this.populationHalfLife;
						if (num3 != null)
						{
							float? num4 = this.populationHalfLife;
							elemGrowthInfo.populationHalfLife = num4.Value;
						}
						float? num5 = this.overPopulationHalfLife;
						if (num5 != null)
						{
							float? num6 = this.overPopulationHalfLife;
							elemGrowthInfo.overPopulationHalfLife = num6.Value;
						}
						float? num7 = this.diffusionScale;
						if (num7 != null)
						{
							float? num8 = this.diffusionScale;
							elemGrowthInfo.diffusionScale = num8.Value;
						}
						int? num9 = this.minCount;
						if (num9 != null)
						{
							int? num10 = this.minCount;
							elemGrowthInfo.minCount = num10.Value;
						}
						int? num11 = this.maxCount;
						if (num11 != null)
						{
							int? num12 = this.maxCount;
							elemGrowthInfo.maxCount = num12.Value;
						}
						int? num13 = this.minDiffusionCount;
						if (num13 != null)
						{
							int? num14 = this.minDiffusionCount;
							elemGrowthInfo.minDiffusionCount = num14.Value;
						}
						byte? b = this.minDiffusionInfestationTickCount;
						if (b != null)
						{
							byte? b2 = this.minDiffusionInfestationTickCount;
							elemGrowthInfo.minDiffusionInfestationTickCount = b2.Value;
						}
						infoList[i] = elemGrowthInfo;
					}
				}
			}

			public virtual bool Test(Element e)
			{
				return true;
			}

			public virtual string Name()
			{
				return null;
			}

			public float? underPopulationDeathRate;

			public float? populationHalfLife;

			public float? overPopulationHalfLife;

			public float? diffusionScale;

			public int? minCount;

			public int? maxCount;

			public int? minDiffusionCount;

			public byte? minDiffusionInfestationTickCount;
		}

		public class StateGrowthRule : Disease.GrowthRule
		{
			public StateGrowthRule(Element.State state)
			{
				this.state = state;
			}

			public override bool Test(Element e)
			{
				return e.IsState(this.state);
			}

			public override string Name()
			{
				return Element.GetStateString(this.state);
			}

			public Element.State state;
		}

		public class ElementGrowthRule : Disease.GrowthRule
		{
			public ElementGrowthRule(SimHashes element)
			{
				this.element = element;
			}

			public override bool Test(Element e)
			{
				return e.id == this.element;
			}

			public override string Name()
			{
				return ElementLoader.FindElementByHash(this.element).name;
			}

			public SimHashes element;
		}

		public class TagGrowthRule : Disease.GrowthRule
		{
			public TagGrowthRule(Tag tag)
			{
				this.tag = tag;
			}

			public override bool Test(Element e)
			{
				return e.HasTag(this.tag);
			}

			public override string Name()
			{
				return this.tag.ProperName();
			}

			public Tag tag;
		}

		public class CompositeGrowthRule
		{
			public string Name()
			{
				return this.name;
			}

			public void Overlay(Disease.GrowthRule rule)
			{
				float? num = rule.underPopulationDeathRate;
				if (num != null)
				{
					float? num2 = rule.underPopulationDeathRate;
					this.underPopulationDeathRate = num2.Value;
				}
				float? num3 = rule.populationHalfLife;
				if (num3 != null)
				{
					float? num4 = rule.populationHalfLife;
					this.populationHalfLife = num4.Value;
				}
				float? num5 = rule.overPopulationHalfLife;
				if (num5 != null)
				{
					float? num6 = rule.overPopulationHalfLife;
					this.overPopulationHalfLife = num6.Value;
				}
				float? num7 = rule.diffusionScale;
				if (num7 != null)
				{
					float? num8 = rule.diffusionScale;
					this.diffusionScale = num8.Value;
				}
				int? num9 = rule.minCount;
				if (num9 != null)
				{
					int? num10 = rule.minCount;
					this.minCount = num10.Value;
				}
				int? num11 = rule.maxCount;
				if (num11 != null)
				{
					int? num12 = rule.maxCount;
					this.maxCount = num12.Value;
				}
				int? num13 = rule.minDiffusionCount;
				if (num13 != null)
				{
					int? num14 = rule.minDiffusionCount;
					this.minDiffusionCount = num14.Value;
				}
				byte? b = rule.minDiffusionInfestationTickCount;
				if (b != null)
				{
					byte? b2 = rule.minDiffusionInfestationTickCount;
					this.minDiffusionInfestationTickCount = b2.Value;
				}
				this.name = rule.Name();
			}

			public float GetHalfLifeForCount(int count)
			{
				if (count < this.minCount)
				{
					return this.populationHalfLife;
				}
				if (count < this.maxCount)
				{
					return this.populationHalfLife;
				}
				return this.overPopulationHalfLife;
			}

			public string name;

			public float underPopulationDeathRate;

			public float populationHalfLife;

			public float overPopulationHalfLife;

			public float diffusionScale;

			public int minCount;

			public int maxCount;

			public int minDiffusionCount;

			public byte minDiffusionInfestationTickCount;
		}

		public struct ElemGrowthInfo
		{
			public void Write(BinaryWriter writer)
			{
				writer.Write(this.underPopulationDeathRate);
				writer.Write(this.populationHalfLife);
				writer.Write(this.overPopulationHalfLife);
				writer.Write(this.diffusionScale);
				writer.Write(this.minCount);
				writer.Write(this.maxCount);
				writer.Write(this.minDiffusionCount);
				writer.Write(this.minDiffusionInfestationTickCount);
			}

			public static void SetBulk(Disease.ElemGrowthInfo[] info, Func<Element, bool> test, Disease.ElemGrowthInfo settings)
			{
				List<Element> elements = ElementLoader.elements;
				for (int i = 0; i < elements.Count; i++)
				{
					if (test(elements[i]))
					{
						info[i] = settings;
					}
				}
			}

			public float CalculateDiseaseCountDelta(int disease_count, float dt)
			{
				float num2;
				if (this.minCount <= disease_count && disease_count <= this.maxCount)
				{
					float num = Disease.HalfLifeToGrowthRate(this.populationHalfLife, dt);
					num2 = (float)disease_count * num - (float)disease_count;
				}
				else if (disease_count < this.minCount)
				{
					num2 = -this.underPopulationDeathRate * dt;
				}
				else
				{
					float num3 = Disease.HalfLifeToGrowthRate(this.overPopulationHalfLife, dt);
					int num4 = disease_count - this.maxCount;
					num2 = (float)num4 * num3 - (float)num4;
				}
				return num2;
			}

			public float underPopulationDeathRate;

			public float populationHalfLife;

			public float overPopulationHalfLife;

			public float diffusionScale;

			public int minCount;

			public int maxCount;

			public int minDiffusionCount;

			public byte minDiffusionInfestationTickCount;
		}

		public abstract class DiseaseComponent
		{
			public abstract object OnInfect(GameObject go, DiseaseInstance diseaseInstance);

			public abstract void OnCure(GameObject go, object instance_data);

			public virtual List<Descriptor> GetSymptoms()
			{
				return null;
			}
		}

		public enum InfectionVector
		{
			Contact,
			Digestion,
			Inhalation
		}

		public enum DiseaseType
		{
			Pathogen,
			Ailment,
			Injury
		}

		public enum Severity
		{
			Benign,
			Minor,
			Major,
			Critical
		}
	}
}
