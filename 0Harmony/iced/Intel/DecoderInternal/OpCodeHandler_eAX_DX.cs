using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_eAX_DX : OpCodeHandler
	{
		public OpCodeHandler_eAX_DX(Code code16, Code code32)
		{
			this.code16 = code16;
			this.code32 = code32;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.Op1Register = Register.DX;
			if (decoder.state.operandSize != OpSize.Size16)
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				instruction.Op0Register = Register.EAX;
				return;
			}
			instruction.InternalSetCodeNoCheck(this.code16);
			instruction.Op0Register = Register.AX;
		}

		private readonly Code code16;

		private readonly Code code32;
	}
}
