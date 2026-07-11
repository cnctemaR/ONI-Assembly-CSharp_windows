using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
	public static SceneInitializer Instance { get; private set; }

	private void Awake()
	{
		Localization.SwapToLocalizedFont();
		string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
		string text = Application.dataPath + Path.DirectorySeparatorChar.ToString() + "Plugins";
		if (!environmentVariable.Contains(text))
		{
			Environment.SetEnvironmentVariable("PATH", environmentVariable + Path.PathSeparator.ToString() + text, EnvironmentVariableTarget.Process);
		}
		SceneInitializer.Instance = this;
		this.PreLoadPrefabs();
	}

	private void OnDestroy()
	{
		SceneInitializer.Instance = null;
	}

	private void PreLoadPrefabs()
	{
		foreach (GameObject gameObject in this.preloadPrefabs)
		{
			if (gameObject != null)
			{
				Util.KInstantiate(gameObject, gameObject.transform.GetPosition(), Quaternion.identity, base.gameObject, null, true, 0);
			}
		}
	}

	public void NewSaveGamePrefab()
	{
		if (this.prefab_NewSaveGame != null && SaveGame.Instance == null)
		{
			Util.KInstantiate(this.prefab_NewSaveGame, base.gameObject, null);
		}
	}

	public void PostLoadPrefabs()
	{
		foreach (GameObject gameObject in this.prefabs)
		{
			if (gameObject != null)
			{
				Util.KInstantiate(gameObject, base.gameObject, null);
			}
		}
	}

	public const int MAXDEPTH = -30000;

	public const int SCREENDEPTH = -1000;

	public GameObject prefab_NewSaveGame;

	public List<GameObject> preloadPrefabs = new List<GameObject>();

	public List<GameObject> prefabs = new List<GameObject>();
}
