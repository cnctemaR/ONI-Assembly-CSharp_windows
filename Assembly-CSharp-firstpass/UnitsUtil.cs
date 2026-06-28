using System;

public static class UnitsUtil
{
	public static bool IsTimeUnit(Units unit)
	{
		return unit == Units.PerSecond || unit == Units.PerDay;
	}

	public static string GetUnitSuffix(Units unit)
	{
		if (unit != Units.Kelvin)
		{
			return string.Empty;
		}
		return "K";
	}
}
