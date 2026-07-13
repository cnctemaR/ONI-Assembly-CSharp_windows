using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpHx : Op
	{
		public OpHx(Register regLo, Register regHi)
		{
			this.regLo = regLo;
			this.regHi = regHi;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			if (!encoder.Verify(operand, OpKind.Register, instruction.GetOpKind(operand)))
			{
				return;
			}
			Register opRegister = instruction.GetOpRegister(operand);
			if (!encoder.Verify(operand, opRegister, this.regLo, this.regHi))
			{
				return;
			}
			encoder.EncoderFlags |= (EncoderFlags)(opRegister - this.regLo << 27);
		}

		private readonly Register regLo;

		private readonly Register regHi;
	}
}
