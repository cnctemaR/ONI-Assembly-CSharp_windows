using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Ms : OpCodeHandlerModRM
	{
		public OpCodeHandler_Ms(Code code16, Code code32, Code code64)
		{
			this.code16 = code16;
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.is64bMode)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
			}
			else if (decoder.state.operandSize == OpSize.Size32)
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

		private readonly Code code64;
	}
}
