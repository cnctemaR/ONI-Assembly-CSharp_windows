using System;
using System.Collections.Generic;
using System.IO;
using Klei;

public class KPlayerPrefs
{
	public KPlayerPrefs()
	{
		this.strings = new Dictionary<string, string>();
		this.ints = new Dictionary<string, int>();
		this.floats = new Dictionary<string, float>();
		KPlayerPrefs._instance = this;
	}

	public static KPlayerPrefs instance
	{
		get
		{
			if (KPlayerPrefs._instance == null)
			{
				KPlayerPrefs._instance = new KPlayerPrefs();
				KPlayerPrefs.PATH = KPlayerPrefs.GetPath();
				try
				{
					KPlayerPrefs._instance = YamlIO.LoadFile<KPlayerPrefs>(KPlayerPrefs.PATH, null, null);
				}
				catch
				{
					Debug.LogWarning("Creating new KPlayerPrefs..");
					KPlayerPrefs._instance = new KPlayerPrefs();
				}
			}
			return KPlayerPrefs._instance;
		}
	}

	public Dictionary<string, string> strings { get; private set; }

	public Dictionary<string, int> ints { get; private set; }

	public Dictionary<string, float> floats { get; private set; }

	public static void DeleteAll()
	{
		KPlayerPrefs.instance.strings.Clear();
		KPlayerPrefs.instance.ints.Clear();
		KPlayerPrefs.instance.floats.Clear();
		KPlayerPrefs.Save();
	}

	private static string GetPath()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		return Path.Combine(Util.RootFolder(), KPlayerPrefs.FILENAME);
	}

	public static void Save()
	{
		try
		{
			YamlIO.Save<KPlayerPrefs>(KPlayerPrefs.instance, KPlayerPrefs.PATH, null);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to save kplayerprefs: " + ex.ToString());
		}
	}

	public void Load()
	{
	}

	public static void DeleteKey(string key)
	{
		KPlayerPrefs.instance.strings.Remove(key);
		KPlayerPrefs.instance.ints.Remove(key);
		KPlayerPrefs.instance.floats.Remove(key);
	}

	public static float GetFloat(string key)
	{
		float num = 0f;
		KPlayerPrefs.instance.floats.TryGetValue(key, out num);
		return num;
	}

	public static float GetFloat(string key, float defaultValue)
	{
		float num = 0f;
		if (!KPlayerPrefs.instance.floats.TryGetValue(key, out num))
		{
			num = defaultValue;
		}
		return num;
	}

	public static int GetInt(string key)
	{
		int num = 0;
		KPlayerPrefs.instance.ints.TryGetValue(key, out num);
		return num;
	}

	public static int GetInt(string key, int defaultValue)
	{
		int num = 0;
		if (!KPlayerPrefs.instance.ints.TryGetValue(key, out num))
		{
			num = defaultValue;
		}
		return num;
	}

	public static string GetString(string key)
	{
		string text = null;
		KPlayerPrefs.instance.strings.TryGetValue(key, out text);
		return text;
	}

	public static string GetString(string key, string defaultValue)
	{
		string text = null;
		if (!KPlayerPrefs.instance.strings.TryGetValue(key, out text))
		{
			text = defaultValue;
		}
		return text;
	}

	public static bool HasKey(string key)
	{
		return KPlayerPrefs.instance.strings.ContainsKey(key) || KPlayerPrefs.instance.ints.ContainsKey(key) || KPlayerPrefs.instance.floats.ContainsKey(key);
	}

	public static void SetFloat(string key, float value)
	{
		if (KPlayerPrefs.instance.floats.ContainsKey(key))
		{
			KPlayerPrefs.instance.floats[key] = value;
		}
		else
		{
			KPlayerPrefs.instance.floats.Add(key, value);
		}
		KPlayerPrefs.Save();
	}

	public static void SetInt(string key, int value)
	{
		if (KPlayerPrefs.instance.ints.ContainsKey(key))
		{
			KPlayerPrefs.instance.ints[key] = value;
		}
		else
		{
			KPlayerPrefs.instance.ints.Add(key, value);
		}
		KPlayerPrefs.Save();
	}

	public static void SetString(string key, string value)
	{
		if (KPlayerPrefs.instance.strings.ContainsKey(key))
		{
			KPlayerPrefs.instance.strings[key] = value;
		}
		else
		{
			KPlayerPrefs.instance.strings.Add(key, value);
		}
		KPlayerPrefs.Save();
	}

	private static KPlayerPrefs _instance;

	public static readonly string FILENAME = "kplayerprefs.yaml";

	private static string PATH;
}
