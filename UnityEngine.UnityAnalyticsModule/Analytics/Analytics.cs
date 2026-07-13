using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Analytics
{
	[NativeHeader("Modules/UnityAnalytics/Public/UnityAnalytics.h")]
	[NativeHeader("Modules/UnityAnalyticsCommon/Public/UnityAnalyticsCommon.h")]
	[NativeHeader("Modules/UnityConnect/UnityConnectSettings.h")]
	[NativeHeader("Modules/UnityAnalytics/Public/Events/UserCustomEvent.h")]
	[StructLayout(LayoutKind.Sequential)]
	public static class Analytics
	{
		public static bool initializeOnStartup
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				return !flag && Analytics.initializeOnStartupInternal;
			}
			set
			{
				bool flag = Analytics.IsInitialized();
				if (flag)
				{
					Analytics.initializeOnStartupInternal = value;
				}
			}
		}

		public static AnalyticsResult ResumeInitialization()
		{
			bool flag = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag)
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
		internal static extern bool IsInitialized();

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

		[StaticAccessor("GetUnityConnectSettings()", StaticAccessorType.Dot)]
		private static string eventUrlInternal
		{
			[NativeMethod("GetEventUrl")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Analytics.get_eventUrlInternal_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[StaticAccessor("GetUnityConnectSettings()", StaticAccessorType.Dot)]
		private static string configUrlInternal
		{
			[NativeMethod("GetConfigUrl")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Analytics.get_configUrlInternal_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[StaticAccessor("GetUnityConnectSettings()", StaticAccessorType.Dot)]
		private static string dashboardUrlInternal
		{
			[NativeMethod("GetDashboardUrl")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Analytics.get_dashboardUrlInternal_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
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

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		[NativeMethod("FlushEvents")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool FlushArchivedEvents();

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private unsafe static AnalyticsResult Transaction(string productId, double amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(productId, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = productId.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(currency, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = currency.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper3;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(receiptPurchaseData, ref managedSpanWrapper3))
				{
					ReadOnlySpan<char> readOnlySpan3 = receiptPurchaseData.AsSpan();
					fixed (char* ptr3 = readOnlySpan3.GetPinnableReference())
					{
						managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, readOnlySpan3.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper4;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(signature, ref managedSpanWrapper4))
				{
					ReadOnlySpan<char> readOnlySpan4 = signature.AsSpan();
					fixed (char* ptr4 = readOnlySpan4.GetPinnableReference())
					{
						managedSpanWrapper4 = new ManagedSpanWrapper((void*)ptr4, readOnlySpan4.Length);
					}
				}
				analyticsResult = Analytics.Transaction_Injected(ref managedSpanWrapper, amount, ref managedSpanWrapper2, ref managedSpanWrapper3, ref managedSpanWrapper4, usingIAPService);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				char* ptr3 = null;
				char* ptr4 = null;
			}
			return analyticsResult;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private unsafe static AnalyticsResult SendCustomEventName(string customEventName)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(customEventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = customEventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				analyticsResult = Analytics.SendCustomEventName_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return analyticsResult;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static AnalyticsResult SendCustomEvent(CustomEventData eventData)
		{
			return Analytics.SendCustomEvent_Injected((eventData == null) ? ((IntPtr)0) : CustomEventData.BindingsMarshaller.ConvertToNative(eventData));
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult IsCustomEventWithLimitEnabled(string customEventName)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(customEventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = customEventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				analyticsResult = Analytics.IsCustomEventWithLimitEnabled_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return analyticsResult;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult EnableCustomEventWithLimit(string customEventName, bool enable)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(customEventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = customEventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				analyticsResult = Analytics.EnableCustomEventWithLimit_Injected(ref managedSpanWrapper, enable);
			}
			finally
			{
				char* ptr = null;
			}
			return analyticsResult;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult IsEventWithLimitEnabled(string eventName, int ver, string prefix)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(eventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = eventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = prefix.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				analyticsResult = Analytics.IsEventWithLimitEnabled_Injected(ref managedSpanWrapper, ver, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return analyticsResult;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult EnableEventWithLimit(string eventName, bool enable, int ver, string prefix)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(eventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = eventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = prefix.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				analyticsResult = Analytics.EnableEventWithLimit_Injected(ref managedSpanWrapper, enable, ver, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return analyticsResult;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult RegisterEventWithLimit(string eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix, string assemblyInfo, bool notifyServer)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(eventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = eventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(vendorKey, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = vendorKey.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper3;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper3))
				{
					ReadOnlySpan<char> readOnlySpan3 = prefix.AsSpan();
					fixed (char* ptr3 = readOnlySpan3.GetPinnableReference())
					{
						managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, readOnlySpan3.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper4;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assemblyInfo, ref managedSpanWrapper4))
				{
					ReadOnlySpan<char> readOnlySpan4 = assemblyInfo.AsSpan();
					fixed (char* ptr4 = readOnlySpan4.GetPinnableReference())
					{
						managedSpanWrapper4 = new ManagedSpanWrapper((void*)ptr4, readOnlySpan4.Length);
					}
				}
				analyticsResult = Analytics.RegisterEventWithLimit_Injected(ref managedSpanWrapper, maxEventPerHour, maxItems, ref managedSpanWrapper2, ver, ref managedSpanWrapper3, ref managedSpanWrapper4, notifyServer);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				char* ptr3 = null;
				char* ptr4 = null;
			}
			return analyticsResult;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult RegisterEventsWithLimit(string[] eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix, string assemblyInfo, bool notifyServer)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(vendorKey, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = vendorKey.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = prefix.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper3;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assemblyInfo, ref managedSpanWrapper3))
				{
					ReadOnlySpan<char> readOnlySpan3 = assemblyInfo.AsSpan();
					fixed (char* ptr3 = readOnlySpan3.GetPinnableReference())
					{
						managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, readOnlySpan3.Length);
					}
				}
				analyticsResult = Analytics.RegisterEventsWithLimit_Injected(eventName, maxEventPerHour, maxItems, ref managedSpanWrapper, ver, ref managedSpanWrapper2, ref managedSpanWrapper3, notifyServer);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				char* ptr3 = null;
			}
			return analyticsResult;
		}

		[ThreadSafe]
		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult SendEventWithLimit(string eventName, object parameters, int ver, string prefix)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(eventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = eventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = prefix.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				analyticsResult = Analytics.SendEventWithLimit_Injected(ref managedSpanWrapper, parameters, ver, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return analyticsResult;
		}

		[ThreadSafe]
		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult SetEventWithLimitEndPoint(string eventName, string endPoint, int ver, string prefix)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(eventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = eventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(endPoint, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = endPoint.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper3;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper3))
				{
					ReadOnlySpan<char> readOnlySpan3 = prefix.AsSpan();
					fixed (char* ptr3 = readOnlySpan3.GetPinnableReference())
					{
						managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, readOnlySpan3.Length);
					}
				}
				analyticsResult = Analytics.SetEventWithLimitEndPoint_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ver, ref managedSpanWrapper3);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				char* ptr3 = null;
			}
			return analyticsResult;
		}

		[ThreadSafe]
		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult SetEventWithLimitPriority(string eventName, AnalyticsEventPriority eventPriority, int ver, string prefix)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(eventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = eventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = prefix.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				analyticsResult = Analytics.SetEventWithLimitPriority_Injected(ref managedSpanWrapper, eventPriority, ver, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return analyticsResult;
		}

		[ThreadSafe]
		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		internal unsafe static AnalyticsResult QueueEvent(string eventName, object parameters, int ver, string prefix)
		{
			AnalyticsResult analyticsResult;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(eventName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = eventName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = prefix.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				analyticsResult = Analytics.QueueEvent_Injected(ref managedSpanWrapper, parameters, ver, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return analyticsResult;
		}

		public static bool playerOptedOut
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				return !flag && Analytics.playerOptedOutInternal;
			}
		}

		public static string eventUrl
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string text;
				if (flag)
				{
					text = string.Empty;
				}
				else
				{
					text = Analytics.eventUrlInternal;
				}
				return text;
			}
		}

		public static string dashboardUrl
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string text;
				if (flag)
				{
					text = string.Empty;
				}
				else
				{
					text = Analytics.dashboardUrlInternal;
				}
				return text;
			}
		}

		public static string configUrl
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string text;
				if (flag)
				{
					text = string.Empty;
				}
				else
				{
					text = Analytics.configUrlInternal;
				}
				return text;
			}
		}

		public static bool limitUserTracking
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				return !flag && Analytics.limitUserTrackingInternal;
			}
			set
			{
				bool flag = Analytics.IsInitialized();
				if (flag)
				{
					Analytics.limitUserTrackingInternal = value;
				}
			}
		}

		public static bool deviceStatsEnabled
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				return !flag && Analytics.deviceStatsEnabledInternal;
			}
			set
			{
				bool flag = Analytics.IsInitialized();
				if (flag)
				{
					Analytics.deviceStatsEnabledInternal = value;
				}
			}
		}

		public static bool enabled
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				return !flag && Analytics.enabledInternal;
			}
			set
			{
				bool flag = Analytics.IsInitialized();
				if (flag)
				{
					Analytics.enabledInternal = value;
				}
			}
		}

		public static AnalyticsResult FlushEvents()
		{
			bool flag = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = (Analytics.FlushArchivedEvents() ? AnalyticsResult.Ok : AnalyticsResult.NotInitialized);
			}
			return analyticsResult;
		}

		[Obsolete("SetUserId is no longer supported", true)]
		public static AnalyticsResult SetUserId(string userId)
		{
			bool flag = string.IsNullOrEmpty(userId);
			if (flag)
			{
				throw new ArgumentException("Cannot set userId to an empty or null string");
			}
			return AnalyticsResult.InvalidData;
		}

		[Obsolete("SetUserGender is no longer supported", true)]
		public static AnalyticsResult SetUserGender(Gender gender)
		{
			return AnalyticsResult.InvalidData;
		}

		[Obsolete("SetUserBirthYear is no longer supported", true)]
		public static AnalyticsResult SetUserBirthYear(int birthYear)
		{
			return AnalyticsResult.InvalidData;
		}

		[Obsolete("SendUserInfoEvent is no longer supported", true)]
		private static AnalyticsResult SendUserInfoEvent(object param)
		{
			return AnalyticsResult.InvalidData;
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
			bool flag = string.IsNullOrEmpty(productId);
			if (flag)
			{
				throw new ArgumentException("Cannot set productId to an empty or null string");
			}
			bool flag2 = string.IsNullOrEmpty(currency);
			if (flag2)
			{
				throw new ArgumentException("Cannot set currency to an empty or null string");
			}
			bool flag3 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag3)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				bool flag4 = receiptPurchaseData == null;
				if (flag4)
				{
					receiptPurchaseData = string.Empty;
				}
				bool flag5 = signature == null;
				if (flag5)
				{
					signature = string.Empty;
				}
				analyticsResult = Analytics.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature, usingIAPService);
			}
			return analyticsResult;
		}

		public static AnalyticsResult CustomEvent(string customEventName)
		{
			bool flag = string.IsNullOrEmpty(customEventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
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
			bool flag = string.IsNullOrEmpty(customEventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
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
			bool flag = string.IsNullOrEmpty(customEventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				bool flag3 = eventData == null;
				if (flag3)
				{
					analyticsResult = Analytics.SendCustomEventName(customEventName);
				}
				else
				{
					CustomEventData customEventData = new CustomEventData(customEventName);
					AnalyticsResult analyticsResult2 = AnalyticsResult.InvalidData;
					try
					{
						customEventData.AddDictionary(eventData);
						analyticsResult2 = Analytics.SendCustomEvent(customEventData);
					}
					finally
					{
						customEventData.Dispose();
					}
					analyticsResult = analyticsResult2;
				}
			}
			return analyticsResult;
		}

		public static AnalyticsResult EnableCustomEvent(string customEventName, bool enabled)
		{
			bool flag = string.IsNullOrEmpty(customEventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.EnableCustomEventWithLimit(customEventName, enabled);
			}
			return analyticsResult;
		}

		public static AnalyticsResult IsCustomEventEnabled(string customEventName)
		{
			bool flag = string.IsNullOrEmpty(customEventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.IsCustomEventWithLimitEnabled(customEventName);
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
			bool flag = string.IsNullOrEmpty(eventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
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
			bool flag = string.IsNullOrEmpty(eventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			bool flag2 = parameters == null;
			if (flag2)
			{
				throw new ArgumentException("Cannot set parameters to null");
			}
			bool flag3 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag3)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.SendEventWithLimit(eventName, parameters, ver, prefix);
			}
			return analyticsResult;
		}

		public static AnalyticsResult SetEventEndPoint(string eventName, string endPoint, int ver = 1, string prefix = "")
		{
			bool flag = string.IsNullOrEmpty(eventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			bool flag2 = endPoint == null;
			if (flag2)
			{
				throw new ArgumentException("Cannot set parameters to null");
			}
			bool flag3 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag3)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.SetEventWithLimitEndPoint(eventName, endPoint, ver, prefix);
			}
			return analyticsResult;
		}

		public static AnalyticsResult SetEventPriority(string eventName, AnalyticsEventPriority eventPriority, int ver = 1, string prefix = "")
		{
			bool flag = string.IsNullOrEmpty(eventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.SetEventWithLimitPriority(eventName, eventPriority, ver, prefix);
			}
			return analyticsResult;
		}

		public static AnalyticsResult EnableEvent(string eventName, bool enabled, int ver = 1, string prefix = "")
		{
			bool flag = string.IsNullOrEmpty(eventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.EnableEventWithLimit(eventName, enabled, ver, prefix);
			}
			return analyticsResult;
		}

		public static AnalyticsResult IsEventEnabled(string eventName, int ver = 1, string prefix = "")
		{
			bool flag = string.IsNullOrEmpty(eventName);
			if (flag)
			{
				throw new ArgumentException("Cannot set event name to an empty or null string");
			}
			bool flag2 = !Analytics.IsInitialized();
			AnalyticsResult analyticsResult;
			if (flag2)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = Analytics.IsEventWithLimitEnabled(eventName, ver, prefix);
			}
			return analyticsResult;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_eventUrlInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_configUrlInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_dashboardUrlInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult Transaction_Injected(ref ManagedSpanWrapper productId, double amount, ref ManagedSpanWrapper currency, ref ManagedSpanWrapper receiptPurchaseData, ref ManagedSpanWrapper signature, bool usingIAPService);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult SendCustomEventName_Injected(ref ManagedSpanWrapper customEventName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult SendCustomEvent_Injected(IntPtr eventData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult IsCustomEventWithLimitEnabled_Injected(ref ManagedSpanWrapper customEventName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult EnableCustomEventWithLimit_Injected(ref ManagedSpanWrapper customEventName, bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult IsEventWithLimitEnabled_Injected(ref ManagedSpanWrapper eventName, int ver, ref ManagedSpanWrapper prefix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult EnableEventWithLimit_Injected(ref ManagedSpanWrapper eventName, bool enable, int ver, ref ManagedSpanWrapper prefix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult RegisterEventWithLimit_Injected(ref ManagedSpanWrapper eventName, int maxEventPerHour, int maxItems, ref ManagedSpanWrapper vendorKey, int ver, ref ManagedSpanWrapper prefix, ref ManagedSpanWrapper assemblyInfo, bool notifyServer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult RegisterEventsWithLimit_Injected(string[] eventName, int maxEventPerHour, int maxItems, ref ManagedSpanWrapper vendorKey, int ver, ref ManagedSpanWrapper prefix, ref ManagedSpanWrapper assemblyInfo, bool notifyServer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult SendEventWithLimit_Injected(ref ManagedSpanWrapper eventName, object parameters, int ver, ref ManagedSpanWrapper prefix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult SetEventWithLimitEndPoint_Injected(ref ManagedSpanWrapper eventName, ref ManagedSpanWrapper endPoint, int ver, ref ManagedSpanWrapper prefix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult SetEventWithLimitPriority_Injected(ref ManagedSpanWrapper eventName, AnalyticsEventPriority eventPriority, int ver, ref ManagedSpanWrapper prefix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult QueueEvent_Injected(ref ManagedSpanWrapper eventName, object parameters, int ver, ref ManagedSpanWrapper prefix);
	}
}
