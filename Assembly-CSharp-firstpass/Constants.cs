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

	public const float G2KG = 0.001f;

	public const float CELSIUS2KELVIN = 273.15f;

	public const float CAL2KCAL = 0.001f;

	public const float KCAL2CAL = 1000f;

	public const float JOULES2CALORIES = 34.285717f;

	public const float HEATOFVAPORIZATION_WATER = 580f;

	public const float DUPLICANT_BODY_TEMPERATURE = 310.15f;

	public const float DUPLICANT_BODY_TEMPERATURE_ACCEPTABLE_RANGE = 5f;

	public const float DUPLICANT_SHC = 3.47f;

	public static readonly Color POSITIVE_COLOR = new Color(0.32156864f, 0.7529412f, 0.4745098f);

	public static readonly string POSITIVE_COLOR_STR = "#" + Constants.POSITIVE_COLOR.ToHexString();

	public static readonly Color NEGATIVE_COLOR = new Color(0.95686275f, 0.2901961f, 0.2784314f);

	public static readonly string NEGATIVE_COLOR_STR = "#" + Constants.NEGATIVE_COLOR.ToHexString();

	public static readonly Color NEUTRAL_COLOR = Color.grey;

	public static readonly string NEUTRAL_COLOR_STR = "#" + Constants.NEUTRAL_COLOR.ToHexString();
}
