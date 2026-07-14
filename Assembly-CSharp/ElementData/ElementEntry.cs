using System;
using System.Runtime.CompilerServices;
using VYaml.Annotations;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace ElementData
{
	[YamlObject(NamingConvention.LowerCamelCase)]
	public class ElementEntry
	{
		public string elementId { get; set; }

		public float specificHeatCapacity { get; set; }

		public float thermalConductivity { get; set; }

		public float solidSurfaceAreaMultiplier { get; set; }

		public float liquidSurfaceAreaMultiplier { get; set; }

		public float gasSurfaceAreaMultiplier { get; set; }

		public float defaultMass { get; set; }

		public float defaultTemperature { get; set; }

		public float defaultPressure { get; set; }

		public float molarMass { get; set; }

		public float lightAbsorptionFactor { get; set; }

		public float radiationAbsorptionFactor { get; set; }

		public float radiationPer1000Mass { get; set; }

		public string lowTempTransitionTarget { get; set; }

		public float? lowTemp { get; set; }

		public string highTempTransitionTarget { get; set; }

		public float? highTemp { get; set; }

		public string lowTempTransitionOreId { get; set; }

		public float lowTempTransitionOreMassConversion { get; set; }

		public string highTempTransitionOreId { get; set; }

		public float highTempTransitionOreMassConversion { get; set; }

		public string sublimateId { get; set; }

		public string sublimateFx { get; set; }

		public float sublimateRate { get; set; }

		public float sublimateEfficiency { get; set; }

		public float sublimateProbability { get; set; }

		public float offGasPercentage { get; set; }

		public string materialCategory { get; set; }

		public string[] tags { get; set; }

		public bool isDisabled { get; set; }

		public float strength { get; set; }

		public float maxMass { get; set; }

		public byte hardness { get; set; }

		public float toxicity { get; set; }

		public float liquidCompression { get; set; }

		public float speed { get; set; }

		public float minHorizontalFlow { get; set; }

		public float minVerticalFlow { get; set; }

		public string convertId { get; set; }

		public float flow { get; set; }

		public int buildMenuSort { get; set; }

		public Element.State state { get; set; }

		public string localizationID { get; set; }

		public string dlcId { get; set; }

		public string refinedMetalTarget { get; set; }

		public ElementComposition[] composition { get; set; }

		public string description
		{
			get
			{
				return this.description_backing ?? ("STRINGS.ELEMENTS." + this.elementId.ToString().ToUpper() + ".DESC");
			}
			set
			{
				this.description_backing = value;
			}
		}

		[Preserve]
		public static void __RegisterVYamlFormatter()
		{
			GeneratedResolver.Register<ElementEntry>(new ElementEntry.ElementEntryGeneratedFormatter());
		}

		private string description_backing;

		[NullableContext(1)]
		[Nullable(0)]
		[Preserve]
		public class ElementEntryGeneratedFormatter : IYamlFormatter<ElementEntry>, IYamlFormatter
		{
			[Preserve]
			public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] ElementEntry value, YamlSerializationContext context)
			{
				if (value == null)
				{
					emitter.WriteNull();
					return;
				}
				emitter.BeginMapping(MappingStyle.Block);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.elementIdKeyUtf8Bytes);
				}
				else
				{
					byte[] array;
					int num;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.elementIdKeyUtf8Bytes, context.Options.NamingConvention, out array, out num);
					emitter.WriteScalar(array.AsSpan<byte>(0, num));
				}
				context.Serialize<string>(ref emitter, value.elementId);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.specificHeatCapacityKeyUtf8Bytes);
				}
				else
				{
					byte[] array2;
					int num2;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.specificHeatCapacityKeyUtf8Bytes, context.Options.NamingConvention, out array2, out num2);
					emitter.WriteScalar(array2.AsSpan<byte>(0, num2));
				}
				context.Serialize<float>(ref emitter, value.specificHeatCapacity);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.thermalConductivityKeyUtf8Bytes);
				}
				else
				{
					byte[] array3;
					int num3;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.thermalConductivityKeyUtf8Bytes, context.Options.NamingConvention, out array3, out num3);
					emitter.WriteScalar(array3.AsSpan<byte>(0, num3));
				}
				context.Serialize<float>(ref emitter, value.thermalConductivity);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.solidSurfaceAreaMultiplierKeyUtf8Bytes);
				}
				else
				{
					byte[] array4;
					int num4;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.solidSurfaceAreaMultiplierKeyUtf8Bytes, context.Options.NamingConvention, out array4, out num4);
					emitter.WriteScalar(array4.AsSpan<byte>(0, num4));
				}
				context.Serialize<float>(ref emitter, value.solidSurfaceAreaMultiplier);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.liquidSurfaceAreaMultiplierKeyUtf8Bytes);
				}
				else
				{
					byte[] array5;
					int num5;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.liquidSurfaceAreaMultiplierKeyUtf8Bytes, context.Options.NamingConvention, out array5, out num5);
					emitter.WriteScalar(array5.AsSpan<byte>(0, num5));
				}
				context.Serialize<float>(ref emitter, value.liquidSurfaceAreaMultiplier);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.gasSurfaceAreaMultiplierKeyUtf8Bytes);
				}
				else
				{
					byte[] array6;
					int num6;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.gasSurfaceAreaMultiplierKeyUtf8Bytes, context.Options.NamingConvention, out array6, out num6);
					emitter.WriteScalar(array6.AsSpan<byte>(0, num6));
				}
				context.Serialize<float>(ref emitter, value.gasSurfaceAreaMultiplier);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.defaultMassKeyUtf8Bytes);
				}
				else
				{
					byte[] array7;
					int num7;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.defaultMassKeyUtf8Bytes, context.Options.NamingConvention, out array7, out num7);
					emitter.WriteScalar(array7.AsSpan<byte>(0, num7));
				}
				context.Serialize<float>(ref emitter, value.defaultMass);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.defaultTemperatureKeyUtf8Bytes);
				}
				else
				{
					byte[] array8;
					int num8;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.defaultTemperatureKeyUtf8Bytes, context.Options.NamingConvention, out array8, out num8);
					emitter.WriteScalar(array8.AsSpan<byte>(0, num8));
				}
				context.Serialize<float>(ref emitter, value.defaultTemperature);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.defaultPressureKeyUtf8Bytes);
				}
				else
				{
					byte[] array9;
					int num9;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.defaultPressureKeyUtf8Bytes, context.Options.NamingConvention, out array9, out num9);
					emitter.WriteScalar(array9.AsSpan<byte>(0, num9));
				}
				context.Serialize<float>(ref emitter, value.defaultPressure);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.molarMassKeyUtf8Bytes);
				}
				else
				{
					byte[] array10;
					int num10;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.molarMassKeyUtf8Bytes, context.Options.NamingConvention, out array10, out num10);
					emitter.WriteScalar(array10.AsSpan<byte>(0, num10));
				}
				context.Serialize<float>(ref emitter, value.molarMass);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.lightAbsorptionFactorKeyUtf8Bytes);
				}
				else
				{
					byte[] array11;
					int num11;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.lightAbsorptionFactorKeyUtf8Bytes, context.Options.NamingConvention, out array11, out num11);
					emitter.WriteScalar(array11.AsSpan<byte>(0, num11));
				}
				context.Serialize<float>(ref emitter, value.lightAbsorptionFactor);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.radiationAbsorptionFactorKeyUtf8Bytes);
				}
				else
				{
					byte[] array12;
					int num12;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.radiationAbsorptionFactorKeyUtf8Bytes, context.Options.NamingConvention, out array12, out num12);
					emitter.WriteScalar(array12.AsSpan<byte>(0, num12));
				}
				context.Serialize<float>(ref emitter, value.radiationAbsorptionFactor);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.radiationPer1000MassKeyUtf8Bytes);
				}
				else
				{
					byte[] array13;
					int num13;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.radiationPer1000MassKeyUtf8Bytes, context.Options.NamingConvention, out array13, out num13);
					emitter.WriteScalar(array13.AsSpan<byte>(0, num13));
				}
				context.Serialize<float>(ref emitter, value.radiationPer1000Mass);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionTargetKeyUtf8Bytes);
				}
				else
				{
					byte[] array14;
					int num14;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionTargetKeyUtf8Bytes, context.Options.NamingConvention, out array14, out num14);
					emitter.WriteScalar(array14.AsSpan<byte>(0, num14));
				}
				context.Serialize<string>(ref emitter, value.lowTempTransitionTarget);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.lowTempKeyUtf8Bytes);
				}
				else
				{
					byte[] array15;
					int num15;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.lowTempKeyUtf8Bytes, context.Options.NamingConvention, out array15, out num15);
					emitter.WriteScalar(array15.AsSpan<byte>(0, num15));
				}
				context.Serialize<float?>(ref emitter, value.lowTemp);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionTargetKeyUtf8Bytes);
				}
				else
				{
					byte[] array16;
					int num16;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionTargetKeyUtf8Bytes, context.Options.NamingConvention, out array16, out num16);
					emitter.WriteScalar(array16.AsSpan<byte>(0, num16));
				}
				context.Serialize<string>(ref emitter, value.highTempTransitionTarget);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.highTempKeyUtf8Bytes);
				}
				else
				{
					byte[] array17;
					int num17;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.highTempKeyUtf8Bytes, context.Options.NamingConvention, out array17, out num17);
					emitter.WriteScalar(array17.AsSpan<byte>(0, num17));
				}
				context.Serialize<float?>(ref emitter, value.highTemp);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionOreIdKeyUtf8Bytes);
				}
				else
				{
					byte[] array18;
					int num18;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionOreIdKeyUtf8Bytes, context.Options.NamingConvention, out array18, out num18);
					emitter.WriteScalar(array18.AsSpan<byte>(0, num18));
				}
				context.Serialize<string>(ref emitter, value.lowTempTransitionOreId);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionOreMassConversionKeyUtf8Bytes);
				}
				else
				{
					byte[] array19;
					int num19;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionOreMassConversionKeyUtf8Bytes, context.Options.NamingConvention, out array19, out num19);
					emitter.WriteScalar(array19.AsSpan<byte>(0, num19));
				}
				context.Serialize<float>(ref emitter, value.lowTempTransitionOreMassConversion);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionOreIdKeyUtf8Bytes);
				}
				else
				{
					byte[] array20;
					int num20;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionOreIdKeyUtf8Bytes, context.Options.NamingConvention, out array20, out num20);
					emitter.WriteScalar(array20.AsSpan<byte>(0, num20));
				}
				context.Serialize<string>(ref emitter, value.highTempTransitionOreId);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionOreMassConversionKeyUtf8Bytes);
				}
				else
				{
					byte[] array21;
					int num21;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionOreMassConversionKeyUtf8Bytes, context.Options.NamingConvention, out array21, out num21);
					emitter.WriteScalar(array21.AsSpan<byte>(0, num21));
				}
				context.Serialize<float>(ref emitter, value.highTempTransitionOreMassConversion);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.sublimateIdKeyUtf8Bytes);
				}
				else
				{
					byte[] array22;
					int num22;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.sublimateIdKeyUtf8Bytes, context.Options.NamingConvention, out array22, out num22);
					emitter.WriteScalar(array22.AsSpan<byte>(0, num22));
				}
				context.Serialize<string>(ref emitter, value.sublimateId);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.sublimateFxKeyUtf8Bytes);
				}
				else
				{
					byte[] array23;
					int num23;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.sublimateFxKeyUtf8Bytes, context.Options.NamingConvention, out array23, out num23);
					emitter.WriteScalar(array23.AsSpan<byte>(0, num23));
				}
				context.Serialize<string>(ref emitter, value.sublimateFx);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.sublimateRateKeyUtf8Bytes);
				}
				else
				{
					byte[] array24;
					int num24;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.sublimateRateKeyUtf8Bytes, context.Options.NamingConvention, out array24, out num24);
					emitter.WriteScalar(array24.AsSpan<byte>(0, num24));
				}
				context.Serialize<float>(ref emitter, value.sublimateRate);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.sublimateEfficiencyKeyUtf8Bytes);
				}
				else
				{
					byte[] array25;
					int num25;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.sublimateEfficiencyKeyUtf8Bytes, context.Options.NamingConvention, out array25, out num25);
					emitter.WriteScalar(array25.AsSpan<byte>(0, num25));
				}
				context.Serialize<float>(ref emitter, value.sublimateEfficiency);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.sublimateProbabilityKeyUtf8Bytes);
				}
				else
				{
					byte[] array26;
					int num26;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.sublimateProbabilityKeyUtf8Bytes, context.Options.NamingConvention, out array26, out num26);
					emitter.WriteScalar(array26.AsSpan<byte>(0, num26));
				}
				context.Serialize<float>(ref emitter, value.sublimateProbability);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.offGasPercentageKeyUtf8Bytes);
				}
				else
				{
					byte[] array27;
					int num27;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.offGasPercentageKeyUtf8Bytes, context.Options.NamingConvention, out array27, out num27);
					emitter.WriteScalar(array27.AsSpan<byte>(0, num27));
				}
				context.Serialize<float>(ref emitter, value.offGasPercentage);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.materialCategoryKeyUtf8Bytes);
				}
				else
				{
					byte[] array28;
					int num28;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.materialCategoryKeyUtf8Bytes, context.Options.NamingConvention, out array28, out num28);
					emitter.WriteScalar(array28.AsSpan<byte>(0, num28));
				}
				context.Serialize<string>(ref emitter, value.materialCategory);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.tagsKeyUtf8Bytes);
				}
				else
				{
					byte[] array29;
					int num29;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.tagsKeyUtf8Bytes, context.Options.NamingConvention, out array29, out num29);
					emitter.WriteScalar(array29.AsSpan<byte>(0, num29));
				}
				context.Serialize<string[]>(ref emitter, value.tags);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.isDisabledKeyUtf8Bytes);
				}
				else
				{
					byte[] array30;
					int num30;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.isDisabledKeyUtf8Bytes, context.Options.NamingConvention, out array30, out num30);
					emitter.WriteScalar(array30.AsSpan<byte>(0, num30));
				}
				context.Serialize<bool>(ref emitter, value.isDisabled);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.strengthKeyUtf8Bytes);
				}
				else
				{
					byte[] array31;
					int num31;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.strengthKeyUtf8Bytes, context.Options.NamingConvention, out array31, out num31);
					emitter.WriteScalar(array31.AsSpan<byte>(0, num31));
				}
				context.Serialize<float>(ref emitter, value.strength);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.maxMassKeyUtf8Bytes);
				}
				else
				{
					byte[] array32;
					int num32;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.maxMassKeyUtf8Bytes, context.Options.NamingConvention, out array32, out num32);
					emitter.WriteScalar(array32.AsSpan<byte>(0, num32));
				}
				context.Serialize<float>(ref emitter, value.maxMass);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.hardnessKeyUtf8Bytes);
				}
				else
				{
					byte[] array33;
					int num33;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.hardnessKeyUtf8Bytes, context.Options.NamingConvention, out array33, out num33);
					emitter.WriteScalar(array33.AsSpan<byte>(0, num33));
				}
				context.Serialize<byte>(ref emitter, value.hardness);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.toxicityKeyUtf8Bytes);
				}
				else
				{
					byte[] array34;
					int num34;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.toxicityKeyUtf8Bytes, context.Options.NamingConvention, out array34, out num34);
					emitter.WriteScalar(array34.AsSpan<byte>(0, num34));
				}
				context.Serialize<float>(ref emitter, value.toxicity);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.liquidCompressionKeyUtf8Bytes);
				}
				else
				{
					byte[] array35;
					int num35;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.liquidCompressionKeyUtf8Bytes, context.Options.NamingConvention, out array35, out num35);
					emitter.WriteScalar(array35.AsSpan<byte>(0, num35));
				}
				context.Serialize<float>(ref emitter, value.liquidCompression);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.speedKeyUtf8Bytes);
				}
				else
				{
					byte[] array36;
					int num36;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.speedKeyUtf8Bytes, context.Options.NamingConvention, out array36, out num36);
					emitter.WriteScalar(array36.AsSpan<byte>(0, num36));
				}
				context.Serialize<float>(ref emitter, value.speed);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.minHorizontalFlowKeyUtf8Bytes);
				}
				else
				{
					byte[] array37;
					int num37;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.minHorizontalFlowKeyUtf8Bytes, context.Options.NamingConvention, out array37, out num37);
					emitter.WriteScalar(array37.AsSpan<byte>(0, num37));
				}
				context.Serialize<float>(ref emitter, value.minHorizontalFlow);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.minVerticalFlowKeyUtf8Bytes);
				}
				else
				{
					byte[] array38;
					int num38;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.minVerticalFlowKeyUtf8Bytes, context.Options.NamingConvention, out array38, out num38);
					emitter.WriteScalar(array38.AsSpan<byte>(0, num38));
				}
				context.Serialize<float>(ref emitter, value.minVerticalFlow);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.convertIdKeyUtf8Bytes);
				}
				else
				{
					byte[] array39;
					int num39;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.convertIdKeyUtf8Bytes, context.Options.NamingConvention, out array39, out num39);
					emitter.WriteScalar(array39.AsSpan<byte>(0, num39));
				}
				context.Serialize<string>(ref emitter, value.convertId);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.flowKeyUtf8Bytes);
				}
				else
				{
					byte[] array40;
					int num40;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.flowKeyUtf8Bytes, context.Options.NamingConvention, out array40, out num40);
					emitter.WriteScalar(array40.AsSpan<byte>(0, num40));
				}
				context.Serialize<float>(ref emitter, value.flow);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.buildMenuSortKeyUtf8Bytes);
				}
				else
				{
					byte[] array41;
					int num41;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.buildMenuSortKeyUtf8Bytes, context.Options.NamingConvention, out array41, out num41);
					emitter.WriteScalar(array41.AsSpan<byte>(0, num41));
				}
				context.Serialize<int>(ref emitter, value.buildMenuSort);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.stateKeyUtf8Bytes);
				}
				else
				{
					byte[] array42;
					int num42;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.stateKeyUtf8Bytes, context.Options.NamingConvention, out array42, out num42);
					emitter.WriteScalar(array42.AsSpan<byte>(0, num42));
				}
				context.Serialize<Element.State>(ref emitter, value.state);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.localizationIDKeyUtf8Bytes);
				}
				else
				{
					byte[] array43;
					int num43;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.localizationIDKeyUtf8Bytes, context.Options.NamingConvention, out array43, out num43);
					emitter.WriteScalar(array43.AsSpan<byte>(0, num43));
				}
				context.Serialize<string>(ref emitter, value.localizationID);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.dlcIdKeyUtf8Bytes);
				}
				else
				{
					byte[] array44;
					int num44;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.dlcIdKeyUtf8Bytes, context.Options.NamingConvention, out array44, out num44);
					emitter.WriteScalar(array44.AsSpan<byte>(0, num44));
				}
				context.Serialize<string>(ref emitter, value.dlcId);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.refinedMetalTargetKeyUtf8Bytes);
				}
				else
				{
					byte[] array45;
					int num45;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.refinedMetalTargetKeyUtf8Bytes, context.Options.NamingConvention, out array45, out num45);
					emitter.WriteScalar(array45.AsSpan<byte>(0, num45));
				}
				context.Serialize<string>(ref emitter, value.refinedMetalTarget);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.compositionKeyUtf8Bytes);
				}
				else
				{
					byte[] array46;
					int num46;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.compositionKeyUtf8Bytes, context.Options.NamingConvention, out array46, out num46);
					emitter.WriteScalar(array46.AsSpan<byte>(0, num46));
				}
				context.Serialize<ElementComposition[]>(ref emitter, value.composition);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntry.ElementEntryGeneratedFormatter.descriptionKeyUtf8Bytes);
				}
				else
				{
					byte[] array47;
					int num47;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntry.ElementEntryGeneratedFormatter.descriptionKeyUtf8Bytes, context.Options.NamingConvention, out array47, out num47);
					emitter.WriteScalar(array47.AsSpan<byte>(0, num47));
				}
				context.Serialize<string>(ref emitter, value.description);
				emitter.EndMapping();
			}

			[Preserve]
			[return: Nullable(2)]
			public ElementEntry Deserialize(ref YamlParser parser, YamlDeserializationContext context)
			{
				if (parser.IsNullScalar())
				{
					parser.Read();
					return null;
				}
				parser.ReadWithVerify(ParseEventType.MappingStart);
				string text = null;
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				float num6 = 0f;
				float num7 = 0f;
				float num8 = 0f;
				float num9 = 0f;
				float num10 = 0f;
				float num11 = 0f;
				float num12 = 0f;
				string text2 = null;
				float? num13 = null;
				string text3 = null;
				float? num14 = null;
				string text4 = null;
				float num15 = 0f;
				string text5 = null;
				float num16 = 0f;
				string text6 = null;
				string text7 = null;
				float num17 = 0f;
				float num18 = 0f;
				float num19 = 0f;
				float num20 = 0f;
				string text8 = null;
				string[] array = null;
				bool flag = false;
				float num21 = 0f;
				float num22 = 0f;
				byte b = 0;
				float num23 = 0f;
				float num24 = 0f;
				float num25 = 0f;
				float num26 = 0f;
				float num27 = 0f;
				string text9 = null;
				float num28 = 0f;
				int num29 = 0;
				Element.State state = Element.State.Vacuum;
				string text10 = null;
				string text11 = null;
				string text12 = null;
				ElementComposition[] array2 = null;
				string text13 = null;
				while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
				{
					if (parser.CurrentEventType != ParseEventType.Scalar)
					{
						throw new YamlSerializerException(parser.CurrentMark, "Custom type deserialization supports only string key");
					}
					ReadOnlySpan<byte> readOnlySpan;
					if (!parser.TryGetScalarAsSpan(out readOnlySpan))
					{
						throw new YamlSerializerException(parser.CurrentMark, "Custom type deserialization supports only string key");
					}
					if (context.Options.NamingConvention != NamingConvention.LowerCamelCase)
					{
						byte[] array3;
						int num30;
						NamingConventionMutator.MutateToThreadStaticBufferUtf8(readOnlySpan, NamingConvention.LowerCamelCase, out array3, out num30);
						readOnlySpan = array3.AsSpan<byte>(0, num30);
					}
					switch (readOnlySpan.Length)
					{
					case 4:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.tagsKeyUtf8Bytes))
						{
							parser.Read();
							array = context.DeserializeWithAlias<string[]>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.flowKeyUtf8Bytes))
						{
							parser.Read();
							num28 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 5:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.speedKeyUtf8Bytes))
						{
							parser.Read();
							num25 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.stateKeyUtf8Bytes))
						{
							parser.Read();
							state = context.DeserializeWithAlias<Element.State>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.dlcIdKeyUtf8Bytes))
						{
							parser.Read();
							text11 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 7:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.lowTempKeyUtf8Bytes))
						{
							parser.Read();
							num13 = context.DeserializeWithAlias<float?>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.maxMassKeyUtf8Bytes))
						{
							parser.Read();
							num22 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 8:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.highTempKeyUtf8Bytes))
						{
							parser.Read();
							num14 = context.DeserializeWithAlias<float?>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.strengthKeyUtf8Bytes))
						{
							parser.Read();
							num21 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.hardnessKeyUtf8Bytes))
						{
							parser.Read();
							b = context.DeserializeWithAlias<byte>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.toxicityKeyUtf8Bytes))
						{
							parser.Read();
							num23 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 9:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.elementIdKeyUtf8Bytes))
						{
							parser.Read();
							text = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.molarMassKeyUtf8Bytes))
						{
							parser.Read();
							num9 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.convertIdKeyUtf8Bytes))
						{
							parser.Read();
							text9 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 10:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.isDisabledKeyUtf8Bytes))
						{
							parser.Read();
							flag = context.DeserializeWithAlias<bool>(ref parser);
							continue;
						}
						break;
					case 11:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.defaultMassKeyUtf8Bytes))
						{
							parser.Read();
							num6 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.sublimateIdKeyUtf8Bytes))
						{
							parser.Read();
							text6 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.sublimateFxKeyUtf8Bytes))
						{
							parser.Read();
							text7 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.compositionKeyUtf8Bytes))
						{
							parser.Read();
							array2 = context.DeserializeWithAlias<ElementComposition[]>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.descriptionKeyUtf8Bytes))
						{
							parser.Read();
							text13 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 13:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.sublimateRateKeyUtf8Bytes))
						{
							parser.Read();
							num17 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.buildMenuSortKeyUtf8Bytes))
						{
							parser.Read();
							num29 = context.DeserializeWithAlias<int>(ref parser);
							continue;
						}
						break;
					case 14:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.localizationIDKeyUtf8Bytes))
						{
							parser.Read();
							text10 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 15:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.defaultPressureKeyUtf8Bytes))
						{
							parser.Read();
							num8 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.minVerticalFlowKeyUtf8Bytes))
						{
							parser.Read();
							num27 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 16:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.offGasPercentageKeyUtf8Bytes))
						{
							parser.Read();
							num20 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.materialCategoryKeyUtf8Bytes))
						{
							parser.Read();
							text8 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 17:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.liquidCompressionKeyUtf8Bytes))
						{
							parser.Read();
							num24 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.minHorizontalFlowKeyUtf8Bytes))
						{
							parser.Read();
							num26 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 18:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.defaultTemperatureKeyUtf8Bytes))
						{
							parser.Read();
							num7 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.refinedMetalTargetKeyUtf8Bytes))
						{
							parser.Read();
							text12 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 19:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.thermalConductivityKeyUtf8Bytes))
						{
							parser.Read();
							num2 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.sublimateEfficiencyKeyUtf8Bytes))
						{
							parser.Read();
							num18 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 20:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.specificHeatCapacityKeyUtf8Bytes))
						{
							parser.Read();
							num = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.radiationPer1000MassKeyUtf8Bytes))
						{
							parser.Read();
							num12 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.sublimateProbabilityKeyUtf8Bytes))
						{
							parser.Read();
							num19 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 21:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.lightAbsorptionFactorKeyUtf8Bytes))
						{
							parser.Read();
							num10 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 22:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionOreIdKeyUtf8Bytes))
						{
							parser.Read();
							text4 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 23:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionTargetKeyUtf8Bytes))
						{
							parser.Read();
							text2 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionOreIdKeyUtf8Bytes))
						{
							parser.Read();
							text5 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 24:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.gasSurfaceAreaMultiplierKeyUtf8Bytes))
						{
							parser.Read();
							num5 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionTargetKeyUtf8Bytes))
						{
							parser.Read();
							text3 = context.DeserializeWithAlias<string>(ref parser);
							continue;
						}
						break;
					case 25:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.radiationAbsorptionFactorKeyUtf8Bytes))
						{
							parser.Read();
							num11 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 26:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.solidSurfaceAreaMultiplierKeyUtf8Bytes))
						{
							parser.Read();
							num3 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 27:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.liquidSurfaceAreaMultiplierKeyUtf8Bytes))
						{
							parser.Read();
							num4 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 34:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.lowTempTransitionOreMassConversionKeyUtf8Bytes))
						{
							parser.Read();
							num15 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					case 35:
						if (readOnlySpan.SequenceEqual<byte>(ElementEntry.ElementEntryGeneratedFormatter.highTempTransitionOreMassConversionKeyUtf8Bytes))
						{
							parser.Read();
							num16 = context.DeserializeWithAlias<float>(ref parser);
							continue;
						}
						break;
					}
					parser.Read();
					parser.SkipCurrentNode();
				}
				parser.ReadWithVerify(ParseEventType.MappingEnd);
				return new ElementEntry
				{
					elementId = text,
					specificHeatCapacity = num,
					thermalConductivity = num2,
					solidSurfaceAreaMultiplier = num3,
					liquidSurfaceAreaMultiplier = num4,
					gasSurfaceAreaMultiplier = num5,
					defaultMass = num6,
					defaultTemperature = num7,
					defaultPressure = num8,
					molarMass = num9,
					lightAbsorptionFactor = num10,
					radiationAbsorptionFactor = num11,
					radiationPer1000Mass = num12,
					lowTempTransitionTarget = text2,
					lowTemp = num13,
					highTempTransitionTarget = text3,
					highTemp = num14,
					lowTempTransitionOreId = text4,
					lowTempTransitionOreMassConversion = num15,
					highTempTransitionOreId = text5,
					highTempTransitionOreMassConversion = num16,
					sublimateId = text6,
					sublimateFx = text7,
					sublimateRate = num17,
					sublimateEfficiency = num18,
					sublimateProbability = num19,
					offGasPercentage = num20,
					materialCategory = text8,
					tags = array,
					isDisabled = flag,
					strength = num21,
					maxMass = num22,
					hardness = b,
					toxicity = num23,
					liquidCompression = num24,
					speed = num25,
					minHorizontalFlow = num26,
					minVerticalFlow = num27,
					convertId = text9,
					flow = num28,
					buildMenuSort = num29,
					state = state,
					localizationID = text10,
					dlcId = text11,
					refinedMetalTarget = text12,
					composition = array2,
					description = text13
				};
			}

			private static readonly byte[] elementIdKeyUtf8Bytes = new byte[] { 101, 108, 101, 109, 101, 110, 116, 73, 100 };

			private static readonly byte[] specificHeatCapacityKeyUtf8Bytes = new byte[]
			{
				115, 112, 101, 99, 105, 102, 105, 99, 72, 101,
				97, 116, 67, 97, 112, 97, 99, 105, 116, 121
			};

			private static readonly byte[] thermalConductivityKeyUtf8Bytes = new byte[]
			{
				116, 104, 101, 114, 109, 97, 108, 67, 111, 110,
				100, 117, 99, 116, 105, 118, 105, 116, 121
			};

			private static readonly byte[] solidSurfaceAreaMultiplierKeyUtf8Bytes = new byte[]
			{
				115, 111, 108, 105, 100, 83, 117, 114, 102, 97,
				99, 101, 65, 114, 101, 97, 77, 117, 108, 116,
				105, 112, 108, 105, 101, 114
			};

			private static readonly byte[] liquidSurfaceAreaMultiplierKeyUtf8Bytes = new byte[]
			{
				108, 105, 113, 117, 105, 100, 83, 117, 114, 102,
				97, 99, 101, 65, 114, 101, 97, 77, 117, 108,
				116, 105, 112, 108, 105, 101, 114
			};

			private static readonly byte[] gasSurfaceAreaMultiplierKeyUtf8Bytes = new byte[]
			{
				103, 97, 115, 83, 117, 114, 102, 97, 99, 101,
				65, 114, 101, 97, 77, 117, 108, 116, 105, 112,
				108, 105, 101, 114
			};

			private static readonly byte[] defaultMassKeyUtf8Bytes = new byte[]
			{
				100, 101, 102, 97, 117, 108, 116, 77, 97, 115,
				115
			};

			private static readonly byte[] defaultTemperatureKeyUtf8Bytes = new byte[]
			{
				100, 101, 102, 97, 117, 108, 116, 84, 101, 109,
				112, 101, 114, 97, 116, 117, 114, 101
			};

			private static readonly byte[] defaultPressureKeyUtf8Bytes = new byte[]
			{
				100, 101, 102, 97, 117, 108, 116, 80, 114, 101,
				115, 115, 117, 114, 101
			};

			private static readonly byte[] molarMassKeyUtf8Bytes = new byte[] { 109, 111, 108, 97, 114, 77, 97, 115, 115 };

			private static readonly byte[] lightAbsorptionFactorKeyUtf8Bytes = new byte[]
			{
				108, 105, 103, 104, 116, 65, 98, 115, 111, 114,
				112, 116, 105, 111, 110, 70, 97, 99, 116, 111,
				114
			};

			private static readonly byte[] radiationAbsorptionFactorKeyUtf8Bytes = new byte[]
			{
				114, 97, 100, 105, 97, 116, 105, 111, 110, 65,
				98, 115, 111, 114, 112, 116, 105, 111, 110, 70,
				97, 99, 116, 111, 114
			};

			private static readonly byte[] radiationPer1000MassKeyUtf8Bytes = new byte[]
			{
				114, 97, 100, 105, 97, 116, 105, 111, 110, 80,
				101, 114, 49, 48, 48, 48, 77, 97, 115, 115
			};

			private static readonly byte[] lowTempTransitionTargetKeyUtf8Bytes = new byte[]
			{
				108, 111, 119, 84, 101, 109, 112, 84, 114, 97,
				110, 115, 105, 116, 105, 111, 110, 84, 97, 114,
				103, 101, 116
			};

			private static readonly byte[] lowTempKeyUtf8Bytes = new byte[] { 108, 111, 119, 84, 101, 109, 112 };

			private static readonly byte[] highTempTransitionTargetKeyUtf8Bytes = new byte[]
			{
				104, 105, 103, 104, 84, 101, 109, 112, 84, 114,
				97, 110, 115, 105, 116, 105, 111, 110, 84, 97,
				114, 103, 101, 116
			};

			private static readonly byte[] highTempKeyUtf8Bytes = new byte[] { 104, 105, 103, 104, 84, 101, 109, 112 };

			private static readonly byte[] lowTempTransitionOreIdKeyUtf8Bytes = new byte[]
			{
				108, 111, 119, 84, 101, 109, 112, 84, 114, 97,
				110, 115, 105, 116, 105, 111, 110, 79, 114, 101,
				73, 100
			};

			private static readonly byte[] lowTempTransitionOreMassConversionKeyUtf8Bytes = new byte[]
			{
				108, 111, 119, 84, 101, 109, 112, 84, 114, 97,
				110, 115, 105, 116, 105, 111, 110, 79, 114, 101,
				77, 97, 115, 115, 67, 111, 110, 118, 101, 114,
				115, 105, 111, 110
			};

			private static readonly byte[] highTempTransitionOreIdKeyUtf8Bytes = new byte[]
			{
				104, 105, 103, 104, 84, 101, 109, 112, 84, 114,
				97, 110, 115, 105, 116, 105, 111, 110, 79, 114,
				101, 73, 100
			};

			private static readonly byte[] highTempTransitionOreMassConversionKeyUtf8Bytes = new byte[]
			{
				104, 105, 103, 104, 84, 101, 109, 112, 84, 114,
				97, 110, 115, 105, 116, 105, 111, 110, 79, 114,
				101, 77, 97, 115, 115, 67, 111, 110, 118, 101,
				114, 115, 105, 111, 110
			};

			private static readonly byte[] sublimateIdKeyUtf8Bytes = new byte[]
			{
				115, 117, 98, 108, 105, 109, 97, 116, 101, 73,
				100
			};

			private static readonly byte[] sublimateFxKeyUtf8Bytes = new byte[]
			{
				115, 117, 98, 108, 105, 109, 97, 116, 101, 70,
				120
			};

			private static readonly byte[] sublimateRateKeyUtf8Bytes = new byte[]
			{
				115, 117, 98, 108, 105, 109, 97, 116, 101, 82,
				97, 116, 101
			};

			private static readonly byte[] sublimateEfficiencyKeyUtf8Bytes = new byte[]
			{
				115, 117, 98, 108, 105, 109, 97, 116, 101, 69,
				102, 102, 105, 99, 105, 101, 110, 99, 121
			};

			private static readonly byte[] sublimateProbabilityKeyUtf8Bytes = new byte[]
			{
				115, 117, 98, 108, 105, 109, 97, 116, 101, 80,
				114, 111, 98, 97, 98, 105, 108, 105, 116, 121
			};

			private static readonly byte[] offGasPercentageKeyUtf8Bytes = new byte[]
			{
				111, 102, 102, 71, 97, 115, 80, 101, 114, 99,
				101, 110, 116, 97, 103, 101
			};

			private static readonly byte[] materialCategoryKeyUtf8Bytes = new byte[]
			{
				109, 97, 116, 101, 114, 105, 97, 108, 67, 97,
				116, 101, 103, 111, 114, 121
			};

			private static readonly byte[] tagsKeyUtf8Bytes = new byte[] { 116, 97, 103, 115 };

			private static readonly byte[] isDisabledKeyUtf8Bytes = new byte[] { 105, 115, 68, 105, 115, 97, 98, 108, 101, 100 };

			private static readonly byte[] strengthKeyUtf8Bytes = new byte[] { 115, 116, 114, 101, 110, 103, 116, 104 };

			private static readonly byte[] maxMassKeyUtf8Bytes = new byte[] { 109, 97, 120, 77, 97, 115, 115 };

			private static readonly byte[] hardnessKeyUtf8Bytes = new byte[] { 104, 97, 114, 100, 110, 101, 115, 115 };

			private static readonly byte[] toxicityKeyUtf8Bytes = new byte[] { 116, 111, 120, 105, 99, 105, 116, 121 };

			private static readonly byte[] liquidCompressionKeyUtf8Bytes = new byte[]
			{
				108, 105, 113, 117, 105, 100, 67, 111, 109, 112,
				114, 101, 115, 115, 105, 111, 110
			};

			private static readonly byte[] speedKeyUtf8Bytes = new byte[] { 115, 112, 101, 101, 100 };

			private static readonly byte[] minHorizontalFlowKeyUtf8Bytes = new byte[]
			{
				109, 105, 110, 72, 111, 114, 105, 122, 111, 110,
				116, 97, 108, 70, 108, 111, 119
			};

			private static readonly byte[] minVerticalFlowKeyUtf8Bytes = new byte[]
			{
				109, 105, 110, 86, 101, 114, 116, 105, 99, 97,
				108, 70, 108, 111, 119
			};

			private static readonly byte[] convertIdKeyUtf8Bytes = new byte[] { 99, 111, 110, 118, 101, 114, 116, 73, 100 };

			private static readonly byte[] flowKeyUtf8Bytes = new byte[] { 102, 108, 111, 119 };

			private static readonly byte[] buildMenuSortKeyUtf8Bytes = new byte[]
			{
				98, 117, 105, 108, 100, 77, 101, 110, 117, 83,
				111, 114, 116
			};

			private static readonly byte[] stateKeyUtf8Bytes = new byte[] { 115, 116, 97, 116, 101 };

			private static readonly byte[] localizationIDKeyUtf8Bytes = new byte[]
			{
				108, 111, 99, 97, 108, 105, 122, 97, 116, 105,
				111, 110, 73, 68
			};

			private static readonly byte[] dlcIdKeyUtf8Bytes = new byte[] { 100, 108, 99, 73, 100 };

			private static readonly byte[] refinedMetalTargetKeyUtf8Bytes = new byte[]
			{
				114, 101, 102, 105, 110, 101, 100, 77, 101, 116,
				97, 108, 84, 97, 114, 103, 101, 116
			};

			private static readonly byte[] compositionKeyUtf8Bytes = new byte[]
			{
				99, 111, 109, 112, 111, 115, 105, 116, 105, 111,
				110
			};

			private static readonly byte[] descriptionKeyUtf8Bytes = new byte[]
			{
				100, 101, 115, 99, 114, 105, 112, 116, 105, 111,
				110
			};
		}
	}
}
