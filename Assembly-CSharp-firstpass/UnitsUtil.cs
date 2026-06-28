using System;

public static class UnitsUtil
{
	public static bool IsTimeUnit(Units unit)
	{
		return unit == Units.PerSecond || unit == Units.PerDay;
	}

	public static string GetUnitSuffix(Units unit)
	{
		string text;
		if (unit != Units.Kelvin)
		{
			text = "";
		}
		else
		{
			text = "K";
		}
		return text;
	}
}
