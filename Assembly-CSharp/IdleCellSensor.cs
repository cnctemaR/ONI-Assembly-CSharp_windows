using System;
using UnityEngine;

public class IdleCellSensor : Sensor
{
	public IdleCellSensor(Sensors sensors)
		: base(sensors)
	{
		this.navigator = base.GetComponent<Navigator>();
		this.brain = base.GetComponent<MinionBrain>();
	}

	public override void Update()
	{
		IdleCellQuery idleCellQuery = PathFinderQueries.idleCellQuery.Reset(this.brain, global::UnityEngine.Random.Range(30, 60));
		(this.brain.GetComponent<Navigator>().GetCurrentAbilities() as MinionPathFinderAbilities).SetIdleNavMaskEnabled(true);
		this.navigator.RunQuery(idleCellQuery);
		(this.brain.GetComponent<Navigator>().GetCurrentAbilities() as MinionPathFinderAbilities).SetIdleNavMaskEnabled(false);
		this.cell = idleCellQuery.GetResultCell();
	}

	public int GetCell()
	{
		return this.cell;
	}

	private MinionBrain brain;

	private Navigator navigator;

	private int cell;
}
