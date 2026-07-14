using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public sealed class StaticNullableFormatter<T> : IYamlFormatter<T?>, IYamlFormatter where T : struct
	{
		public StaticNullableFormatter([Nullable(new byte[] { 1, 0 })] IYamlFormatter<T> underlyingFormatter)
		{
			this.underlyingFormatter = underlyingFormatter;
		}

		public void Serialize(ref Utf8YamlEmitter emitter, T? value, [Nullable(1)] YamlSerializationContext context)
		{
			if (value != null)
			{
				this.underlyingFormatter.Serialize(ref emitter, value.GetValueOrDefault(), context);
				return;
			}
			emitter.WriteNull();
		}

		public T? Deserialize(ref YamlParser parser, [Nullable(1)] YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			return new T?(this.underlyingFormatter.Deserialize(ref parser, context));
		}

		[Nullable(new byte[] { 1, 0 })]
		private readonly IYamlFormatter<T> underlyingFormatter;
	}
}
