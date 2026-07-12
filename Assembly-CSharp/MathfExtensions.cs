using System;

public static class MathfExtensions
{
	public static long Max(this long a, long b)
	{
		if (a < b)
		{
			return b;
		}
		return a;
	}

	public static long Min(this long a, long b)
	{
		if (a > b)
		{
			return b;
		}
		return a;
	}
}
