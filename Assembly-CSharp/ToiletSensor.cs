using System;

public class ToiletSensor : Sensor
{
	public ToiletSensor(Sensors sensors)
		: base(sensors)
	{
		this.navigator = base.GetComponent<Navigator>();
	}

	public override void Update()
	{
		IUsable usable = null;
		int num = int.MaxValue;
		bool flag = false;
		foreach (IUsable usable2 in Components.Toilets.Items)
		{
			if (usable2.IsUsable())
			{
				flag = true;
				int navigationCost = this.navigator.GetNavigationCost(Grid.PosToCell(usable2.transform.GetPosition()));
				if (navigationCost != -1 && navigationCost < num)
				{
					usable = usable2;
					num = navigationCost;
				}
			}
		}
		bool flag2 = Components.Toilets.Count > 0;
		if (usable != this.toilet || flag2 != this.areThereAnyToilets || this.areThereAnyUsableToilets != flag)
		{
			this.toilet = usable;
			this.areThereAnyToilets = flag2;
			this.areThereAnyUsableToilets = flag;
			base.Trigger(-752545459, null);
		}
	}

	public bool AreThereAnyToilets()
	{
		return this.areThereAnyToilets;
	}

	public bool AreThereAnyUsableToilets()
	{
		return this.areThereAnyUsableToilets;
	}

	public IUsable GetNearestUsableToilet()
	{
		return this.toilet;
	}

	private Navigator navigator;

	private IUsable toilet;

	private bool areThereAnyToilets;

	private bool areThereAnyUsableToilets;
}
