using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Android
{
	[NativeConditional("PLATFORM_ANDROID")]
	[StaticAccessor("AndroidApp", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/AndroidJNI/Public/AndroidApp.bindings.h")]
	internal static class AndroidApp
	{
		public static AndroidJavaObject Context
		{
			get
			{
				AndroidApp.AcquireContextAndActivity();
				return AndroidApp.m_Context;
			}
		}

		public static AndroidJavaObject Activity
		{
			get
			{
				AndroidApp.AcquireContextAndActivity();
				return AndroidApp.m_Activity;
			}
		}

		private static void AcquireContextAndActivity()
		{
			bool flag = AndroidApp.m_Context != null;
			if (!flag)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidApp.m_Context = androidJavaClass.GetStatic<AndroidJavaObject>("currentContext");
					AndroidApp.m_Activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
			}
		}

		public static extern IntPtr UnityPlayerRaw
		{
			[ThreadSafe]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static AndroidJavaObject UnityPlayer
		{
			get
			{
				bool flag = AndroidApp.m_UnityPlayer != null;
				AndroidJavaObject androidJavaObject;
				if (flag)
				{
					androidJavaObject = AndroidApp.m_UnityPlayer;
				}
				else
				{
					AndroidApp.m_UnityPlayer = new AndroidJavaObject(AndroidApp.UnityPlayerRaw);
					androidJavaObject = AndroidApp.m_UnityPlayer;
				}
				return androidJavaObject;
			}
		}

		private static AndroidJavaObject m_Context;

		private static AndroidJavaObject m_Activity;

		private static AndroidJavaObject m_UnityPlayer;
	}
}
