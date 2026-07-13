using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Android
{
	[StaticAccessor("AndroidApplication", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/AndroidJNI/Public/AndroidApplication.bindings.h")]
	public static class AndroidApplication
	{
		internal static extern IntPtr UnityPlayerRaw
		{
			[ThreadSafe]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private static extern IntPtr CurrentContextRaw
		{
			[ThreadSafe]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private static extern IntPtr CurrentActivityRaw
		{
			[ThreadSafe]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private static void AcquireMainThreadSynchronizationContext()
		{
			AndroidApplication.m_MainThreadSynchronizationContext = SynchronizationContext.Current;
			bool flag = AndroidApplication.m_MainThreadSynchronizationContext == null;
			if (flag)
			{
				throw new Exception("Failed to acquire main thread synchronization context");
			}
		}

		public static AndroidJavaObject currentContext
		{
			get
			{
				bool flag = AndroidApplication.m_Context != null;
				AndroidJavaObject androidJavaObject;
				if (flag)
				{
					androidJavaObject = AndroidApplication.m_Context;
				}
				else
				{
					AndroidApplication.m_Context = new AndroidJavaObjectUnityOwned(AndroidApplication.CurrentContextRaw);
					androidJavaObject = AndroidApplication.m_Context;
				}
				return androidJavaObject;
			}
		}

		public static AndroidJavaObject currentActivity
		{
			get
			{
				bool flag = AndroidApplication.m_Activity != null;
				AndroidJavaObject androidJavaObject;
				if (flag)
				{
					androidJavaObject = AndroidApplication.m_Activity;
				}
				else
				{
					AndroidApplication.m_Activity = new AndroidJavaObjectUnityOwned(AndroidApplication.CurrentActivityRaw);
					androidJavaObject = AndroidApplication.m_Activity;
				}
				return androidJavaObject;
			}
		}

		public static AndroidJavaObject unityPlayer
		{
			get
			{
				bool flag = AndroidApplication.m_UnityPlayer != null;
				AndroidJavaObject androidJavaObject;
				if (flag)
				{
					androidJavaObject = AndroidApplication.m_UnityPlayer;
				}
				else
				{
					AndroidApplication.m_UnityPlayer = new AndroidJavaObjectUnityOwned(AndroidApplication.UnityPlayerRaw);
					androidJavaObject = AndroidApplication.m_UnityPlayer;
				}
				return androidJavaObject;
			}
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private static void SetCurrentConfiguration(AndroidConfiguration config)
		{
			AndroidApplication.m_CurrentConfiguration = config;
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private static AndroidConfiguration GetCurrentConfiguration()
		{
			return AndroidApplication.m_CurrentConfiguration;
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private static void DispatchConfigurationChanged(bool notifySubscribers)
		{
			if (notifySubscribers)
			{
				Action<AndroidConfiguration> action = AndroidApplication.onConfigurationChanged;
				if (action != null)
				{
					action(AndroidApplication.m_CurrentConfiguration);
				}
			}
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private static void SetCurrentInsets(AndroidInsets insets)
		{
			AndroidApplication.m_CurrentAndroidInsets = insets;
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private static AndroidInsets GetCurrentInsets()
		{
			return AndroidApplication.m_CurrentAndroidInsets;
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private static void DispatchInsetsChanged()
		{
			Action<AndroidInsets> action = AndroidApplication.onInsetsChanged;
			if (action != null)
			{
				action(AndroidApplication.m_CurrentAndroidInsets);
			}
		}

		public static AndroidConfiguration currentConfiguration
		{
			get
			{
				return AndroidApplication.m_CurrentConfiguration;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<AndroidConfiguration> onConfigurationChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static event Action<AndroidInsets> onInsetsChanged;

		public static void InvokeOnUIThread(Action action)
		{
			AndroidJNI.InvokeAttached(delegate
			{
				AndroidApplication.unityPlayer.Call("runOnUiThread", new object[]
				{
					new AndroidJavaRunnable(action.Invoke)
				});
			});
		}

		public static void InvokeOnUnityMainThread(Action action)
		{
			AndroidApplication.m_MainThreadSynchronizationContext.Send(delegate(object o)
			{
				action();
			}, null);
		}

		private static SynchronizationContext m_MainThreadSynchronizationContext;

		private static AndroidJavaObjectUnityOwned m_Context;

		private static AndroidJavaObjectUnityOwned m_Activity;

		private static AndroidJavaObjectUnityOwned m_UnityPlayer;

		private static AndroidConfiguration m_CurrentConfiguration;

		private static AndroidInsets m_CurrentAndroidInsets;
	}
}
