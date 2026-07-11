using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CopyTextFieldToClipboard")]
public class CopyTextFieldToClipboard : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.button.onClick += this.OnClick;
	}

	private void OnClick()
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = this.GetText();
		textEditor.SelectAll();
		textEditor.Copy();
	}

	public KButton button;

	public Func<string> GetText;
}
