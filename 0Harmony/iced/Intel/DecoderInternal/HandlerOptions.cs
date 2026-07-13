using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal readonly struct HandlerOptions
	{
		public HandlerOptions(OpCodeHandler handler, DecoderOptions options)
		{
			this.handler = handler;
			this.options = options;
		}

		public readonly OpCodeHandler handler;

		public readonly DecoderOptions options;
	}
}
