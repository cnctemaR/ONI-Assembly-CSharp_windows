using System;
using System.Collections.Generic;
using Klei;
using STRINGS;
using UnityEngine;

public class CreditsScreen : KScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.AddCredits(this.TeamCreditsFile);
		foreach (string text in LocString.GetStrings(typeof(UI.CREDITSSCREEN.THIRD_PARTY)))
		{
			GameObject gameObject = Util.KInstantiateUI(this.entryPrefab, this.entryContainer.gameObject, true);
			gameObject.GetComponent<LocText>().text = text;
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
		for (int i = 0; i < array.GetLength(1); i++)
		{
			string text = string.Format("{0} {1}", array[0, i], array[1, i]);
			if (!(text == " "))
			{
				list.Add(text);
			}
		}
		list.Shuffle<string>();
		foreach (string text2 in list)
		{
			GameObject gameObject = Util.KInstantiateUI(this.entryPrefab, this.entryContainer.gameObject, true);
			gameObject.GetComponent<LocText>().text = text2;
		}
	}

	public GameObject entryPrefab;

	public Transform entryContainer;

	public KButton CloseButton;

	public TextAsset TeamCreditsFile;
}
