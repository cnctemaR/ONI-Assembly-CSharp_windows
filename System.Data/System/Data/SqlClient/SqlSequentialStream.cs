using System;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.SqlClient
{
	internal sealed class SqlSequentialStream : Stream
	{
		internal SqlSequentialStream(SqlDataReader reader, int columnIndex)
		{
			this._reader = reader;
			this._columnIndex = columnIndex;
			this._currentTask = null;
			this._disposalTokenSource = new CancellationTokenSource();
			if (reader.Command != null && reader.Command.CommandTimeout != 0)
			{
				this._readTimeout = (int)Math.Min((long)reader.Command.CommandTimeout * 1000L, 2147483647L);
				return;
			}
			this._readTimeout = -1;
		}

		public override bool CanRead
		{
			get
			{
				return this._reader != null && !this._reader.IsClosed;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get
			{
				throw ADP.NotSupported();
			}
		}

		public override long Position
		{
			get
			{
				throw ADP.NotSupported();
			}
			set
			{
				throw ADP.NotSupported();
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this._readTimeout;
			}
			set
			{
				if (value > 0 || value == -1)
				{
					this._readTimeout = value;
					return;
				}
				throw ADP.ArgumentOutOfRange("value");
			}
		}

		internal int ColumnIndex
		{
			get
			{
				return this._columnIndex;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			SqlSequentialStream.ValidateReadParameters(buffer, offset, count);
			if (!this.CanRead)
			{
				throw ADP.ObjectDisposed(this);
			}
			if (this._currentTask != null)
			{
				throw ADP.AsyncOperationPending();
			}
			int bytesInternalSequential;
			try
			{
				bytesInternalSequential = this._reader.GetBytesInternalSequential(this._columnIndex, buffer, offset, count, new long?((long)this._readTimeout));
			}
			catch (SqlException ex)
			{
				throw ADP.ErrorReadingFromStream(ex);
			}
			return bytesInternalSequential;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			SqlSequentialStream.ValidateReadParameters(buffer, offset, count);
			TaskCompletionSource<int> completion = new TaskCompletionSource<int>();
			if (!this.CanRead)
			{
				completion.SetException(ADP.ExceptionWithStackTrace(ADP.ObjectDisposed(this)));
			}
			else
			{
				try
				{
					if (Interlocked.CompareExchange<Task>(ref this._currentTask, completion.Task, null) != null)
					{
						completion.SetException(ADP.ExceptionWithStackTrace(ADP.AsyncOperationPending()));
					}
					else
					{
						CancellationTokenSource combinedTokenSource;
						if (!cancellationToken.CanBeCanceled)
						{
							combinedTokenSource = this._disposalTokenSource;
						}
						else
						{
							combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._disposalTokenSource.Token);
						}
						int num = 0;
						Task<int> task = null;
						SqlDataReader reader = this._reader;
						if (reader != null && !cancellationToken.IsCancellationRequested && !this._disposalTokenSource.Token.IsCancellationRequested)
						{
							task = reader.GetBytesAsync(this._columnIndex, buffer, offset, count, this._readTimeout, combinedTokenSource.Token, out num);
						}
						if (task == null)
						{
							this._currentTask = null;
							if (cancellationToken.IsCancellationRequested)
							{
								completion.SetCanceled();
							}
							else if (!this.CanRead)
							{
								completion.SetException(ADP.ExceptionWithStackTrace(ADP.ObjectDisposed(this)));
							}
							else
							{
								completion.SetResult(num);
							}
							if (combinedTokenSource != this._disposalTokenSource)
							{
								combinedTokenSource.Dispose();
							}
						}
						else
						{
							task.ContinueWith(delegate(Task<int> t)
							{
								this._currentTask = null;
								if (t.Status == TaskStatus.RanToCompletion && this.CanRead)
								{
									completion.SetResult(t.Result);
								}
								else if (t.Status == TaskStatus.Faulted)
								{
									if (t.Exception.InnerException is SqlException)
									{
										completion.SetException(ADP.ExceptionWithStackTrace(ADP.ErrorReadingFromStream(t.Exception.InnerException)));
									}
									else
									{
										completion.SetException(t.Exception.InnerException);
									}
								}
								else if (!this.CanRead)
								{
									completion.SetException(ADP.ExceptionWithStackTrace(ADP.ObjectDisposed(this)));
								}
								else
								{
									completion.SetCanceled();
								}
								if (combinedTokenSource != this._disposalTokenSource)
								{
									combinedTokenSource.Dispose();
								}
							}, TaskScheduler.Default);
						}
					}
				}
				catch (Exception ex)
				{
					completion.TrySetException(ex);
					Interlocked.CompareExchange<Task>(ref this._currentTask, null, completion.Task);
					throw;
				}
			}
			return completion.Task;
		}

		public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.ReadAsync(array, offset, count, CancellationToken.None), asyncCallback, asyncState);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw ADP.NotSupported();
		}

		public override void SetLength(long value)
		{
			throw ADP.NotSupported();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw ADP.NotSupported();
		}

		internal void SetClosed()
		{
			this._disposalTokenSource.Cancel();
			this._reader = null;
			Task currentTask = this._currentTask;
			if (currentTask != null)
			{
				((IAsyncResult)currentTask).AsyncWaitHandle.WaitOne();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.SetClosed();
			}
			base.Dispose(disposing);
		}

		internal static void ValidateReadParameters(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw ADP.ArgumentNull("buffer");
			}
			if (offset < 0)
			{
				throw ADP.ArgumentOutOfRange("offset");
			}
			if (count < 0)
			{
				throw ADP.ArgumentOutOfRange("count");
			}
			try
			{
				if (checked(offset + count) > buffer.Length)
				{
					throw ExceptionBuilder.InvalidOffsetLength();
				}
			}
			catch (OverflowException)
			{
				throw ExceptionBuilder.InvalidOffsetLength();
			}
		}

		private SqlDataReader _reader;

		private int _columnIndex;

		private Task _currentTask;

		private int _readTimeout;

		private CancellationTokenSource _disposalTokenSource;
	}
}
