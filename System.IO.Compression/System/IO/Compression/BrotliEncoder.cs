using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Compression
{
	public struct BrotliEncoder : IDisposable
	{
		public BrotliEncoder(int quality, int window)
		{
			this._disposed = false;
			this._state = Interop.Brotli.BrotliEncoderCreateInstance(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (this._state.IsInvalid)
			{
				throw new IOException("Failed to create BrotliEncoder instance");
			}
			this.SetQuality(quality);
			this.SetWindow(window);
		}

		internal void InitializeEncoder()
		{
			this.EnsureNotDisposed();
			this._state = Interop.Brotli.BrotliEncoderCreateInstance(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (this._state.IsInvalid)
			{
				throw new IOException("Failed to create BrotliEncoder instance");
			}
		}

		internal void EnsureInitialized()
		{
			this.EnsureNotDisposed();
			if (this._state == null)
			{
				this.InitializeEncoder();
			}
		}

		public void Dispose()
		{
			this._disposed = true;
			SafeBrotliEncoderHandle state = this._state;
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
				throw new ObjectDisposedException("BrotliEncoder", "Can not access a closed Encoder.");
			}
		}

		internal void SetQuality(int quality)
		{
			this.EnsureNotDisposed();
			if (this._state == null || this._state.IsInvalid || this._state.IsClosed)
			{
				this.InitializeEncoder();
			}
			if (quality < 0 || quality > 11)
			{
				throw new ArgumentOutOfRangeException("quality", SR.Format("Provided BrotliEncoder Quality of {0} is not between the minimum value of {1} and the maximum value of {2}", quality, 0, 11));
			}
			if (!Interop.Brotli.BrotliEncoderSetParameter(this._state, BrotliEncoderParameter.Quality, (uint)quality))
			{
				throw new InvalidOperationException(SR.Format("The BrotliEncoder {0} can not be changed at current encoder state.", "Quality"));
			}
		}

		internal void SetWindow(int window)
		{
			this.EnsureNotDisposed();
			if (this._state == null || this._state.IsInvalid || this._state.IsClosed)
			{
				this.InitializeEncoder();
			}
			if (window < 10 || window > 24)
			{
				throw new ArgumentOutOfRangeException("window", SR.Format("Provided BrotliEncoder Window of {0} is not between the minimum value of {1} and the maximum value of {2}", window, 10, 24));
			}
			if (!Interop.Brotli.BrotliEncoderSetParameter(this._state, BrotliEncoderParameter.LGWin, (uint)window))
			{
				throw new InvalidOperationException(SR.Format("The BrotliEncoder {0} can not be changed at current encoder state.", "Window"));
			}
		}

		public static int GetMaxCompressedLength(int length)
		{
			if (length < 0 || length > 2147483132)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (length == 0)
			{
				return 1;
			}
			int num = length >> 24;
			int num2 = (((length & 16777215) > 1048576) ? 4 : 3);
			int num3 = 2 + 4 * num + num2 + 1;
			return length + num3;
		}

		internal OperationStatus Flush(Memory<byte> destination, out int bytesWritten)
		{
			return this.Flush(destination.Span, out bytesWritten);
		}

		public OperationStatus Flush(Span<byte> destination, out int bytesWritten)
		{
			int num;
			return this.Compress(ReadOnlySpan<byte>.Empty, destination, out num, out bytesWritten, BrotliEncoderOperation.Flush);
		}

		internal OperationStatus Compress(ReadOnlyMemory<byte> source, Memory<byte> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock)
		{
			return this.Compress(source.Span, destination.Span, out bytesConsumed, out bytesWritten, isFinalBlock);
		}

		public OperationStatus Compress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock)
		{
			return this.Compress(source, destination, out bytesConsumed, out bytesWritten, isFinalBlock ? BrotliEncoderOperation.Finish : BrotliEncoderOperation.Process);
		}

		internal unsafe OperationStatus Compress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten, BrotliEncoderOperation operation)
		{
			this.EnsureInitialized();
			bytesWritten = 0;
			bytesConsumed = 0;
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
						if (!Interop.Brotli.BrotliEncoderCompressStream(this._state, operation, ref intPtr2, &ptr3, ref intPtr, &ptr4, out intPtr3))
						{
							return OperationStatus.InvalidData;
						}
						bytesConsumed += source.Length - (int)intPtr2;
						bytesWritten += destination.Length - (int)intPtr;
						if ((int)intPtr == destination.Length && !Interop.Brotli.BrotliEncoderHasMoreOutput(this._state) && (int)intPtr2 == 0)
						{
							return OperationStatus.Done;
						}
						source = source.Slice(source.Length - (int)intPtr2);
						destination = destination.Slice(destination.Length - (int)intPtr);
					}
				}
			}
			return OperationStatus.DestinationTooSmall;
		}

		public static bool TryCompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
		{
			return BrotliEncoder.TryCompress(source, destination, out bytesWritten, 11, 22);
		}

		public unsafe static bool TryCompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten, int quality, int window)
		{
			if (quality < 0 || quality > 11)
			{
				throw new ArgumentOutOfRangeException("quality", SR.Format("Provided BrotliEncoder Quality of {0} is not between the minimum value of {1} and the maximum value of {2}", quality, 0, 11));
			}
			if (window < 10 || window > 24)
			{
				throw new ArgumentOutOfRangeException("window", SR.Format("Provided BrotliEncoder Window of {0} is not between the minimum value of {1} and the maximum value of {2}", window, 10, 24));
			}
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
			{
				byte* ptr = reference;
				fixed (byte* reference2 = MemoryMarshal.GetReference<byte>(destination))
				{
					byte* ptr2 = reference2;
					IntPtr intPtr = (IntPtr)destination.Length;
					bool flag = Interop.Brotli.BrotliEncoderCompress(quality, window, 0, (IntPtr)source.Length, ptr, ref intPtr, ptr2);
					bytesWritten = (int)intPtr;
					return flag;
				}
			}
		}

		internal SafeBrotliEncoderHandle _state;

		private bool _disposed;
	}
}
