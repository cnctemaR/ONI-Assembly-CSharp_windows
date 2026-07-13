using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class InvalidHandler : OpCodeHandler
	{
		public InvalidHandler()
			: base(EncFlags2.None, EncFlags3.Bit16or32 | EncFlags3.Bit64, false, null, Array2.Empty<Op>())
		{
		}

		public override void Encode(Encoder encoder, in Instruction instruction)
		{
			encoder.ErrorMessage = "Can't encode an invalid instruction";
		}

		internal const string ERROR_MESSAGE = "Can't encode an invalid instruction";
	}
}
