using System;
using KSerialization;
using UnityEngine;

public class Immigration : KMonoBehaviour, ISaveLoadableJson
{
	public MinionStartingStats MinionStats
	{
		get
		{
			return this.availableMinionStats;
		}
	}

	protected override void OnPrefabInit()
	{
		this.bImmigrantAvailable = false;
		Immigration.Instance = this;
		int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
		this.timeBeforeSpawn = this.spawnInterval[num];
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.bImmigrantAvailable)
		{
			this.availableMinionStats = new MinionStartingStats(false);
		}
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
		int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
		return this.spawnTable[num];
	}

	public float GetTimeRemaining()
	{
		return this.timeBeforeSpawn;
	}

	private void Update()
	{
		this.timeBeforeSpawn -= Time.deltaTime;
		if (this.timeBeforeSpawn <= 0f)
		{
			this.bImmigrantAvailable = true;
			this.availableMinionStats = new MinionStartingStats(false);
			this.spawnIdx++;
			int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
			this.timeBeforeSpawn = this.spawnInterval[num];
		}
	}

	public float[] spawnInterval;

	public int[] spawnTable;

	private MinionStartingStats availableMinionStats;

	[Serialize]
	public float timeBeforeSpawn = float.PositiveInfinity;

	[Serialize]
	private bool bImmigrantAvailable;

	[Serialize]
	private int spawnIdx;

	public static Immigration Instance;
}
