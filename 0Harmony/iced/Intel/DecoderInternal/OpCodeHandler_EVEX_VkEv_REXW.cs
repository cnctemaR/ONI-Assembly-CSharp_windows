using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_VkEv_REXW : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_VkEv_REXW(Register baseReg, Code code32)
		{
			this.baseReg = baseReg;
			this.code32 = code32;
			this.code64 = Code.INVALID;
		}

		public OpCodeHandler_EVEX_VkEv_REXW(Register baseReg, Code code32, Code code64)
		{
			this.baseReg = baseReg;
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((((decoder.state.zs.flags & StateFlags.b) | (StateFlags)decoder.state.vvvv_invalidCheck) & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
			{
				decoder.SetInvalidInstruction();
			}
			Register register;
			if ((decoder.state.zs.flags & (StateFlags)decoder.is64bMode_and_W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
				register = Register.RAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				register = Register.EAX;
			}
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase + decoder.state.extraRegisterBaseEVEX) + this.baseReg;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + register;
				return;
			}
			decoder.SetInvalidInstruction();
		}

		private readonly Register baseReg;

		private readonly Code code32;

		private readonly Code code64;
	}
}
