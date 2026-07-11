using System;
using UnityEngine;

public class ColorSet : ScriptableObject
{
	public string settingName;

	[Header("Logic")]
	public Color32 logicOn;

	public Color32 logicOff;

	public Color32 logicDisconnected;

	public Color32 logicOnText;

	public Color32 logicOffText;

	[Header("Decor")]
	public Color32 decorPositive;

	public Color32 decorNegative;

	public Color32 decorBaseline;

	public Color32 decorHighlightPositive;

	public Color32 decorHighlightNegative;

	[Header("Crop Overlay")]
	public Color32 cropHalted;

	public Color32 cropGrowing;

	public Color32 cropGrown;
}
