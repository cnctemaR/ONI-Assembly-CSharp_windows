using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal abstract class OpCodeHandlerReader
	{
		public abstract int ReadHandlers(ref TableDeserializer deserializer, [Nullable(new byte[] { 1, 2 })] OpCodeHandler[] result, int resultIndex);
	}
}
