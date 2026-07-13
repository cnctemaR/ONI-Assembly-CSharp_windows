using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class StreamCodeWriter : CodeWriter
	{
		public StreamCodeWriter(Stream stream)
		{
			this.Stream = stream;
		}

		public override void WriteByte(byte value)
		{
			this.Stream.WriteByte(value);
		}

		public readonly Stream Stream;
	}
}
