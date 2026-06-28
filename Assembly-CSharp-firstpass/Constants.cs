using System;
using UnityEngine;

public static class Constants
{
	public const float GlobalTimeScale = 1f;

	public const float HOURS = 3600f;

	public const float MINUTES = 60f;

	public const float SECONDS_PER_CYCLE = 600f;

	public const float PER_DAY_TO_PER_CYCLE = 144f;

	public const float PER_CYCLE_TO_PER_DAY = 0.0069444445f;

	public const float SecondsPerScheduleBlock = 25f;

	public const float DaytimeDurationInSeconds = 525f;

	public const float NightimeDurationInSeconds = 75f;

	public const float DaytimeDurationInPercentage = 0.875f;

	public const float StartTimeInSeconds = 50f;

	public const int ScheduleBlocksPerCycle = 24;

	public const float CircuitOverloadTime = 6f;

	public const int AutoSaveDayInterval = 1;

	public const float ICE_DIG_TIME = 4f;

	public const float SORTKEY_MAX = 9999f;

	public const string BULLETSTRING = "• ";

	public const string TABSTRING = "    ";

	public const string TABBULLETSTRING = "    • ";

	public const float W2KW = 0.001f;

	public const float KW2W = 1000f;

	public const float HEAT_KW_DISPLAY_FUDGEFACTOR = 0.005f;

	public const float G2KG = 0.001f;

	public const float KG2G = 1000f;

	public const float CELSIUS2KELVIN = 273.15f;

	public const float CAL2KCAL = 0.001f;

	public const float KCAL2CAL = 1000f;

	public const float HEATOFVAPORIZATION_WATER = 580f;

	public const float DefaultEntityThickness = 0.01f;

	public const float DefaultSurfaceArea = 10f;

	public static readonly Color POSITIVE_COLOR = new Color(0.32156864f, 0.7529412f, 0.4745098f);

	public static readonly string POSITIVE_COLOR_STR = "#" + Constants.POSITIVE_COLOR.ToHexString();

	public static readonly Color NEGATIVE_COLOR = new Color(0.95686275f, 0.2901961f, 0.2784314f);

	public static readonly string NEGATIVE_COLOR_STR = "#" + Constants.NEGATIVE_COLOR.ToHexString();

	public static readonly Color NEUTRAL_COLOR = Color.grey;

	public static readonly string NEUTRAL_COLOR_STR = "#" + Constants.NEUTRAL_COLOR.ToHexString();
}
