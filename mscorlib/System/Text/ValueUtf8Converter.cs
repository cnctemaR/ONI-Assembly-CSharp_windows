using System;
using System.Buffers;

namespace System.Text
{
	internal ref struct ValueUtf8Converter
	{
		public ValueUtf8Converter(Span<byte> initialBuffer)
		{
			this._arrayToReturnToPool = null;
			this._bytes = initialBuffer;
		}

		public unsafe Span<byte> ConvertAndTerminateString(ReadOnlySpan<char> value)
		{
			int num = Encoding.UTF8.GetMaxByteCount(value.Length) + 1;
			if (this._bytes.Length < num)
			{
				this.Dispose();
				this._arrayToReturnToPool = ArrayPool<byte>.Shared.Rent(num);
				this._bytes = new Span<byte>(this._arrayToReturnToPool);
			}
			int bytes = Encoding.UTF8.GetBytes(value, this._bytes);
			*this._bytes[bytes] = 0;
			return this._bytes.Slice(0, bytes + 1);
		}

		public void Dispose()
		{
			byte[] arrayToReturnToPool = this._arrayToReturnToPool;
			if (arrayToReturnToPool != null)
			{
				this._arrayToReturnToPool = null;
				ArrayPool<byte>.Shared.Return(arrayToReturnToPool, false);
			}
		}

		private byte[] _arrayToReturnToPool;

		private Span<byte> _bytes;
	}
}
