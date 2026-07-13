using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_AnotherTable : OpCodeHandler
	{
		public OpCodeHandler_AnotherTable(OpCodeHandler[] otherTable)
		{
			if (otherTable == null)
			{
				throw new ArgumentNullException("otherTable");
			}
			this.otherTable = otherTable;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			decoder.DecodeTable(this.otherTable, ref instruction);
		}

		private readonly OpCodeHandler[] otherTable;
	}
}
