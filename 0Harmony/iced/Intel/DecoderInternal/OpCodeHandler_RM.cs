using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_RM : OpCodeHandlerModRM
	{
		public OpCodeHandler_RM(OpCodeHandler reg, OpCodeHandler mem)
		{
			if (reg == null)
			{
				throw new ArgumentNullException("reg");
			}
			this.reg = reg;
			if (mem == null)
			{
				throw new ArgumentNullException("mem");
			}
			this.mem = mem;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			((decoder.state.mod == 3U) ? this.reg : this.mem).Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler reg;

		private readonly OpCodeHandler mem;
	}
}
