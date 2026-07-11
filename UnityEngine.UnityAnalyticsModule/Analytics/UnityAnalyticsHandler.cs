using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Analytics
{
	[NativeHeader("Modules/UnityAnalytics/Public/UnityAnalytics.h")]
	[NativeHeader("Modules/UnityAnalytics/CoreStats/UnityConnectClient.h")]
	[NativeHeader("Modules/UnityAnalytics/Public/Events/UserCustomEvent.h")]
	[StructLayout(LayoutKind.Sequential)]
	internal class UnityAnalyticsHandler : IDisposable
	{
		public UnityAnalyticsHandler()
		{
			this.m_Ptr = UnityAnalyticsHandler.Internal_Create(this);
		}

		~UnityAnalyticsHandler()
		{
			this.Destroy();
		}

		private void Destroy()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				UnityAnalyticsHandler.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			this.Destroy();
			GC.SuppressFinalize(this);
		}

		public bool IsInitialized()
		{
			return this.m_Ptr != IntPtr.Zero;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr Internal_Create(UnityAnalyticsHandler u);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Destroy(IntPtr ptr);

		[StaticAccessor("GetUnityConnectClient()", StaticAccessorType.Dot)]
		public static extern bool limitUserTracking
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetUnityConnectClient()", StaticAccessorType.Dot)]
		public static extern bool deviceStatsEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool FlushEvents();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SetUserId(string userId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SetUserGender(Gender gender);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SetUserBirthYear(int birthYear);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult Transaction(string productId, double amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SendCustomEventName(string customEventName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SendCustomEvent(CustomEventData eventData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix, string assemblyInfo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SendEvent(string eventName, object parameters, int ver, string prefix);

		[NonSerialized]
		internal IntPtr m_Ptr;
	}
}
