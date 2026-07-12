using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OpenURLButtons")]
public class OpenURLButtons : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		for (int i = 0; i < this.buttonData.Count; i++)
		{
			OpenURLButtons.URLButtonData data = this.buttonData[i];
			GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, base.gameObject, true);
			string text = Strings.Get(data.stringKey);
			gameObject.GetComponentInChildren<LocText>().SetText(text);
			switch (data.urlType)
			{
			case OpenURLButtons.URLButtonType.url:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					this.OpenURL(data.url);
				};
				break;
			case OpenURLButtons.URLButtonType.platformUrl:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					this.OpenPlatformURL(data.url);
				};
				break;
			case OpenURLButtons.URLButtonType.patchNotes:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					this.OpenPatchNotes();
				};
				break;
			case OpenURLButtons.URLButtonType.feedbackScreen:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					this.OpenFeedbackScreen();
				};
				break;
			}
		}
	}

	public void OpenPatchNotes()
	{
		Util.KInstantiateUI(this.patchNotesScreenPrefab, FrontEndManager.Instance.gameObject, true);
	}

	public void OpenFeedbackScreen()
	{
		Util.KInstantiateUI(this.feedbackScreenPrefab.gameObject, FrontEndManager.Instance.gameObject, true);
	}

	public void OpenURL(string URL)
	{
		App.OpenWebURL(URL);
	}

	public void OpenPlatformURL(string URL)
	{
		if (DistributionPlatform.Inst.Platform == "Steam" && DistributionPlatform.Inst.Initialized)
		{
			DistributionPlatform.Inst.GetAuthTicket(delegate(byte[] ticket)
			{
				string text2 = string.Concat(Array.ConvertAll<byte, string>(ticket, (byte x) => x.ToString("X2")));
				App.OpenWebURL(URL.Replace("{SteamID}", DistributionPlatform.Inst.LocalUser.Id.ToInt64().ToString()).Replace("{SteamTicket}", text2));
			});
			return;
		}
		string text = URL.Replace("{SteamID}", "").Replace("{SteamTicket}", "");
		App.OpenWebURL("https://accounts.klei.com/login?goto={gotoUrl}".Replace("{gotoUrl}", WebUtility.HtmlEncode(text)));
	}

	public GameObject buttonPrefab;

	public List<OpenURLButtons.URLButtonData> buttonData;

	[SerializeField]
	private GameObject patchNotesScreenPrefab;

	[SerializeField]
	private FeedbackScreen feedbackScreenPrefab;

	public enum URLButtonType
	{
		url,
		platformUrl,
		patchNotes,
		feedbackScreen
	}

	[Serializable]
	public class URLButtonData
	{
		public string stringKey;

		public OpenURLButtons.URLButtonType urlType;

		public string url;
	}
}
