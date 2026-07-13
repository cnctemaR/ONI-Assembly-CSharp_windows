using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_VK_R : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_VK_R(Code code, Register gpr)
		{
			this.code = code;
			this.gpr = gpr;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (((decoder.state.vvvv_invalidCheck | decoder.state.zs.extraRegisterBase) & decoder.invalidCheckMask) != 0U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = (int)decoder.state.reg + Register.K0;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + this.gpr;
				return;
			}
			decoder.SetInvalidInstruction();
		}

		private readonly Code code;

		private readonly Register gpr;
	}
}
