using System;
using System.Collections.Generic;
using Klei.AI;

public class SafeCellSensor : Sensor
{
	private SafeCellQuery.SafeFlags GetIgnoredFlags()
	{
		SafeCellQuery.SafeFlags safeFlags = (SafeCellQuery.SafeFlags)0;
		foreach (string text in this.ignoredFlagsSets.Keys)
		{
			SafeCellQuery.SafeFlags safeFlags2 = this.ignoredFlagsSets[text];
			safeFlags |= safeFlags2;
		}
		return safeFlags;
	}

	public void AddIgnoredFlagsSet(string setID, SafeCellQuery.SafeFlags flagsToIgnore)
	{
		if (this.ignoredFlagsSets.ContainsKey(setID))
		{
			this.ignoredFlagsSets[setID] = flagsToIgnore;
			return;
		}
		this.ignoredFlagsSets.Add(setID, flagsToIgnore);
	}

	public void RemoveIgnoredFlagsSet(string setID)
	{
		if (this.ignoredFlagsSets.ContainsKey(setID))
		{
			this.ignoredFlagsSets.Remove(setID);
		}
	}

	public SafeCellSensor(Sensors sensors, bool startEnabled = true)
		: base(sensors, startEnabled)
	{
		this.navigator = base.GetComponent<Navigator>();
		this.brain = base.GetComponent<MinionBrain>();
		this.prefabid = base.GetComponent<KPrefabID>();
		this.traits = base.GetComponent<Traits>();
	}

	public override void Update()
	{
		if (!this.prefabid.HasTag(GameTags.Idle))
		{
			this.cell = Grid.InvalidCell;
			return;
		}
		bool flag = this.HasSafeCell();
		this.RunSafeCellQuery(false);
		bool flag2 = this.HasSafeCell();
		if (flag2 != flag)
		{
			if (flag2)
			{
				this.sensors.Trigger(982561777, null);
				return;
			}
			this.sensors.Trigger(506919987, null);
		}
	}

	public void RunSafeCellQuery(bool avoid_light)
	{
		this.cell = this.RunAndGetSafeCellQueryResult(avoid_light);
		if (this.cell == Grid.PosToCell(this.navigator))
		{
			this.cell = Grid.InvalidCell;
		}
	}

	public int RunAndGetSafeCellQueryResult(bool avoid_light)
	{
		MinionPathFinderAbilities minionPathFinderAbilities = (MinionPathFinderAbilities)this.navigator.GetCurrentAbilities();
		minionPathFinderAbilities.SetIdleNavMaskEnabled(true);
		SafeCellQuery safeCellQuery = PathFinderQueries.safeCellQuery.Reset(this.brain, avoid_light, this.GetIgnoredFlags());
		this.navigator.RunQuery(safeCellQuery);
		minionPathFinderAbilities.SetIdleNavMaskEnabled(false);
		this.cell = safeCellQuery.GetResultCell();
		return this.cell;
	}

	public int GetSensorCell()
	{
		return this.cell;
	}

	public int GetCellQuery()
	{
		if (this.cell == Grid.InvalidCell)
		{
			this.RunSafeCellQuery(false);
		}
		return this.cell;
	}

	public int GetSleepCellQuery()
	{
		if (this.cell == Grid.InvalidCell)
		{
			this.RunSafeCellQuery(!this.traits.HasTrait("NightLight"));
		}
		return this.cell;
	}

	public bool HasSafeCell()
	{
		return this.cell != Grid.InvalidCell && this.cell != Grid.PosToCell(this.sensors);
	}

	private MinionBrain brain;

	private Navigator navigator;

	private KPrefabID prefabid;

	private Traits traits;

	private int cell = Grid.InvalidCell;

	private Dictionary<string, SafeCellQuery.SafeFlags> ignoredFlagsSets = new Dictionary<string, SafeCellQuery.SafeFlags>();
}
