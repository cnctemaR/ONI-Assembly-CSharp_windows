using System;
using System.Runtime.CompilerServices;
using VYaml.Annotations;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace ElementData
{
	[YamlObject(NamingConvention.LowerCamelCase)]
	public class ElementEntryCollection
	{
		public ElementEntry[] elements { get; set; }

		[Preserve]
		public static void __RegisterVYamlFormatter()
		{
			GeneratedResolver.Register<ElementEntryCollection>(new ElementEntryCollection.ElementEntryCollectionGeneratedFormatter());
		}

		[NullableContext(1)]
		[Nullable(0)]
		[Preserve]
		public class ElementEntryCollectionGeneratedFormatter : IYamlFormatter<ElementEntryCollection>, IYamlFormatter
		{
			[Preserve]
			public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] ElementEntryCollection value, YamlSerializationContext context)
			{
				if (value == null)
				{
					emitter.WriteNull();
					return;
				}
				emitter.BeginMapping(MappingStyle.Block);
				if (context.Options.NamingConvention == NamingConvention.LowerCamelCase)
				{
					emitter.WriteScalar(ElementEntryCollection.ElementEntryCollectionGeneratedFormatter.elementsKeyUtf8Bytes);
				}
				else
				{
					byte[] array;
					int num;
					NamingConventionMutator.MutateToThreadStaticBufferUtf8(ElementEntryCollection.ElementEntryCollectionGeneratedFormatter.elementsKeyUtf8Bytes, context.Options.NamingConvention, out array, out num);
					emitter.WriteScalar(array.AsSpan<byte>(0, num));
				}
				context.Serialize<ElementEntry[]>(ref emitter, value.elements);
				emitter.EndMapping();
			}

			[Preserve]
			[return: Nullable(2)]
			public ElementEntryCollection Deserialize(ref YamlParser parser, YamlDeserializationContext context)
			{
				if (parser.IsNullScalar())
				{
					parser.Read();
					return null;
				}
				parser.ReadWithVerify(ParseEventType.MappingStart);
				ElementEntry[] array = null;
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
						byte[] array2;
						int num;
						NamingConventionMutator.MutateToThreadStaticBufferUtf8(readOnlySpan, NamingConvention.LowerCamelCase, out array2, out num);
						readOnlySpan = array2.AsSpan<byte>(0, num);
					}
					if (readOnlySpan.Length == 8 && readOnlySpan.SequenceEqual<byte>(ElementEntryCollection.ElementEntryCollectionGeneratedFormatter.elementsKeyUtf8Bytes))
					{
						parser.Read();
						array = context.DeserializeWithAlias<ElementEntry[]>(ref parser);
					}
					else
					{
						parser.Read();
						parser.SkipCurrentNode();
					}
				}
				parser.ReadWithVerify(ParseEventType.MappingEnd);
				return new ElementEntryCollection
				{
					elements = array
				};
			}

			private static readonly byte[] elementsKeyUtf8Bytes = new byte[] { 101, 108, 101, 109, 101, 110, 116, 115 };
		}
	}
}
