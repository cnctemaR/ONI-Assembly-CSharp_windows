using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpModRM_rm_mem_only : Op
	{
		public OpModRM_rm_mem_only(bool mustUseSib)
		{
			this.mustUseSib = mustUseSib;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			if (this.mustUseSib)
			{
				encoder.EncoderFlags |= EncoderFlags.MustUseSib;
			}
			encoder.AddRegOrMem(in instruction, operand, Register.None, Register.None, true, false);
		}

		private readonly bool mustUseSib;
	}
}
