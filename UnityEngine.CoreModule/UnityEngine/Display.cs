using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Provides access to a display / screen for rendering operations.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/DisplayManager.h")]
	public class Display
	{
		internal Display()
		{
			this.nativeDisplay = new IntPtr(0);
		}

		internal Display(IntPtr nativeDisplay)
		{
			this.nativeDisplay = nativeDisplay;
		}

		/// <summary>
		///   <para>Horizontal resolution that the display is rendering at.</para>
		/// </summary>
		public int renderingWidth
		{
			get
			{
				int num = 0;
				int num2 = 0;
				Display.GetRenderingExtImpl(this.nativeDisplay, out num, out num2);
				return num;
			}
		}

		/// <summary>
		///   <para>Vertical resolution that the display is rendering at.</para>
		/// </summary>
		public int renderingHeight
		{
			get
			{
				int num = 0;
				int num2 = 0;
				Display.GetRenderingExtImpl(this.nativeDisplay, out num, out num2);
				return num2;
			}
		}

		/// <summary>
		///   <para>Horizontal native display resolution.</para>
		/// </summary>
		public int systemWidth
		{
			get
			{
				int num = 0;
				int num2 = 0;
				Display.GetSystemExtImpl(this.nativeDisplay, out num, out num2);
				return num;
			}
		}

		/// <summary>
		///   <para>Vertical native display resolution.</para>
		/// </summary>
		public int systemHeight
		{
			get
			{
				int num = 0;
				int num2 = 0;
				Display.GetSystemExtImpl(this.nativeDisplay, out num, out num2);
				return num2;
			}
		}

		/// <summary>
		///   <para>Color RenderBuffer.</para>
		/// </summary>
		public RenderBuffer colorBuffer
		{
			get
			{
				RenderBuffer renderBuffer;
				RenderBuffer renderBuffer2;
				Display.GetRenderingBuffersImpl(this.nativeDisplay, out renderBuffer, out renderBuffer2);
				return renderBuffer;
			}
		}

		/// <summary>
		///   <para>Depth RenderBuffer.</para>
		/// </summary>
		public RenderBuffer depthBuffer
		{
			get
			{
				RenderBuffer renderBuffer;
				RenderBuffer renderBuffer2;
				Display.GetRenderingBuffersImpl(this.nativeDisplay, out renderBuffer, out renderBuffer2);
				return renderBuffer2;
			}
		}

		/// <summary>
		///   <para>Gets the state of the display and returns true if the display is active and false if otherwise.</para>
		/// </summary>
		public bool active
		{
			get
			{
				return Display.GetActiveImp(this.nativeDisplay);
			}
		}

		/// <summary>
		///   <para>Activate an external display. Eg. Secondary Monitors connected to the System.</para>
		/// </summary>
		public void Activate()
		{
			Display.ActivateDisplayImpl(this.nativeDisplay, 0, 0, 60);
		}

		/// <summary>
		///   <para>This overloaded function available for Windows allows specifying desired Window Width, Height and Refresh Rate.</para>
		/// </summary>
		/// <param name="width">Desired Width of the Window (for Windows only. On Linux and Mac uses Screen Width).</param>
		/// <param name="height">Desired Height of the Window (for Windows only. On Linux and Mac uses Screen Height).</param>
		/// <param name="refreshRate">Desired Refresh Rate.</param>
		public void Activate(int width, int height, int refreshRate)
		{
			Display.ActivateDisplayImpl(this.nativeDisplay, width, height, refreshRate);
		}

		/// <summary>
		///   <para>Set rendering size and position on screen (Windows only).</para>
		/// </summary>
		/// <param name="width">Change Window Width (Windows Only).</param>
		/// <param name="height">Change Window Height (Windows Only).</param>
		/// <param name="x">Change Window Position X (Windows Only).</param>
		/// <param name="y">Change Window Position Y (Windows Only).</param>
		public void SetParams(int width, int height, int x, int y)
		{
			Display.SetParamsImpl(this.nativeDisplay, width, height, x, y);
		}

		/// <summary>
		///   <para>Sets rendering resolution for the display.</para>
		/// </summary>
		/// <param name="w">Rendering width in pixels.</param>
		/// <param name="h">Rendering height in pixels.</param>
		public void SetRenderingResolution(int w, int h)
		{
			Display.SetRenderingResolutionImpl(this.nativeDisplay, w, h);
		}

		[Obsolete("MultiDisplayLicense has been deprecated.", false)]
		public static bool MultiDisplayLicense()
		{
			return true;
		}

		/// <summary>
		///   <para>Query relative mouse coordinates.</para>
		/// </summary>
		/// <param name="inputMouseCoordinates">Mouse Input Position as Coordinates.</param>
		public static Vector3 RelativeMouseAt(Vector3 inputMouseCoordinates)
		{
			int num = 0;
			int num2 = 0;
			int num3 = (int)inputMouseCoordinates.x;
			int num4 = (int)inputMouseCoordinates.y;
			Vector3 vector;
			vector.z = (float)Display.RelativeMouseAtImpl(num3, num4, out num, out num2);
			vector.x = (float)num;
			vector.y = (float)num2;
			return vector;
		}

		/// <summary>
		///   <para>Main Display.</para>
		/// </summary>
		public static Display main
		{
			get
			{
				return Display._mainDisplay;
			}
		}

		[RequiredByNativeCode]
		private static void RecreateDisplayList(IntPtr[] nativeDisplay)
		{
			if (nativeDisplay.Length != 0)
			{
				Display.displays = new Display[nativeDisplay.Length];
				for (int i = 0; i < nativeDisplay.Length; i++)
				{
					Display.displays[i] = new Display(nativeDisplay[i]);
				}
				Display._mainDisplay = Display.displays[0];
			}
		}

		[RequiredByNativeCode]
		private static void FireDisplaysUpdated()
		{
			if (Display.onDisplaysUpdated != null)
			{
				Display.onDisplaysUpdated();
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Display.DisplaysUpdatedDelegate onDisplaysUpdated;

		[FreeFunction("UnityDisplayManager_DisplaySystemResolution")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSystemExtImpl(IntPtr nativeDisplay, out int w, out int h);

		[FreeFunction("UnityDisplayManager_DisplayRenderingResolution")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRenderingExtImpl(IntPtr nativeDisplay, out int w, out int h);

		[FreeFunction("UnityDisplayManager_GetRenderingBuffersWrapper")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRenderingBuffersImpl(IntPtr nativeDisplay, out RenderBuffer color, out RenderBuffer depth);

		[FreeFunction("UnityDisplayManager_SetRenderingResolution")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderingResolutionImpl(IntPtr nativeDisplay, int w, int h);

		[FreeFunction("UnityDisplayManager_ActivateDisplay")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ActivateDisplayImpl(IntPtr nativeDisplay, int width, int height, int refreshRate);

		[FreeFunction("UnityDisplayManager_SetDisplayParam")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetParamsImpl(IntPtr nativeDisplay, int width, int height, int x, int y);

		[FreeFunction("UnityDisplayManager_RelativeMouseAt")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RelativeMouseAtImpl(int x, int y, out int rx, out int ry);

		[FreeFunction("UnityDisplayManager_DisplayActive")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetActiveImp(IntPtr nativeDisplay);

		// Note: this type is marked as 'beforefieldinit'.
		static Display()
		{
			Display.onDisplaysUpdated = null;
		}

		internal IntPtr nativeDisplay;

		/// <summary>
		///   <para>The list of currently connected Displays. Contains at least one (main) display.</para>
		/// </summary>
		public static Display[] displays = new Display[]
		{
			new Display()
		};

		private static Display _mainDisplay = Display.displays[0];

		public delegate void DisplaysUpdatedDelegate();
	}
}
