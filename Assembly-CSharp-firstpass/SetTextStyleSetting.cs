using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SetTextStyleSetting : KMonoBehaviour
{
	public static void ApplyStyle(TextMeshProUGUI sdfText, TextStyleSetting style)
	{
		if (!sdfText)
		{
			return;
		}
		if (!style)
		{
			return;
		}
		sdfText.enableWordWrapping = style.enableWordWrapping;
		sdfText.enableKerning = true;
		sdfText.extraPadding = true;
		sdfText.fontSize = (float)style.fontSize;
		sdfText.color = style.textColor;
		sdfText.font = style.sdfFont;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void SetStyle(TextStyleSetting newstyle)
	{
		if (this.sdfText == null)
		{
			this.sdfText = base.GetComponent<TextMeshProUGUI>();
		}
		if (this.currentStyle != newstyle)
		{
			this.currentStyle = newstyle;
			this.style = this.currentStyle;
			SetTextStyleSetting.ApplyStyle(this.sdfText, this.style);
		}
	}

	[MyCmpGet]
	private Text text;

	[MyCmpGet]
	private TextMeshProUGUI sdfText;

	[SerializeField]
	private TextStyleSetting style;

	private TextStyleSetting currentStyle;

	public enum TextStyle
	{
		Standard,
		Header
	}
}
