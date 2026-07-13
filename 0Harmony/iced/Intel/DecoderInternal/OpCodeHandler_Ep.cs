using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Ep : OpCodeHandlerModRM
	{
		public OpCodeHandler_Ep(Code code16, Code code32, Code code64)
		{
			this.code16 = code16;
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.state.operandSize == OpSize.Size64 && (decoder.options & DecoderOptions.AMD) == DecoderOptions.None)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
			}
			else if (decoder.state.operandSize == OpSize.Size16)
			{
				instruction.InternalSetCodeNoCheck(this.code16);
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
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code code16;

		private readonly Code code32;

		private readonly Code code64;
	}
}
