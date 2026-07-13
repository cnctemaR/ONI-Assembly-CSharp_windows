using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VW : OpCodeHandlerModRM
	{
		public OpCodeHandler_VW(Code codeR, Code codeM)
		{
			this.codeR = codeR;
			this.codeM = codeM;
		}

		public OpCodeHandler_VW(Code code)
		{
			this.codeR = code;
			this.codeM = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.XMM0;
			if (decoder.state.mod == 3U)
			{
				instruction.InternalSetCodeNoCheck(this.codeR);
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.XMM0;
				return;
			}
			instruction.InternalSetCodeNoCheck(this.codeM);
			instruction.Op1Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code codeR;

		private readonly Code codeM;
	}
}
