using System;

public class SafeCellSensor : Sensor
{
	public SafeCellSensor(Sensors sensors)
		: base(sensors)
	{
		this.navigator = base.GetComponent<Navigator>();
		this.brain = base.GetComponent<MinionBrain>();
		this.prefabid = base.GetComponent<KPrefabID>();
	}

	public override void Update()
	{
		if (!this.prefabid.HasTag(GameTags.Idle))
		{
			this.cell = Grid.InvalidCell;
			return;
		}
		SafeCellQuery safeCellQuery = PathFinderQueries.safeCellQuery.Reset(this.brain);
		this.navigator.RunQuery(safeCellQuery);
		bool flag = this.HasSafeCell();
		this.cell = safeCellQuery.GetResultCell();
		if (this.cell == Grid.PosToCell(this.navigator))
		{
			this.cell = Grid.InvalidCell;
		}
		bool flag2 = this.HasSafeCell();
		if (flag2 != flag)
		{
			if (flag2)
			{
				this.sensors.Trigger(982561777, null);
			}
			else
			{
				this.sensors.Trigger(506919987, null);
			}
		}
	}

	public int GetCell()
	{
		return this.cell;
	}

	public bool HasSafeCell()
	{
		return this.cell != Grid.InvalidCell && this.cell != Grid.PosToCell(this.sensors);
	}

	private MinionBrain brain;

	private Navigator navigator;

	private KPrefabID prefabid;

	private int cell = Grid.InvalidCell;
}
