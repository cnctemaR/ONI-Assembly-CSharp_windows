using System;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
	private void Awake()
	{
		global::Debug.Log("Development Build: OU-" + 232512U.ToString(), null);
		KPlayerPrefs.instance.Load();
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		for (int i = 0; i < this.SpawnPrefabs.Length; i++)
		{
			if (this.SpawnPrefabs[i] != null)
			{
				Util.KInstantiate(this.SpawnPrefabs[i], base.gameObject, null);
			}
		}
		AudioMixer.Create();
		App.LoadScene("frontend");
	}

	public const string BUILD_PREFIX = "OU";

	public GameObject[] SpawnPrefabs;
}
