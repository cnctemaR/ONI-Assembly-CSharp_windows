using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpModRM_regF0 : Op
	{
		public OpModRM_regF0(Register regLo, Register regHi)
		{
			this.regLo = regLo;
			this.regHi = regHi;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			if (encoder.Bitness != 64 && instruction.GetOpKind(operand) == OpKind.Register && instruction.GetOpRegister(operand) >= this.regLo + 8 && instruction.GetOpRegister(operand) <= this.regLo + 15)
			{
				encoder.EncoderFlags |= EncoderFlags.PF0;
				encoder.AddModRMRegister(in instruction, operand, this.regLo + 8, this.regLo + 15);
				return;
			}
			encoder.AddModRMRegister(in instruction, operand, this.regLo, this.regHi);
		}

		private readonly Register regLo;

		private readonly Register regHi;
	}
}
