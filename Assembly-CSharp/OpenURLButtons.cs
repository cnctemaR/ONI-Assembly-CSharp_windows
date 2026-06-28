using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenURLButtons : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		for (int i = 0; i < this.buttonData.Count; i++)
		{
			OpenURLButtons.URLButtonData data = this.buttonData[i];
			GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, base.gameObject, true);
			string text = Strings.Get(data.stringKey);
			gameObject.GetComponentInChildren<LocText>().SetText(text);
			OpenURLButtons.URLButtonType urlType = data.urlType;
			if (urlType != OpenURLButtons.URLButtonType.url)
			{
				if (urlType == OpenURLButtons.URLButtonType.patchNotes)
				{
					gameObject.GetComponent<KButton>().onClick += delegate
					{
						this.OpenPatchNotes();
					};
				}
			}
			else
			{
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					this.OpenURL(data.url);
				};
			}
		}
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
