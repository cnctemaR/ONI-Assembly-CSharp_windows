using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_VT_RT_HT : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_VT_RT_HT(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.invalidCheckMask != 0U && (decoder.state.vvvv > 7U || decoder.state.zs.extraRegisterBase != 0U))
			{
				decoder.SetInvalidInstruction();
			}
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = (int)decoder.state.reg + Register.TMM0;
			instruction.Op2Register = (int)(decoder.state.vvvv & 7U) + Register.TMM0;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)decoder.state.rm + Register.TMM0;
				if (decoder.invalidCheckMask != 0U && (decoder.state.zs.extraBaseRegisterBase != 0U || decoder.state.reg == decoder.state.vvvv || decoder.state.reg == decoder.state.rm || decoder.state.rm == decoder.state.vvvv))
				{
					decoder.SetInvalidInstruction();
					return;
				}
			}
			else
			{
				decoder.SetInvalidInstruction();
			}
		}

		private readonly Code code;
	}
}
