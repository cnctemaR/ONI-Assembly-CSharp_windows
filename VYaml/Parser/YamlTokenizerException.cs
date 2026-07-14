using System;
using System.Runtime.CompilerServices;

namespace VYaml.Parser
{
	internal class YamlTokenizerException : Exception
	{
		[NullableContext(1)]
		public YamlTokenizerException(in Marker marker, string message)
			: base(string.Format("{0} at {1}", message, marker))
		{
		}
	}
}
