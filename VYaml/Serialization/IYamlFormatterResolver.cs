using System;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(2)]
	public interface IYamlFormatterResolver
	{
		[return: Nullable(new byte[] { 2, 1 })]
		IYamlFormatter<T> GetFormatter<T>();
	}
}
