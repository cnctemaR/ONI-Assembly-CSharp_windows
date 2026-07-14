using System;
using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class BitArrayFormatter : IYamlFormatter<BitArray>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] BitArray value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				bool flag = value.Get(i);
				array[i] = (flag ? 49 : 48);
			}
			emitter.WriteScalar(array.AsSpan<byte>(0, value.Length));
			ArrayPool<byte>.Shared.Return(array, false);
		}

		[return: Nullable(2)]
		public unsafe BitArray Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			ReadOnlySpan<byte> scalarAsUtf = parser.GetScalarAsUtf8();
			BitArray bitArray = new BitArray(scalarAsUtf.Length);
			for (int i = 0; i < scalarAsUtf.Length; i++)
			{
				bitArray.Set(i, *scalarAsUtf[i] == 49);
			}
			parser.Read();
			return bitArray;
		}

		public static readonly BitArrayFormatter Instance = new BitArrayFormatter();
	}
}
