using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Android
{
	[NativeHeader("Modules/AndroidJNI/Public/AndroidInsets.bindings.h")]
	[StaticAccessor("AndroidInsets", StaticAccessorType.DoubleColon)]
	[RequiredByNativeCode]
	internal class AndroidInsets
	{
		internal AndroidInsets()
		{
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private void SetNativeHandle(IntPtr ptr)
		{
			this.m_NativeHandle = ptr;
		}

		private static Rect InternalGetAndroidInsets(IntPtr handle, AndroidInsets.AndroidInsetsType type)
		{
			Rect rect;
			AndroidInsets.InternalGetAndroidInsets_Injected(handle, type, out rect);
			return rect;
		}

		internal Rect GetInsets(AndroidInsets.AndroidInsetsType type)
		{
			bool flag = this.m_NativeHandle == IntPtr.Zero;
			if (flag)
			{
				throw new Exception("You can only query insets from within AndroidApplication.$onInsetsChanged");
			}
			return AndroidInsets.InternalGetAndroidInsets(this.m_NativeHandle, type);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetAndroidInsets_Injected(IntPtr handle, AndroidInsets.AndroidInsetsType type, out Rect ret);

		private IntPtr m_NativeHandle;

		[Flags]
		internal enum AndroidInsetsType
		{
			StatusBars = 1,
			NavigationBars = 2,
			CaptionBar = 4,
			IME = 8,
			SystemGestures = 16,
			MandatorySystemGestures = 32,
			TappableElement = 64,
			DisplayCutout = 128
		}
	}
}
