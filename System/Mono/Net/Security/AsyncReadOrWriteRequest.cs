using System;

namespace Mono.Net.Security
{
	internal abstract class AsyncReadOrWriteRequest : AsyncProtocolRequest
	{
		protected BufferOffsetSize UserBuffer { get; }

		protected int CurrentSize { get; set; }

		public AsyncReadOrWriteRequest(MobileAuthenticatedStream parent, bool sync, byte[] buffer, int offset, int size)
			: base(parent, sync)
		{
			this.UserBuffer = new BufferOffsetSize(buffer, offset, size);
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}]", base.Name, this.UserBuffer);
		}
	}
}
