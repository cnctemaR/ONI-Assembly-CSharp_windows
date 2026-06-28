using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GameClock : KMonoBehaviour, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		GameClock.Instance = this;
		this.time = 50f;
	}

	private void Update()
	{
		this.UpdateTime(Time.deltaTime);
		this.timePlayed += Time.unscaledDeltaTime;
	}

	private void LateUpdate()
	{
		this.frame++;
	}

	private void UpdateTime(float dt)
	{
		int day = this.GetDay();
		this.time += dt;
		int day2 = this.GetDay();
		int num = day2 - day;
		if (!this.isNight && this.IsNighttime())
		{
			this.isNight = true;
			this.Trigger(-722330267, null);
		}
		if (this.isNight && !this.IsNighttime())
		{
			this.isNight = false;
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				this.Trigger(631075836, null);
			}
			if (day2 != 0 && day2 % 1 == 0)
			{
				this.DoAutoSave(day2);
			}
		}
	}

	public float GetSecondsSinceStartOfDay()
	{
		return this.time - (float)this.GetDay() * 600f;
	}

	public float GetCurrentDayAsPercentage()
	{
		return this.GetSecondsSinceStartOfDay() / 600f;
	}

	public float GetTime()
	{
		return this.time;
	}

	public int GetFrame()
	{
		return this.frame;
	}

	public int GetDay()
	{
		return (int)(this.time / 600f);
	}

	public bool IsNighttime()
	{
		return GameClock.Instance.GetCurrentDayAsPercentage() >= 0.875f;
	}

	public void SetTime(float new_time)
	{
		float num = Mathf.Max(new_time - this.time, 0f);
		this.UpdateTime(num);
		this.time = new_time;
	}

	public float GetTimePlayedInSeconds()
	{
		return this.timePlayed;
	}

	private void DoAutoSave(int day)
	{
		day++;
		this.newDayMetric[GameClock.NewCycleKey] = day;
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(this.newDayMetric);
		string text = SaveLoader.GetActiveSaveFilePath();
		if (text == null)
		{
			text = SaveLoader.GetAutosaveFilePath();
		}
		text = text.Replace(".sav", string.Empty);
		text = text + " Cycle " + day.ToString();
		text = SaveScreen.GetValidSaveFilename(text);
		string autoSavePrefix = SaveLoader.GetAutoSavePrefix();
		text = autoSavePrefix + Path.GetFileName(text);
		int num = 1;
		while (File.Exists(text))
		{
			text = text.Replace(".sav", string.Empty);
			text = SaveScreen.GetValidSaveFilename(string.Concat(new object[] { text, " (", num, ")" }));
			num++;
		}
		Game.Instance.StartDelayedSave(text, true, false);
	}

	public static GameClock Instance;

	[Serialize]
	private int frame;

	[Serialize]
	private float time;

	[Serialize]
	private float timePlayed;

	private bool isNight;

	public static readonly string NewCycleKey = "NewCycle";

	private Dictionary<string, object> newDayMetric = new Dictionary<string, object> { 
	{
		GameClock.NewCycleKey,
		null
	} };
}
