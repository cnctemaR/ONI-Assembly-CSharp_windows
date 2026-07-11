using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/InfoScreenPlainText")]
public class InfoScreenPlainText : KMonoBehaviour
{
	public void SetText(string text)
	{
		this.locText.text = text;
	}

	[SerializeField]
	private LocText locText;
}
