using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Steamworks;

public class KleiAccount : ThreadedHttps<KleiAccount>
{
	public KleiAccount()
	{
		this.CLIENT_KEY = "ONI";
		this.LIVE_ENDPOINT = "login.kleientertainment.com/login/LoginViaSteam";
		this.serviceName = "KleiAccount";
		this.ClearSteamTicket();
	}

	protected override void OnReplyRecieved(WebResponse response)
	{
		if (response == null)
		{
			KleiAccount.KleiUserID = null;
			this.gotUserID();
			return;
		}
		Stream responseStream = response.GetResponseStream();
		StreamReader streamReader = new StreamReader(responseStream);
		string text = streamReader.ReadToEnd();
		streamReader.Close();
		responseStream.Close();
		KleiAccount.AccountReply accountReply = JsonConvert.DeserializeObject<KleiAccount.AccountReply>(text);
		if (!accountReply.Error)
		{
			KleiAccount.KleiUserID = ((!(accountReply.UserID == string.Empty)) ? accountReply.UserID : null);
			this.gotUserID();
		}
		else
		{
			this.gotUserID();
		}
		base.End();
	}

	private string EncodeToAsciiHEX(byte[] data)
	{
		string text = string.Empty;
		for (int i = 0; i < data.Length; i++)
		{
			text += data[i].ToString("X2");
		}
		return text;
	}

	public string PostRawData(Dictionary<string, object> data)
	{
		string text = JsonConvert.SerializeObject(data);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return "OK";
	}

	public void SendSteamTicket(KleiAccount.GetUserIDdelegate cb)
	{
		if (KleiAccount.KleiUserID == null)
		{
			this.gotUserID = cb;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("SteamTicket", this.EncodeToAsciiHEX(this.GetEncryptedTicket()));
			dictionary.Add("Game", this.CLIENT_KEY);
			dictionary.Add("NoEmail", true);
			base.Start();
			this.PostRawData(dictionary);
		}
		else
		{
			cb();
		}
	}

	public byte[] GetEncryptedTicket()
	{
		byte[] array = this.SteamTicket();
		if (array == null || array.Length == 0)
		{
			if (SteamManager.Initialized)
			{
				uint num = 0U;
				SteamUser.GetAuthSessionTicket(this.authSessionTicket, this.authSessionTicket.Length, out num);
				if (num > 0U)
				{
					array = new byte[num];
					Array.Copy(this.authSessionTicket, array, (long)((ulong)num));
					this.SetSteamTicket(array);
				}
			}
		}
		return array;
	}

	public byte[] SteamTicket()
	{
		return this.steamTicket;
	}

	public void SetSteamTicket(byte[] ticket)
	{
		this.steamTicket = ticket;
	}

	public void ClearSteamTicket()
	{
		this.steamTicket = null;
	}

	public const string KleiAccountKey = "KleiAccount";

	private const string SteamTicketFieldName = "SteamTicket";

	private const string GameIDFieldName = "Game";

	private const string EmailFieldName = "NoEmail";

	private const string ErrorFieldName = "Error";

	private const string UserIDFieldName = "UserID";

	private const string SteamTicketKey = "STEAM_TICKET";

	public static string KleiUserID;

	private KleiAccount.GetUserIDdelegate gotUserID;

	private byte[] authSessionTicket = new byte[2048];

	private byte[] steamTicket;

	private struct AccountReply
	{
		public string UserID;

		public string Token;

		public bool Error;

		public string SupplementaryData;
	}

	public delegate void GetUserIDdelegate();
}
