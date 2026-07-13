using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpImm : Op
	{
		public OpImm(byte value)
		{
			this.value = value;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			if (!encoder.Verify(operand, OpKind.Immediate8, instruction.GetOpKind(operand)))
			{
				return;
			}
			if (instruction.Immediate8 != this.value)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": Expected 0x");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(this.value, "X2");
				defaultInterpolatedStringHandler.AppendLiteral(", actual: 0x");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(instruction.Immediate8, "X2");
				encoder.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
		}

		public override OpKind GetImmediateOpKind()
		{
			return OpKind.Immediate8;
		}

		private readonly byte value;
	}
}
