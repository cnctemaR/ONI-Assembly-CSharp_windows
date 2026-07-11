using System;

public static class UnitsUtil
{
	public static bool IsTimeUnit(Units unit)
	{
		return unit - Units.PerDay <= 1;
	}

	public static string GetUnitSuffix(Units unit)
	{
		if (unit == Units.Kelvin)
		{
			return "K";
		}
		return "";
	}
}
