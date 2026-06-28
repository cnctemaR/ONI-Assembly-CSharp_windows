using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

public class KleiAccount : ThreadedHttps<KleiAccount>
{
	public KleiAccount()
	{
		this.CLIENT_KEY = "ONI";
		this.LIVE_ENDPOINT = "login.kleientertainment.com" + DistributionPlatform.Inst.AccountLoginEndpoint;
		this.serviceName = "KleiAccount";
		this.ClearAuthTicket();
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
			Debug.Log("[Account] Got login for user " + accountReply.UserID, null);
			KleiAccount.KleiUserID = ((!(accountReply.UserID == string.Empty)) ? accountReply.UserID : null);
			this.gotUserID();
		}
		else
		{
			Debug.Log("[Account] Error logging in: " + text, null);
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

	public void AuthenticateUser(KleiAccount.GetUserIDdelegate cb)
	{
		if (KleiAccount.KleiUserID == null)
		{
			Debug.Log("[Account] Requesting auth ticket from " + DistributionPlatform.Inst.Name, null);
			this.gotUserID = cb;
			byte[] array = this.AuthTicket();
			if (array == null || array.Length == 0)
			{
				if (DistributionPlatform.Initialized)
				{
					DistributionPlatform.Inst.GetAuthTicket(new DistributionPlatform.AuthTicketHandler(this.OnAuthTicketObtained));
				}
			}
			else
			{
				this.OnAuthTicketObtained(array);
			}
		}
		else
		{
			cb();
		}
	}

	public void OnAuthTicketObtained(byte[] ticket)
	{
		if (0 < ticket.Length)
		{
			byte[] array = new byte[ticket.Length];
			Array.Copy(ticket, array, ticket.Length);
			this.SetAuthTicket(array);
			base.Start();
			Dictionary<string, object> dictionary = this.BuildLoginRequest(array);
			this.PostRawData(dictionary);
		}
		else
		{
			this.gotUserID();
		}
	}

	public byte[] AuthTicket()
	{
		return this.authTicket;
	}

	public void SetAuthTicket(byte[] ticket)
	{
		this.authTicket = ticket;
	}

	public void ClearAuthTicket()
	{
		this.authTicket = null;
	}

	private Dictionary<string, object> BuildLoginRequest(byte[] ticket)
	{
		return new Dictionary<string, object>
		{
			{
				"SteamTicket",
				this.EncodeToAsciiHEX(ticket)
			},
			{ "Game", this.CLIENT_KEY },
			{ "NoEmail", true }
		};
	}

	public const string KleiAccountKey = "KleiAccount";

	private const string GameIDFieldName = "Game";

	private const string EmailFieldName = "NoEmail";

	private const string ErrorFieldName = "Error";

	private const string UserIDFieldName = "UserID";

	public static string KleiUserID;

	private KleiAccount.GetUserIDdelegate gotUserID;

	private const string AuthTicketKey = "AUTH_TICKET";

	private byte[] authTicket;

	private const string TicketFieldName = "SteamTicket";

	private struct AccountReply
	{
		public string UserID;

		public string Token;

		public bool Error;

		public string SupplementaryData;
	}

	public delegate void GetUserIDdelegate();
}
