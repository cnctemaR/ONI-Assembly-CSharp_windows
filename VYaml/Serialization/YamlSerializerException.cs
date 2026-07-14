using System;
using System.Runtime.CompilerServices;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class YamlSerializerException : Exception
	{
		[NullableContext(2)]
		public static void ThrowInvalidType<T>()
		{
			throw new YamlSerializerException(string.Format("Cannot detect a value of type: {0}", typeof(T)));
		}

		public static void ThrowInvalidType<[Nullable(2)] T>(string value)
		{
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of {0}, {1}", typeof(T), value));
		}

		public YamlSerializerException(string message)
			: base(message)
		{
		}

		public YamlSerializerException(Marker mark, string message)
			: base(string.Format("{0} at {1}", message, mark))
		{
		}
	}
}
