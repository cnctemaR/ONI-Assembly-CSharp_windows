using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Simple5_a32 : OpCodeHandler
	{
		public OpCodeHandler_Simple5_a32(Code code16, Code code32, Code code64)
		{
			this.codes = new Code3(code16, code32, code64);
		}

		[NullableContext(1)]
		public unsafe override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.state.addressSize != OpSize.Size32 && decoder.invalidCheckMask != 0U)
			{
				decoder.SetInvalidInstruction();
			}
			UIntPtr uintPtr = (UIntPtr)decoder.state.addressSize;
			instruction.InternalSetCodeNoCheck((Code)(*((ref this.codes.codes.FixedElementField) + (UIntPtr)((ulong)uintPtr * 2UL))));
		}

		private readonly Code3 codes;
	}
}
