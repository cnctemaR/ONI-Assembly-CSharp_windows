using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;

public class CreditsScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (TextAsset textAsset in this.creditsFiles)
		{
			this.AddCredits(textAsset);
		}
		this.CloseButton.onClick += this.Close;
	}

	public void Close()
	{
		this.Deactivate();
	}

	private void AddCredits(TextAsset csv)
	{
		string[,] array = CSVReader.SplitCsvGrid(csv.text, csv.name);
		List<string> list = new List<string>();
		for (int i = 1; i < array.GetLength(1); i++)
		{
			string text = string.Format("{0} {1}", array[0, i], array[1, i]);
			if (!(text == " "))
			{
				list.Add(text);
			}
		}
		list.Shuffle<string>();
		string text2 = array[0, 0];
		GameObject gameObject = Util.KInstantiateUI(this.teamHeaderPrefab, this.entryContainer.gameObject, true);
		gameObject.GetComponent<LocText>().text = text2;
		this.teamContainers.Add(text2, gameObject);
		foreach (string text3 in list)
		{
			Util.KInstantiateUI(this.entryPrefab, this.teamContainers[text2], true).GetComponent<LocText>().text = text3;
		}
	}

	public GameObject entryPrefab;

	public GameObject teamHeaderPrefab;

	private Dictionary<string, GameObject> teamContainers = new Dictionary<string, GameObject>();

	public Transform entryContainer;

	public KButton CloseButton;

	public TextAsset[] creditsFiles;
}
