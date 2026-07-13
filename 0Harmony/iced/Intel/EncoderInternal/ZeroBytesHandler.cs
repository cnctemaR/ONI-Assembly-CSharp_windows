using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class ZeroBytesHandler : OpCodeHandler
	{
		public ZeroBytesHandler(Code code)
			: base(EncFlags2.None, EncFlags3.Bit16or32 | EncFlags3.Bit64, true, null, Array2.Empty<Op>())
		{
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction)
		{
		}
	}
}
