using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_RIbIb : OpCodeHandlerModRM
	{
		public OpCodeHandler_RIbIb(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			if (decoder.state.mod == 3U)
			{
				instruction.Op0Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.XMM0;
			}
			else
			{
				decoder.SetInvalidInstruction();
			}
			instruction.Op1Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
			instruction.Op2Kind = OpKind.Immediate8_2nd;
			instruction.InternalImmediate8_2nd = decoder.ReadByte();
		}

		private readonly Code code;
	}
}
