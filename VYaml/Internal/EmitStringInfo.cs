using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;

namespace VYaml.Internal
{
	internal readonly struct EmitStringInfo
	{
		public EmitStringInfo(int lines, bool needsQuotes, bool isReservedWord)
		{
			this.Lines = lines;
			this.NeedsQuotes = needsQuotes;
			this.IsReservedWord = isReservedWord;
		}

		[NullableContext(1)]
		public ScalarStyle SuggestScalarStyle(YamlEmitOptions options)
		{
			if (this.Lines > 1)
			{
				return ScalarStyle.Literal;
			}
			if (!this.NeedsQuotes)
			{
				return ScalarStyle.Plain;
			}
			return options.StringQuoteStyle;
		}

		public readonly int Lines;

		public readonly bool NeedsQuotes;

		public readonly bool IsReservedWord;
	}
}
