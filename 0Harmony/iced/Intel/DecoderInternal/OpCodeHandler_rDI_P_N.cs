using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_rDI_P_N : OpCodeHandlerModRM
	{
		public OpCodeHandler_rDI_P_N(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			if (decoder.state.addressSize == OpSize.Size64)
			{
				instruction.Op0Kind = OpKind.MemorySegRDI;
			}
			else if (decoder.state.addressSize == OpSize.Size32)
			{
				instruction.Op0Kind = OpKind.MemorySegEDI;
			}
			else
			{
				instruction.Op0Kind = OpKind.MemorySegDI;
			}
			instruction.Op1Register = (int)decoder.state.reg + Register.MM0;
			if (decoder.state.mod == 3U)
			{
				instruction.Op2Register = (int)decoder.state.rm + Register.MM0;
				return;
			}
			decoder.SetInvalidInstruction();
		}

		private readonly Code code;
	}
}
