using System;
using KSerialization;

public class Immigration : KMonoBehaviour, ISaveLoadable, ISim200ms
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
			this.availableMinionStats = new MinionStartingStats(false);
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

	public float[] spawnInterval;

	public int[] spawnTable;

	private MinionStartingStats availableMinionStats;

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
