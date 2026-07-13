using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Simple4 : OpCodeHandler
	{
		public OpCodeHandler_Simple4(Code code32, Code code64)
		{
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.zs.flags & StateFlags.W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
				return;
			}
			instruction.InternalSetCodeNoCheck(this.code32);
		}

		private readonly Code code32;

		private readonly Code code64;
	}
}
