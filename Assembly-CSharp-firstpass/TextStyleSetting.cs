using System;
using TMPro;
using UnityEngine;

public class TextStyleSetting : ScriptableObject
{
	public void Init(TMP_FontAsset _sdfFont, int _fontSize, Color _color, bool _enableWordWrapping)
	{
		this.sdfFont = _sdfFont;
		this.fontSize = _fontSize;
		this.textColor = _color;
		this.enableWordWrapping = _enableWordWrapping;
	}

	public TMP_FontAsset sdfFont;

	public int fontSize;

	public Color textColor;

	public bool enableWordWrapping = true;
}
