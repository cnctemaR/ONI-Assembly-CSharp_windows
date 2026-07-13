using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpJ : Op
	{
		public OpJ(OpKind opKind, int immSize)
		{
			this.opKind = opKind;
			this.immSize = immSize;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.AddBranch(this.opKind, this.immSize, in instruction, operand);
		}

		public override OpKind GetNearBranchOpKind()
		{
			return this.opKind;
		}

		private readonly OpKind opKind;

		private readonly int immSize;
	}
}
