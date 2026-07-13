using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpRegSTi : Op
	{
		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			if (!encoder.Verify(operand, OpKind.Register, instruction.GetOpKind(operand)))
			{
				return;
			}
			Register opRegister = instruction.GetOpRegister(operand);
			if (!encoder.Verify(operand, opRegister, Register.ST0, Register.ST7))
			{
				return;
			}
			encoder.OpCode |= (uint)(opRegister - Register.ST0);
		}
	}
}
