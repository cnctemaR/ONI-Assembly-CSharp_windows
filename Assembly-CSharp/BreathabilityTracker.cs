using System;
using UnityEngine;

public class BreathabilityTracker : WorldTracker
{
	public BreathabilityTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		if (Components.LiveMinionIdentities.GetWorldItems(base.WorldID, false).Count == 0)
		{
			base.AddPoint(0f);
			return;
		}
		int num2 = 0;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.GetWorldItems(base.WorldID, false))
		{
			OxygenBreather component = minionIdentity.GetComponent<OxygenBreather>();
			if (!(component == null))
			{
				OxygenBreather.IGasProvider currentGasProvider = component.GetCurrentGasProvider();
				num2++;
				if (!component.IsOutOfOxygen)
				{
					num += 100f;
					if (currentGasProvider.IsLowOxygen())
					{
						num -= 50f;
					}
				}
			}
		}
		num /= (float)num2;
		base.AddPoint((float)Mathf.RoundToInt(num));
	}

	public override string FormatValueString(float value)
	{
		return value.ToString() + "%";
	}
}
