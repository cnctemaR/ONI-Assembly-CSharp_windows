using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class OpenURLButtons : KMonoBehaviour
{
	public OpenURLButtons()
	{
		KeyValuePair<LocString, Action<OpenURLButtons>>[] array = new KeyValuePair<LocString, Action<OpenURLButtons>>[3];
		array[0] = new KeyValuePair<LocString, Action<OpenURLButtons>>(UI.DEVELOPMENTBUILDS.ALPHA.MESSAGES.MAILINGLIST, delegate(OpenURLButtons o)
		{
			o.OpenURL("http://eepurl.com/cw2Nrn");
		});
		array[1] = new KeyValuePair<LocString, Action<OpenURLButtons>>(UI.DEVELOPMENTBUILDS.ALPHA.MESSAGES.FORUMBUTTON, delegate(OpenURLButtons o)
		{
			o.OpenURL("http://forums.kleientertainment.com/forum/118-oxygen-not-included/");
		});
		array[2] = new KeyValuePair<LocString, Action<OpenURLButtons>>(UI.DEVELOPMENTBUILDS.ALPHA.MESSAGES.PATCHNOTES, delegate(OpenURLButtons o)
		{
			o.OpenPatchNotes();
		});
		this.buttons = array;
		base..ctor();
	}

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

	private KeyValuePair<LocString, Action<OpenURLButtons>>[] buttons;

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
