using System;

public class PickupableSensor : Sensor
{
	public PickupableSensor(Sensors sensors)
		: base(sensors)
	{
		this.worker = base.GetComponent<Worker>();
		this.pathProber = base.GetComponent<PathProber>();
	}

	private int CompareFetchChores(FetchChore a, FetchChore b)
	{
		int num = b.masterPriority - a.masterPriority;
		if (num == 0)
		{
			int num2 = int.MaxValue;
			if (b.destination != null)
			{
				num2 = this.pathProber.GetCost(Grid.PosToCell(b.destination));
			}
			int num3 = int.MaxValue;
			if (a.destination != null)
			{
				num3 = this.pathProber.GetCost(Grid.PosToCell(a.destination));
			}
			if (num2 == PathProber.InvalidCost)
			{
				num2 = int.MaxValue;
			}
			if (num3 == PathProber.InvalidCost)
			{
				num3 = int.MaxValue;
			}
			return num3 - num2;
		}
		return num;
	}

	public override void Update()
	{
		GlobalChoreProvider.Instance.fetchChores.Sort(new Comparison<FetchChore>(this.CompareFetchChores));
		FetchManagerUpdater.UpdatePickups(this.pathProber, FetchManager.Instance.pickupables, this.worker);
	}

	private PathProber pathProber;

	private Worker worker;
}
