using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnityEngine.Analytics
{
	/// <summary>
	///   <para>Unity Analytics provides insight into your game users e.g. DAU, MAU.</para>
	/// </summary>
	public static class Analytics
	{
		internal static UnityAnalyticsHandler GetUnityAnalyticsHandler()
		{
			if (Analytics.s_UnityAnalyticsHandler == null)
			{
				Analytics.s_UnityAnalyticsHandler = new UnityAnalyticsHandler();
			}
			UnityAnalyticsHandler unityAnalyticsHandler;
			if (Analytics.s_UnityAnalyticsHandler.IsInitialized())
			{
				unityAnalyticsHandler = Analytics.s_UnityAnalyticsHandler;
			}
			else
			{
				unityAnalyticsHandler = null;
			}
			return unityAnalyticsHandler;
		}

		/// <summary>
		///   <para>Controls whether to limit user tracking at runtime.</para>
		/// </summary>
		public static bool limitUserTracking
		{
			get
			{
				return UnityAnalyticsHandler.limitUserTracking;
			}
			set
			{
				UnityAnalyticsHandler.limitUserTracking = value;
			}
		}

		/// <summary>
		///   <para>Controls whether the sending of device stats at runtime is enabled.</para>
		/// </summary>
		public static bool deviceStatsEnabled
		{
			get
			{
				return UnityAnalyticsHandler.deviceStatsEnabled;
			}
			set
			{
				UnityAnalyticsHandler.deviceStatsEnabled = value;
			}
		}

		/// <summary>
		///   <para>Controls whether the Analytics service is enabled at runtime.</para>
		/// </summary>
		public static bool enabled
		{
			get
			{
				UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
				return unityAnalyticsHandler != null && unityAnalyticsHandler.enabled;
			}
			set
			{
				UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
				if (unityAnalyticsHandler != null)
				{
					unityAnalyticsHandler.enabled = value;
				}
			}
		}

		/// <summary>
		///   <para>Attempts to flush immediately all queued analytics events to the network and filesystem cache if possible (optional).</para>
		/// </summary>
		public static AnalyticsResult FlushEvents()
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = ((!unityAnalyticsHandler.FlushEvents()) ? AnalyticsResult.NotInitialized : AnalyticsResult.Ok);
			}
			return analyticsResult;
		}

		/// <summary>
		///   <para>User Demographics (optional).</para>
		/// </summary>
		/// <param name="userId">User id.</param>
		public static AnalyticsResult SetUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentException("Cannot set userId to an empty or null string");
			}
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.SetUserId(userId);
			}
			return analyticsResult;
		}

		/// <summary>
		///   <para>User Demographics (optional).</para>
		/// </summary>
		/// <param name="gender">Gender of user can be "Female", "Male", or "Unknown".</param>
		public static AnalyticsResult SetUserGender(Gender gender)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.SetUserGender(gender);
			}
			return analyticsResult;
		}

		/// <summary>
		///   <para>User Demographics (optional).</para>
		/// </summary>
		/// <param name="birthYear">Birth year of user. Must be 4-digit year format, only.</param>
		public static AnalyticsResult SetUserBirthYear(int birthYear)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (Analytics.s_UnityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.SetUserBirthYear(birthYear);
			}
			return analyticsResult;
		}

		/// <summary>
		///   <para>Tracking Monetization (optional).</para>
		/// </summary>
		/// <param name="productId">The id of the purchased item.</param>
		/// <param name="amount">The price of the item.</param>
		/// <param name="currency">Abbreviation of the currency used for the transaction. For example “USD” (United States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list of currency abbreviations.</param>
		/// <param name="receiptPurchaseData">Receipt data (iOS)  receipt ID (android)  for in-app purchases to verify purchases with Apple iTunes / Google Play. Use null in the absence of receipts.</param>
		/// <param name="signature">Android receipt signature. If using native Android use the INAPP_DATA_SIGNATURE string containing the signature of the purchase data that was signed with the private key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme. Pass in null in absence of a signature.</param>
		/// <param name="usingIAPService">Set to true when using UnityIAP.</param>
		public static AnalyticsResult Transaction(string productId, decimal amount, string currency)
		{
			return Analytics.Transaction(productId, amount, currency, null, null, false);
		}

		/// <summary>
		///   <para>Tracking Monetization (optional).</para>
		/// </summary>
		/// <param name="productId">The id of the purchased item.</param>
		/// <param name="amount">The price of the item.</param>
		/// <param name="currency">Abbreviation of the currency used for the transaction. For example “USD” (United States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list of currency abbreviations.</param>
		/// <param name="receiptPurchaseData">Receipt data (iOS)  receipt ID (android)  for in-app purchases to verify purchases with Apple iTunes / Google Play. Use null in the absence of receipts.</param>
		/// <param name="signature">Android receipt signature. If using native Android use the INAPP_DATA_SIGNATURE string containing the signature of the purchase data that was signed with the private key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme. Pass in null in absence of a signature.</param>
		/// <param name="usingIAPService">Set to true when using UnityIAP.</param>
		public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature)
		{
			return Analytics.Transaction(productId, amount, currency, receiptPurchaseData, signature, false);
		}

		/// <summary>
		///   <para>Tracking Monetization (optional).</para>
		/// </summary>
		/// <param name="productId">The id of the purchased item.</param>
		/// <param name="amount">The price of the item.</param>
		/// <param name="currency">Abbreviation of the currency used for the transaction. For example “USD” (United States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list of currency abbreviations.</param>
		/// <param name="receiptPurchaseData">Receipt data (iOS)  receipt ID (android)  for in-app purchases to verify purchases with Apple iTunes / Google Play. Use null in the absence of receipts.</param>
		/// <param name="signature">Android receipt signature. If using native Android use the INAPP_DATA_SIGNATURE string containing the signature of the purchase data that was signed with the private key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme. Pass in null in absence of a signature.</param>
		/// <param name="usingIAPService">Set to true when using UnityIAP.</param>
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
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
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
				analyticsResult = unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature, usingIAPService);
			}
			return analyticsResult;
		}

		/// <summary>
		///   <para>Custom Events (optional).</para>
		/// </summary>
		/// <param name="customEventName"></param>
		public static AnalyticsResult CustomEvent(string customEventName)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.SendCustomEventName(customEventName);
			}
			return analyticsResult;
		}

		/// <summary>
		///   <para>Custom Events (optional).</para>
		/// </summary>
		/// <param name="customEventName"></param>
		/// <param name="position"></param>
		public static AnalyticsResult CustomEvent(string customEventName, Vector3 position)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				CustomEventData customEventData = new CustomEventData(customEventName);
				customEventData.AddDouble("x", (double)Convert.ToDecimal(position.x));
				customEventData.AddDouble("y", (double)Convert.ToDecimal(position.y));
				customEventData.AddDouble("z", (double)Convert.ToDecimal(position.z));
				analyticsResult = unityAnalyticsHandler.SendCustomEvent(customEventData);
			}
			return analyticsResult;
		}

		public static AnalyticsResult CustomEvent(string customEventName, IDictionary<string, object> eventData)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else if (eventData == null)
			{
				analyticsResult = unityAnalyticsHandler.SendCustomEventName(customEventName);
			}
			else
			{
				CustomEventData customEventData = new CustomEventData(customEventName);
				customEventData.AddDictionary(eventData);
				analyticsResult = unityAnalyticsHandler.SendCustomEvent(customEventData);
			}
			return analyticsResult;
		}

		/// <summary>
		///   <para>This API is used for registering a Runtime Analytics event. It is meant for internal use only and is likely to change in the future. User code should never use this API.</para>
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="maxEventPerHour">Hourly limit for this event name.</param>
		/// <param name="maxItems">Maximum number of items in this event.</param>
		/// <param name="vendorKey">Vendor key name.</param>
		/// <param name="prefix">Optional event name prefix value.</param>
		/// <param name="ver">Event version number.</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey = "", string prefix = "")
		{
			string text = string.Empty;
			text = Assembly.GetCallingAssembly().FullName;
			return Analytics.RegisterEvent(eventName, maxEventPerHour, maxItems, vendorKey, 1, prefix, text);
		}

		/// <summary>
		///   <para>This API is used for registering a Runtime Analytics event. It is meant for internal use only and is likely to change in the future. User code should never use this API.</para>
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="maxEventPerHour">Hourly limit for this event name.</param>
		/// <param name="maxItems">Maximum number of items in this event.</param>
		/// <param name="vendorKey">Vendor key name.</param>
		/// <param name="prefix">Optional event name prefix value.</param>
		/// <param name="ver">Event version number.</param>
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
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.RegisterEvent(eventName, maxEventPerHour, maxItems, vendorKey, ver, prefix, assemblyInfo);
			}
			return analyticsResult;
		}

		/// <summary>
		///   <para>This API is used to send a Runtime Analytics event. It is meant for internal use only and is likely to change in the future. User code should never use this API.</para>
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="ver">Event version number.</param>
		/// <param name="prefix">Optional event name prefix value.</param>
		/// <param name="parameters">Additional event data.</param>
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
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.SendEvent(eventName, parameters, ver, prefix);
			}
			return analyticsResult;
		}

		private static UnityAnalyticsHandler s_UnityAnalyticsHandler;
	}
}
