using System;
using System.Runtime.CompilerServices;

namespace VYaml.Parser
{
	[NullableContext(1)]
	[Nullable(0)]
	public class YamlParserException : Exception
	{
		public static void Throw(in Marker marker, string message)
		{
			throw new YamlParserException(in marker, message);
		}

		public YamlParserException(in Marker marker, string message)
			: base(string.Format("{0} at {1}", message, marker))
		{
		}
	}
}
