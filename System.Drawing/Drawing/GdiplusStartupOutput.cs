using System;

namespace System.Drawing
{
	internal struct GdiplusStartupOutput
	{
		internal static GdiplusStartupOutput MakeGdiplusStartupOutput()
		{
			GdiplusStartupOutput gdiplusStartupOutput = default(GdiplusStartupOutput);
			gdiplusStartupOutput.NotificationHook = (gdiplusStartupOutput.NotificationUnhook = IntPtr.Zero);
			return gdiplusStartupOutput;
		}

		internal IntPtr NotificationHook;

		internal IntPtr NotificationUnhook;
	}
}
