using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/InfoScreenLineItem")]
public class InfoScreenLineItem : KMonoBehaviour
{
	public void SetText(string text)
	{
		this.locText.text = text;
	}

	public void SetTooltip(string tooltip)
	{
		this.toolTip.toolTip = tooltip;
	}

	[SerializeField]
	private LocText locText;

	[SerializeField]
	private ToolTip toolTip;

	private string text;

	private string tooltip;
}
