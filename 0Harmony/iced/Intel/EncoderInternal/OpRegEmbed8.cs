using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpRegEmbed8 : Op
	{
		public OpRegEmbed8(Register regLo, Register regHi)
		{
			this.regLo = regLo;
			this.regHi = regHi;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.AddReg(in instruction, operand, this.regLo, this.regHi);
		}

		private readonly Register regLo;

		private readonly Register regHi;
	}
}
