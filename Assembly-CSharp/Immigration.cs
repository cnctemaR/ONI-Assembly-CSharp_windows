using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class Immigration : KMonoBehaviour, ISaveLoadable, ISim200ms, IPersonalPriorityManager
{
	public static void DestroyInstance()
	{
		Immigration.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		this.bImmigrantAvailable = false;
		Immigration.Instance = this;
		int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
		this.timeBeforeSpawn = this.spawnInterval[num];
		this.ResetPersonalPriorities();
	}

	public bool ImmigrantsAvailable
	{
		get
		{
			return this.bImmigrantAvailable;
		}
	}

	public int SpawnMinions()
	{
		this.bImmigrantAvailable = false;
		this.spawnIdx++;
		int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
		this.timeBeforeSpawn = this.spawnInterval[num];
		return this.spawnTable[num];
	}

	public float GetTimeRemaining()
	{
		return this.timeBeforeSpawn;
	}

	public float GetTotalWaitTime()
	{
		int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
		return this.spawnInterval[num];
	}

	public void Sim200ms(float dt)
	{
		if (this.stopped || this.bImmigrantAvailable)
		{
			return;
		}
		this.timeBeforeSpawn -= dt;
		this.timeBeforeSpawn = Math.Max(this.timeBeforeSpawn, 0f);
		if (this.timeBeforeSpawn <= 0f)
		{
			this.bImmigrantAvailable = true;
		}
	}

	public void Stop()
	{
		this.stopped = true;
		this.bImmigrantAvailable = false;
		this.timeBeforeSpawn = this.spawnInterval[Math.Min(this.spawnIdx, this.spawnInterval.Length - 1)];
	}

	public void Restart()
	{
		this.stopped = false;
	}

	public int GetPersonalPriority(ChoreGroup group, out bool auto_assigned)
	{
		auto_assigned = false;
		int num;
		if (!this.defaultPersonalPriorities.TryGetValue(group.IdHash, out num))
		{
			num = 3;
		}
		return num;
	}

	public void SetPersonalPriority(ChoreGroup group, int value, bool is_auto_assigned)
	{
		this.defaultPersonalPriorities[group.IdHash] = value;
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return 0;
	}

	public bool CanRoleManageChoreGroup(ChoreGroup group)
	{
		return false;
	}

	public void ApplyDefaultPersonalPriorities(GameObject minion)
	{
		IPersonalPriorityManager instance = Immigration.Instance;
		IPersonalPriorityManager component = minion.GetComponent<ChoreConsumer>();
		foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups.resources)
		{
			bool flag;
			int personalPriority = instance.GetPersonalPriority(choreGroup, out flag);
			component.SetPersonalPriority(choreGroup, personalPriority, false);
		}
	}

	public void ResetPersonalPriorities()
	{
		bool advancedPersonalPriorities = Game.Instance.advancedPersonalPriorities;
		foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups.resources)
		{
			this.defaultPersonalPriorities[choreGroup.IdHash] = ((!advancedPersonalPriorities) ? 3 : choreGroup.DefaultPersonalPriority);
		}
	}

	public bool IsChoreGroupDisabled(ChoreGroup g)
	{
		return false;
	}

	public float[] spawnInterval;

	public int[] spawnTable;

	[Serialize]
	private Dictionary<HashedString, int> defaultPersonalPriorities = new Dictionary<HashedString, int>();

	[Serialize]
	public float timeBeforeSpawn = float.PositiveInfinity;

	[Serialize]
	private bool bImmigrantAvailable;

	[Serialize]
	private int spawnIdx;

	[Serialize]
	private bool stopped;

	public static Immigration Instance;
}
