using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Iw_Ib : OpCodeHandler
	{
		public OpCodeHandler_Iw_Ib(Code code16, Code code32, Code code64)
		{
			this.code16 = code16;
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.Op0Kind = OpKind.Immediate16;
			instruction.InternalImmediate16 = decoder.ReadUInt16();
			instruction.Op1Kind = OpKind.Immediate8_2nd;
			instruction.InternalImmediate8_2nd = decoder.ReadByte();
			if (decoder.is64bMode)
			{
				if (decoder.state.operandSize != OpSize.Size16)
				{
					instruction.InternalSetCodeNoCheck(this.code64);
					return;
				}
				instruction.InternalSetCodeNoCheck(this.code16);
				return;
			}
			else
			{
				if (decoder.state.operandSize == OpSize.Size32)
				{
					instruction.InternalSetCodeNoCheck(this.code32);
					return;
				}
				instruction.InternalSetCodeNoCheck(this.code16);
				return;
			}
		}

		private readonly Code code16;

		private readonly Code code32;

		private readonly Code code64;
	}
}
