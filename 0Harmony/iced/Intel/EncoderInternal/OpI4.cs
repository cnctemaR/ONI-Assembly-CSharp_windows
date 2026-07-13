using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpI4 : Op
	{
		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			OpKind opKind = instruction.GetOpKind(operand);
			if (!encoder.Verify(operand, OpKind.Immediate8, opKind))
			{
				return;
			}
			if (instruction.Immediate8 > 15)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": Immediate value must be 0-15, but value is 0x");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(instruction.Immediate8, "X2");
				encoder.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			encoder.ImmSize = ImmSize.Size1;
			encoder.Immediate |= (uint)instruction.Immediate8;
		}

		public override OpKind GetImmediateOpKind()
		{
			return OpKind.Immediate8;
		}
	}
}
