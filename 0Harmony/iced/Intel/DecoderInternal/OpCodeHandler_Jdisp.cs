using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Jdisp : OpCodeHandler
	{
		public OpCodeHandler_Jdisp(Code code16, Code code32)
		{
			this.code16 = code16;
			this.code32 = code32;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.state.operandSize != OpSize.Size16)
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				instruction.Op0Kind = OpKind.NearBranch32;
				instruction.NearBranch32 = decoder.ReadUInt32();
				return;
			}
			instruction.InternalSetCodeNoCheck(this.code16);
			instruction.Op0Kind = OpKind.NearBranch16;
			instruction.InternalNearBranch16 = decoder.ReadUInt16();
		}

		private readonly Code code16;

		private readonly Code code32;
	}
}
