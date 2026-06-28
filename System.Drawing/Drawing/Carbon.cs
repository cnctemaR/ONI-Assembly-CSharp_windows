using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Drawing
{
	[SuppressUnmanagedCodeSecurity]
	internal static class Carbon
	{
		static Carbon()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (string.Equals(assembly.GetName().Name, "System.Windows.Forms"))
				{
					Type type = assembly.GetType("System.Windows.Forms.XplatUICarbon");
					if (type != null)
					{
						Carbon.hwnd_delegate = (Delegate)type.GetField("HwndDelegate", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
					}
				}
			}
		}

		internal static CarbonContext GetCGContextForView(IntPtr handle)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr intPtr3 = IntPtr.Zero;
			intPtr3 = Carbon.GetControlOwner(handle);
			if (handle == IntPtr.Zero || intPtr3 == IntPtr.Zero)
			{
				intPtr2 = Carbon.GetQDGlobalsThePort();
				Carbon.CreateCGContextForPort(intPtr2, ref intPtr);
				Rect rect = Carbon.CGDisplayBounds(Carbon.CGMainDisplayID());
				return new CarbonContext(intPtr2, intPtr, (int)rect.size.width, (int)rect.size.height);
			}
			QDRect qdrect = default(QDRect);
			Rect rect2 = default(Rect);
			intPtr2 = Carbon.GetWindowPort(intPtr3);
			intPtr = Carbon.GetContext(intPtr2);
			Carbon.GetWindowBounds(intPtr3, 32U, ref qdrect);
			Carbon.HIViewGetBounds(handle, ref rect2);
			Carbon.HIViewConvertRect(ref rect2, handle, IntPtr.Zero);
			if (rect2.size.height < 0f)
			{
				rect2.size.height = 0f;
			}
			if (rect2.size.width < 0f)
			{
				rect2.size.width = 0f;
			}
			Carbon.CGContextTranslateCTM(intPtr, rect2.origin.x, (float)(qdrect.bottom - qdrect.top) - (rect2.origin.y + rect2.size.height));
			Rect rect3 = new Rect(0f, 0f, rect2.size.width, rect2.size.height);
			Carbon.CGContextSaveGState(intPtr);
			Rectangle[] array = (Rectangle[])Carbon.hwnd_delegate.DynamicInvoke(new object[] { handle });
			if (array != null && array.Length > 0)
			{
				int num = array.Length;
				Carbon.CGContextBeginPath(intPtr);
				Carbon.CGContextAddRect(intPtr, rect3);
				for (int i = 0; i < num; i++)
				{
					Carbon.CGContextAddRect(intPtr, new Rect((float)array[i].X, rect2.size.height - (float)array[i].Y - (float)array[i].Height, (float)array[i].Width, (float)array[i].Height));
				}
				Carbon.CGContextClosePath(intPtr);
				Carbon.CGContextEOClip(intPtr);
			}
			else
			{
				Carbon.CGContextBeginPath(intPtr);
				Carbon.CGContextAddRect(intPtr, rect3);
				Carbon.CGContextClosePath(intPtr);
				Carbon.CGContextClip(intPtr);
			}
			return new CarbonContext(intPtr2, intPtr, (int)rect2.size.width, (int)rect2.size.height);
		}

		internal static IntPtr GetContext(IntPtr port)
		{
			IntPtr zero = IntPtr.Zero;
			object obj = Carbon.lockobj;
			lock (obj)
			{
				Carbon.CreateCGContextForPort(port, ref zero);
			}
			return zero;
		}

		internal static void ReleaseContext(IntPtr port, IntPtr context)
		{
			Carbon.CGContextRestoreGState(context);
			object obj = Carbon.lockobj;
			lock (obj)
			{
				Carbon.CFRelease(context);
			}
		}

		[DllImport("libobjc.dylib")]
		public static extern IntPtr objc_getClass(string className);

		[DllImport("libobjc.dylib")]
		public static extern IntPtr objc_msgSend(IntPtr basePtr, IntPtr selector, string argument);

		[DllImport("libobjc.dylib")]
		public static extern IntPtr objc_msgSend(IntPtr basePtr, IntPtr selector);

		[DllImport("libobjc.dylib")]
		public static extern void objc_msgSend_stret(ref Rect arect, IntPtr basePtr, IntPtr selector);

		[DllImport("libobjc.dylib")]
		public static extern IntPtr sel_registerName(string selectorName);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern IntPtr CGMainDisplayID();

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern Rect CGDisplayBounds(IntPtr display);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern int HIViewGetBounds(IntPtr vHnd, ref Rect r);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern int HIViewConvertRect(ref Rect r, IntPtr a, IntPtr b);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern IntPtr GetControlOwner(IntPtr aView);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern int GetWindowBounds(IntPtr wHnd, uint reg, ref QDRect rect);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern IntPtr GetWindowPort(IntPtr hWnd);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern IntPtr GetQDGlobalsThePort();

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CreateCGContextForPort(IntPtr port, ref IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CFRelease(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void QDBeginCGContext(IntPtr port, ref IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void QDEndCGContext(IntPtr port, ref IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern int CGContextClipToRect(IntPtr context, Rect clip);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern int CGContextClipToRects(IntPtr context, Rect[] clip_rects, int count);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextTranslateCTM(IntPtr context, float tx, float ty);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextScaleCTM(IntPtr context, float x, float y);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextFlush(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextSynchronize(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern IntPtr CGPathCreateMutable();

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGPathAddRects(IntPtr path, IntPtr _void, Rect[] rects, int count);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGPathAddRect(IntPtr path, IntPtr _void, Rect rect);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextAddRects(IntPtr context, Rect[] rects, int count);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextAddRect(IntPtr context, Rect rect);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextBeginPath(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextClosePath(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextAddPath(IntPtr context, IntPtr path);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextClip(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextEOClip(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextEOFillPath(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextSaveGState(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextRestoreGState(IntPtr context);

		internal static Hashtable contextReference = new Hashtable();

		internal static object lockobj = new object();

		internal static Delegate hwnd_delegate;
	}
}
