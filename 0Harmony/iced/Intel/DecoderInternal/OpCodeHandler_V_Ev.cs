using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_V_Ev : OpCodeHandlerModRM
	{
		public OpCodeHandler_V_Ev(Code codeW0, Code codeW1)
		{
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			Register register;
			if (decoder.state.operandSize != OpSize.Size64)
			{
				instruction.InternalSetCodeNoCheck(this.codeW0);
				register = Register.EAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.codeW1);
				register = Register.RAX;
			}
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.XMM0;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + register;
				return;
			}
			instruction.Op1Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code codeW0;

		private readonly Code codeW1;
	}
}
