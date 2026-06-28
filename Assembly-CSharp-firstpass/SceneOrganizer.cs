using System;
using UnityEngine;

public class SceneOrganizer : MonoBehaviour
{
	public static SceneOrganizer Instance { get; private set; }

	private void Awake()
	{
		SceneOrganizer.Instance = this;
		this.mFolders = new GameObject[32];
		for (int i = 0; i < 32; i++)
		{
			this.mFolders[i] = Util.NewGameObject(base.gameObject, ((Folder)i).ToString());
			this.mFolders[i].isStatic = true;
		}
	}

	private void OnDestroy()
	{
		SceneOrganizer.Instance = null;
	}

	public GameObject GetFolder(Folder folder)
	{
		return this.mFolders[(int)folder];
	}

	private GameObject[] mFolders;
}
