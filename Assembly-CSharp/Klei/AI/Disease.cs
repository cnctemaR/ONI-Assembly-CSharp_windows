using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public abstract class Disease : Resource
	{
		public Disease(string id, byte strength, Disease.RangeInfo temperature_range, Disease.RangeInfo temperature_half_lives, Disease.RangeInfo pressure_range, Disease.RangeInfo pressure_half_lives)
			: base(id, null, null)
		{
			this.name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
			this.id = id;
			this.overlayColour = Assets.instance.DiseaseVisualization.GetInfo(id).overlayColour;
			this.temperatureRange = temperature_range;
			this.temperatureHalfLives = temperature_half_lives;
			this.pressureRange = pressure_range;
			this.pressureHalfLives = pressure_half_lives;
			this.PopulateElemGrowthInfo();
			this.ApplyRules();
			string text = Strings.Get("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".LEGEND_HOVERTEXT").ToString();
			this.overlayLegendHovertext = text + DUPLICANTS.DISEASES.LEGEND_POSTAMBLE;
			Attribute attribute = new Attribute(id + "Min", "Minimum" + id.ToString(), string.Empty, string.Empty, 0f, Attribute.Display.Normal, false, null, null);
			Attribute attribute2 = new Attribute(id + "Max", "Maximum" + id.ToString(), string.Empty, string.Empty, 10000000f, Attribute.Display.Normal, false, null, null);
			this.amountDeltaAttribute = new Attribute(id + "Delta", id.ToString(), string.Empty, string.Empty, 0f, Attribute.Display.Normal, false, null, null);
			this.amount = new Amount(id, id + " " + DUPLICANTS.DISEASES.GERMS, id + " " + DUPLICANTS.DISEASES.GERMS, attribute, attribute2, this.amountDeltaAttribute, false, Units.Flat, 0.01f, true, null, null);
			Db.Get().Attributes.Add(attribute);
			Db.Get().Attributes.Add(attribute2);
			Db.Get().Attributes.Add(this.amountDeltaAttribute);
			this.cureSpeedBase = new Attribute(id + "CureSpeed", false, Attribute.Display.Normal, false, 0f, null, null);
			this.cureSpeedBase.BaseValue = 1f;
			this.cureSpeedBase.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			Db.Get().Attributes.Add(this.cureSpeedBase);
		}

		public new string Name
		{
			get
			{
				return Strings.Get(this.name);
			}
		}

		protected virtual void PopulateElemGrowthInfo()
		{
			this.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			this.AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(0f),
				minCountPerKG = new float?(100f),
				populationHalfLife = new float?(float.PositiveInfinity),
				maxCountPerKG = new float?(1000f),
				overPopulationHalfLife = new float?(float.PositiveInfinity),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			this.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
			this.AddExposureRule(new ExposureRule
			{
				populationHalfLife = new float?(float.PositiveInfinity)
			});
		}

		protected void AddGrowthRule(GrowthRule g)
		{
			if (this.growthRules == null)
			{
				this.growthRules = new List<GrowthRule>();
				global::Debug.Assert(g.GetType() == typeof(GrowthRule), "First rule must be a fully defined base rule.");
				float? underPopulationDeathRate = g.underPopulationDeathRate;
				global::Debug.Assert(underPopulationDeathRate != null, "First rule must be a fully defined base rule.");
				float? populationHalfLife = g.populationHalfLife;
				global::Debug.Assert(populationHalfLife != null, "First rule must be a fully defined base rule.");
				float? overPopulationHalfLife = g.overPopulationHalfLife;
				global::Debug.Assert(overPopulationHalfLife != null, "First rule must be a fully defined base rule.");
				float? diffusionScale = g.diffusionScale;
				global::Debug.Assert(diffusionScale != null, "First rule must be a fully defined base rule.");
				float? minCountPerKG = g.minCountPerKG;
				global::Debug.Assert(minCountPerKG != null, "First rule must be a fully defined base rule.");
				float? maxCountPerKG = g.maxCountPerKG;
				global::Debug.Assert(maxCountPerKG != null, "First rule must be a fully defined base rule.");
				int? minDiffusionCount = g.minDiffusionCount;
				global::Debug.Assert(minDiffusionCount != null, "First rule must be a fully defined base rule.");
				byte? minDiffusionInfestationTickCount = g.minDiffusionInfestationTickCount;
				global::Debug.Assert(minDiffusionInfestationTickCount != null, "First rule must be a fully defined base rule.");
			}
			else
			{
				global::Debug.Assert(g.GetType() != typeof(GrowthRule), "Subsequent rules should not be base rules");
			}
			this.growthRules.Add(g);
		}

		protected void AddExposureRule(ExposureRule g)
		{
			if (this.exposureRules == null)
			{
				this.exposureRules = new List<ExposureRule>();
				global::Debug.Assert(g.GetType() == typeof(ExposureRule), "First rule must be a fully defined base rule.");
				float? populationHalfLife = g.populationHalfLife;
				global::Debug.Assert(populationHalfLife != null, "First rule must be a fully defined base rule.");
			}
			else
			{
				global::Debug.Assert(g.GetType() != typeof(ExposureRule), "Subsequent rules should not be base rules");
			}
			this.exposureRules.Add(g);
		}

		public CompositeGrowthRule GetGrowthRuleForElement(Element e)
		{
			CompositeGrowthRule compositeGrowthRule = new CompositeGrowthRule();
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

		public CompositeExposureRule GetExposureRuleForElement(Element e)
		{
			CompositeExposureRule compositeExposureRule = new CompositeExposureRule();
			if (this.exposureRules != null)
			{
				for (int i = 0; i < this.exposureRules.Count; i++)
				{
					if (this.exposureRules[i].Test(e))
					{
						compositeExposureRule.Overlay(this.exposureRules[i]);
					}
				}
			}
			return compositeExposureRule;
		}

		public TagGrowthRule GetGrowthRuleForTag(Tag t)
		{
			if (this.growthRules != null)
			{
				for (int i = 0; i < this.growthRules.Count; i++)
				{
					TagGrowthRule tagGrowthRule = this.growthRules[i] as TagGrowthRule;
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

		protected void InitializeElemGrowthArray(ref ElemGrowthInfo[] infoArray, ElemGrowthInfo default_value)
		{
			List<Element> elements = ElementLoader.elements;
			infoArray = new ElemGrowthInfo[elements.Count];
			for (int i = 0; i < elements.Count; i++)
			{
				infoArray[i] = default_value;
			}
			infoArray[ElementLoader.GetElementIndex(SimHashes.Polypropylene)] = new ElemGrowthInfo
			{
				underPopulationDeathRate = 2.6666667f,
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				minCountPerKG = 0f,
				maxCountPerKG = float.PositiveInfinity,
				minDiffusionCount = int.MaxValue,
				diffusionScale = 1f,
				minDiffusionInfestationTickCount = byte.MaxValue
			};
			infoArray[ElementLoader.GetElementIndex(SimHashes.Vacuum)] = new ElemGrowthInfo
			{
				underPopulationDeathRate = 0f,
				populationHalfLife = 0f,
				overPopulationHalfLife = 0f,
				minCountPerKG = 0f,
				maxCountPerKG = float.PositiveInfinity,
				diffusionScale = 0f,
				minDiffusionInfestationTickCount = byte.MaxValue
			};
		}

		protected void InitializeElemExposureArray(ref ElemExposureInfo[] infoArray, ElemExposureInfo default_value)
		{
			List<Element> elements = ElementLoader.elements;
			infoArray = new ElemExposureInfo[elements.Count];
			for (int i = 0; i < elements.Count; i++)
			{
				infoArray[i] = default_value;
			}
		}

		public float GetGrowthRateForTags(HashSet<Tag> tags, bool overpopulated)
		{
			float num = 1f;
			if (this.growthRules != null)
			{
				for (int i = 0; i < this.growthRules.Count; i++)
				{
					TagGrowthRule tagGrowthRule = this.growthRules[i] as TagGrowthRule;
					if (tagGrowthRule != null && tags.Contains(tagGrowthRule.tag))
					{
						num *= Disease.HalfLifeToGrowthRate(((!overpopulated) ? tagGrowthRule.populationHalfLife : tagGrowthRule.overPopulationHalfLife).Value, 1f);
					}
				}
			}
			return num;
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

		public List<Descriptor> GetQuantitativeDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE, GameUtil.GetFormattedTemperature(this.temperatureRange.minViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(this.temperatureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE_TOOLTIP, new object[]
			{
				GameUtil.GetFormattedTemperature(this.temperatureRange.minViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false),
				GameUtil.GetFormattedTemperature(this.temperatureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false),
				GameUtil.GetFormattedTemperature(this.temperatureRange.minGrowth, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false),
				GameUtil.GetFormattedTemperature(this.temperatureRange.maxGrowth, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)
			}), Descriptor.DescriptorType.Information, false));
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.PRESSURE_RANGE, GameUtil.GetFormattedMass(this.pressureRange.minViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedMass(this.pressureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.PRESSURE_RANGE_TOOLTIP, new object[]
			{
				GameUtil.GetFormattedMass(this.pressureRange.minViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"),
				GameUtil.GetFormattedMass(this.pressureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"),
				GameUtil.GetFormattedMass(this.pressureRange.minGrowth, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"),
				GameUtil.GetFormattedMass(this.pressureRange.maxGrowth, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")
			}), Descriptor.DescriptorType.Information, false));
			List<GrowthRule> list2 = new List<GrowthRule>();
			List<GrowthRule> list3 = new List<GrowthRule>();
			List<GrowthRule> list4 = new List<GrowthRule>();
			List<GrowthRule> list5 = new List<GrowthRule>();
			List<GrowthRule> list6 = new List<GrowthRule>();
			foreach (GrowthRule growthRule in this.growthRules)
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

		private List<Descriptor> BuildGrowthInfoDescriptors(List<GrowthRule> rules, string section_text, string section_tooltip, string item_tooltip)
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

		public HashedString id;

		public float strength;

		public Disease.RangeInfo temperatureRange;

		public Disease.RangeInfo temperatureHalfLives;

		public Disease.RangeInfo pressureRange;

		public Disease.RangeInfo pressureHalfLives;

		public List<GrowthRule> growthRules;

		public List<ExposureRule> exposureRules;

		public ElemGrowthInfo[] elemGrowthInfo;

		public ElemExposureInfo[] elemExposureInfo;

		public Color32 overlayColour = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

		public string overlayLegendHovertext;

		public Amount amount;

		public Attribute amountDeltaAttribute;

		public Attribute cureSpeedBase;

		public static readonly ElemGrowthInfo DEFAULT_GROWTH_INFO = new ElemGrowthInfo
		{
			underPopulationDeathRate = 0f,
			populationHalfLife = float.PositiveInfinity,
			overPopulationHalfLife = float.PositiveInfinity,
			minCountPerKG = 0f,
			maxCountPerKG = float.PositiveInfinity,
			minDiffusionCount = 0,
			diffusionScale = 1f,
			minDiffusionInfestationTickCount = byte.MaxValue
		};

		public static ElemExposureInfo DEFAULT_EXPOSURE_INFO = new ElemExposureInfo
		{
			populationHalfLife = float.PositiveInfinity
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
	}
}
