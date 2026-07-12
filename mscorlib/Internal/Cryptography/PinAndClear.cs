using System;
using System.Runtime.InteropServices;

namespace Internal.Cryptography
{
	internal struct PinAndClear : IDisposable
	{
		internal static PinAndClear Track(byte[] data)
		{
			return new PinAndClear
			{
				_gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned),
				_data = data
			};
		}

		public void Dispose()
		{
			Array.Clear(this._data, 0, this._data.Length);
			this._gcHandle.Free();
		}

		private byte[] _data;

		private GCHandle _gcHandle;
	}
}
