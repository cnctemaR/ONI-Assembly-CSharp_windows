using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public class MotdServerClient
{
	private static string MotdServerUrl
	{
		get
		{
			return "https://klei-motd.s3.amazonaws.com/oni/" + MotdServerClient.GetLocalePathSuffix();
		}
	}

	private static string MotdLocalPath
	{
		get
		{
			return "motd_local/" + MotdServerClient.GetLocalePathSuffix();
		}
	}

	private static string MotdLocalImagePath
	{
		get
		{
			return "motd_local/" + MotdServerClient.GetLocalePathModifier() + "image";
		}
	}

	private static string GetLocalePathModifier()
	{
		string text = string.Empty;
		Localization.Locale locale = Localization.GetLocale();
		if (locale != null)
		{
			Localization.Language lang = locale.Lang;
			if (lang == Localization.Language.Korean || lang == Localization.Language.Russian || lang == Localization.Language.Chinese)
			{
				text = locale.Code + "/";
			}
		}
		return text;
	}

	private static string GetLocalePathSuffix()
	{
		return MotdServerClient.GetLocalePathModifier() + "motd.json";
	}

	public void GetMotd(Action<MotdServerClient.MotdResponse, string> cb)
	{
		this.m_callback = cb;
		MotdServerClient.MotdResponse localResponse = this.GetLocalMotd(MotdServerClient.MotdLocalPath);
		this.GetWebMotd(MotdServerClient.MotdServerUrl, localResponse, delegate(MotdServerClient.MotdResponse response, string err)
		{
			if (err == null)
			{
				this.doCallback(response, err);
			}
			else
			{
				global::Debug.LogWarning("Could not retrieve web motd from " + MotdServerClient.MotdServerUrl + ", falling back to local - err: " + err);
				this.doCallback(localResponse, null);
			}
		});
	}

	private MotdServerClient.MotdResponse GetLocalMotd(string filePath)
	{
		TextAsset textAsset = Resources.Load<TextAsset>(filePath.Replace(".json", string.Empty));
		MotdServerClient.MotdResponse motdResponse = JsonConvert.DeserializeObject<MotdServerClient.MotdResponse>(textAsset.ToString());
		motdResponse.image_texture = Resources.Load<Texture2D>(MotdServerClient.MotdLocalImagePath);
		return motdResponse;
	}

	private void GetWebMotd(string url, MotdServerClient.MotdResponse localMotd, Action<MotdServerClient.MotdResponse, string> cb)
	{
		MotdServerClient.<GetWebMotd>c__AnonStorey1 <GetWebMotd>c__AnonStorey = new MotdServerClient.<GetWebMotd>c__AnonStorey1();
		<GetWebMotd>c__AnonStorey.cb = cb;
		<GetWebMotd>c__AnonStorey.localMotd = localMotd;
		Action<string, string> action = delegate(string response, string err)
		{
			MotdServerClient.<GetWebMotd>c__AnonStorey1.<GetWebMotd>c__AnonStorey2 <GetWebMotd>c__AnonStorey2 = new MotdServerClient.<GetWebMotd>c__AnonStorey1.<GetWebMotd>c__AnonStorey2();
			<GetWebMotd>c__AnonStorey2.<>f__ref$1 = <GetWebMotd>c__AnonStorey;
			if (err != null)
			{
				<GetWebMotd>c__AnonStorey.cb(null, err);
				return;
			}
			MotdServerClient.<GetWebMotd>c__AnonStorey1.<GetWebMotd>c__AnonStorey2 <GetWebMotd>c__AnonStorey3 = <GetWebMotd>c__AnonStorey2;
			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
			jsonSerializerSettings.Error = delegate(object sender, ErrorEventArgs args)
			{
				args.ErrorContext.Handled = true;
			};
			<GetWebMotd>c__AnonStorey3.responseStruct = JsonConvert.DeserializeObject<MotdServerClient.MotdResponse>(response, jsonSerializerSettings);
			if (<GetWebMotd>c__AnonStorey2.responseStruct == null)
			{
				<GetWebMotd>c__AnonStorey.cb(null, "Invalid json from server:" + response);
			}
			else if (<GetWebMotd>c__AnonStorey2.responseStruct.version <= <GetWebMotd>c__AnonStorey.localMotd.version)
			{
				global::Debug.Log(string.Concat(new object[]
				{
					"Using local MOTD at version: ",
					<GetWebMotd>c__AnonStorey.localMotd.version,
					", web version at ",
					<GetWebMotd>c__AnonStorey2.responseStruct.version
				}));
				<GetWebMotd>c__AnonStorey.cb(<GetWebMotd>c__AnonStorey.localMotd, null);
			}
			else
			{
				UnityWebRequest unityWebRequest = new UnityWebRequest();
				unityWebRequest.downloadHandler = new DownloadHandlerTexture();
				SimpleNetworkCache.LoadFromCacheOrDownload("motd_image", <GetWebMotd>c__AnonStorey2.responseStruct.image_url, <GetWebMotd>c__AnonStorey2.responseStruct.image_version, unityWebRequest, delegate(UnityWebRequest wr)
				{
					string text = null;
					if (string.IsNullOrEmpty(wr.error))
					{
						global::Debug.Log(string.Concat(new object[]
						{
							"Using web MOTD at version: ",
							<GetWebMotd>c__AnonStorey2.responseStruct.version,
							", local version at ",
							<GetWebMotd>c__AnonStorey2.<>f__ref$1.localMotd.version
						}));
						<GetWebMotd>c__AnonStorey2.responseStruct.image_texture = DownloadHandlerTexture.GetContent(wr);
					}
					else
					{
						text = "SimpleNetworkCache - " + wr.error;
					}
					<GetWebMotd>c__AnonStorey2.<>f__ref$1.cb(<GetWebMotd>c__AnonStorey2.responseStruct, text);
					wr.Dispose();
				});
			}
		};
		this.getAsyncRequest(url, action);
	}

	private void getAsyncRequest(string url, Action<string, string> cb)
	{
		UnityWebRequest motdRequest = UnityWebRequest.Get(url);
		motdRequest.SetRequestHeader("Content-Type", "application/json");
		AsyncOperation asyncOperation = motdRequest.SendWebRequest();
		asyncOperation.completed += delegate(AsyncOperation operation)
		{
			cb(motdRequest.downloadHandler.text, motdRequest.error);
			motdRequest.Dispose();
		};
	}

	public void UnregisterCallback()
	{
		this.m_callback = null;
	}

	private void doCallback(MotdServerClient.MotdResponse response, string error)
	{
		if (this.m_callback != null)
		{
			this.m_callback(response, error);
		}
		else
		{
			global::Debug.Log("Motd Response receieved, but callback was unregistered");
		}
	}

	private Action<MotdServerClient.MotdResponse, string> m_callback;

	public class MotdResponse
	{
		public int version { get; set; }

		public string image_header_text { get; set; }

		public int image_version { get; set; }

		public string image_url { get; set; }

		public string image_link_url { get; set; }

		public string news_header_text { get; set; }

		public string news_body_text { get; set; }

		public string patch_notes_summary { get; set; }

		public string patch_notes_link_url { get; set; }

		public string last_update_time { get; set; }

		public string next_update_time { get; set; }

		public string update_text_override { get; set; }

		[JsonIgnore]
		public Texture2D image_texture { get; set; }
	}
}
