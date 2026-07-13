using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_RvMw_Gw : OpCodeHandlerModRM
	{
		public OpCodeHandler_RvMw_Gw(Code code16, Code code32)
		{
			this.code16 = code16;
			this.code32 = code32;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			Register register;
			if (decoder.state.operandSize != OpSize.Size16)
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				instruction.Op1Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.EAX;
				register = Register.EAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code16);
				instruction.Op1Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.AX;
				register = Register.AX;
			}
			if (decoder.state.mod == 3U)
			{
				instruction.Op0Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + register;
				return;
			}
			instruction.Op0Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code code16;

		private readonly Code code32;
	}
}
