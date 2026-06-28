using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class OpenURLButtons : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		for (int i = 0; i < this.URLs.Length; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, base.gameObject, true);
			gameObject.GetComponentInChildren<LocText>().SetText(this.URLs[i].Key.text);
			string click_url = this.URLs[i].Value;
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				this.onPress(click_url);
			};
		}
	}

	public void onPress(string URL)
	{
		Application.OpenURL(URL);
	}

	public GameObject buttonPrefab;

	private KeyValuePair<LocString, string>[] URLs = new KeyValuePair<LocString, string>[]
	{
		new KeyValuePair<LocString, string>(UI.DEVELOPMENTBUILDS.ALPHA.MESSAGES.MAILINGLIST, "http://eepurl.com/cw2Nrn"),
		new KeyValuePair<LocString, string>(UI.DEVELOPMENTBUILDS.ALPHA.MESSAGES.FORUMBUTTON, "http://forums.kleientertainment.com/forum/118-oxygen-not-included/")
	};
}
