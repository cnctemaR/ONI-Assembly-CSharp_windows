using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpJdisp : Op
	{
		public OpJdisp(int displSize)
		{
			this.displSize = displSize;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.AddBranchDisp(this.displSize, in instruction, operand);
		}

		public override OpKind GetNearBranchOpKind()
		{
			if (this.displSize != 2)
			{
				return OpKind.NearBranch32;
			}
			return OpKind.NearBranch16;
		}

		private readonly int displSize;
	}
}
