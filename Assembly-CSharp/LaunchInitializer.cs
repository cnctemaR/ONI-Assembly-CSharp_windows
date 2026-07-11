using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
	private void Update()
	{
		if (this.numWaitFrames > Time.renderedFrameCount)
		{
			return;
		}
		if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
		{
			global::Debug.LogError("Machine does not support RGBAFloat32");
		}
		GraphicsOptionsScreen.SetResolutionFromPrefs();
		Util.ApplyInvariantCultureToThread(Thread.CurrentThread);
		global::Debug.Log("release Build: LU-" + 364722U.ToString());
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		KPlayerPrefs.instance.Load();
		KFMOD.Initialize();
		for (int i = 0; i < this.SpawnPrefabs.Length; i++)
		{
			GameObject gameObject = this.SpawnPrefabs[i];
			if (gameObject != null)
			{
				Util.KInstantiate(gameObject, base.gameObject, null);
			}
		}
		LaunchInitializer.DeleteLingeringFiles();
		base.enabled = false;
	}

	private static void DeleteLingeringFiles()
	{
		string[] array = new string[] { "fmod.log", "load_stats_0.json", "OxygenNotIncluded_Data/output_log.txt" };
		string directoryName = Path.GetDirectoryName(Application.dataPath);
		foreach (string text in array)
		{
			string text2 = Path.Combine(directoryName, text);
			try
			{
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
			}
			catch (Exception ex)
			{
				global::Debug.LogWarning(ex);
			}
		}
	}

	public const string BUILD_PREFIX = "LU";

	public GameObject[] SpawnPrefabs;

	[SerializeField]
	private int numWaitFrames = 1;
}
