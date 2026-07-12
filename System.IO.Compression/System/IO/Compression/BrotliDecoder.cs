using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Compression
{
	public struct BrotliDecoder : IDisposable
	{
		internal void InitializeDecoder()
		{
			this._state = Interop.Brotli.BrotliDecoderCreateInstance(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (this._state.IsInvalid)
			{
				throw new IOException("Failed to create BrotliDecoder instance");
			}
		}

		internal void EnsureInitialized()
		{
			this.EnsureNotDisposed();
			if (this._state == null)
			{
				this.InitializeDecoder();
			}
		}

		public void Dispose()
		{
			this._disposed = true;
			SafeBrotliDecoderHandle state = this._state;
			if (state == null)
			{
				return;
			}
			state.Dispose();
		}

		private void EnsureNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("BrotliDecoder", "Can not access a closed Decoder.");
			}
		}

		public unsafe OperationStatus Decompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
		{
			this.EnsureInitialized();
			bytesConsumed = 0;
			bytesWritten = 0;
			if (Interop.Brotli.BrotliDecoderIsFinished(this._state))
			{
				return OperationStatus.Done;
			}
			IntPtr intPtr = (IntPtr)destination.Length;
			IntPtr intPtr2 = (IntPtr)source.Length;
			while ((int)intPtr > 0)
			{
				fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
				{
					byte* ptr = reference;
					fixed (byte* reference2 = MemoryMarshal.GetReference<byte>(destination))
					{
						byte* ptr2 = reference2;
						byte* ptr3 = ptr;
						byte* ptr4 = ptr2;
						IntPtr intPtr3;
						int num = Interop.Brotli.BrotliDecoderDecompressStream(this._state, ref intPtr2, &ptr3, ref intPtr, &ptr4, out intPtr3);
						if (num == 0)
						{
							return OperationStatus.InvalidData;
						}
						bytesConsumed += source.Length - (int)intPtr2;
						bytesWritten += destination.Length - (int)intPtr;
						switch (num)
						{
						case 1:
							return OperationStatus.Done;
						case 3:
							return OperationStatus.DestinationTooSmall;
						}
						source = source.Slice(source.Length - (int)intPtr2);
						destination = destination.Slice(destination.Length - (int)intPtr);
						if (num == 2 && source.Length == 0)
						{
							return OperationStatus.NeedMoreData;
						}
					}
				}
			}
			return OperationStatus.DestinationTooSmall;
		}

		public unsafe static bool TryDecompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
		{
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
			{
				byte* ptr = reference;
				fixed (byte* reference2 = MemoryMarshal.GetReference<byte>(destination))
				{
					byte* ptr2 = reference2;
					IntPtr intPtr = (IntPtr)destination.Length;
					bool flag = Interop.Brotli.BrotliDecoderDecompress((IntPtr)source.Length, ptr, ref intPtr, ptr2);
					bytesWritten = (int)intPtr;
					return flag;
				}
			}
		}

		private SafeBrotliDecoderHandle _state;

		private bool _disposed;
	}
}
