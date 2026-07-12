using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
	internal class SNIPacket : IDisposable, IEquatable<SNIPacket>
	{
		public SNIPacket()
		{
		}

		public SNIPacket(int capacity)
		{
			this.Allocate(capacity);
		}

		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				this._description = value;
			}
		}

		public int DataLeft
		{
			get
			{
				return this._length - this._offset;
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this._data == null;
			}
		}

		public void Dispose()
		{
			this.Release();
		}

		public void SetCompletionCallback(SNIAsyncCallback completionCallback)
		{
			this._completionCallback = completionCallback;
		}

		public void InvokeCompletionCallback(uint sniErrorCode)
		{
			this._completionCallback(this, sniErrorCode);
		}

		public void Allocate(int capacity)
		{
			if (this._data != null && this._data.Length < capacity)
			{
				if (this._isBufferFromArrayPool)
				{
					this._arrayPool.Return(this._data, false);
				}
				this._data = null;
			}
			if (this._data == null)
			{
				this._data = this._arrayPool.Rent(capacity);
				this._isBufferFromArrayPool = true;
			}
			this._capacity = capacity;
			this._length = 0;
			this._offset = 0;
		}

		public SNIPacket Clone()
		{
			SNIPacket snipacket = new SNIPacket(this._capacity);
			Buffer.BlockCopy(this._data, 0, snipacket._data, 0, this._capacity);
			snipacket._length = this._length;
			snipacket._description = this._description;
			snipacket._completionCallback = this._completionCallback;
			return snipacket;
		}

		public void GetData(byte[] buffer, ref int dataSize)
		{
			Buffer.BlockCopy(this._data, 0, buffer, 0, this._length);
			dataSize = this._length;
		}

		public void SetData(byte[] data, int length)
		{
			this._data = data;
			this._length = length;
			this._capacity = data.Length;
			this._offset = 0;
			this._isBufferFromArrayPool = false;
		}

		public int TakeData(SNIPacket packet, int size)
		{
			int num = this.TakeData(packet._data, packet._length, size);
			packet._length += num;
			return num;
		}

		public void AppendData(byte[] data, int size)
		{
			Buffer.BlockCopy(data, 0, this._data, this._length, size);
			this._length += size;
		}

		public void AppendPacket(SNIPacket packet)
		{
			Buffer.BlockCopy(packet._data, 0, this._data, this._length, packet._length);
			this._length += packet._length;
		}

		public int TakeData(byte[] buffer, int dataOffset, int size)
		{
			if (this._offset >= this._length)
			{
				return 0;
			}
			if (this._offset + size > this._length)
			{
				size = this._length - this._offset;
			}
			Buffer.BlockCopy(this._data, this._offset, buffer, dataOffset, size);
			this._offset += size;
			return size;
		}

		public void Release()
		{
			if (this._data != null)
			{
				if (this._isBufferFromArrayPool)
				{
					this._arrayPool.Return(this._data, false);
				}
				this._data = null;
				this._capacity = 0;
			}
			this.Reset();
		}

		public void Reset()
		{
			this._length = 0;
			this._offset = 0;
			this._description = null;
			this._completionCallback = null;
		}

		public void ReadFromStreamAsync(Stream stream, SNIAsyncCallback callback)
		{
			bool error = false;
			stream.ReadAsync(this._data, 0, this._capacity, CancellationToken.None).ContinueWith(delegate(Task<int> t)
			{
				AggregateException exception = t.Exception;
				Exception ex = ((exception != null) ? exception.InnerException : null);
				if (ex != null)
				{
					SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, 35U, ex);
					error = true;
				}
				else
				{
					this._length = t.Result;
					if (this._length == 0)
					{
						SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, 0U, 2U, string.Empty);
						error = true;
					}
				}
				if (error)
				{
					this.Release();
				}
				callback(this, error ? 1U : 0U);
			}, CancellationToken.None, TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		public void ReadFromStream(Stream stream)
		{
			this._length = stream.Read(this._data, 0, this._capacity);
		}

		public void WriteToStream(Stream stream)
		{
			stream.Write(this._data, 0, this._length);
		}

		public async void WriteToStreamAsync(Stream stream, SNIAsyncCallback callback, SNIProviders provider, bool disposeAfterWriteAsync = false)
		{
			uint status = 0U;
			try
			{
				await stream.WriteAsync(this._data, 0, this._length, CancellationToken.None).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				SNILoadHandle.SingletonInstance.LastError = new SNIError(provider, 35U, ex);
				status = 1U;
			}
			callback(this, status);
			if (disposeAfterWriteAsync)
			{
				this.Dispose();
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			SNIPacket snipacket = obj as SNIPacket;
			return snipacket != null && this.Equals(snipacket);
		}

		public bool Equals(SNIPacket packet)
		{
			return packet != null && packet == this;
		}

		private byte[] _data;

		private int _length;

		private int _capacity;

		private int _offset;

		private string _description;

		private SNIAsyncCallback _completionCallback;

		private ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

		private bool _isBufferFromArrayPool;
	}
}
