using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	[Serializable]
	internal class EncoderException : Exception
	{
		public Instruction Instruction { get; }

		public EncoderException(string message, in Instruction instruction)
			: base(message)
		{
			this.Instruction = instruction;
		}

		protected EncoderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
