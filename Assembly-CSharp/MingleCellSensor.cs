using System;
using UnityEngine;

public class MingleCellSensor : Sensor
{
	public MingleCellSensor(Sensors sensors)
		: base(sensors)
	{
		this.navigator = base.GetComponent<Navigator>();
		this.brain = base.GetComponent<MinionBrain>();
		this.scheduable = base.GetComponent<Schedulable>();
	}

	public bool IsAllowed()
	{
		return ScheduleManager.Instance.IsAllowed(this.scheduable, Db.Get().ScheduleBlockTypes.Recreation);
	}

	public override void Update()
	{
		if (!this.IsAllowed())
		{
			return;
		}
		this.cell = Grid.InvalidCell;
		int num = int.MaxValue;
		ListPool<int, MingleCellSensor>.PooledList pooledList = ListPool<int, MingleCellSensor>.Allocate();
		int num2 = 50;
		foreach (int num3 in Game.Instance.mingleCellTracker.mingleCells)
		{
			if (this.brain.IsCellClear(num3))
			{
				int navigationCost = this.navigator.GetNavigationCost(num3);
				if (navigationCost != -1)
				{
					if (num3 == Grid.InvalidCell || navigationCost < num)
					{
						this.cell = num3;
						num = navigationCost;
					}
					if (navigationCost < num2)
					{
						pooledList.Add(num3);
					}
				}
			}
		}
		if (pooledList.Count > 0)
		{
			this.cell = pooledList[global::UnityEngine.Random.Range(0, pooledList.Count)];
		}
		pooledList.Recycle();
	}

	public int GetCell()
	{
		return this.cell;
	}

	private MinionBrain brain;

	private Navigator navigator;

	private Schedulable scheduable;

	private int cell;
}
