using System;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.SqlClient
{
	internal sealed class SqlSequentialTextReader : TextReader
	{
		internal SqlSequentialTextReader(SqlDataReader reader, int columnIndex, Encoding encoding)
		{
			this._reader = reader;
			this._columnIndex = columnIndex;
			this._encoding = encoding;
			this._decoder = encoding.GetDecoder();
			this._leftOverBytes = null;
			this._peekedChar = -1;
			this._currentTask = null;
			this._disposalTokenSource = new CancellationTokenSource();
		}

		internal int ColumnIndex
		{
			get
			{
				return this._columnIndex;
			}
		}

		public override int Peek()
		{
			if (this._currentTask != null)
			{
				throw ADP.AsyncOperationPending();
			}
			if (this.IsClosed)
			{
				throw ADP.ObjectDisposed(this);
			}
			if (!this.HasPeekedChar)
			{
				this._peekedChar = this.Read();
			}
			return this._peekedChar;
		}

		public override int Read()
		{
			if (this._currentTask != null)
			{
				throw ADP.AsyncOperationPending();
			}
			if (this.IsClosed)
			{
				throw ADP.ObjectDisposed(this);
			}
			int num = -1;
			if (this.HasPeekedChar)
			{
				num = this._peekedChar;
				this._peekedChar = -1;
			}
			else
			{
				char[] array = new char[1];
				if (this.InternalRead(array, 0, 1) == 1)
				{
					num = (int)array[0];
				}
			}
			return num;
		}

		public override int Read(char[] buffer, int index, int count)
		{
			SqlSequentialTextReader.ValidateReadParameters(buffer, index, count);
			if (this.IsClosed)
			{
				throw ADP.ObjectDisposed(this);
			}
			if (this._currentTask != null)
			{
				throw ADP.AsyncOperationPending();
			}
			int num = 0;
			int num2 = count;
			if (num2 > 0 && this.HasPeekedChar)
			{
				buffer[index + num] = (char)this._peekedChar;
				num++;
				num2--;
				this._peekedChar = -1;
			}
			return num + this.InternalRead(buffer, index + num, num2);
		}

		public override Task<int> ReadAsync(char[] buffer, int index, int count)
		{
			SqlSequentialTextReader.ValidateReadParameters(buffer, index, count);
			TaskCompletionSource<int> completion = new TaskCompletionSource<int>();
			if (this.IsClosed)
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
						bool flag = true;
						int charsRead = 0;
						int adjustedIndex = index;
						int charsNeeded = count;
						if (this.HasPeekedChar && charsNeeded > 0)
						{
							int peekedChar = this._peekedChar;
							if (peekedChar >= 0)
							{
								buffer[adjustedIndex] = (char)peekedChar;
								int num = adjustedIndex;
								adjustedIndex = num + 1;
								num = charsRead;
								charsRead = num + 1;
								num = charsNeeded;
								charsNeeded = num - 1;
								this._peekedChar = -1;
							}
						}
						int byteBufferUsed;
						byte[] byteBuffer = this.PrepareByteBuffer(charsNeeded, out byteBufferUsed);
						if (byteBufferUsed < byteBuffer.Length || byteBuffer.Length == 0)
						{
							SqlDataReader reader = this._reader;
							if (reader != null)
							{
								int num2;
								Task<int> bytesAsync = reader.GetBytesAsync(this._columnIndex, byteBuffer, byteBufferUsed, byteBuffer.Length - byteBufferUsed, -1, this._disposalTokenSource.Token, out num2);
								if (bytesAsync == null)
								{
									byteBufferUsed += num2;
								}
								else
								{
									flag = false;
									bytesAsync.ContinueWith(delegate(Task<int> t)
									{
										this._currentTask = null;
										if (t.Status == TaskStatus.RanToCompletion && !this.IsClosed)
										{
											try
											{
												int result = t.Result;
												byteBufferUsed += result;
												if (byteBufferUsed > 0)
												{
													charsRead += this.DecodeBytesToChars(byteBuffer, byteBufferUsed, buffer, adjustedIndex, charsNeeded);
												}
												completion.SetResult(charsRead);
												return;
											}
											catch (Exception ex2)
											{
												completion.SetException(ex2);
												return;
											}
										}
										if (this.IsClosed)
										{
											completion.SetException(ADP.ExceptionWithStackTrace(ADP.ObjectDisposed(this)));
											return;
										}
										if (t.Status == TaskStatus.Faulted)
										{
											if (t.Exception.InnerException is SqlException)
											{
												completion.SetException(ADP.ExceptionWithStackTrace(ADP.ErrorReadingFromStream(t.Exception.InnerException)));
												return;
											}
											completion.SetException(t.Exception.InnerException);
											return;
										}
										else
										{
											completion.SetCanceled();
										}
									}, TaskScheduler.Default);
								}
								if (flag && byteBufferUsed > 0)
								{
									charsRead += this.DecodeBytesToChars(byteBuffer, byteBufferUsed, buffer, adjustedIndex, charsNeeded);
								}
							}
							else
							{
								completion.SetException(ADP.ExceptionWithStackTrace(ADP.ObjectDisposed(this)));
							}
						}
						if (flag)
						{
							this._currentTask = null;
							if (this.IsClosed)
							{
								completion.SetCanceled();
							}
							else
							{
								completion.SetResult(charsRead);
							}
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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.SetClosed();
			}
			base.Dispose(disposing);
		}

		internal void SetClosed()
		{
			this._disposalTokenSource.Cancel();
			this._reader = null;
			this._peekedChar = -1;
			Task currentTask = this._currentTask;
			if (currentTask != null)
			{
				((IAsyncResult)currentTask).AsyncWaitHandle.WaitOne();
			}
		}

		private int InternalRead(char[] buffer, int index, int count)
		{
			int num2;
			try
			{
				int num;
				byte[] array = this.PrepareByteBuffer(count, out num);
				num += this._reader.GetBytesInternalSequential(this._columnIndex, array, num, array.Length - num, null);
				if (num > 0)
				{
					num2 = this.DecodeBytesToChars(array, num, buffer, index, count);
				}
				else
				{
					num2 = 0;
				}
			}
			catch (SqlException ex)
			{
				throw ADP.ErrorReadingFromStream(ex);
			}
			return num2;
		}

		private byte[] PrepareByteBuffer(int numberOfChars, out int byteBufferUsed)
		{
			byte[] array;
			if (numberOfChars == 0)
			{
				array = Array.Empty<byte>();
				byteBufferUsed = 0;
			}
			else
			{
				int maxByteCount = this._encoding.GetMaxByteCount(numberOfChars);
				if (this._leftOverBytes != null)
				{
					if (this._leftOverBytes.Length > maxByteCount)
					{
						array = this._leftOverBytes;
						byteBufferUsed = array.Length;
					}
					else
					{
						array = new byte[maxByteCount];
						Buffer.BlockCopy(this._leftOverBytes, 0, array, 0, this._leftOverBytes.Length);
						byteBufferUsed = this._leftOverBytes.Length;
					}
				}
				else
				{
					array = new byte[maxByteCount];
					byteBufferUsed = 0;
				}
			}
			return array;
		}

		private int DecodeBytesToChars(byte[] inBuffer, int inBufferCount, char[] outBuffer, int outBufferOffset, int outBufferCount)
		{
			int num;
			int num2;
			bool flag;
			this._decoder.Convert(inBuffer, 0, inBufferCount, outBuffer, outBufferOffset, outBufferCount, false, out num, out num2, out flag);
			if (!flag && num < inBufferCount)
			{
				this._leftOverBytes = new byte[inBufferCount - num];
				Buffer.BlockCopy(inBuffer, num, this._leftOverBytes, 0, this._leftOverBytes.Length);
			}
			else
			{
				this._leftOverBytes = null;
			}
			return num2;
		}

		private bool IsClosed
		{
			get
			{
				return this._reader == null;
			}
		}

		private bool HasPeekedChar
		{
			get
			{
				return this._peekedChar >= 0;
			}
		}

		internal static void ValidateReadParameters(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw ADP.ArgumentNull("buffer");
			}
			if (index < 0)
			{
				throw ADP.ArgumentOutOfRange("index");
			}
			if (count < 0)
			{
				throw ADP.ArgumentOutOfRange("count");
			}
			try
			{
				if (checked(index + count) > buffer.Length)
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

		private Encoding _encoding;

		private Decoder _decoder;

		private byte[] _leftOverBytes;

		private int _peekedChar;

		private Task _currentTask;

		private CancellationTokenSource _disposalTokenSource;
	}
}
