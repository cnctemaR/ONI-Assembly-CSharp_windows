using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public abstract class Stream : MarshalByRefObject, IDisposable
	{
		public abstract bool CanRead { get; }

		public abstract bool CanSeek { get; }

		public abstract bool CanWrite { get; }

		[ComVisible(false)]
		public virtual bool CanTimeout
		{
			get
			{
				return false;
			}
		}

		public abstract long Length { get; }

		public abstract long Position { get; set; }

		public void Dispose()
		{
			this.Close();
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual void Close()
		{
			this.Dispose(true);
		}

		[ComVisible(false)]
		public virtual int ReadTimeout
		{
			get
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
			set
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
		}

		[ComVisible(false)]
		public virtual int WriteTimeout
		{
			get
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
			set
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
		}

		public static Stream Synchronized(Stream stream)
		{
			throw new NotImplementedException();
		}

		[Obsolete("CreateWaitHandle is due for removal.  Use \"new ManualResetEvent(false)\" instead.")]
		protected virtual WaitHandle CreateWaitHandle()
		{
			return new ManualResetEvent(false);
		}

		public abstract void Flush();

		public abstract int Read([In] [Out] byte[] buffer, int offset, int count);

		public virtual int ReadByte()
		{
			byte[] array = new byte[1];
			if (this.Read(array, 0, 1) == 1)
			{
				return (int)array[0];
			}
			return -1;
		}

		public abstract long Seek(long offset, SeekOrigin origin);

		public abstract void SetLength(long value);

		public abstract void Write(byte[] buffer, int offset, int count);

		public virtual void WriteByte(byte value)
		{
			this.Write(new byte[] { value }, 0, 1);
		}

		public virtual IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("This stream does not support reading");
			}
			StreamAsyncResult streamAsyncResult = new StreamAsyncResult(state);
			try
			{
				int num = this.Read(buffer, offset, count);
				streamAsyncResult.SetComplete(null, num);
			}
			catch (Exception ex)
			{
				streamAsyncResult.SetComplete(ex, 0);
			}
			if (callback != null)
			{
				callback(streamAsyncResult);
			}
			return streamAsyncResult;
		}

		public virtual IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("This stream does not support writing");
			}
			StreamAsyncResult streamAsyncResult = new StreamAsyncResult(state);
			try
			{
				this.Write(buffer, offset, count);
				streamAsyncResult.SetComplete(null);
			}
			catch (Exception ex)
			{
				streamAsyncResult.SetComplete(ex);
			}
			if (callback != null)
			{
				callback.BeginInvoke(streamAsyncResult, null, null);
			}
			return streamAsyncResult;
		}

		public virtual int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			StreamAsyncResult streamAsyncResult = asyncResult as StreamAsyncResult;
			if (streamAsyncResult == null || streamAsyncResult.NBytes == -1)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (streamAsyncResult.Done)
			{
				throw new InvalidOperationException("EndRead already called.");
			}
			streamAsyncResult.Done = true;
			if (streamAsyncResult.Exception != null)
			{
				throw streamAsyncResult.Exception;
			}
			return streamAsyncResult.NBytes;
		}

		public virtual void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			StreamAsyncResult streamAsyncResult = asyncResult as StreamAsyncResult;
			if (streamAsyncResult == null || streamAsyncResult.NBytes != -1)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (streamAsyncResult.Done)
			{
				throw new InvalidOperationException("EndWrite already called.");
			}
			streamAsyncResult.Done = true;
			if (streamAsyncResult.Exception != null)
			{
				throw streamAsyncResult.Exception;
			}
		}

		public static readonly Stream Null = new NullStream();
	}
}
