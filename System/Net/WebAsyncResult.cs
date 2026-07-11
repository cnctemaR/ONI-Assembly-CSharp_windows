using System;
using System.IO;

namespace System.Net
{
	internal class WebAsyncResult : SimpleAsyncResult
	{
		public WebAsyncResult(AsyncCallback cb, object state)
			: base(cb, state)
		{
		}

		public WebAsyncResult(HttpWebRequest request, AsyncCallback cb, object state)
			: base(cb, state)
		{
			this.AsyncObject = request;
		}

		public WebAsyncResult(AsyncCallback cb, object state, byte[] buffer, int offset, int size)
			: base(cb, state)
		{
			this.buffer = buffer;
			this.offset = offset;
			this.size = size;
		}

		internal void Reset()
		{
			this.nbytes = 0;
			this.response = null;
			this.buffer = null;
			this.offset = 0;
			this.size = 0;
			base.Reset_internal();
		}

		internal void SetCompleted(bool synch, int nbytes)
		{
			this.nbytes = nbytes;
			base.SetCompleted_internal(synch);
		}

		internal void SetCompleted(bool synch, Stream writeStream)
		{
			this.writeStream = writeStream;
			base.SetCompleted_internal(synch);
		}

		internal void SetCompleted(bool synch, HttpWebResponse response)
		{
			this.response = response;
			base.SetCompleted_internal(synch);
		}

		internal void DoCallback()
		{
			base.DoCallback_internal();
		}

		internal int NBytes
		{
			get
			{
				return this.nbytes;
			}
			set
			{
				this.nbytes = value;
			}
		}

		internal IAsyncResult InnerAsyncResult
		{
			get
			{
				return this.innerAsyncResult;
			}
			set
			{
				this.innerAsyncResult = value;
			}
		}

		internal Stream WriteStream
		{
			get
			{
				return this.writeStream;
			}
		}

		internal HttpWebResponse Response
		{
			get
			{
				return this.response;
			}
		}

		internal byte[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		internal int Offset
		{
			get
			{
				return this.offset;
			}
		}

		internal int Size
		{
			get
			{
				return this.size;
			}
		}

		private int nbytes;

		private IAsyncResult innerAsyncResult;

		private HttpWebResponse response;

		private Stream writeStream;

		private byte[] buffer;

		private int offset;

		private int size;

		public bool EndCalled;

		public bool AsyncWriteAll;

		public HttpWebRequest AsyncObject;
	}
}
