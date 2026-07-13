using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpVsib : Op
	{
		public OpVsib(Register regLo, Register regHi)
		{
			this.vsibIndexRegLo = regLo;
			this.vsibIndexRegHi = regHi;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			encoder.EncoderFlags |= EncoderFlags.MustUseSib;
			encoder.AddRegOrMem(in instruction, operand, Register.None, Register.None, this.vsibIndexRegLo, this.vsibIndexRegHi, true, false);
		}

		private readonly Register vsibIndexRegLo;

		private readonly Register vsibIndexRegHi;
	}
}
