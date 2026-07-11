using System;
using System.IO;
using Klei;

public class KPrivacyPrefs
{
	public bool disableDataCollection { get; set; }

	public static KPrivacyPrefs instance
	{
		get
		{
			if (KPrivacyPrefs._instance == null)
			{
				KPrivacyPrefs.Load();
			}
			return KPrivacyPrefs._instance;
		}
	}

	public static string GetPath()
	{
		return Path.Combine(KPrivacyPrefs.GetDirectory(), KPrivacyPrefs.FILENAME);
	}

	public static string GetDirectory()
	{
		return Path.Combine(Path.Combine(Util.GetKleiRootPath(), "Agreements"), Util.GetTitleFolderName());
	}

	public static void Save()
	{
		try
		{
			if (!Directory.Exists(KPrivacyPrefs.GetDirectory()))
			{
				Directory.CreateDirectory(KPrivacyPrefs.GetDirectory());
			}
			YamlIO.Save<KPrivacyPrefs>(KPrivacyPrefs.instance, KPrivacyPrefs.GetPath(), null);
		}
		catch (Exception ex)
		{
			KPrivacyPrefs.LogError(ex.ToString());
		}
	}

	public static void Load()
	{
		try
		{
			if (KPrivacyPrefs._instance == null)
			{
				KPrivacyPrefs._instance = new KPrivacyPrefs();
			}
			string path = KPrivacyPrefs.GetPath();
			if (File.Exists(path))
			{
				string text = File.ReadAllText(path);
				KPrivacyPrefs._instance = YamlIO.Parse<KPrivacyPrefs>(text, path, null, null);
				if (KPrivacyPrefs._instance == null)
				{
					KPrivacyPrefs.LogError("Exception while loading privacy prefs:" + path);
					KPrivacyPrefs._instance = new KPrivacyPrefs();
				}
			}
		}
		catch (Exception ex)
		{
			KPrivacyPrefs.LogError(ex.ToString());
		}
	}

	private static void LogError(string msg)
	{
		Debug.LogWarning(msg);
	}

	private static KPrivacyPrefs _instance;

	public static readonly string FILENAME = "kprivacyprefs.yaml";
}
