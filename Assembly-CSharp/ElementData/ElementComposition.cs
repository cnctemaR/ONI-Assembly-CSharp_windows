using System;
using System.Runtime.CompilerServices;
using VYaml.Annotations;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace ElementData
{
	[YamlObject(NamingConvention.LowerCamelCase)]
	public class ElementComposition
	{
		public string elementID { get; set; }

		public float percentage { get; set; }

		[Preserve]
		public static void __RegisterVYamlFormatter()
		{
			GeneratedResolver.Register<ElementComposition>(new ElementComposition.ElementCompositionGeneratedFormatter());
		}

		[NullableContext(1)]
		[Nullable(0)]
		[Preserve]
		public class ElementCompositionGeneratedFormatter : IYamlFormatter<ElementComposition>, IYamlFormatter
		{
			[Preserve]
			public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] ElementComposition value, YamlSerializationContext context)
			{
				if (value == null)
				{
					emitter.WriteNull();
					return;
				}
				emitter.BeginMapping(MappingStyle.Block);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementComposition.ElementCompositionGeneratedFormatter.elementIDKeyUtf8Bytes);
				}
				else
				{
					byte[] array;
					int num;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementComposition.ElementCompositionGeneratedFormatter.elementIDKeyUtf8Bytes, context.Options.NamingConvention, out array, out num);
					emitter.WriteScalar(array.AsSpan<byte>(0, num));
				}
				context.Serialize<string>(ref emitter, value.elementID);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementComposition.ElementCompositionGeneratedFormatter.percentageKeyUtf8Bytes);
				}
				else
				{
					byte[] array2;
					int num2;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementComposition.ElementCompositionGeneratedFormatter.percentageKeyUtf8Bytes, context.Options.NamingConvention, out array2, out num2);
					emitter.WriteScalar(array2.AsSpan<byte>(0, num2));
				}
				context.Serialize<float>(ref emitter, value.percentage);
				emitter.EndMapping();
			}

			[Preserve]
			[return: Nullable(2)]
			public ElementComposition Deserialize(ref YamlParser parser, YamlDeserializationContext context)
			{
				if (parser.IsNullScalar())
				{
					parser.Read();
					return null;
				}
				parser.ReadWithVerify(ParseEventType.MappingStart);
				string text = null;
				float num = 0f;
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
						byte[] array;
						int num2;
						NamingConventionMutator.MutateToThreadStaticBufferUtf8(readOnlySpan, NamingConvention.LowerCamelCase, out array, out num2);
						readOnlySpan = array.AsSpan<byte>(0, num2);
					}
					int length = readOnlySpan.Length;
					if (length != 9)
					{
						if (length == 10)
						{
							if (readOnlySpan.SequenceEqual<byte>(ElementComposition.ElementCompositionGeneratedFormatter.percentageKeyUtf8Bytes))
							{
								parser.Read();
								num = context.DeserializeWithAlias<float>(ref parser);
								continue;
							}
						}
					}
					else if (readOnlySpan.SequenceEqual<byte>(ElementComposition.ElementCompositionGeneratedFormatter.elementIDKeyUtf8Bytes))
					{
						parser.Read();
						text = context.DeserializeWithAlias<string>(ref parser);
						continue;
					}
					parser.Read();
					parser.SkipCurrentNode();
				}
				parser.ReadWithVerify(ParseEventType.MappingEnd);
				return new ElementComposition
				{
					elementID = text,
					percentage = num
				};
			}

			private static readonly byte[] elementIDKeyUtf8Bytes = new byte[] { 101, 108, 101, 109, 101, 110, 116, 73, 68 };

			private static readonly byte[] percentageKeyUtf8Bytes = new byte[] { 112, 101, 114, 99, 101, 110, 116, 97, 103, 101 };
		}
	}
}
