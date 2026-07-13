using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpModRM_reg : Op
	{
		public OpModRM_reg(Register regLo, Register regHi)
		{
			this.regLo = regLo;
			this.regHi = regHi;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.AddModRMRegister(in instruction, operand, this.regLo, this.regHi);
		}

		private readonly Register regLo;

		private readonly Register regHi;
	}
}
