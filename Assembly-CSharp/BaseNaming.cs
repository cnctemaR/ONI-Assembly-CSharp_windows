using System;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BaseNaming : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.GenerateBaseName();
		this.inputField.onValueChanged.AddListener(delegate
		{
			Util.ScrubInputField(this.inputField);
		});
		this.shuffleBaseNameButton.onClick += this.GenerateBaseName;
		this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
	}

	private void OnEndEdit(string newName)
	{
		if (string.IsNullOrEmpty(newName))
		{
			return;
		}
		SaveGame.Instance.SetBaseName(newName);
		string text = newName;
		if (!text.Contains(".sav"))
		{
			text += ".sav";
		}
		string savePrefix = SaveLoader.GetSavePrefix();
		if (!text.Contains(savePrefix))
		{
			text = savePrefix + text;
		}
		SaveLoader.SetActiveSaveFilePath(text);
	}

	private void GenerateBaseName()
	{
		string text = this.GenerateBaseNameString();
		((LocText)this.inputField.placeholder).text = text;
		this.inputField.text = text;
		this.OnEndEdit(text);
	}

	private string GenerateBaseNameString()
	{
		string text = NAMEGEN.COLONY.FORMATS.GetRandom<string>();
		text = this.ReplaceStringWithRandom(text, "{noun}", NAMEGEN.COLONY.NOUN);
		text = this.ReplaceStringWithRandom(text, "{adjective}", NAMEGEN.COLONY.ADJECTIVE);
		text = this.ReplaceStringWithRandom(text, "{adjective2}", NAMEGEN.COLONY.ADJECTIVE);
		text = this.ReplaceStringWithRandom(text, "{adjective3}", NAMEGEN.COLONY.ADJECTIVE);
		return this.ReplaceStringWithRandom(text, "{adjective4}", NAMEGEN.COLONY.ADJECTIVE);
	}

	private string ReplaceStringWithRandom(string fullString, string replacementKey, string[] replacementValues)
	{
		if (!fullString.Contains(replacementKey))
		{
			return fullString;
		}
		return fullString.Replace(replacementKey, replacementValues.GetRandom<string>());
	}

	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private KButton shuffleBaseNameButton;
}
