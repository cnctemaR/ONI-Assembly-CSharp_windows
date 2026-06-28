using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;

public class CreditsScreen : KScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.AddCredits(this.TeamCreditsFile, true);
		foreach (TextAsset textAsset in this.AdditionalCreditsFiles)
		{
			this.AddCredits(textAsset, false);
		}
	}

	public void Close()
	{
		this.Deactivate();
	}

	private void AddCredits(TextAsset csv, bool randomizeOrder)
	{
		string[,] array = CSVReader.SplitCsvGrid(csv.text, csv.name);
		List<string> list = new List<string>();
		for (int i = 0; i < array.GetLength(1); i++)
		{
			string text = string.Format("{0} {1}", array[0, i], array[1, i]);
			if (!(text == " "))
			{
				list.Add(text);
			}
		}
		if (randomizeOrder)
		{
			list.Shuffle<string>();
		}
		foreach (string text2 in list)
		{
			string text3 = text2.Replace("|", "\n");
			GameObject gameObject = Util.KInstantiateUI(this.entryPrefab, this.entryContainer.gameObject, true);
			gameObject.GetComponent<LocText>().text = text3;
		}
	}

	public GameObject entryPrefab;

	public Transform entryContainer;

	public TextAsset TeamCreditsFile;

	public TextAsset[] AdditionalCreditsFiles;
}
