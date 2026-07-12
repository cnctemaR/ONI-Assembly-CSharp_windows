using System;
using UnityEngine;

public class DistributionPlatform : MonoBehaviour
{
	public static bool Initialized
	{
		get
		{
			return DistributionPlatform.Impl != null && DistributionPlatform.Impl.Initialized;
		}
	}

	public static DistributionPlatform.Implementation Inst
	{
		get
		{
			return DistributionPlatform.Impl;
		}
	}

	public static void Initialize()
	{
		if (DistributionPlatform.sImpl == null)
		{
			DistributionPlatform.sImpl = new GameObject("DistributionPlatform").AddComponent<SteamDistributionPlatform>();
			if (!SteamManager.Initialized)
			{
				global::Debug.LogError("Steam not initialized in time.");
			}
		}
	}

	public static event global::System.Action onExitRequest;

	public static void RequestExit()
	{
		if (DistributionPlatform.onExitRequest != null)
		{
			DistributionPlatform.onExitRequest();
		}
	}

	public static event global::System.Action onDlcAuthenticationFailed;

	public static void TriggerDlcAuthenticationFailed()
	{
		if (DistributionPlatform.onDlcAuthenticationFailed != null)
		{
			DistributionPlatform.onDlcAuthenticationFailed();
		}
	}

	private static DistributionPlatform.Implementation Impl
	{
		get
		{
			return DistributionPlatform.sImpl;
		}
	}

	private static DistributionPlatform.Implementation sImpl;

	public interface Implementation
	{
		bool Initialized { get; }

		string Name { get; }

		string Platform { get; }

		string AccountLoginEndpoint { get; }

		string MetricsClientKey { get; }

		string MetricsUserIDField { get; }

		DistributionPlatform.User LocalUser { get; }

		bool IsArchiveBranch { get; }

		string ApplyWordFilter(string text);

		void GetAuthTicket(DistributionPlatform.AuthTicketHandler callback);

		bool IsDLCPurchased(string dlcID);

		bool IsDLCSubscribed(string dlcID);

		void ToggleDLCSubscription(string dlcID);
	}

	public delegate void AuthTicketHandler(byte[] ticket);

	public abstract class UserId
	{
		public abstract ulong ToInt64();
	}

	public abstract class User
	{
		public abstract DistributionPlatform.UserId Id { get; }

		public abstract string Name { get; }
	}
}
