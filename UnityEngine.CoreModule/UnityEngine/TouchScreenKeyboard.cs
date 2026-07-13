using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeConditional("ENABLE_ONSCREEN_KEYBOARD")]
	[NativeHeader("Runtime/Export/TouchScreenKeyboard/TouchScreenKeyboard.bindings.h")]
	[NativeHeader("Runtime/Input/KeyboardOnScreen.h")]
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
		private unsafe static IntPtr TouchScreenKeyboard_InternalConstructorHelper(ref TouchScreenKeyboard_InternalConstructorHelperArguments arguments, string text, string textPlaceholder)
		{
			IntPtr intPtr;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = text.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(textPlaceholder, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = textPlaceholder.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				intPtr = TouchScreenKeyboard.TouchScreenKeyboard_InternalConstructorHelper_Injected(ref arguments, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return intPtr;
		}

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
						goto IL_0051;
					}
				}
				else if (runtimePlatform2 <= RuntimePlatform.Switch)
				{
					if (runtimePlatform2 != RuntimePlatform.PS4 && runtimePlatform2 - RuntimePlatform.tvOS > 1)
					{
						goto IL_0051;
					}
				}
				else if (runtimePlatform2 - RuntimePlatform.GameCoreXboxSeries > 2 && runtimePlatform2 - RuntimePlatform.VisionOS > 1)
				{
					goto IL_0051;
				}
				return true;
				IL_0051:
				return false;
			}
		}

		internal static bool disableInPlaceEditing { get; set; }

		public static bool isInPlaceEditingAllowed
		{
			get
			{
				bool disableInPlaceEditing = TouchScreenKeyboard.disableInPlaceEditing;
				return !disableInPlaceEditing && TouchScreenKeyboard.IsInPlaceEditingAllowed();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsInPlaceEditingAllowed();

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

		public unsafe string text
		{
			[NativeName("GetText")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					TouchScreenKeyboard.get_text_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			[NativeName("SetText")]
			set
			{
				try
				{
					IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					TouchScreenKeyboard.set_text_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
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

		public static extern TouchScreenKeyboard.InputFieldAppearance inputFieldAppearance
		{
			[NativeName("GetInputFieldAppearance")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public bool active
		{
			[NativeName("IsActive")]
			get
			{
				IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TouchScreenKeyboard.get_active_Injected(intPtr);
			}
			[NativeName("SetActive")]
			set
			{
				IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TouchScreenKeyboard.set_active_Injected(intPtr, value);
			}
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

		public TouchScreenKeyboard.Status status
		{
			[NativeName("GetKeyboardStatus")]
			get
			{
				IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TouchScreenKeyboard.get_status_Injected(intPtr);
			}
		}

		public int characterLimit
		{
			[NativeName("GetCharacterLimit")]
			get
			{
				IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TouchScreenKeyboard.get_characterLimit_Injected(intPtr);
			}
			[NativeName("SetCharacterLimit")]
			set
			{
				IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TouchScreenKeyboard.set_characterLimit_Injected(intPtr, value);
			}
		}

		public bool canGetSelection
		{
			[NativeName("CanGetSelection")]
			get
			{
				IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TouchScreenKeyboard.get_canGetSelection_Injected(intPtr);
			}
		}

		public bool canSetSelection
		{
			[NativeName("CanSetSelection")]
			get
			{
				IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TouchScreenKeyboard.get_canSetSelection_Injected(intPtr);
			}
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
				bool flag = string.IsNullOrEmpty(this.text);
				if (flag)
				{
					TouchScreenKeyboard.SetSelection(0, 0);
				}
				else
				{
					bool flag2 = value.start < 0 || value.length < 0 || value.start + value.length > this.text.Length;
					if (flag2)
					{
						throw new ArgumentOutOfRangeException("selection", "Selection is out of range.");
					}
					TouchScreenKeyboard.SetSelection(value.start, value.length);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSelection(out int start, out int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSelection(int start, int length);

		public TouchScreenKeyboardType type
		{
			[NativeName("GetKeyboardType")]
			get
			{
				IntPtr intPtr = TouchScreenKeyboard.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TouchScreenKeyboard.get_type_Injected(intPtr);
			}
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
		private static extern IntPtr TouchScreenKeyboard_InternalConstructorHelper_Injected(ref TouchScreenKeyboard_InternalConstructorHelperArguments arguments, ref ManagedSpanWrapper text, ref ManagedSpanWrapper textPlaceholder);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_text_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_text_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_active_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_active_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TouchScreenKeyboard.Status get_status_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_characterLimit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_characterLimit_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canGetSelection_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canSetSelection_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TouchScreenKeyboardType get_type_Injected(IntPtr _unity_self);

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

		public enum InputFieldAppearance
		{
			Customizable,
			AlwaysVisible,
			AlwaysHidden
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

			[Obsolete("consumesOutsideTouches is deprecated and will be removed in a future version where Unity will always process touch input outside of the on-screen keyboard (consumesOutsideTouches = false)")]
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

			[NativeConditional("PLATFORM_ANDROID")]
			[FreeFunction("TouchScreenKeyboard_SetAndroidKeyboardConsumesOutsideTouches")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void TouchScreenKeyboard_SetAndroidKeyboardConsumesOutsideTouches(bool enable);

			[NativeConditional("PLATFORM_ANDROID")]
			[FreeFunction("TouchScreenKeyboard_GetAndroidKeyboardConsumesOutsideTouches")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool TouchScreenKeyboard_GetAndroidKeyboardConsumesOutsideTouches();
		}

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(TouchScreenKeyboard touchScreenKeyboard)
			{
				return touchScreenKeyboard.m_Ptr;
			}
		}
	}
}
