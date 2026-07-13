using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpIb : Op
	{
		public OpIb(OpKind opKind)
		{
			this.opKind = opKind;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			ImmSize immSize = encoder.ImmSize;
			if (immSize != ImmSize.Size1)
			{
				if (immSize != ImmSize.Size2)
				{
					OpKind opKind = instruction.GetOpKind(operand);
					if (!encoder.Verify(operand, this.opKind, opKind))
					{
						return;
					}
					encoder.ImmSize = ImmSize.Size1;
					encoder.Immediate = (uint)instruction.Immediate8;
					return;
				}
				else
				{
					if (!encoder.Verify(operand, OpKind.Immediate8_2nd, instruction.GetOpKind(operand)))
					{
						return;
					}
					encoder.ImmSize = ImmSize.Size2_1;
					encoder.ImmediateHi = (uint)instruction.Immediate8_2nd;
					return;
				}
			}
			else
			{
				if (!encoder.Verify(operand, OpKind.Immediate8_2nd, instruction.GetOpKind(operand)))
				{
					return;
				}
				encoder.ImmSize = ImmSize.Size1_1;
				encoder.ImmediateHi = (uint)instruction.Immediate8_2nd;
				return;
			}
		}

		public override OpKind GetImmediateOpKind()
		{
			return this.opKind;
		}

		private readonly OpKind opKind;
	}
}
