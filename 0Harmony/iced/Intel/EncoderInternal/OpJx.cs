using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpJx : Op
	{
		public OpJx(int immSize)
		{
			this.immSize = immSize;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.AddBranchX(this.immSize, in instruction, operand);
		}

		public override OpKind GetNearBranchOpKind()
		{
			return base.GetNearBranchOpKind();
		}

		private readonly int immSize;
	}
}
