using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_IbReg2 : OpCodeHandler
	{
		public OpCodeHandler_IbReg2(Code code16, Code code32)
		{
			this.code16 = code16;
			this.code32 = code32;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.Op0Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
			if (decoder.state.operandSize != OpSize.Size16)
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				instruction.Op1Register = Register.EAX;
				return;
			}
			instruction.InternalSetCodeNoCheck(this.code16);
			instruction.Op1Register = Register.AX;
		}

		private readonly Code code16;

		private readonly Code code32;
	}
}
