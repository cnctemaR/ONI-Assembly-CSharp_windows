using System;
using UnityEngine;

public class SceneOrganizer : MonoBehaviour
{
	public static SceneOrganizer Instance { get; private set; }

	private void Awake()
	{
		SceneOrganizer.Instance = this;
		this.mFolders = new GameObject[34];
	}

	private void OnDestroy()
	{
		SceneOrganizer.Instance = null;
	}

	public GameObject GetFolder(Folder folder)
	{
		GameObject gameObject = this.mFolders[(int)folder];
		if (gameObject == null)
		{
			GameObject gameObject2 = base.gameObject;
			if (folder != Folder.GlobalDoNotDestroy)
			{
				if (this.dynamicRoot == null)
				{
					this.dynamicRoot = Util.NewGameObject(null, "SceneOrganizerDynamic");
				}
				gameObject2 = this.dynamicRoot;
			}
			gameObject = Util.NewGameObject(gameObject2, folder.ToString());
			this.mFolders[(int)folder] = gameObject;
		}
		return gameObject;
	}

	private GameObject[] mFolders;

	private GameObject dynamicRoot;
}
