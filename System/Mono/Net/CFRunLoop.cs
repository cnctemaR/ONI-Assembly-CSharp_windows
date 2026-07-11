using System;
using System.Runtime.InteropServices;

namespace Mono.Net
{
	internal class CFRunLoop : CFObject
	{
		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern void CFRunLoopAddSource(IntPtr rl, IntPtr source, IntPtr mode);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern void CFRunLoopRemoveSource(IntPtr rl, IntPtr source, IntPtr mode);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern int CFRunLoopRunInMode(IntPtr mode, double seconds, bool returnAfterSourceHandled);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFRunLoopGetCurrent();

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern void CFRunLoopStop(IntPtr rl);

		public CFRunLoop(IntPtr handle, bool own)
			: base(handle, own)
		{
		}

		public static CFRunLoop CurrentRunLoop
		{
			get
			{
				return new CFRunLoop(CFRunLoop.CFRunLoopGetCurrent(), false);
			}
		}

		public void AddSource(IntPtr source, CFString mode)
		{
			CFRunLoop.CFRunLoopAddSource(base.Handle, source, mode.Handle);
		}

		public void RemoveSource(IntPtr source, CFString mode)
		{
			CFRunLoop.CFRunLoopRemoveSource(base.Handle, source, mode.Handle);
		}

		public int RunInMode(CFString mode, double seconds, bool returnAfterSourceHandled)
		{
			return CFRunLoop.CFRunLoopRunInMode(mode.Handle, seconds, returnAfterSourceHandled);
		}

		public void Stop()
		{
			CFRunLoop.CFRunLoopStop(base.Handle);
		}
	}
}
