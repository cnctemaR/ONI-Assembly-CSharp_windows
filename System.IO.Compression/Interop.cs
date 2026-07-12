using System;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static class Interop
{
	internal static class Brotli
	{
		[DllImport("__Internal")]
		internal static extern SafeBrotliDecoderHandle BrotliDecoderCreateInstance(IntPtr allocFunc, IntPtr freeFunc, IntPtr opaque);

		[DllImport("__Internal")]
		internal unsafe static extern int BrotliDecoderDecompressStream(SafeBrotliDecoderHandle state, ref IntPtr availableIn, byte** nextIn, ref IntPtr availableOut, byte** nextOut, out IntPtr totalOut);

		[DllImport("__Internal")]
		internal unsafe static extern bool BrotliDecoderDecompress(IntPtr availableInput, byte* inBytes, ref IntPtr availableOutput, byte* outBytes);

		[DllImport("__Internal")]
		internal static extern void BrotliDecoderDestroyInstance(IntPtr state);

		[DllImport("__Internal")]
		internal static extern bool BrotliDecoderIsFinished(SafeBrotliDecoderHandle state);

		[DllImport("__Internal")]
		internal static extern SafeBrotliEncoderHandle BrotliEncoderCreateInstance(IntPtr allocFunc, IntPtr freeFunc, IntPtr opaque);

		[DllImport("__Internal")]
		internal static extern bool BrotliEncoderSetParameter(SafeBrotliEncoderHandle state, BrotliEncoderParameter parameter, uint value);

		[DllImport("__Internal")]
		internal unsafe static extern bool BrotliEncoderCompressStream(SafeBrotliEncoderHandle state, BrotliEncoderOperation op, ref IntPtr availableIn, byte** nextIn, ref IntPtr availableOut, byte** nextOut, out IntPtr totalOut);

		[DllImport("__Internal")]
		internal static extern bool BrotliEncoderHasMoreOutput(SafeBrotliEncoderHandle state);

		[DllImport("__Internal")]
		internal static extern void BrotliEncoderDestroyInstance(IntPtr state);

		[DllImport("__Internal")]
		internal unsafe static extern bool BrotliEncoderCompress(int quality, int window, int v, IntPtr availableInput, byte* inBytes, ref IntPtr availableOutput, byte* outBytes);
	}

	internal static class Libraries
	{
		internal const string CompressionNative = "__Internal";
	}
}
