using System;
using UnityEngine;

public class InfoScreenPlainText : KMonoBehaviour
{
	public void SetText(string text)
	{
		this.locText.text = text;
	}

	[SerializeField]
	private LocText locText;
}
