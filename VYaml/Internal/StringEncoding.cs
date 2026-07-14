using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace VYaml.Internal
{
	internal static class StringEncoding
	{
		[Nullable(1)]
		public static readonly Encoding Utf8 = new UTF8Encoding(false);
	}
}
