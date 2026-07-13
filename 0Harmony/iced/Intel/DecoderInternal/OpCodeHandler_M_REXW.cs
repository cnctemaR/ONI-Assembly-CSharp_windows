using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_M_REXW : OpCodeHandlerModRM
	{
		public OpCodeHandler_M_REXW(Code code32, Code code64)
		{
			this.code32 = code32;
			this.code64 = code64;
		}

		public OpCodeHandler_M_REXW(Code code32, Code code64, HandlerFlags flags32, HandlerFlags flags64)
		{
			this.code32 = code32;
			this.code64 = code64;
			this.flags32 = flags32;
			this.flags64 = flags64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.zs.flags & StateFlags.W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code32);
			}
			if (decoder.state.mod == 3U)
			{
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.Op0Kind = OpKind.Memory;
			HandlerFlags handlerFlags = (((decoder.state.zs.flags & StateFlags.W) != (StateFlags)0U) ? this.flags64 : this.flags32);
			if ((handlerFlags & (HandlerFlags.Xacquire | HandlerFlags.Xrelease)) != HandlerFlags.None)
			{
				decoder.SetXacquireXrelease(ref instruction);
			}
			decoder.state.zs.flags = decoder.state.zs.flags | (StateFlags)((handlerFlags & HandlerFlags.Lock) << 10);
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code code32;

		private readonly Code code64;

		private readonly HandlerFlags flags32;

		private readonly HandlerFlags flags64;
	}
}
