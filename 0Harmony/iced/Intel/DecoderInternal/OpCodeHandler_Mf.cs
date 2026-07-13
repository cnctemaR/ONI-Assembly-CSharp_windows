using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Mf : OpCodeHandlerModRM
	{
		public OpCodeHandler_Mf(Code code)
		{
			this.code16 = code;
			this.code32 = code;
		}

		public OpCodeHandler_Mf(Code code16, Code code32)
		{
			this.code16 = code16;
			this.code32 = code32;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.state.operandSize != OpSize.Size16)
			{
				instruction.InternalSetCodeNoCheck(this.code32);
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code16);
			}
			instruction.Op0Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code code16;

		private readonly Code code32;
	}
}
