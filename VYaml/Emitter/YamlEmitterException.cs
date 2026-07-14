using System;
using System.Runtime.CompilerServices;

namespace VYaml.Emitter
{
	public class YamlEmitterException : Exception
	{
		[NullableContext(1)]
		public YamlEmitterException(string message)
			: base(message)
		{
		}
	}
}
