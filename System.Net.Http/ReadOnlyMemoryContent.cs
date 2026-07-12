using System;
using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public sealed class ReadOnlyMemoryContent : HttpContent
	{
		public ReadOnlyMemoryContent(ReadOnlyMemory<byte> content)
		{
			throw new PlatformNotSupportedException();
		}

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			throw new PlatformNotSupportedException();
		}

		protected internal override bool TryComputeLength(out long length)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
