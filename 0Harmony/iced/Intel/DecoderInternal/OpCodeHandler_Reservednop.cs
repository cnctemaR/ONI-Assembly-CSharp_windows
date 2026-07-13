using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_Reservednop : OpCodeHandlerModRM
	{
		public OpCodeHandler_Reservednop(OpCodeHandler reservedNopHandler, OpCodeHandler otherHandler)
		{
			if (reservedNopHandler == null)
			{
				throw new ArgumentNullException("reservedNopHandler");
			}
			this.reservedNopHandler = reservedNopHandler;
			if (otherHandler == null)
			{
				throw new ArgumentNullException("otherHandler");
			}
			this.otherHandler = otherHandler;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			(((decoder.options & DecoderOptions.ForceReservedNop) != DecoderOptions.None) ? this.reservedNopHandler : this.otherHandler).Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler reservedNopHandler;

		private readonly OpCodeHandler otherHandler;
	}
}
