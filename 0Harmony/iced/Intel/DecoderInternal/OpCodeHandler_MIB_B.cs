using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_MIB_B : OpCodeHandlerModRM
	{
		public OpCodeHandler_MIB_B(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.state.reg > 3U || (decoder.state.zs.extraRegisterBase & decoder.invalidCheckMask) != 0U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op1Register = (int)decoder.state.reg + Register.BND0;
			instruction.Op0Kind = OpKind.Memory;
			decoder.ReadOpMem_MPX(ref instruction);
			if (decoder.invalidCheckMask != 0U && instruction.MemoryBase == Register.RIP)
			{
				decoder.SetInvalidInstruction();
			}
		}

		private readonly Code code;
	}
}
