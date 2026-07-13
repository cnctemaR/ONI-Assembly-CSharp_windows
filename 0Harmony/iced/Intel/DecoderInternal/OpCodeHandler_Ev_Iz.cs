using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Ev_Iz : OpCodeHandlerModRM
	{
		public OpCodeHandler_Ev_Iz(Code code16, Code code32, Code code64)
		{
			this.codes = new Code3(code16, code32, code64);
		}

		public OpCodeHandler_Ev_Iz(Code code16, Code code32, Code code64, HandlerFlags flags)
		{
			this.codes = new Code3(code16, code32, code64);
			this.flags = flags;
		}

		[NullableContext(1)]
		public unsafe override void Decode(Decoder decoder, ref Instruction instruction)
		{
			UIntPtr uintPtr = (UIntPtr)decoder.state.operandSize;
			instruction.InternalSetCodeNoCheck((Code)(*((ref this.codes.codes.FixedElementField) + (UIntPtr)((ulong)uintPtr * 2UL))));
			if (decoder.state.mod < 3U)
			{
				decoder.state.zs.flags = decoder.state.zs.flags | (StateFlags)((this.flags & HandlerFlags.Lock) << 10);
				instruction.Op0Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction);
			}
			else
			{
				instruction.Op0Register = ((int)uintPtr << 4) + (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.AX;
			}
			if ((uint)uintPtr == 1U)
			{
				instruction.Op1Kind = OpKind.Immediate32;
				instruction.Immediate32 = decoder.ReadUInt32();
				return;
			}
			if ((uint)uintPtr == 2U)
			{
				instruction.Op1Kind = OpKind.Immediate32to64;
				instruction.Immediate32 = decoder.ReadUInt32();
				return;
			}
			instruction.Op1Kind = OpKind.Immediate16;
			instruction.InternalImmediate16 = decoder.ReadUInt16();
		}

		private readonly Code3 codes;

		private readonly HandlerFlags flags;
	}
}
