using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_PushIb2 : OpCodeHandler
	{
		public OpCodeHandler_PushIb2(Code code16, Code code32, Code code64)
		{
			this.code16 = code16;
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.is64bMode)
			{
				if (decoder.state.operandSize != OpSize.Size16)
				{
					instruction.InternalSetCodeNoCheck(this.code64);
					instruction.Op0Kind = OpKind.Immediate8to64;
					instruction.InternalImmediate8 = decoder.ReadByte();
					return;
				}
				instruction.InternalSetCodeNoCheck(this.code16);
				instruction.Op0Kind = OpKind.Immediate8to16;
				instruction.InternalImmediate8 = decoder.ReadByte();
				return;
			}
			else
			{
				if (decoder.state.operandSize == OpSize.Size32)
				{
					instruction.InternalSetCodeNoCheck(this.code32);
					instruction.Op0Kind = OpKind.Immediate8to32;
					instruction.InternalImmediate8 = decoder.ReadByte();
					return;
				}
				instruction.InternalSetCodeNoCheck(this.code16);
				instruction.Op0Kind = OpKind.Immediate8to16;
				instruction.InternalImmediate8 = decoder.ReadByte();
				return;
			}
		}

		private readonly Code code16;

		private readonly Code code32;

		private readonly Code code64;
	}
}
