using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class MotdDataFetchRequest : IDisposable
{
	public void Dispose()
	{
		this.onCompleteFn = null;
	}

	public void Fetch(string url)
	{
		MotdDataFetchRequest.FetchWebMotdJson(url, delegate(MotdData webMotd)
		{
			if (webMotd == null)
			{
				global::Debug.LogWarning("MOTD Error: failed to get web motd json");
				this.<Fetch>g__CompleteWith|4_1(null);
				return;
			}
			MotdDataFetchRequest.FetchWebMotdImagesFor(webMotd, delegate(bool isOk)
			{
				using (List<MotdData_Box>.Enumerator enumerator = webMotd.boxesLive.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.resolvedImage.IsNullOrDestroyed())
						{
							isOk = false;
						}
					}
				}
				if (!isOk)
				{
					global::Debug.LogWarning("MOTD Error: couldn't fetch all web motd images");
					this.<Fetch>g__CompleteWith|4_1(null);
					return;
				}
				MotdDataFetchRequest.WriteCachedMotdImages(webMotd);
				this.<Fetch>g__CompleteWith|4_1(webMotd);
			});
		});
	}

	public void OnComplete(Action<MotdData> callbackFn)
	{
		if (this.isComplete)
		{
			callbackFn(this.data);
			return;
		}
		this.onCompleteFn = (Action<MotdData>)Delegate.Combine(this.onCompleteFn, callbackFn);
	}

	public static void FetchWebMotdJson(string url, Action<MotdData> onCompleteFn)
	{
		UnityWebRequest webRequest = UnityWebRequest.Get(url);
		webRequest.timeout = 3;
		webRequest.SetRequestHeader("Content-Type", "application/json");
		webRequest.SendWebRequest().completed += delegate(AsyncOperation operation)
		{
			if (string.IsNullOrEmpty(webRequest.error))
			{
				onCompleteFn(MotdData.Parse(webRequest.downloadHandler.text));
			}
			else
			{
				global::Debug.LogWarning("MOTD Error: failed to fetch web motd. " + webRequest.error);
				onCompleteFn(null);
			}
			webRequest.Dispose();
		};
	}

	public static void FetchWebMotdImagesFor(MotdData motdData, Action<bool> onCompleteFn)
	{
		using (List<MotdData_Box>.Enumerator enumerator = motdData.boxesLive.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.image == null)
				{
					onCompleteFn(false);
					return;
				}
			}
		}
		int runningTasks = 0;
		int successfullyFetchedImages = 0;
		using (List<MotdData_Box>.Enumerator enumerator = motdData.boxesLive.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MotdData_Box box = enumerator.Current;
				runningTasks++;
				MotdDataFetchRequest.FetchWebMotdImage(box.image, delegate(Texture2D resolvedImage)
				{
					runningTasks--;
					box.resolvedImage = resolvedImage;
					if (box.resolvedImage != null)
					{
						successfullyFetchedImages++;
					}
					if (runningTasks == 0)
					{
						onCompleteFn(successfullyFetchedImages == motdData.boxesLive.Count);
					}
				});
			}
		}
		if (runningTasks == 0)
		{
			onCompleteFn(true);
		}
	}

	public static void FetchWebMotdImage(string url, Action<Texture2D> onCompleteFn)
	{
		Texture2D texture2D = MotdDataFetchRequest.ReadCachedMotdImage(url);
		if (texture2D != null)
		{
			onCompleteFn(texture2D);
			return;
		}
		UnityWebRequest webRequest = UnityWebRequest.Get(url);
		webRequest.timeout = 3;
		webRequest.SendWebRequest().completed += delegate(AsyncOperation operation)
		{
			if (string.IsNullOrEmpty(webRequest.error))
			{
				onCompleteFn(MotdDataFetchRequest.ParseImage(webRequest.downloadHandler.data));
			}
			else
			{
				global::Debug.LogWarning("MOTD Error: failed to fetch web image at " + url + ". " + webRequest.error);
				onCompleteFn(null);
			}
			webRequest.Dispose();
		};
	}

	public static string GetCachePath()
	{
		return Path.Combine(Util.CacheFolder(), "motd");
	}

	public static string GetCachedFilePath(string filePath)
	{
		return Path.Combine(Util.CacheFolder(), "motd", Path.GetFileName(filePath));
	}

	public static void WriteCachedMotdImages(MotdData data)
	{
		if (data == null)
		{
			return;
		}
		try
		{
			if (Directory.Exists(MotdDataFetchRequest.GetCachePath()))
			{
				Directory.Delete(MotdDataFetchRequest.GetCachePath(), true);
			}
			Directory.CreateDirectory(MotdDataFetchRequest.GetCachePath());
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(string.Format("MOTD Error: Failed to clear old image cache --- {0}", ex));
		}
		try
		{
			if (Directory.Exists(MotdDataFetchRequest.GetCachePath()))
			{
				using (List<MotdData_Box>.Enumerator enumerator = data.boxesLive.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MotdData_Box motdData_Box = enumerator.Current;
						if (motdData_Box.image != null && motdData_Box.resolvedImage != null)
						{
							File.WriteAllBytes(MotdDataFetchRequest.GetCachedFilePath(motdData_Box.image), motdData_Box.resolvedImage.EncodeToPNG());
						}
					}
					goto IL_00B3;
				}
			}
			global::Debug.LogWarning("MOTD Error: Failed to write cached motd images, couldn't find a valid cache directory");
			IL_00B3:;
		}
		catch (Exception ex2)
		{
			global::Debug.LogWarning(string.Format("MOTD Error: Failed to write cached motd images --- {0}", ex2));
		}
	}

	public static Texture2D ReadCachedMotdImage(string url)
	{
		string fileName = Path.GetFileName(url);
		string cachedFilePath = MotdDataFetchRequest.GetCachedFilePath(fileName);
		if (!File.Exists(cachedFilePath))
		{
			return null;
		}
		Texture2D texture2D;
		try
		{
			texture2D = MotdDataFetchRequest.ParseImage(File.ReadAllBytes(cachedFilePath));
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(string.Format("MOTD Error: Can't load cached motd image \"{0}\" --- {1}", fileName, ex));
			texture2D = null;
		}
		return texture2D;
	}

	public static string GetLocaleCode()
	{
		Localization.Locale locale = Localization.GetLocale();
		if (locale != null)
		{
			Localization.Language lang = locale.Lang;
			if (lang == Localization.Language.Chinese || lang - Localization.Language.Korean <= 1)
			{
				return locale.Code;
			}
		}
		return null;
	}

	public static Texture2D ParseImage(byte[] buffer)
	{
		if (MotdDataFetchRequest.<ParseImage>g__IsPng|14_0(buffer) || MotdDataFetchRequest.<ParseImage>g__IsJpg|14_1(buffer))
		{
			Texture2D texture2D = new Texture2D(0, 0);
			texture2D.LoadImage(buffer);
			return texture2D;
		}
		if (MotdDataFetchRequest.<ParseImage>g__IsKleiTex|14_2(buffer))
		{
			global::Debug.LogWarning("MOTD Error: Couldn't load image - KTEX isn't supported yet.");
			return null;
		}
		global::Debug.LogWarning("MOTD Error: Couldn't load image - Unsupported image file format.");
		return null;
	}

	public static void GetUrlParams(out string platformCode, out string languageCode)
	{
		platformCode = "default";
		if ((((Localization.GetLocale() == null) ? null : new Localization.Language?(Localization.GetLocale().Lang)) ?? Localization.Language.Japanese) == Localization.Language.Chinese)
		{
			languageCode = "schinese";
			return;
		}
		languageCode = "en";
	}

	public static string BuildUrl()
	{
		string text;
		string text2;
		MotdDataFetchRequest.GetUrlParams(out text, out text2);
		return "https://motd.klei.com/motd.json/?game=oni&platform=" + text + "&lang=" + text2;
	}

	[CompilerGenerated]
	private void <Fetch>g__CompleteWith|4_1(MotdData data)
	{
		this.data = data;
		this.isComplete = true;
		if (this.onCompleteFn != null)
		{
			this.onCompleteFn(data);
		}
	}

	[CompilerGenerated]
	internal static bool <ParseImage>g__IsPng|14_0(byte[] buffer)
	{
		return buffer[0] == 137 && buffer[1] == 80 && buffer[2] == 78 && buffer[3] == 71 && buffer[4] == 13 && buffer[5] == 10 && buffer[6] == 26 && buffer[7] == 10;
	}

	[CompilerGenerated]
	internal static bool <ParseImage>g__IsJpg|14_1(byte[] buffer)
	{
		return buffer[0] == byte.MaxValue && buffer[1] == 216 && buffer[6] == 74 && buffer[7] == 70 && buffer[8] == 73 && buffer[9] == 70;
	}

	[CompilerGenerated]
	internal static bool <ParseImage>g__IsKleiTex|14_2(byte[] buffer)
	{
		return buffer[0] == 75 && buffer[1] == 84 && buffer[2] == 69 && buffer[3] == 88;
	}

	private MotdData data;

	private bool isComplete;

	private Action<MotdData> onCompleteFn;
}
