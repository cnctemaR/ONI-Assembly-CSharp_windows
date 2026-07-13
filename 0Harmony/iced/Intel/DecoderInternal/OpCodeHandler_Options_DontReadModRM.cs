using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_Options_DontReadModRM : OpCodeHandlerModRM
	{
		public OpCodeHandler_Options_DontReadModRM(OpCodeHandler defaultHandler, OpCodeHandler handler1, DecoderOptions options1)
		{
			if (defaultHandler == null)
			{
				throw new ArgumentNullException("defaultHandler");
			}
			this.defaultHandler = defaultHandler;
			this.infos = new HandlerOptions[]
			{
				new HandlerOptions(handler1, options1)
			};
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			OpCodeHandler handler = this.defaultHandler;
			DecoderOptions options = decoder.options;
			foreach (HandlerOptions handlerOptions in this.infos)
			{
				if ((options & handlerOptions.options) != DecoderOptions.None)
				{
					handler = handlerOptions.handler;
					break;
				}
			}
			handler.Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler defaultHandler;

		private readonly HandlerOptions[] infos;
	}
}
