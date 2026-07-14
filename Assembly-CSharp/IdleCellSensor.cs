using System;
using UnityEngine;

public class IdleCellSensor : Sensor
{
	public IdleCellSensor(Sensors sensors)
		: base(sensors)
	{
		this.navigator = base.GetComponent<Navigator>();
		this.brain = base.GetComponent<MinionBrain>();
		this.prefabid = base.GetComponent<KPrefabID>();
		this.brain.Subscribe(1589886948, new Action<object>(this.OnMinionSpawned));
	}

	private void OnMinionSpawned(object obj)
	{
		this.swimMonitor = this.brain.GetSMI<SwimMonitor.Instance>();
		this.canSwim = this.swimMonitor != null && this.swimMonitor.CanSwim();
		this.brain.Unsubscribe(1589886948);
	}

	public override void Update()
	{
		if (!this.prefabid.HasTag(GameTags.Idle))
		{
			this.cell = Grid.InvalidCell;
			return;
		}
		this.canSwim = this.swimMonitor != null && this.swimMonitor.CanSwim();
		MinionPathFinderAbilities minionPathFinderAbilities = (MinionPathFinderAbilities)this.navigator.GetCurrentAbilities();
		minionPathFinderAbilities.SetIdleNavMaskEnabled(true);
		IdleCellQuery idleCellQuery = PathFinderQueries.idleCellQuery.Reset(this.brain, global::UnityEngine.Random.Range(30, 60), this.canSwim);
		this.navigator.RunQuery(idleCellQuery);
		minionPathFinderAbilities.SetIdleNavMaskEnabled(false);
		this.cell = idleCellQuery.GetResultCell();
	}

	public int GetCell()
	{
		return this.cell;
	}

	private MinionBrain brain;

	private Navigator navigator;

	private SwimMonitor.Instance swimMonitor;

	private KPrefabID prefabid;

	private int cell;

	private bool canSwim;
}
