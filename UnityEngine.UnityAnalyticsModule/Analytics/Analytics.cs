using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Analytics
{
	[NativeHeader("Modules/UnityAnalytics/Public/Events/UserCustomEvent.h")]
	[NativeHeader("Modules/UnityAnalytics/Public/UnityAnalytics.h")]
	[StructLayout(LayoutKind.Sequential)]
	public static class Analytics
	{
		public static bool initializeOnStartup
		{
			get
			{
				return Analytics.IsInitialized() && Analytics.initializeOnStartupInternal;
			}
			set
			{
				if (Analytics.IsInitialized())
				{
					Analytics.initializeOnStartupInternal = value;
				}
			}
		}

		public static AnalyticsResult ResumeInitialization()
		{
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.ResumeInitializationInternal();
			}
			return analyticsResult;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[NativeMethod("ResumeInitialization")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult ResumeInitializationInternal();

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static extern bool initializeOnStartupInternal
		{
			[NativeMethod("GetInitializeOnStartup")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetInitializeOnStartup")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsInitialized();

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static extern bool enabledInternal
		{
			[NativeMethod("GetEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static extern bool playerOptedOutInternal
		{
			[NativeMethod("GetPlayerOptedOut")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static extern bool limitUserTrackingInternal
		{
			[NativeMethod("GetLimitUserTracking")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetLimitUserTracking")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static extern bool deviceStatsEnabledInternal
		{
			[NativeMethod("GetDeviceStatsEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetDeviceStatsEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeMethod("FlushEvents")]
		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool FlushArchivedEvents();

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult Transaction(string productId, double amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService);

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult SendCustomEventName(string customEventName);

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult SendCustomEvent(CustomEventData eventData);

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AnalyticsResult RegisterEventWithLimit(string eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix, string assemblyInfo, bool notifyServer);

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AnalyticsResult RegisterEventsWithLimit(string[] eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix, string assemblyInfo, bool notifyServer);

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AnalyticsResult SendEventWithLimit(string eventName, object parameters, int ver, string prefix);

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool QueueEvent(string eventName, object parameters, int ver, string prefix);

		public static bool playerOptedOut
		{
			get
			{
				return Analytics.IsInitialized() && Analytics.playerOptedOutInternal;
			}
		}

		public static bool limitUserTracking
		{
			get
			{
				return Analytics.IsInitialized() && Analytics.limitUserTrackingInternal;
			}
			set
			{
				if (Analytics.IsInitialized())
				{
					Analytics.limitUserTrackingInternal = value;
				}
			}
		}

		public static bool deviceStatsEnabled
		{
			get
			{
				return Analytics.IsInitialized() && Analytics.deviceStatsEnabledInternal;
			}
			set
			{
				if (Analytics.IsInitialized())
				{
					Analytics.deviceStatsEnabledInternal = value;
				}
			}
		}

		public static bool enabled
		{
			get
			{
				return Analytics.IsInitialized() && Analytics.enabledInternal;
			}
			set
			{
				if (Analytics.IsInitialized())
				{
					Analytics.enabledInternal = value;
				}
			}
		}

		public static AnalyticsResult FlushEvents()
		{
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = ((!Analytics.FlushArchivedEvents()) ? AnalyticsResult.NotInitialized : AnalyticsResult.Ok);
			}
			return analyticsResult;
		}

		public static AnalyticsResult SetUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentException("Cannot set userId to an empty or null string");
			}
			return Analytics.SendUserInfoEvent(new Analytics.UserInfo
			{
				custom_userid = userId
			});
		}

		public static AnalyticsResult SetUserGender(Gender gender)
		{
			return Analytics.SendUserInfoEvent(new Analytics.UserInfo
			{
				sex = ((gender != Gender.Male) ? ((gender != Gender.Female) ? "U" : "F") : "M")
			});
		}

		public static AnalyticsResult SetUserBirthYear(int birthYear)
		{
			return Analytics.SendUserInfoEvent(new Analytics.UserInfoBirthYear
			{
				birth_year = birthYear
			});
		}

		private static AnalyticsResult SendUserInfoEvent(object param)
		{
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				Analytics.QueueEvent("userInfo", param, 1, string.Empty);
				analyticsResult = AnalyticsResult.Ok;
			}
			return analyticsResult;
		}

		public static AnalyticsResult Transaction(string productId, decimal amount, string currency)
		{
			return Analytics.Transaction(productId, amount, currency, null, null, false);
		}

		public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature)
		{
			return Analytics.Transaction(productId, amount, currency, receiptPurchaseData, signature, false);
		}

		public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService)
		{
			if (string.IsNullOrEmpty(productId))
			{
				throw new ArgumentException("Cannot set productId to an empty or null string");
			}
			if (string.IsNullOrEmpty(currency))
			{
				throw new ArgumentException("Cannot set currency to an empty or null string");
			}
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				if (receiptPurchaseData == null)
				{
					receiptPurchaseData = string.Empty;
				}
				if (signature == null)
				{
					signature = string.Empty;
				}
				analyticsResult = Analytics.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature, usingIAPService);
			}
			return analyticsResult;
		}

		public static AnalyticsResult CustomEvent(string customEventName)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.SendCustomEventName(customEventName);
			}
			return analyticsResult;
		}

		public static AnalyticsResult CustomEvent(string customEventName, Vector3 position)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				CustomEventData customEventData = new CustomEventData(customEventName);
				customEventData.AddDouble("x", (double)Convert.ToDecimal(position.x));
				customEventData.AddDouble("y", (double)Convert.ToDecimal(position.y));
				customEventData.AddDouble("z", (double)Convert.ToDecimal(position.z));
				AnalyticsResult analyticsResult2 = Analytics.SendCustomEvent(customEventData);
				customEventData.Dispose();
				analyticsResult = analyticsResult2;
			}
			return analyticsResult;
		}

		public static AnalyticsResult CustomEvent(string customEventName, IDictionary<string, object> eventData)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else if (eventData == null)
			{
				analyticsResult = Analytics.SendCustomEventName(customEventName);
			}
			else
			{
				CustomEventData customEventData = new CustomEventData(customEventName);
				customEventData.AddDictionary(eventData);
				AnalyticsResult analyticsResult2 = Analytics.SendCustomEvent(customEventData);
				customEventData.Dispose();
				analyticsResult = analyticsResult2;
			}
			return analyticsResult;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey = "", string prefix = "")
		{
			string text = string.Empty;
			text = Assembly.GetCallingAssembly().FullName;
			return Analytics.RegisterEvent(eventName, maxEventPerHour, maxItems, vendorKey, 1, prefix, text);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix = "")
		{
			string text = string.Empty;
			text = Assembly.GetCallingAssembly().FullName;
			return Analytics.RegisterEvent(eventName, maxEventPerHour, maxItems, vendorKey, ver, prefix, text);
		}

		private static AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix, string assemblyInfo)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.RegisterEventWithLimit(eventName, maxEventPerHour, maxItems, vendorKey, ver, prefix, assemblyInfo, true);
			}
			return analyticsResult;
		}

		public static AnalyticsResult SendEvent(string eventName, object parameters, int ver = 1, string prefix = "")
		{
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			if (parameters == null)
			{
				throw new ArgumentException("Cannot set parameters to null");
			}
			AnalyticsResult analyticsResult;
			if (!Analytics.IsInitialized())
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.SendEventWithLimit(eventName, parameters, ver, prefix);
			}
			return analyticsResult;
		}

		[Serializable]
		private struct UserInfo
		{
			public string custom_userid;

			public string sex;
		}

		[Serializable]
		private struct UserInfoBirthYear
		{
			public int birth_year;
		}
	}
}
