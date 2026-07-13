using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpIq : Op
	{
		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			if (!encoder.Verify(operand, OpKind.Immediate64, instruction.GetOpKind(operand)))
			{
				return;
			}
			encoder.ImmSize = ImmSize.Size8;
			ulong immediate = instruction.Immediate64;
			encoder.Immediate = (uint)immediate;
			encoder.ImmediateHi = (uint)(immediate >> 32);
		}

		public override OpKind GetImmediateOpKind()
		{
			return OpKind.Immediate64;
		}
	}
}
