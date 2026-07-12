using System;
using UnityEngine;

public abstract class UIConstants
{
	public const float MINIMUM_SCREEN_WIDTH = 1280f;

	public const float MINIMUM_SCREEN_HEIGHT = 720f;

	public static readonly Vector2 MINIMUM_SCREEN_SIZE = new Vector2(1280f, 720f);

	public static readonly string ColorPrefixWhite = "<color=#ffffffff>";

	public static readonly string ColorPrefixYellow = "<color=#ffff00ff>";

	public static readonly string ColorPrefixGreen = "<color=#5FDB37FF>";

	public static readonly string ColorPrefixRed = "<color=#F44A47FF>";

	public static readonly string ColorSuffix = "</color>";
}
