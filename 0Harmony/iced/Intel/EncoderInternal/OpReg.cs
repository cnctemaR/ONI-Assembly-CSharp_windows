using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpReg : Op
	{
		public OpReg(Register register)
		{
			this.register = register;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.Verify(operand, OpKind.Register, instruction.GetOpKind(operand));
			encoder.Verify(operand, this.register, instruction.GetOpRegister(operand));
		}

		private readonly Register register;
	}
}
