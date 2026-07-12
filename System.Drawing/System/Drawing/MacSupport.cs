using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	internal static class MacSupport
	{
		static MacSupport()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (string.Equals(assembly.GetName().Name, "System.Windows.Forms"))
				{
					Type type = assembly.GetType("System.Windows.Forms.XplatUICarbon");
					if (type != null)
					{
						MacSupport.hwnd_delegate = (Delegate)type.GetTypeInfo().GetField("HwndDelegate", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
					}
				}
			}
		}

		internal static CocoaContext GetCGContextForNSView(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			IntPtr intPtr = MacSupport.objc_msgSend(MacSupport.objc_getClass("NSView"), MacSupport.sel_registerName("focusView"));
			IntPtr intPtr2 = IntPtr.Zero;
			if (intPtr != handle)
			{
				if (!MacSupport.bool_objc_msgSend(handle, MacSupport.sel_registerName("lockFocusIfCanDraw")))
				{
					return null;
				}
				intPtr2 = handle;
			}
			IntPtr intPtr3 = MacSupport.objc_msgSend(MacSupport.objc_msgSend(MacSupport.objc_msgSend(handle, MacSupport.sel_registerName("window")), MacSupport.sel_registerName("graphicsContext")), MacSupport.sel_registerName("graphicsPort"));
			bool flag = MacSupport.bool_objc_msgSend(handle, MacSupport.sel_registerName("isFlipped"));
			MacSupport.CGContextSaveGState(intPtr3);
			Size size;
			if (IntPtr.Size == 4)
			{
				CGRect32 cgrect = default(CGRect32);
				MacSupport.objc_msgSend_stret(ref cgrect, handle, MacSupport.sel_registerName("bounds"));
				if (flag)
				{
					MacSupport.CGContextTranslateCTM32(intPtr3, cgrect.origin.x, cgrect.size.height);
					MacSupport.CGContextScaleCTM32(intPtr3, 1f, -1f);
				}
				size = new Size((int)cgrect.size.width, (int)cgrect.size.height);
			}
			else
			{
				CGRect64 cgrect2 = default(CGRect64);
				MacSupport.objc_msgSend_stret(ref cgrect2, handle, MacSupport.sel_registerName("bounds"));
				if (flag)
				{
					MacSupport.CGContextTranslateCTM64(intPtr3, cgrect2.origin.x, cgrect2.size.height);
					MacSupport.CGContextScaleCTM64(intPtr3, 1.0, -1.0);
				}
				size = new Size((int)cgrect2.size.width, (int)cgrect2.size.height);
			}
			return new CocoaContext(intPtr2, intPtr3, size.Width, size.Height);
		}

		internal static CarbonContext GetCGContextForView(IntPtr handle)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr intPtr3 = IntPtr.Zero;
			if (IntPtr.Size == 8)
			{
				throw new NotSupportedException();
			}
			intPtr3 = MacSupport.GetControlOwner(handle);
			if (handle == IntPtr.Zero || intPtr3 == IntPtr.Zero)
			{
				intPtr2 = MacSupport.GetQDGlobalsThePort();
				MacSupport.CreateCGContextForPort(intPtr2, ref intPtr);
				CGRect32 cgrect = MacSupport.CGDisplayBounds32(MacSupport.CGMainDisplayID());
				return new CarbonContext(intPtr2, intPtr, (int)cgrect.size.width, (int)cgrect.size.height);
			}
			QDRect qdrect = default(QDRect);
			CGRect32 cgrect2 = default(CGRect32);
			intPtr2 = MacSupport.GetWindowPort(intPtr3);
			intPtr = MacSupport.GetContext(intPtr2);
			MacSupport.GetWindowBounds(intPtr3, 32U, ref qdrect);
			MacSupport.HIViewGetBounds(handle, ref cgrect2);
			MacSupport.HIViewConvertRect(ref cgrect2, handle, IntPtr.Zero);
			if (cgrect2.size.height < 0f)
			{
				cgrect2.size.height = 0f;
			}
			if (cgrect2.size.width < 0f)
			{
				cgrect2.size.width = 0f;
			}
			MacSupport.CGContextTranslateCTM32(intPtr, cgrect2.origin.x, (float)(qdrect.bottom - qdrect.top) - (cgrect2.origin.y + cgrect2.size.height));
			CGRect32 cgrect3 = new CGRect32(0f, 0f, cgrect2.size.width, cgrect2.size.height);
			MacSupport.CGContextSaveGState(intPtr);
			Rectangle[] array = (Rectangle[])MacSupport.hwnd_delegate.DynamicInvoke(new object[] { handle });
			if (array != null && array.Length != 0)
			{
				int num = array.Length;
				MacSupport.CGContextBeginPath(intPtr);
				MacSupport.CGContextAddRect32(intPtr, cgrect3);
				for (int i = 0; i < num; i++)
				{
					MacSupport.CGContextAddRect32(intPtr, new CGRect32((float)array[i].X, cgrect2.size.height - (float)array[i].Y - (float)array[i].Height, (float)array[i].Width, (float)array[i].Height));
				}
				MacSupport.CGContextClosePath(intPtr);
				MacSupport.CGContextEOClip(intPtr);
			}
			else
			{
				MacSupport.CGContextBeginPath(intPtr);
				MacSupport.CGContextAddRect32(intPtr, cgrect3);
				MacSupport.CGContextClosePath(intPtr);
				MacSupport.CGContextClip(intPtr);
			}
			return new CarbonContext(intPtr2, intPtr, (int)cgrect2.size.width, (int)cgrect2.size.height);
		}

		internal static IntPtr GetContext(IntPtr port)
		{
			IntPtr zero = IntPtr.Zero;
			object obj = MacSupport.lockobj;
			lock (obj)
			{
				MacSupport.CreateCGContextForPort(port, ref zero);
			}
			return zero;
		}

		internal static void ReleaseContext(IntPtr port, IntPtr context)
		{
			MacSupport.CGContextRestoreGState(context);
			object obj = MacSupport.lockobj;
			lock (obj)
			{
				MacSupport.CFRelease(context);
			}
		}

		[DllImport("libobjc.dylib")]
		public static extern IntPtr objc_getClass(string className);

		[DllImport("libobjc.dylib")]
		public static extern IntPtr objc_msgSend(IntPtr basePtr, IntPtr selector, string argument);

		[DllImport("libobjc.dylib")]
		public static extern IntPtr objc_msgSend(IntPtr basePtr, IntPtr selector);

		[DllImport("libobjc.dylib")]
		public static extern void objc_msgSend_stret(ref CGRect32 arect, IntPtr basePtr, IntPtr selector);

		[DllImport("libobjc.dylib")]
		public static extern void objc_msgSend_stret(ref CGRect64 arect, IntPtr basePtr, IntPtr selector);

		[DllImport("libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend(IntPtr handle, IntPtr selector);

		[DllImport("libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend_IntPtr(IntPtr handle, IntPtr selector, IntPtr argument);

		[DllImport("libobjc.dylib")]
		public static extern IntPtr sel_registerName(string selectorName);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern IntPtr CGMainDisplayID();

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon", EntryPoint = "CGDisplayBounds")]
		internal static extern CGRect32 CGDisplayBounds32(IntPtr display);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern int HIViewGetBounds(IntPtr vHnd, ref CGRect32 r);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern int HIViewConvertRect(ref CGRect32 r, IntPtr a, IntPtr b);

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

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon", EntryPoint = "CGContextTranslateCTM")]
		internal static extern void CGContextTranslateCTM32(IntPtr context, float tx, float ty);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon", EntryPoint = "CGContextScaleCTM")]
		internal static extern void CGContextScaleCTM32(IntPtr context, float x, float y);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon", EntryPoint = "CGContextTranslateCTM")]
		internal static extern void CGContextTranslateCTM64(IntPtr context, double tx, double ty);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon", EntryPoint = "CGContextScaleCTM")]
		internal static extern void CGContextScaleCTM64(IntPtr context, double x, double y);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextFlush(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextSynchronize(IntPtr context);

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern IntPtr CGPathCreateMutable();

		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon", EntryPoint = "CGContextAddRect")]
		internal static extern void CGContextAddRect32(IntPtr context, CGRect32 rect);

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
