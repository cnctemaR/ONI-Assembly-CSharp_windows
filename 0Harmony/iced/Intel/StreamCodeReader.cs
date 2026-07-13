using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class StreamCodeReader : CodeReader
	{
		public StreamCodeReader(Stream stream)
		{
			this.Stream = stream;
		}

		public override int ReadByte()
		{
			return this.Stream.ReadByte();
		}

		public readonly Stream Stream;
	}
}
