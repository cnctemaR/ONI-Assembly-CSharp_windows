using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpIw : Op
	{
		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			if (!encoder.Verify(operand, OpKind.Immediate16, instruction.GetOpKind(operand)))
			{
				return;
			}
			encoder.ImmSize = ImmSize.Size2;
			encoder.Immediate = (uint)instruction.Immediate16;
		}

		public override OpKind GetImmediateOpKind()
		{
			return OpKind.Immediate16;
		}
	}
}
