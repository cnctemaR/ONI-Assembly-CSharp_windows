using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using STRINGS;
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

	private static string MotdLocalImagePath(int imageVersion)
	{
		return MotdServerClient.MotdLocalImagePath(imageVersion, Localization.GetLocale());
	}

	private static string FallbackMotdLocalImagePath(int imageVersion)
	{
		return MotdServerClient.MotdLocalImagePath(imageVersion, null);
	}

	private static string MotdLocalImagePath(int imageVersion, Localization.Locale locale)
	{
		return "motd_local/" + MotdServerClient.GetLocalePathModifier(locale) + "image_" + imageVersion.ToString();
	}

	private static string GetLocalePathModifier()
	{
		return MotdServerClient.GetLocalePathModifier(Localization.GetLocale());
	}

	private static string GetLocalePathModifier(Localization.Locale locale)
	{
		string text = "";
		if (locale != null)
		{
			Localization.Language lang = locale.Lang;
			if (lang == Localization.Language.Chinese || lang - Localization.Language.Korean <= 1)
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
			MotdServerClient.MotdResponse motdResponse;
			if (err == null)
			{
				global::Debug.Assert(response.image_texture != null, "Attempting to return response with no image texture");
				motdResponse = response;
			}
			else
			{
				global::Debug.LogWarning("Could not retrieve web motd from " + MotdServerClient.MotdServerUrl + ", falling back to local - err: " + err);
				motdResponse = localResponse;
			}
			if (Localization.GetSelectedLanguageType() == Localization.SelectedLanguageType.UGC)
			{
				global::Debug.Log("Language Mod detected, MOTD strings falling back to local file");
				motdResponse.image_header_text = UI.FRONTEND.MOTD.IMAGE_HEADER;
				motdResponse.news_header_text = UI.FRONTEND.MOTD.NEWS_HEADER;
				motdResponse.news_body_text = UI.FRONTEND.MOTD.NEWS_BODY;
				motdResponse.patch_notes_summary = UI.FRONTEND.MOTD.PATCH_NOTES_SUMMARY;
				motdResponse.vanilla_update_data.update_text_override = UI.FRONTEND.MOTD.UPDATE_TEXT;
				motdResponse.expansion1_update_data.update_text_override = UI.FRONTEND.MOTD.UPDATE_TEXT_EXPANSION1;
			}
			this.doCallback(motdResponse, null);
		});
	}

	private MotdServerClient.MotdResponse GetLocalMotd(string filePath)
	{
		TextAsset textAsset = Resources.Load<TextAsset>(filePath.Replace(".json", ""));
		this.m_localMotd = JsonConvert.DeserializeObject<MotdServerClient.MotdResponse>(textAsset.ToString());
		string text = MotdServerClient.MotdLocalImagePath(this.m_localMotd.image_version);
		this.m_localMotd.image_texture = Resources.Load<Texture2D>(text);
		if (this.m_localMotd.image_texture == null)
		{
			string text2 = MotdServerClient.FallbackMotdLocalImagePath(this.m_localMotd.image_version);
			if (text2 != text)
			{
				global::Debug.Log("Could not load " + text + ", falling back to " + text2);
				text = text2;
				this.m_localMotd.image_texture = Resources.Load<Texture2D>(text);
			}
		}
		global::Debug.Assert(this.m_localMotd.image_texture != null, "Failed to load " + text);
		return this.m_localMotd;
	}

	private void GetWebMotd(string url, MotdServerClient.MotdResponse localMotd, Action<MotdServerClient.MotdResponse, string> cb)
	{
		MotdServerClient.<>c__DisplayClass16_0 CS$<>8__locals1 = new MotdServerClient.<>c__DisplayClass16_0();
		CS$<>8__locals1.localMotd = localMotd;
		CS$<>8__locals1.cb = cb;
		Action<string, string> action = delegate(string response, string err)
		{
			MotdServerClient.<>c__DisplayClass16_1 CS$<>8__locals2 = new MotdServerClient.<>c__DisplayClass16_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			DebugUtil.DevAssert(CS$<>8__locals1.localMotd.image_texture != null, "Local MOTD image_texture is no longer loaded", null);
			if (CS$<>8__locals1.localMotd.image_texture == null)
			{
				CS$<>8__locals1.cb(null, "Local image_texture has been unloaded since we requested the MOTD");
				return;
			}
			if (err != null)
			{
				CS$<>8__locals1.cb(null, err);
				return;
			}
			MotdServerClient.<>c__DisplayClass16_1 CS$<>8__locals3 = CS$<>8__locals2;
			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
			jsonSerializerSettings.Error = delegate(object sender, ErrorEventArgs args)
			{
				args.ErrorContext.Handled = true;
			};
			CS$<>8__locals3.responseStruct = JsonConvert.DeserializeObject<MotdServerClient.MotdResponse>(response, jsonSerializerSettings);
			if (CS$<>8__locals2.responseStruct == null)
			{
				CS$<>8__locals1.cb(null, "Invalid json from server:" + response);
				return;
			}
			if (CS$<>8__locals2.responseStruct.version <= CS$<>8__locals1.localMotd.version)
			{
				global::Debug.Log("Using local MOTD at version: " + CS$<>8__locals1.localMotd.version.ToString() + ", web version at " + CS$<>8__locals2.responseStruct.version.ToString());
				CS$<>8__locals1.cb(CS$<>8__locals1.localMotd, null);
				return;
			}
			UnityWebRequest unityWebRequest = new UnityWebRequest();
			unityWebRequest.downloadHandler = new DownloadHandlerTexture();
			SimpleNetworkCache.LoadFromCacheOrDownload("motd_image", CS$<>8__locals2.responseStruct.image_url, CS$<>8__locals2.responseStruct.image_version, unityWebRequest, delegate(UnityWebRequest wr)
			{
				string text = null;
				if (string.IsNullOrEmpty(wr.error))
				{
					global::Debug.Log("Using web MOTD at version: " + CS$<>8__locals2.responseStruct.version.ToString() + ", local version at " + CS$<>8__locals2.CS$<>8__locals1.localMotd.version.ToString());
					CS$<>8__locals2.responseStruct.image_texture = DownloadHandlerTexture.GetContent(wr);
				}
				else
				{
					text = "Failed to load image: " + CS$<>8__locals2.responseStruct.image_url + " SimpleNetworkCache - " + wr.error;
				}
				CS$<>8__locals2.CS$<>8__locals1.cb(CS$<>8__locals2.responseStruct, text);
				wr.Dispose();
			});
		};
		this.getAsyncRequest(url, action);
	}

	private void getAsyncRequest(string url, Action<string, string> cb)
	{
		UnityWebRequest motdRequest = UnityWebRequest.Get(url);
		motdRequest.SetRequestHeader("Content-Type", "application/json");
		motdRequest.SendWebRequest().completed += delegate(AsyncOperation operation)
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
			return;
		}
		global::Debug.Log("Motd Response receieved, but callback was unregistered");
	}

	private Action<MotdServerClient.MotdResponse, string> m_callback;

	private MotdServerClient.MotdResponse m_localMotd;

	public class MotdUpdateData
	{
		public string last_update_time { get; set; }

		public string next_update_time { get; set; }

		public string update_text_override { get; set; }
	}

	public class MotdResponse
	{
		public int version { get; set; }

		public string image_header_text { get; set; }

		public int image_version { get; set; }

		public string image_url { get; set; }

		public string image_link_url { get; set; }

		public string image_rail_link_url { get; set; }

		public string news_header_text { get; set; }

		public string news_body_text { get; set; }

		public string patch_notes_summary { get; set; }

		public string patch_notes_link_url { get; set; }

		public string patch_notes_rail_link_url { get; set; }

		public MotdServerClient.MotdUpdateData vanilla_update_data { get; set; }

		public MotdServerClient.MotdUpdateData expansion1_update_data { get; set; }

		public string latest_update_build { get; set; }

		[JsonIgnore]
		public Texture2D image_texture { get; set; }
	}
}
