using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenURLButtons : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void OpenPatchNotes()
	{
		this.patchNotesScreen.SetActive(true);
	}

	public void OpenURL(string URL)
	{
		Application.OpenURL(URL);
	}

	public GameObject buttonPrefab;

	public List<OpenURLButtons.URLButtonData> buttonData;

	[SerializeField]
	private GameObject patchNotesScreen;

	public enum URLButtonType
	{
		url,
		patchNotes
	}

	[Serializable]
	public class URLButtonData
	{
		public string stringKey;

		public OpenURLButtons.URLButtonType urlType;

		public string url;
	}
}
