using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpA : Op
	{
		public OpA(int size)
		{
			this.size = size;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.AddFarBranch(in instruction, operand, this.size);
		}

		public override OpKind GetFarBranchOpKind()
		{
			if (this.size != 2)
			{
				return OpKind.FarBranch32;
			}
			return OpKind.FarBranch16;
		}

		private readonly int size;
	}
}
