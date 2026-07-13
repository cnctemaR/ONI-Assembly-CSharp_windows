using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpModRM_rm : Op
	{
		public OpModRM_rm(Register regLo, Register regHi)
		{
			this.regLo = regLo;
			this.regHi = regHi;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.AddRegOrMem(in instruction, operand, this.regLo, this.regHi, true, true);
		}

		private readonly Register regLo;

		private readonly Register regHi;
	}
}
