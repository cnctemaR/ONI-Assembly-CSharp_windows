using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(2)]
	[Nullable(0)]
	internal readonly struct HandlerInfo
	{
		[NullableContext(1)]
		public HandlerInfo(OpCodeHandler handler)
		{
			this.handler = handler;
			this.handlers = null;
		}

		public HandlerInfo([Nullable(new byte[] { 1, 2 })] OpCodeHandler[] handlers)
		{
			this.handler = null;
			this.handlers = handlers;
		}

		public readonly OpCodeHandler handler;

		public readonly OpCodeHandler[] handlers;
	}
}
