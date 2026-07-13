using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/TouchScreenKeyboard/TouchScreenKeyboard.bindings.h")]
	[NativeHeader("Runtime/Input/KeyboardOnScreen.h")]
	[NativeConditional("ENABLE_ONSCREEN_KEYBOARD")]
	public class TouchScreenKeyboard
	{
		[FreeFunction("TouchScreenKeyboard_Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		private void Destroy()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				TouchScreenKeyboard.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		~TouchScreenKeyboard()
		{
			this.Destroy();
		}

		public TouchScreenKeyboard(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder, int characterLimit)
		{
			TouchScreenKeyboard_InternalConstructorHelperArguments touchScreenKeyboard_InternalConstructorHelperArguments = default(TouchScreenKeyboard_InternalConstructorHelperArguments);
			touchScreenKeyboard_InternalConstructorHelperArguments.keyboardType = Convert.ToUInt32(keyboardType);
			touchScreenKeyboard_InternalConstructorHelperArguments.autocorrection = Convert.ToUInt32(autocorrection);
			touchScreenKeyboard_InternalConstructorHelperArguments.multiline = Convert.ToUInt32(multiline);
			touchScreenKeyboard_InternalConstructorHelperArguments.secure = Convert.ToUInt32(secure);
			touchScreenKeyboard_InternalConstructorHelperArguments.alert = Convert.ToUInt32(alert);
			touchScreenKeyboard_InternalConstructorHelperArguments.characterLimit = characterLimit;
			this.m_Ptr = TouchScreenKeyboard.TouchScreenKeyboard_InternalConstructorHelper(ref touchScreenKeyboard_InternalConstructorHelperArguments, text, textPlaceholder);
		}

		[FreeFunction("TouchScreenKeyboard_InternalConstructorHelper")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr TouchScreenKeyboard_InternalConstructorHelper(ref TouchScreenKeyboard_InternalConstructorHelperArguments arguments, string text, string textPlaceholder);

		public static bool isSupported
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				RuntimePlatform runtimePlatform = platform;
				RuntimePlatform runtimePlatform2 = runtimePlatform;
				if (runtimePlatform2 <= RuntimePlatform.MetroPlayerARM)
				{
					if (runtimePlatform2 != RuntimePlatform.IPhonePlayer && runtimePlatform2 != RuntimePlatform.Android && runtimePlatform2 - RuntimePlatform.WebGLPlayer > 3)
					{
						goto IL_0063;
					}
				}
				else if (runtimePlatform2 != RuntimePlatform.PS4)
				{
					switch (runtimePlatform2)
					{
					case RuntimePlatform.tvOS:
					case RuntimePlatform.Switch:
					case RuntimePlatform.Stadia:
					case RuntimePlatform.GameCoreXboxSeries:
					case RuntimePlatform.GameCoreXboxOne:
					case RuntimePlatform.PS5:
						break;
					case RuntimePlatform.Lumin:
					case RuntimePlatform.CloudRendering:
						goto IL_0063;
					default:
						if (runtimePlatform2 != RuntimePlatform.VisionOS)
						{
							goto IL_0063;
						}
						break;
					}
				}
				return true;
				IL_0063:
				return false;
			}
		}

		internal static bool disableInPlaceEditing { get; set; }

		public static bool isInPlaceEditingAllowed
		{
			get
			{
				bool disableInPlaceEditing = TouchScreenKeyboard.disableInPlaceEditing;
				return disableInPlaceEditing && false;
			}
		}

		internal static bool isRequiredToForceOpen
		{
			get
			{
				return TouchScreenKeyboard.IsRequiredToForceOpen();
			}
		}

		[FreeFunction("TouchScreenKeyboard_IsRequiredToForceOpen")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsRequiredToForceOpen();

		public static TouchScreenKeyboard Open(string text, [DefaultValue("TouchScreenKeyboardType.Default")] TouchScreenKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder, [DefaultValue("0")] int characterLimit)
		{
			return new TouchScreenKeyboard(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, characterLimit);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder)
		{
			int num = 0;
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert)
		{
			int num = 0;
			string text2 = "";
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, alert, text2, num);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
		{
			int num = 0;
			string text2 = "";
			bool flag = false;
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, multiline, secure, flag, text2, num);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline)
		{
			int num = 0;
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, multiline, flag2, flag, text2, num);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection)
		{
			int num = 0;
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			return TouchScreenKeyboard.Open(text, keyboardType, autocorrection, flag3, flag2, flag, text2, num);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType)
		{
			int num = 0;
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			return TouchScreenKeyboard.Open(text, keyboardType, flag4, flag3, flag2, flag, text2, num);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text)
		{
			int num = 0;
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			TouchScreenKeyboardType touchScreenKeyboardType = TouchScreenKeyboardType.Default;
			return TouchScreenKeyboard.Open(text, touchScreenKeyboardType, flag4, flag3, flag2, flag, text2, num);
		}

		public extern string text
		{
			[NativeName("GetText")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetText")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool hideInput
		{
			[NativeName("IsInputHidden")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetInputHidden")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool active
		{
			[NativeName("IsActive")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetActive")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("TouchScreenKeyboard_GetDone")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetDone(IntPtr ptr);

		[Obsolete("Property done is deprecated, use status instead")]
		public bool done
		{
			get
			{
				return TouchScreenKeyboard.GetDone(this.m_Ptr);
			}
		}

		[FreeFunction("TouchScreenKeyboard_GetWasCanceled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetWasCanceled(IntPtr ptr);

		[Obsolete("Property wasCanceled is deprecated, use status instead.")]
		public bool wasCanceled
		{
			get
			{
				return TouchScreenKeyboard.GetWasCanceled(this.m_Ptr);
			}
		}

		public extern TouchScreenKeyboard.Status status
		{
			[NativeName("GetKeyboardStatus")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int characterLimit
		{
			[NativeName("GetCharacterLimit")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetCharacterLimit")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool canGetSelection
		{
			[NativeName("CanGetSelection")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool canSetSelection
		{
			[NativeName("CanSetSelection")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public RangeInt selection
		{
			get
			{
				RangeInt rangeInt;
				TouchScreenKeyboard.GetSelection(out rangeInt.start, out rangeInt.length);
				return rangeInt;
			}
			set
			{
				bool flag = value.start < 0 || value.length < 0 || value.start + value.length > this.text.Length;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("selection", "Selection is out of range.");
				}
				TouchScreenKeyboard.SetSelection(value.start, value.length);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSelection(out int start, out int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSelection(int start, int length);

		public extern TouchScreenKeyboardType type
		{
			[NativeName("GetKeyboardType")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public int targetDisplay
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		[NativeConditional("ENABLE_ONSCREEN_KEYBOARD", "RectT<float>()")]
		public static Rect area
		{
			[NativeName("GetRect")]
			get
			{
				Rect rect;
				TouchScreenKeyboard.get_area_Injected(out rect);
				return rect;
			}
		}

		public static extern bool visible
		{
			[NativeName("IsVisible")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_area_Injected(out Rect ret);

		[NonSerialized]
		internal IntPtr m_Ptr;

		public enum Status
		{
			Visible,
			Done,
			Canceled,
			LostFocus
		}

		public class Android
		{
			[Obsolete("TouchScreenKeyboard.Android.closeKeyboardOnOutsideTap is obsolete. Use TouchScreenKeyboard.Android.consumesOutsideTouches instead (UnityUpgradable) -> UnityEngine.TouchScreenKeyboard/Android.consumesOutsideTouches")]
			public static bool closeKeyboardOnOutsideTap
			{
				get
				{
					return TouchScreenKeyboard.Android.consumesOutsideTouches;
				}
				set
				{
					TouchScreenKeyboard.Android.consumesOutsideTouches = value;
				}
			}

			public static bool consumesOutsideTouches
			{
				get
				{
					return TouchScreenKeyboard.Android.TouchScreenKeyboard_GetAndroidKeyboardConsumesOutsideTouches();
				}
				set
				{
					TouchScreenKeyboard.Android.TouchScreenKeyboard_SetAndroidKeyboardConsumesOutsideTouches(value);
				}
			}

			[FreeFunction("TouchScreenKeyboard_SetAndroidKeyboardConsumesOutsideTouches")]
			[NativeConditional("PLATFORM_ANDROID")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void TouchScreenKeyboard_SetAndroidKeyboardConsumesOutsideTouches(bool enable);

			[FreeFunction("TouchScreenKeyboard_GetAndroidKeyboardConsumesOutsideTouches")]
			[NativeConditional("PLATFORM_ANDROID")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool TouchScreenKeyboard_GetAndroidKeyboardConsumesOutsideTouches();
		}
	}
}
