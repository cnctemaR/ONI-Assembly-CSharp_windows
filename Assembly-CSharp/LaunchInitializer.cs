using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
	public static string BuildPrefix()
	{
		return "U42";
	}

	private void Update()
	{
		if (this.numWaitFrames > Time.renderedFrameCount)
		{
			return;
		}
		if (!DistributionPlatform.Initialized)
		{
			if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
			{
				global::Debug.LogError("Machine does not support RGBAFloat32");
			}
			GraphicsOptionsScreen.SetSettingsFromPrefs();
			Util.ApplyInvariantCultureToThread(Thread.CurrentThread);
			global::Debug.Log("Current date: " + global::System.DateTime.Now.ToString());
			global::Debug.Log("release Build: " + BuildWatermark.GetBuildText());
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			KPlayerPrefs.instance.Load();
			DistributionPlatform.Initialize();
		}
		if (!DistributionPlatform.Inst.IsDLCStatusReady())
		{
			return;
		}
		global::Debug.Log("DistributionPlatform initialized.");
		global::Debug.Log("release Build: " + BuildWatermark.GetBuildText());
		global::Debug.Log(string.Format("EXPANSION1 installed: {0}  active: {1}", DlcManager.IsExpansion1Installed(), DlcManager.IsExpansion1Active()));
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

	private const string BASE_BUILD_PREFIX = "U42";

	public GameObject[] SpawnPrefabs;

	[SerializeField]
	private int numWaitFrames = 1;
}
