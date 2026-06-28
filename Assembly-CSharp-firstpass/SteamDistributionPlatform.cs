using System;
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

	private void InitializeLocalUser()
	{
		if (SteamManager.Initialized)
		{
			CSteamID steamID = global::Steamworks.SteamUser.GetSteamID();
			string personaName = SteamFriends.GetPersonaName();
			this.mLocalUser = new SteamDistributionPlatform.SteamUser(steamID, personaName);
		}
	}

	private SteamDistributionPlatform.SteamUser mLocalUser = null;

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
