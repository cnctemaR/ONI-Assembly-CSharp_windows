using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicViewer : KScreen
{
	public void ShowComic(ComicData comic, bool isVictoryComic)
	{
		for (int i = 0; i < Mathf.Max(comic.images.Length, comic.stringKeys.Length); i++)
		{
			GameObject gameObject = Util.KInstantiateUI(this.panelPrefab, this.contentContainer, true);
			this.activePanels.Add(gameObject);
			gameObject.GetComponentInChildren<Image>().sprite = comic.images[i];
			gameObject.GetComponentInChildren<LocText>().SetText(comic.stringKeys[i]);
		}
		this.closeButton.ClearOnClick();
		if (isVictoryComic)
		{
			this.closeButton.onClick += delegate
			{
				this.Stop();
				base.Show(false);
			};
		}
		else
		{
			this.closeButton.onClick += delegate
			{
				this.Stop();
			};
		}
	}

	public void Stop()
	{
		this.OnStop();
		base.Show(false);
		base.gameObject.SetActive(false);
	}

	public GameObject panelPrefab;

	public GameObject contentContainer;

	public List<GameObject> activePanels = new List<GameObject>();

	public KButton closeButton;

	public global::System.Action OnStop;
}
