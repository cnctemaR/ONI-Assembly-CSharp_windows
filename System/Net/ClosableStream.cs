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
			this._onClose = onClose;
		}

		public override void Close()
		{
			if (Interlocked.Increment(ref this._closed) == 1)
			{
				EventHandler onClose = this._onClose;
				if (onClose == null)
				{
					return;
				}
				onClose(this, new EventArgs());
			}
		}

		private readonly EventHandler _onClose;

		private int _closed;
	}
}
