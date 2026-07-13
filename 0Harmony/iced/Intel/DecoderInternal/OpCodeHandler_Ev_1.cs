using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Ev_1 : OpCodeHandlerModRM
	{
		public OpCodeHandler_Ev_1(Code code16, Code code32, Code code64)
		{
			this.codes = new Code3(code16, code32, code64);
		}

		[NullableContext(1)]
		public unsafe override void Decode(Decoder decoder, ref Instruction instruction)
		{
			UIntPtr uintPtr = (UIntPtr)decoder.state.operandSize;
			instruction.InternalSetCodeNoCheck((Code)(*((ref this.codes.codes.FixedElementField) + (UIntPtr)((ulong)uintPtr * 2UL))));
			instruction.Op1Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = 1U;
			decoder.state.zs.flags = decoder.state.zs.flags | StateFlags.NoImm;
			if (decoder.state.mod == 3U)
			{
				instruction.Op0Register = ((int)uintPtr << 4) + (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.AX;
				return;
			}
			instruction.Op0Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code3 codes;
	}
}
