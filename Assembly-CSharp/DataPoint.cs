using System;

public struct DataPoint
{
	public DataPoint(float start, float end, float value)
	{
		this.periodStart = start;
		this.periodEnd = end;
		this.periodValue = value;
	}

	public float periodStart;

	public float periodEnd;

	public float periodValue;
}
