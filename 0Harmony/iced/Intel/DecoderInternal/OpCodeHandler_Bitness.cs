using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_Bitness : OpCodeHandler
	{
		public OpCodeHandler_Bitness(OpCodeHandler handler1632, OpCodeHandler handler64)
		{
			this.handler1632 = handler1632;
			this.handler64 = handler64;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			OpCodeHandler opCodeHandler;
			if (decoder.is64bMode)
			{
				opCodeHandler = this.handler64;
			}
			else
			{
				opCodeHandler = this.handler1632;
			}
			if (opCodeHandler.HasModRM)
			{
				decoder.ReadModRM();
			}
			opCodeHandler.Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler handler1632;

		private readonly OpCodeHandler handler64;
	}
}
