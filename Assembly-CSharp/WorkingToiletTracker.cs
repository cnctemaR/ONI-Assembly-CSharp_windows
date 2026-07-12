using System;
using System.Collections.Generic;

public class WorkingToiletTracker : WorldTracker
{
	public WorkingToiletTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		int num = 0;
		using (IEnumerator<IUsable> enumerator = Components.Toilets.WorldItemsEnumerate(base.WorldID, true).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsUsable())
				{
					num++;
				}
			}
		}
		base.AddPoint((float)num);
	}

	public override string FormatValueString(float value)
	{
		return value.ToString();
	}
}
