using System;

namespace System.Drawing
{
	internal struct GdiplusStartupInput
	{
		internal static GdiplusStartupInput MakeGdiplusStartupInput()
		{
			return new GdiplusStartupInput
			{
				GdiplusVersion = 1U,
				DebugEventCallback = IntPtr.Zero,
				SuppressBackgroundThread = 0,
				SuppressExternalCodecs = 0
			};
		}

		internal uint GdiplusVersion;

		internal IntPtr DebugEventCallback;

		internal int SuppressBackgroundThread;

		internal int SuppressExternalCodecs;
	}
}
