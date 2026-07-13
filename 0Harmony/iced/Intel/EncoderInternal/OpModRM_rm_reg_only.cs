using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpModRM_rm_reg_only : Op
	{
		public OpModRM_rm_reg_only(Register regLo, Register regHi)
		{
			this.regLo = regLo;
			this.regHi = regHi;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.AddRegOrMem(in instruction, operand, this.regLo, this.regHi, false, true);
		}

		private readonly Register regLo;

		private readonly Register regHi;
	}
}
