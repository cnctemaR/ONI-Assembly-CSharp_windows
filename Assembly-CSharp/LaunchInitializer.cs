using System;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
	private void Awake()
	{
		GraphicsOptionsScreen.SetResolutionFromPrefs();
		LaunchInitializer.ApplyCultureToThread(Thread.CurrentThread);
		global::Debug.Log("Development Build: CU-" + 274136U.ToString(), null);
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		KPlayerPrefs.instance.Load();
		KFMOD.Initialize();
		for (int i = 0; i < this.SpawnPrefabs.Length; i++)
		{
			if (this.SpawnPrefabs[i] != null)
			{
				Util.KInstantiate(this.SpawnPrefabs[i], base.gameObject, null);
			}
		}
		LaunchInitializer.DeleteLingeringFiles();
	}

	private static void DeleteLingeringFiles()
	{
		string[] array = new string[] { "fmod.log", "load_stats_0.json" };
		string directoryName = Path.GetDirectoryName(Application.dataPath);
		foreach (string text in array)
		{
			string text2 = Path.Combine(directoryName, text);
			global::Debug.Log(text2, null);
			try
			{
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
			}
			catch (Exception ex)
			{
				global::Debug.LogWarning(ex, null);
			}
		}
	}

	public static void ApplyCultureToThread(Thread thread)
	{
		if (Application.platform != RuntimePlatform.WindowsEditor)
		{
			thread.CurrentCulture = CultureInfo.InvariantCulture;
		}
	}

	public const string BUILD_PREFIX = "CU";

	public GameObject[] SpawnPrefabs;
}
