using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_Group : OpCodeHandlerModRM
	{
		public OpCodeHandler_Group(OpCodeHandler[] groupHandlers)
		{
			if (groupHandlers == null)
			{
				throw new ArgumentNullException("groupHandlers");
			}
			this.groupHandlers = groupHandlers;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			this.groupHandlers[(int)decoder.state.reg].Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler[] groupHandlers;
	}
}
