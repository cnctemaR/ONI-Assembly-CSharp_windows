using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_Options1632 : OpCodeHandler
	{
		public OpCodeHandler_Options1632(OpCodeHandler defaultHandler, OpCodeHandler handler1, DecoderOptions options1)
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
			this.infoOptions = options1;
		}

		public OpCodeHandler_Options1632(OpCodeHandler defaultHandler, OpCodeHandler handler1, DecoderOptions options1, OpCodeHandler handler2, DecoderOptions options2)
		{
			if (defaultHandler == null)
			{
				throw new ArgumentNullException("defaultHandler");
			}
			this.defaultHandler = defaultHandler;
			HandlerOptions[] array = new HandlerOptions[2];
			int num = 0;
			if (handler1 == null)
			{
				throw new ArgumentNullException("handler1");
			}
			array[num] = new HandlerOptions(handler1, options1);
			int num2 = 1;
			if (handler2 == null)
			{
				throw new ArgumentNullException("handler2");
			}
			array[num2] = new HandlerOptions(handler2, options2);
			this.infos = array;
			this.infoOptions = options1 | options2;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			OpCodeHandler handler = this.defaultHandler;
			DecoderOptions options = decoder.options;
			if (!decoder.is64bMode && (decoder.options & this.infoOptions) != DecoderOptions.None)
			{
				foreach (HandlerOptions handlerOptions in this.infos)
				{
					if ((options & handlerOptions.options) != DecoderOptions.None)
					{
						handler = handlerOptions.handler;
						break;
					}
				}
			}
			if (handler.HasModRM)
			{
				decoder.ReadModRM();
			}
			handler.Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler defaultHandler;

		private readonly HandlerOptions[] infos;

		private readonly DecoderOptions infoOptions;
	}
}
