using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal static class OpCodeHandlers
	{
		static OpCodeHandlers()
		{
			uint[] encFlags = EncoderData.EncFlags1;
			uint[] encFlags2 = EncoderData.EncFlags2;
			uint[] encFlags3 = EncoderData.EncFlags3;
			OpCodeHandler[] array = new OpCodeHandler[4936];
			int i = 0;
			InvalidHandler invalidHandler = new InvalidHandler();
			while (i < encFlags.Length)
			{
				EncFlags3 encFlags4 = (EncFlags3)encFlags3[i];
				OpCodeHandler opCodeHandler;
				switch (encFlags4 & EncFlags3.EncodingMask)
				{
				case EncFlags3.None:
				{
					Code code = (Code)i;
					if (code == Code.INVALID)
					{
						opCodeHandler = invalidHandler;
					}
					else if (code <= Code.DeclareQword)
					{
						opCodeHandler = new DeclareDataHandler(code);
					}
					else if (code == Code.Zero_bytes)
					{
						opCodeHandler = new ZeroBytesHandler(code);
					}
					else
					{
						opCodeHandler = new LegacyHandler((EncFlags1)encFlags[i], (EncFlags2)encFlags2[i], encFlags4);
					}
					break;
				}
				case (EncFlags3)1U:
					opCodeHandler = new VexHandler((EncFlags1)encFlags[i], (EncFlags2)encFlags2[i], encFlags4);
					break;
				case (EncFlags3)2U:
					opCodeHandler = new EvexHandler((EncFlags1)encFlags[i], (EncFlags2)encFlags2[i], encFlags4);
					break;
				case EncFlags3.OperandSizeShift:
					opCodeHandler = new XopHandler((EncFlags1)encFlags[i], (EncFlags2)encFlags2[i], encFlags4);
					break;
				case (EncFlags3)4U:
					opCodeHandler = new D3nowHandler((EncFlags2)encFlags2[i], encFlags4);
					break;
				case EncFlags3.AddressSizeShift:
					opCodeHandler = invalidHandler;
					break;
				default:
					throw new InvalidOperationException();
				}
				array[i] = opCodeHandler;
				i++;
			}
			if (i != array.Length)
			{
				throw new InvalidOperationException();
			}
			OpCodeHandlers.Handlers = array;
		}

		[Nullable(1)]
		public static readonly OpCodeHandler[] Handlers;
	}
}
