using System;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
	private void Awake()
	{
		GraphicsOptionsScreen.SetResolutionFromPrefs();
		if (Application.platform != RuntimePlatform.WindowsEditor)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		}
		global::Debug.Log("Development Build: R2-" + 269752U.ToString(), null);
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		KPlayerPrefs.instance.Load();
		for (int i = 0; i < this.SpawnPrefabs.Length; i++)
		{
			if (this.SpawnPrefabs[i] != null)
			{
				Util.KInstantiate(this.SpawnPrefabs[i], base.gameObject, null);
			}
		}
	}

	public const string BUILD_PREFIX = "R2";

	public GameObject[] SpawnPrefabs;
}
