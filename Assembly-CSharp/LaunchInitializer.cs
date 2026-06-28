using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchInitializer : MonoBehaviour
{
	private void Awake()
	{
		if (SteamManager.Initialized && SteamUGCService.HasInstalledLanguage())
		{
			base.gameObject.AddComponent<SteamUGCService>();
			SteamUGCService.LoadTranslation();
		}
		else
		{
			string localizationFilePath = Localization.GetLocalizationFilePath();
			if (File.Exists(localizationFilePath))
			{
				Localization.LoadTranslation(localizationFilePath);
			}
		}
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		for (int i = 0; i < this.SpawnPrefabs.Length; i++)
		{
			if (this.SpawnPrefabs[i] != null)
			{
				Util.KInstantiate(this.SpawnPrefabs[i], base.gameObject, null);
			}
		}
		AudioMixer.Create();
		SceneManager.LoadScene("frontend");
	}

	public GameObject[] SpawnPrefabs;
}
