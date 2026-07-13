using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_Gq_HK_RK : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_Gq_HK_RK(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.invalidCheckMask != 0U && decoder.state.vvvv > 7U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.RAX;
			instruction.Op1Register = (int)(decoder.state.vvvv & 7U) + Register.K0;
			if (decoder.state.mod == 3U)
			{
				instruction.Op2Register = (int)decoder.state.rm + Register.K0;
				return;
			}
			decoder.SetInvalidInstruction();
		}

		private readonly Code code;
	}
}
