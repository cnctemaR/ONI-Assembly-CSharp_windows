using System;
using UnityEngine;

public class BalloonStandCellSensor : Sensor
{
	public BalloonStandCellSensor(Sensors sensors)
		: base(sensors)
	{
		this.navigator = base.GetComponent<Navigator>();
		this.brain = base.GetComponent<MinionBrain>();
	}

	public override void Update()
	{
		this.cell = Grid.InvalidCell;
		int num = int.MaxValue;
		ListPool<int, BalloonStandCellSensor>.PooledList pooledList = ListPool<int, BalloonStandCellSensor>.Allocate();
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
						int num4 = Grid.CellRight(num3);
						int num5 = Grid.CellRight(num4);
						int num6 = Grid.CellLeft(num3);
						int num7 = Grid.CellLeft(num6);
						CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(this.cell);
						CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(num7);
						CavityInfo cavityForCell3 = Game.Instance.roomProber.GetCavityForCell(num5);
						bool flag = false;
						if (cavityForCell != null && cavityForCell3 != null && cavityForCell2 != null)
						{
							flag = cavityForCell.handle == cavityForCell3.handle && cavityForCell.handle == cavityForCell2.handle;
						}
						if (flag && this.navigator.NavGrid.NavTable.IsValid(num4, NavType.Floor) && this.navigator.NavGrid.NavTable.IsValid(num6, NavType.Floor) && this.navigator.NavGrid.NavTable.IsValid(num5, NavType.Floor) && this.navigator.NavGrid.NavTable.IsValid(num7, NavType.Floor))
						{
							pooledList.Add(num3);
						}
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

	private int cell;
}
