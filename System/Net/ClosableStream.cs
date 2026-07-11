using System;
using System.IO;
using System.Threading;

namespace System.Net
{
	internal class ClosableStream : DelegatedStream
	{
		internal ClosableStream(Stream stream, EventHandler onClose)
			: base(stream)
		{
			this.onClose = onClose;
		}

		public override void Close()
		{
			if (Interlocked.Increment(ref this.closed) == 1 && this.onClose != null)
			{
				this.onClose(this, new EventArgs());
			}
		}

		private EventHandler onClose;

		private int closed;
	}
}
