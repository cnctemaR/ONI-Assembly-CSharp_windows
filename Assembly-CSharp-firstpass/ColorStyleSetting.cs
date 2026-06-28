using System;
using UnityEngine;

public class ColorStyleSetting : ScriptableObject
{
	public void Init(Color _color)
	{
		this.activeColor = _color;
		this.inactiveColor = _color;
		this.disabledColor = _color;
		this.disabledActiveColor = _color;
		this.hoverColor = _color;
		this.disabledhoverColor = _color;
	}

	public Color activeColor;

	public Color inactiveColor;

	public Color disabledColor;

	public Color disabledActiveColor;

	public Color hoverColor;

	public Color disabledhoverColor = Color.grey;
}
