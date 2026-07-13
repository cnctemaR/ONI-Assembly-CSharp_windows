using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpId : Op
	{
		public OpId(OpKind opKind)
		{
			this.opKind = opKind;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			OpKind opKind = instruction.GetOpKind(operand);
			if (!encoder.Verify(operand, this.opKind, opKind))
			{
				return;
			}
			encoder.ImmSize = ImmSize.Size4;
			encoder.Immediate = instruction.Immediate32;
		}

		public override OpKind GetImmediateOpKind()
		{
			return this.opKind;
		}

		private readonly OpKind opKind;
	}
}
