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
			Util.ScrubInputField(this.inputField, false);
		});
		this.shuffleBaseNameButton.onClick += this.GenerateBaseName;
		this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
	}

	private void OnEndEdit(string newName)
	{
		if (Localization.HasDirtyWords(newName))
		{
			this.inputField.text = this.GenerateBaseNameString();
			newName = this.inputField.text;
		}
		if (!string.IsNullOrEmpty(newName))
		{
			this.inputField.text = newName;
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
		string text = LocString.GetStrings(typeof(NAMEGEN.COLONY.FORMATS)).GetRandom<string>();
		text = this.ReplaceStringWithRandom(text, "{noun}", LocString.GetStrings(typeof(NAMEGEN.COLONY.NOUN)));
		string[] strings = LocString.GetStrings(typeof(NAMEGEN.COLONY.ADJECTIVE));
		text = this.ReplaceStringWithRandom(text, "{adjective}", strings);
		text = this.ReplaceStringWithRandom(text, "{adjective2}", strings);
		text = this.ReplaceStringWithRandom(text, "{adjective3}", strings);
		return this.ReplaceStringWithRandom(text, "{adjective4}", strings);
	}

	private string ReplaceStringWithRandom(string fullString, string replacementKey, string[] replacementValues)
	{
		string text;
		if (!fullString.Contains(replacementKey))
		{
			text = fullString;
		}
		else
		{
			text = fullString.Replace(replacementKey, replacementValues.GetRandom<string>());
		}
		return text;
	}

	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private KButton shuffleBaseNameButton;
}
