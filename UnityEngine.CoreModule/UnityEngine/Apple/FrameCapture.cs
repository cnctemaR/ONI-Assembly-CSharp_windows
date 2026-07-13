using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Apple
{
	[NativeConditional("PLATFORM_APPLE")]
	[NativeHeader("Runtime/Export/Apple/FrameCaptureMetalScriptBindings.h")]
	public class FrameCapture
	{
		private FrameCapture()
		{
		}

		[FreeFunction("FrameCaptureMetalScripting::IsDestinationSupported")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsDestinationSupportedImpl(FrameCaptureDestination dest);

		[FreeFunction("FrameCaptureMetalScripting::BeginCapture")]
		private unsafe static void BeginCaptureImpl(FrameCaptureDestination dest, string path)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				FrameCapture.BeginCaptureImpl_Injected(dest, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("FrameCaptureMetalScripting::EndCapture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndCaptureImpl();

		[FreeFunction("FrameCaptureMetalScripting::CaptureNextFrame")]
		private unsafe static void CaptureNextFrameImpl(FrameCaptureDestination dest, string path)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				FrameCapture.CaptureNextFrameImpl_Injected(dest, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public static bool IsDestinationSupported(FrameCaptureDestination dest)
		{
			bool flag = dest != FrameCaptureDestination.DevTools && dest != FrameCaptureDestination.GPUTraceDocument;
			if (flag)
			{
				throw new ArgumentException("dest", "Argument dest has bad value (not one of FrameCaptureDestination enum values)");
			}
			return FrameCapture.IsDestinationSupportedImpl(dest);
		}

		public static void BeginCaptureToXcode()
		{
			bool flag = !FrameCapture.IsDestinationSupported(FrameCaptureDestination.DevTools);
			if (flag)
			{
				throw new InvalidOperationException("Frame Capture with DevTools is not supported.");
			}
			FrameCapture.BeginCaptureImpl(FrameCaptureDestination.DevTools, null);
		}

		public static void BeginCaptureToFile(string path)
		{
			bool flag = !FrameCapture.IsDestinationSupported(FrameCaptureDestination.GPUTraceDocument);
			if (flag)
			{
				throw new InvalidOperationException("Frame Capture to file is not supported.");
			}
			bool flag2 = string.IsNullOrEmpty(path);
			if (flag2)
			{
				throw new ArgumentException("path", "Path must be supplied when capture destination is GPUTraceDocument.");
			}
			bool flag3 = Path.GetExtension(path) != ".gputrace";
			if (flag3)
			{
				throw new ArgumentException("path", "Destination file should have .gputrace extension.");
			}
			FrameCapture.BeginCaptureImpl(FrameCaptureDestination.GPUTraceDocument, new Uri(path).AbsoluteUri);
		}

		public static void EndCapture()
		{
			FrameCapture.EndCaptureImpl();
		}

		public static void CaptureNextFrameToXcode()
		{
			bool flag = !FrameCapture.IsDestinationSupported(FrameCaptureDestination.DevTools);
			if (flag)
			{
				throw new InvalidOperationException("Frame Capture with DevTools is not supported.");
			}
			FrameCapture.CaptureNextFrameImpl(FrameCaptureDestination.DevTools, null);
		}

		public static void CaptureNextFrameToFile(string path)
		{
			bool flag = !FrameCapture.IsDestinationSupported(FrameCaptureDestination.GPUTraceDocument);
			if (flag)
			{
				throw new InvalidOperationException("Frame Capture to file is not supported.");
			}
			bool flag2 = string.IsNullOrEmpty(path);
			if (flag2)
			{
				throw new ArgumentException("path", "Path must be supplied when capture destination is GPUTraceDocument.");
			}
			bool flag3 = Path.GetExtension(path) != ".gputrace";
			if (flag3)
			{
				throw new ArgumentException("path", "Destination file should have .gputrace extension.");
			}
			FrameCapture.CaptureNextFrameImpl(FrameCaptureDestination.GPUTraceDocument, new Uri(path).AbsoluteUri);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginCaptureImpl_Injected(FrameCaptureDestination dest, ref ManagedSpanWrapper path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CaptureNextFrameImpl_Injected(FrameCaptureDestination dest, ref ManagedSpanWrapper path);
	}
}
