using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal abstract class OpCodeHandler
	{
		protected OpCodeHandler()
		{
		}

		protected OpCodeHandler(bool hasModRM)
		{
			this.HasModRM = hasModRM;
		}

		[NullableContext(1)]
		public abstract void Decode(Decoder decoder, ref Instruction instruction);

		public readonly bool HasModRM;
	}
}
