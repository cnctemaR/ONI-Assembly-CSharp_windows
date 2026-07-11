using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SeasonManager : KMonoBehaviour, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDay));
		if (this.currentSeasonIndex >= this.SeasonLoop.Length)
		{
			this.currentSeasonIndex = this.SeasonLoop.Length - 1;
			this.currentSeasonsCyclesElapsed = int.MaxValue;
		}
		this.UpdateState();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (GameClock.Instance != null)
		{
			GameClock.Instance.Unsubscribe(631075836, new Action<object>(this.OnNewDay));
		}
	}

	private void OnNewDay(object data)
	{
		this.currentSeasonsCyclesElapsed++;
		this.UpdateState();
	}

	private void UpdateState()
	{
		SeasonManager.Season season = this.seasons[this.SeasonLoop[this.currentSeasonIndex]];
		if (this.currentSeasonsCyclesElapsed >= season.durationInCycles)
		{
			this.currentSeasonIndex = (this.currentSeasonIndex + 1) % this.SeasonLoop.Length;
			this.ResetSeasonProgress();
		}
	}

	private void ResetSeasonProgress()
	{
		SeasonManager.Season season = this.seasons[this.SeasonLoop[this.currentSeasonIndex]];
		this.currentSeasonsCyclesElapsed = 0;
		this.bombardmentOn = false;
		this.bombardmentPeriodRemaining = season.secondsBombardmentOff.Get();
		this.secondsUntilNextBombardment = season.secondsBetweenBombardments.Get();
	}

	public void Sim200ms(float dt)
	{
		SeasonManager.Season season = this.seasons[this.SeasonLoop[this.currentSeasonIndex]];
		this.bombardmentPeriodRemaining -= dt;
		if (this.bombardmentPeriodRemaining <= 0f)
		{
			float num = this.bombardmentPeriodRemaining;
			this.bombardmentOn = !this.bombardmentOn;
			this.bombardmentPeriodRemaining = ((!this.bombardmentOn) ? season.secondsBombardmentOff.Get() : season.secondsBombardmentOn.Get());
			if (this.bombardmentPeriodRemaining != 0f)
			{
				this.bombardmentPeriodRemaining += num;
			}
		}
		if (this.bombardmentOn && season.bombardmentInfo != null && season.bombardmentInfo.Length > 0)
		{
			if (this.activeMeteorBackground == null)
			{
				this.activeMeteorBackground = Util.KInstantiate(EffectPrefabs.Instance.MeteorBackground, null, null);
				this.activeMeteorBackground.transform.SetPosition(new Vector3(125f, 435f, 25f));
				this.activeMeteorBackground.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
			}
			this.secondsUntilNextBombardment -= dt;
			if (this.secondsUntilNextBombardment <= 0f)
			{
				float num2 = this.secondsUntilNextBombardment;
				this.DoBombardment(season.bombardmentInfo);
				this.secondsUntilNextBombardment = season.secondsBetweenBombardments.Get();
				if (this.secondsUntilNextBombardment != 0f)
				{
					this.secondsUntilNextBombardment += num2;
				}
			}
		}
		else if (this.activeMeteorBackground != null)
		{
			ParticleSystem component = this.activeMeteorBackground.GetComponent<ParticleSystem>();
			component.Stop();
			if (!component.IsAlive())
			{
				global::UnityEngine.Object.Destroy(this.activeMeteorBackground);
				this.activeMeteorBackground = null;
			}
		}
	}

	private void DoBombardment(SeasonManager.BombardmentInfo[] bombardment_info)
	{
		float num = 0f;
		foreach (SeasonManager.BombardmentInfo bombardmentInfo in bombardment_info)
		{
			num += bombardmentInfo.weight;
		}
		num = global::UnityEngine.Random.Range(0f, num);
		SeasonManager.BombardmentInfo bombardmentInfo2 = bombardment_info[0];
		int num2 = 0;
		while (num - bombardmentInfo2.weight > 0f)
		{
			num -= bombardmentInfo2.weight;
			bombardmentInfo2 = bombardment_info[++num2];
		}
		this.SpawnBombard(bombardmentInfo2.prefab);
	}

	private GameObject SpawnBombard(string prefab)
	{
		Vector3 vector = new Vector3(global::UnityEngine.Random.value * (float)Grid.WidthInCells, 1.2f * (float)Grid.HeightInCells, Grid.GetLayerZ(Grid.SceneLayer.FXFront));
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(prefab), vector, Quaternion.identity, null, null, true, 0);
		gameObject.SetActive(true);
		return gameObject;
	}

	public bool CurrentSeasonHasBombardment()
	{
		SeasonManager.Season season = this.seasons[this.SeasonLoop[this.currentSeasonIndex]];
		return season.bombardmentInfo != null && season.bombardmentInfo.Length > 0;
	}

	public float TimeUntilNextBombardment()
	{
		return (!this.CurrentSeasonHasBombardment()) ? float.MaxValue : ((!this.bombardmentOn) ? this.bombardmentPeriodRemaining : 0f);
	}

	public void ForceBeginMeteorSeasonWithShower()
	{
		for (int i = 0; i < this.SeasonLoop.Length; i++)
		{
			if (this.SeasonLoop[i] == "MeteorShower")
			{
				this.currentSeasonIndex = i;
			}
		}
		this.ResetSeasonProgress();
		SeasonManager.Season season = this.seasons[this.SeasonLoop[this.currentSeasonIndex]];
		this.bombardmentOn = true;
		this.bombardmentPeriodRemaining = season.secondsBombardmentOn.Get();
	}

	[ContextMenu("Bombard")]
	public void Debug_Bombardment()
	{
		SeasonManager.BombardmentInfo[] bombardmentInfo = this.seasons[this.SeasonLoop[this.currentSeasonIndex]].bombardmentInfo;
		this.DoBombardment(bombardmentInfo);
	}

	[ContextMenu("Force Shower")]
	public void Debug_ForceShower()
	{
		this.bombardmentOn = true;
		this.bombardmentPeriodRemaining = float.MaxValue;
		this.secondsUntilNextBombardment = 0f;
	}

	public void DrawDebugger()
	{
	}

	[Serialize]
	private int currentSeasonIndex = int.MaxValue;

	[Serialize]
	private int currentSeasonsCyclesElapsed = int.MaxValue;

	[Serialize]
	private float bombardmentPeriodRemaining;

	[Serialize]
	private bool bombardmentOn;

	[Serialize]
	private float secondsUntilNextBombardment;

	private GameObject activeMeteorBackground;

	private const string SEASONNAME_DEFAULT = "Default";

	private const string SEASONNAME_METEORSHOWER = "MeteorShower";

	private Dictionary<string, SeasonManager.Season> seasons = new Dictionary<string, SeasonManager.Season>
	{
		{
			"Default",
			new SeasonManager.Season
			{
				durationInCycles = 4
			}
		},
		{
			"MeteorShower",
			new SeasonManager.Season
			{
				durationInCycles = 10,
				secondsBombardmentOff = new MathUtil.MinMax(300f, 1200f),
				secondsBombardmentOn = new MathUtil.MinMax(100f, 400f),
				secondsBetweenBombardments = new MathUtil.MinMax(1f, 1.5f),
				meteorBackground = true,
				bombardmentInfo = new SeasonManager.BombardmentInfo[]
				{
					new SeasonManager.BombardmentInfo
					{
						prefab = IronCometConfig.ID,
						weight = 1f
					},
					new SeasonManager.BombardmentInfo
					{
						prefab = RockCometConfig.ID,
						weight = 1f
					},
					new SeasonManager.BombardmentInfo
					{
						prefab = DustCometConfig.ID,
						weight = 5f
					}
				}
			}
		}
	};

	private string[] SeasonLoop = new string[] { "Default", "MeteorShower" };

	private struct BombardmentInfo
	{
		public string prefab;

		public float weight;
	}

	private struct Season
	{
		public string name;

		public int durationInCycles;

		public MathUtil.MinMax secondsBombardmentOff;

		public MathUtil.MinMax secondsBombardmentOn;

		public MathUtil.MinMax secondsBetweenBombardments;

		public bool meteorBackground;

		public SeasonManager.BombardmentInfo[] bombardmentInfo;
	}
}
