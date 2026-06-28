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
		for (int i = 0; i < this.buttons.Length; i++)
		{
			KeyValuePair<LocString, Action<OpenURLButtons>> button = this.buttons[i];
			GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, base.gameObject, true);
			gameObject.GetComponentInChildren<LocText>().SetText(this.buttons[i].Key.text);
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				button.Value(this);
			};
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

	private KeyValuePair<LocString, Action<OpenURLButtons>>[] buttons;

	[SerializeField]
	private GameObject patchNotesScreen;
}
