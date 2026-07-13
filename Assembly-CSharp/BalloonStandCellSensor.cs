using System;
using UnityEngine;

public class BalloonStandCellSensor : Sensor
{
	public BalloonStandCellSensor(Sensors sensors)
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
		ListPool<int[], BalloonStandCellSensor>.PooledList pooledList = ListPool<int[], BalloonStandCellSensor>.Allocate();
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
						if (cavityForCell != null)
						{
							if (cavityForCell3 != null && cavityForCell3.handle == cavityForCell.handle && this.navigator.NavGrid.NavTable.IsValid(num4, NavType.Floor) && this.navigator.NavGrid.NavTable.IsValid(num5, NavType.Floor))
							{
								pooledList.Add(new int[] { num3, num5 });
							}
							if (cavityForCell2 != null && cavityForCell2.handle == cavityForCell.handle && this.navigator.NavGrid.NavTable.IsValid(num6, NavType.Floor) && this.navigator.NavGrid.NavTable.IsValid(num7, NavType.Floor))
							{
								pooledList.Add(new int[] { num3, num7 });
							}
						}
					}
				}
			}
		}
		if (pooledList.Count > 0)
		{
			int[] array = pooledList[global::UnityEngine.Random.Range(0, pooledList.Count)];
			this.cell = array[0];
			this.standCell = array[1];
		}
		else if (Components.Telepads.Count > 0)
		{
			Telepad telepad = Components.Telepads.Items[0];
			if (telepad == null || !telepad.GetComponent<Operational>().IsOperational)
			{
				return;
			}
			int num8 = Grid.PosToCell(telepad.transform.GetPosition());
			num8 = Grid.CellLeft(num8);
			int num9 = Grid.CellRight(num8);
			int num10 = Grid.CellRight(num9);
			bool cavityForCell4 = Game.Instance.roomProber.GetCavityForCell(num8) != null;
			CavityInfo cavityForCell5 = Game.Instance.roomProber.GetCavityForCell(num10);
			if (cavityForCell4 && cavityForCell5 != null && this.navigator.GetNavigationCost(num8) != -1 && this.navigator.GetNavigationCost(num9) != -1 && this.navigator.GetNavigationCost(num10) != -1)
			{
				this.cell = num8;
				this.standCell = num10;
			}
		}
		pooledList.Recycle();
	}

	public int GetCell()
	{
		return this.cell;
	}

	public int GetStandCell()
	{
		return this.standCell;
	}

	private MinionBrain brain;

	private Navigator navigator;

	private Schedulable scheduable;

	private int cell;

	private int standCell;
}
