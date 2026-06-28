using System;
using System.Collections.Generic;

namespace UnityEngine.Analytics
{
	public static class Analytics
	{
		internal static UnityAnalyticsHandler GetUnityAnalyticsHandler()
		{
			if (Analytics.s_UnityAnalyticsHandler == null)
			{
				Analytics.s_UnityAnalyticsHandler = new UnityAnalyticsHandler();
			}
			return Analytics.s_UnityAnalyticsHandler;
		}

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
				analyticsResult = unityAnalyticsHandler.FlushEvents();
			}
			return analyticsResult;
		}

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

		public static AnalyticsResult Transaction(string productId, decimal amount, string currency)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, null, null);
			}
			return analyticsResult;
		}

		public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature);
			}
			return analyticsResult;
		}

		public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			AnalyticsResult analyticsResult;
			if (unityAnalyticsHandler == null)
			{
				analyticsResult = AnalyticsResult.NotInitialized;
			}
			else
			{
				analyticsResult = unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature, usingIAPService);
			}
			return analyticsResult;
		}

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
				analyticsResult = unityAnalyticsHandler.CustomEvent(customEventName);
			}
			return analyticsResult;
		}

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
				customEventData.Add("x", (double)Convert.ToDecimal(position.x));
				customEventData.Add("y", (double)Convert.ToDecimal(position.y));
				customEventData.Add("z", (double)Convert.ToDecimal(position.z));
				analyticsResult = unityAnalyticsHandler.CustomEvent(customEventData);
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
				analyticsResult = unityAnalyticsHandler.CustomEvent(customEventName);
			}
			else
			{
				CustomEventData customEventData = new CustomEventData(customEventName);
				customEventData.Add(eventData);
				analyticsResult = unityAnalyticsHandler.CustomEvent(customEventData);
			}
			return analyticsResult;
		}

		private static UnityAnalyticsHandler s_UnityAnalyticsHandler;
	}
}
