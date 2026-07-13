using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[StaticAccessor("GetScreenManager()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	[NativeHeader("Runtime/Graphics/WindowLayout.h")]
	[NativeHeader("Runtime/Graphics/ScreenManager.h")]
	public sealed class Screen
	{
		public static extern int width
		{
			[NativeMethod(Name = "GetWidth", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int height
		{
			[NativeMethod(Name = "GetHeight", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float dpi
		{
			[NativeName("GetDPI")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RequestOrientation(ScreenOrientation orient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ScreenOrientation GetScreenOrientation();

		public static ScreenOrientation orientation
		{
			get
			{
				return Screen.GetScreenOrientation();
			}
			set
			{
				bool flag = value == ScreenOrientation.Unknown;
				if (flag)
				{
					Debug.Log("ScreenOrientation.Unknown is deprecated. Please use ScreenOrientation.AutoRotation");
					value = ScreenOrientation.AutoRotation;
				}
				Screen.RequestOrientation(value);
			}
		}

		[NativeProperty("ScreenTimeout")]
		public static extern int sleepTimeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("GetIsOrientationEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsOrientationEnabled(EnabledOrientation orient);

		[NativeName("SetIsOrientationEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetOrientationEnabled(EnabledOrientation orient, bool enabled);

		public static bool autorotateToPortrait
		{
			get
			{
				return Screen.IsOrientationEnabled(EnabledOrientation.kAutorotateToPortrait);
			}
			set
			{
				Screen.SetOrientationEnabled(EnabledOrientation.kAutorotateToPortrait, value);
			}
		}

		public static bool autorotateToPortraitUpsideDown
		{
			get
			{
				return Screen.IsOrientationEnabled(EnabledOrientation.kAutorotateToPortraitUpsideDown);
			}
			set
			{
				Screen.SetOrientationEnabled(EnabledOrientation.kAutorotateToPortraitUpsideDown, value);
			}
		}

		public static bool autorotateToLandscapeLeft
		{
			get
			{
				return Screen.IsOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeLeft);
			}
			set
			{
				Screen.SetOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeLeft, value);
			}
		}

		public static bool autorotateToLandscapeRight
		{
			get
			{
				return Screen.IsOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeRight);
			}
			set
			{
				Screen.SetOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeRight, value);
			}
		}

		public static Resolution currentResolution
		{
			get
			{
				Resolution resolution;
				Screen.get_currentResolution_Injected(out resolution);
				return resolution;
			}
		}

		public static extern bool fullScreen
		{
			[NativeName("IsFullscreen")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("RequestSetFullscreenFromScript")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern FullScreenMode fullScreenMode
		{
			[NativeName("GetFullscreenMode")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("RequestSetFullscreenModeFromScript")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Rect safeArea
		{
			get
			{
				Rect rect;
				Screen.get_safeArea_Injected(out rect);
				return rect;
			}
		}

		public static Rect[] cutouts
		{
			[FreeFunction("ScreenScripting::GetCutouts")]
			get
			{
				Rect[] array2;
				try
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Screen.get_cutouts_Injected(out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Rect[] array;
					blittableArrayWrapper.Unmarshal<Rect>(ref array);
					array2 = array;
				}
				return array2;
			}
		}

		[NativeName("RequestResolution")]
		public static void SetResolution(int width, int height, FullScreenMode fullscreenMode, RefreshRate preferredRefreshRate)
		{
			Screen.SetResolution_Injected(width, height, fullscreenMode, ref preferredRefreshRate);
		}

		[Obsolete("SetResolution(int, int, FullScreenMode, int) is obsolete. Use SetResolution(int, int, FullScreenMode, RefreshRate) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetResolution(int width, int height, FullScreenMode fullscreenMode, [DefaultValue("0")] int preferredRefreshRate)
		{
			bool flag = preferredRefreshRate < 0;
			if (flag)
			{
				preferredRefreshRate = 0;
			}
			Screen.SetResolution(width, height, fullscreenMode, new RefreshRate
			{
				numerator = (uint)preferredRefreshRate,
				denominator = 1U
			});
		}

		public static void SetResolution(int width, int height, FullScreenMode fullscreenMode)
		{
			Screen.SetResolution(width, height, fullscreenMode, new RefreshRate
			{
				numerator = 0U,
				denominator = 1U
			});
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("SetResolution(int, int, bool, int) is obsolete. Use SetResolution(int, int, FullScreenMode, RefreshRate) instead.")]
		public static void SetResolution(int width, int height, bool fullscreen, [DefaultValue("0")] int preferredRefreshRate)
		{
			bool flag = preferredRefreshRate < 0;
			if (flag)
			{
				preferredRefreshRate = 0;
			}
			Screen.SetResolution(width, height, fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, new RefreshRate
			{
				numerator = (uint)preferredRefreshRate,
				denominator = 1U
			});
		}

		public static void SetResolution(int width, int height, bool fullscreen)
		{
			Screen.SetResolution(width, height, fullscreen, 0);
		}

		[NativeName("SetRequestedMSAASamples")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMSAASamples(int numSamples);

		[NativeName("GetRequestedMSAASamples")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMSAASamples();

		public static int msaaSamples
		{
			get
			{
				return Screen.GetMSAASamples();
			}
		}

		public static Vector2Int mainWindowPosition
		{
			get
			{
				return Screen.GetMainWindowPosition();
			}
		}

		public static DisplayInfo mainWindowDisplayInfo
		{
			get
			{
				return Screen.GetMainWindowDisplayInfo();
			}
		}

		public static void GetDisplayLayout(List<DisplayInfo> displayLayout)
		{
			bool flag = displayLayout == null;
			if (flag)
			{
				throw new ArgumentNullException();
			}
			Screen.GetDisplayLayoutImpl(displayLayout);
		}

		public static AsyncOperation MoveMainWindowTo(in DisplayInfo display, Vector2Int position)
		{
			return Screen.MoveMainWindowImpl(in display, position);
		}

		[FreeFunction("GetMainWindowPosition")]
		private static Vector2Int GetMainWindowPosition()
		{
			Vector2Int vector2Int;
			Screen.GetMainWindowPosition_Injected(out vector2Int);
			return vector2Int;
		}

		[FreeFunction("GetMainWindowDisplayInfo")]
		private static DisplayInfo GetMainWindowDisplayInfo()
		{
			DisplayInfo displayInfo;
			Screen.GetMainWindowDisplayInfo_Injected(out displayInfo);
			return displayInfo;
		}

		[FreeFunction("GetDisplayLayout")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDisplayLayoutImpl(List<DisplayInfo> displayLayout);

		[FreeFunction("MoveMainWindow")]
		private static AsyncOperation MoveMainWindowImpl(in DisplayInfo display, Vector2Int position)
		{
			IntPtr intPtr = Screen.MoveMainWindowImpl_Injected(in display, ref position);
			return (intPtr == 0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr);
		}

		public static Resolution[] resolutions
		{
			[FreeFunction("ScreenScripting::GetResolutions")]
			get
			{
				Resolution[] array2;
				try
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Screen.get_resolutions_Injected(out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Resolution[] array;
					blittableArrayWrapper.Unmarshal<Resolution>(ref array);
					array2 = array;
				}
				return array2;
			}
		}

		public static extern float brightness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use Cursor.lockState and Cursor.visible instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool lockCursor
		{
			get
			{
				return CursorLockMode.Locked == Cursor.lockState;
			}
			set
			{
				if (value)
				{
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_currentResolution_Injected(out Resolution ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_safeArea_Injected(out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cutouts_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetResolution_Injected(int width, int height, FullScreenMode fullscreenMode, [In] ref RefreshRate preferredRefreshRate);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMainWindowPosition_Injected(out Vector2Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMainWindowDisplayInfo_Injected(out DisplayInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr MoveMainWindowImpl_Injected(in DisplayInfo display, [In] ref Vector2Int position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_resolutions_Injected(out BlittableArrayWrapper ret);
	}
}
