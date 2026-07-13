using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_Group8x8 : OpCodeHandlerModRM
	{
		public OpCodeHandler_Group8x8(OpCodeHandler[] tableLow, OpCodeHandler[] tableHigh)
		{
			if (tableLow.Length != 8)
			{
				throw new ArgumentOutOfRangeException("tableLow");
			}
			if (tableHigh.Length != 8)
			{
				throw new ArgumentOutOfRangeException("tableHigh");
			}
			this.tableLow = tableLow;
			this.tableHigh = tableHigh;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			OpCodeHandler opCodeHandler;
			if (decoder.state.mod == 3U)
			{
				opCodeHandler = this.tableHigh[(int)decoder.state.reg];
			}
			else
			{
				opCodeHandler = this.tableLow[(int)decoder.state.reg];
			}
			opCodeHandler.Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler[] tableLow;

		private readonly OpCodeHandler[] tableHigh;
	}
}
