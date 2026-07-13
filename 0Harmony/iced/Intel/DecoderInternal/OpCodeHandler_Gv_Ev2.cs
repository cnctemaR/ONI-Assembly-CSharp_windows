using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Gv_Ev2 : OpCodeHandlerModRM
	{
		public OpCodeHandler_Gv_Ev2(Code code16, Code code32, Code code64)
		{
			this.codes = new Code3(code16, code32, code64);
		}

		[NullableContext(1)]
		public unsafe override void Decode(Decoder decoder, ref Instruction instruction)
		{
			UIntPtr uintPtr = (UIntPtr)decoder.state.operandSize;
			instruction.InternalSetCodeNoCheck((Code)(*((ref this.codes.codes.FixedElementField) + (UIntPtr)((ulong)uintPtr * 2UL))));
			instruction.Op0Register = ((int)uintPtr << 4) + (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.AX;
			if (decoder.state.mod != 3U)
			{
				instruction.Op1Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction);
				return;
			}
			uint num = decoder.state.rm + decoder.state.zs.extraBaseRegisterBase;
			if (decoder.state.operandSize != OpSize.Size16)
			{
				instruction.Op1Register = (int)num + Register.EAX;
				return;
			}
			instruction.Op1Register = (int)num + Register.AX;
		}

		private readonly Code3 codes;
	}
}
