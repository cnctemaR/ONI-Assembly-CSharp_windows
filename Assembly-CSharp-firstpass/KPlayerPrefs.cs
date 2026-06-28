using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

public class KPlayerPrefs : YamlIO<KPlayerPrefs>
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
					YamlIO<KPlayerPrefs>.LoadFile(KPlayerPrefs.PATH);
				}
				catch
				{
					global::Debug.LogWarning("Creating new KPlayerPrefs..", null);
					KPlayerPrefs._instance = new KPlayerPrefs();
					KPlayerPrefs.Save();
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
		PlayerPrefs.DeleteAll();
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
			KPlayerPrefs.instance.Save(KPlayerPrefs.PATH);
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning("Failed to save kplayerprefs: " + ex.ToString(), null);
		}
	}

	public void Load()
	{
	}

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
		KPlayerPrefs.instance.strings.Remove(key);
		KPlayerPrefs.instance.ints.Remove(key);
		KPlayerPrefs.instance.floats.Remove(key);
	}

	public static float GetFloat(string key)
	{
		KPlayerPrefs.PullFloat(key);
		float num = 0f;
		KPlayerPrefs.instance.floats.TryGetValue(key, out num);
		return num;
	}

	public static float GetFloat(string key, float defaultValue)
	{
		KPlayerPrefs.PullFloat(key);
		float num = 0f;
		if (!KPlayerPrefs.instance.floats.TryGetValue(key, out num))
		{
			num = defaultValue;
		}
		return num;
	}

	public static int GetInt(string key)
	{
		KPlayerPrefs.PullInt(key);
		int num = 0;
		KPlayerPrefs.instance.ints.TryGetValue(key, out num);
		return num;
	}

	public static int GetInt(string key, int defaultValue)
	{
		KPlayerPrefs.PullInt(key);
		int num = 0;
		if (!KPlayerPrefs.instance.ints.TryGetValue(key, out num))
		{
			num = defaultValue;
		}
		return num;
	}

	public static string GetString(string key)
	{
		KPlayerPrefs.PullString(key);
		string text = null;
		KPlayerPrefs.instance.strings.TryGetValue(key, out text);
		return text;
	}

	public static string GetString(string key, string defaultValue)
	{
		KPlayerPrefs.PullString(key);
		string text = null;
		if (!KPlayerPrefs.instance.strings.TryGetValue(key, out text))
		{
			text = defaultValue;
		}
		return text;
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key) || KPlayerPrefs.instance.strings.ContainsKey(key) || KPlayerPrefs.instance.ints.ContainsKey(key) || KPlayerPrefs.instance.floats.ContainsKey(key);
	}

	public static void SetFloat(string key, float value)
	{
		PlayerPrefs.DeleteKey(key);
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
		PlayerPrefs.DeleteKey(key);
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
		PlayerPrefs.DeleteKey(key);
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

	private static void PullFloat(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			float @float = PlayerPrefs.GetFloat(key);
			PlayerPrefs.DeleteKey(key);
			KPlayerPrefs.SetFloat(key, @float);
		}
	}

	private static void PullInt(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			int @int = PlayerPrefs.GetInt(key);
			PlayerPrefs.DeleteKey(key);
			KPlayerPrefs.SetInt(key, @int);
		}
	}

	private static void PullString(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			string @string = PlayerPrefs.GetString(key);
			PlayerPrefs.DeleteKey(key);
			KPlayerPrefs.SetString(key, @string);
		}
	}

	private static KPlayerPrefs _instance;

	public static readonly string FILENAME = "kplayerprefs.yaml";

	private static string PATH;
}
