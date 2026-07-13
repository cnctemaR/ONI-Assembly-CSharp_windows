using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

internal class SteamDistributionPlatform : MonoBehaviour, DistributionPlatform.Implementation
{
	public bool Initialized
	{
		get
		{
			return SteamManager.Initialized;
		}
	}

	public string Name
	{
		get
		{
			return "Steam";
		}
	}

	public string Platform
	{
		get
		{
			return "Steam";
		}
	}

	public string AccountLoginEndpoint
	{
		get
		{
			return "/login/LoginViaSteam";
		}
	}

	public string MetricsClientKey
	{
		get
		{
			return "2Ehpf6QcWdCXV8eqbbiJBkrqD6xc8waX";
		}
	}

	public string MetricsUserIDField
	{
		get
		{
			return "SteamUserID";
		}
	}

	public DistributionPlatform.User LocalUser
	{
		get
		{
			if (this.mLocalUser == null)
			{
				this.InitializeLocalUser();
			}
			return this.mLocalUser;
		}
	}

	public bool IsArchiveBranch
	{
		get
		{
			string text;
			SteamApps.GetCurrentBetaName(out text, 100);
			global::Debug.Log("Checking which steam branch we're on. Got: [" + text + "]");
			return !(text == "") && !(text == "default") && !(text == "release");
		}
	}

	public bool IsPreviousVersionBranch
	{
		get
		{
			string text;
			SteamApps.GetCurrentBetaName(out text, 100);
			return text == "public_previous_update";
		}
	}

	public bool IsDLCStatusReady()
	{
		return true;
	}

	public string ApplyWordFilter(string text)
	{
		return text;
	}

	public void GetAuthTicket(DistributionPlatform.AuthTicketHandler handler)
	{
		uint num = 0U;
		byte[] array = new byte[2048];
		global::Steamworks.SteamUser.GetAuthSessionTicket(array, array.Length, out num);
		byte[] array2 = new byte[num];
		if (0U < num)
		{
			Array.Copy(array, array2, (long)((ulong)num));
		}
		handler(array2);
	}

	public bool IsDLCPurchased(string dlcID)
	{
		bool purchasedDLC = false;
		if (SteamManager.Initialized)
		{
			uint steamDlcID;
			DebugUtil.AssertArgs(this.DLCtoSteamIDMap.TryGetValue(dlcID, out steamDlcID), new object[] { "DLC does not exist ", dlcID });
			this.GetAuthTicket(delegate(byte[] ticket)
			{
				CSteamID steamID = global::Steamworks.SteamUser.GetSteamID();
				global::Steamworks.SteamUser.BeginAuthSession(ticket, ticket.Length, steamID);
				EUserHasLicenseForAppResult euserHasLicenseForAppResult = global::Steamworks.SteamUser.UserHasLicenseForApp(steamID, new AppId_t(steamDlcID));
				purchasedDLC = euserHasLicenseForAppResult == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense;
				global::Steamworks.SteamUser.EndAuthSession(steamID);
			});
		}
		else if (Application.isEditor)
		{
			purchasedDLC = true;
		}
		return purchasedDLC;
	}

	public bool IsDLCSubscribed(string dlcID)
	{
		uint num;
		if (!this.DLCtoSteamIDMap.TryGetValue(dlcID, out num))
		{
			DebugUtil.LogWarningArgs(new object[] { "Missing dlcID in DLCtoSteamIDMap", dlcID });
			return false;
		}
		if (SteamManager.Initialized)
		{
			return SteamApps.BIsDlcInstalled(new AppId_t(num));
		}
		return Application.isEditor;
	}

	public void ToggleDLCSubscription(string dlcID)
	{
		global::Debug.Log("Steam: Toggling DLC " + dlcID);
		if (this.IsDLCPurchased(dlcID))
		{
			if (this.IsDLCSubscribed(dlcID))
			{
				SteamApps.UninstallDLC(new AppId_t(1452490U));
				global::Debug.Log("Switching to base game");
			}
			else
			{
				SteamApps.InstallDLC(new AppId_t(1452490U));
				global::Debug.Log("Switching to " + dlcID);
			}
			SteamApps.MarkContentCorrupt(false);
			Application.OpenURL("steam://rungameid/" + 457140U.ToString());
			App.Quit();
		}
	}

	private void InitializeLocalUser()
	{
		if (SteamManager.Initialized)
		{
			CSteamID steamID = global::Steamworks.SteamUser.GetSteamID();
			string personaName = SteamFriends.GetPersonaName();
			this.mLocalUser = new SteamDistributionPlatform.SteamUser(steamID, personaName);
		}
	}

	private SteamDistributionPlatform.SteamUser mLocalUser;

	private Dictionary<string, uint> DLCtoSteamIDMap = new Dictionary<string, uint>
	{
		{ "EXPANSION1_ID", 1452490U },
		{ "DLC2_ID", 2952300U },
		{ "DLC3_ID", 3302470U },
		{ "DLC4_ID", 3655420U },
		{ "COSMETIC1_ID", 4157740U }
	};

	public class SteamUserId : DistributionPlatform.UserId
	{
		public SteamUserId(CSteamID id)
		{
			this.mSteamId = id;
		}

		public override string ToString()
		{
			return this.mSteamId.ToString();
		}

		public override ulong ToInt64()
		{
			return this.mSteamId.m_SteamID;
		}

		private CSteamID mSteamId;
	}

	public class SteamUser : DistributionPlatform.User
	{
		public SteamUser(CSteamID id, string name)
		{
			this.mId = new SteamDistributionPlatform.SteamUserId(id);
			this.mName = name;
		}

		public override DistributionPlatform.UserId Id
		{
			get
			{
				return this.mId;
			}
		}

		public override string Name
		{
			get
			{
				return this.mName;
			}
		}

		private SteamDistributionPlatform.SteamUserId mId;

		private string mName;
	}
}
