using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GameClock : KMonoBehaviour, ISaveLoadable, ISim33ms, IRender1000ms
{
	public static void DestroyInstance()
	{
		GameClock.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		GameClock.Instance = this;
		this.timeSinceStartOfCycle = 50f;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.time != 0f)
		{
			this.cycle = (int)(this.time / 600f);
			this.timeSinceStartOfCycle = Mathf.Max(this.time - (float)this.cycle * 600f, 0f);
			this.time = 0f;
		}
	}

	public void Sim33ms(float dt)
	{
		this.AddTime(dt);
	}

	public void Render1000ms(float dt)
	{
		this.timePlayed += dt;
	}

	private void LateUpdate()
	{
		this.frame++;
	}

	private void AddTime(float dt)
	{
		this.timeSinceStartOfCycle += dt;
		bool flag = false;
		while (this.timeSinceStartOfCycle >= 600f)
		{
			this.cycle++;
			this.timeSinceStartOfCycle -= 600f;
			base.Trigger(631075836, null);
			flag = true;
		}
		if (!this.isNight && this.IsNighttime())
		{
			this.isNight = true;
			base.Trigger(-722330267, null);
		}
		if (this.isNight && !this.IsNighttime())
		{
			this.isNight = false;
		}
		if (flag && SaveGame.Instance.autoSaveCycleInterval > 0 && this.cycle % SaveGame.Instance.autoSaveCycleInterval == 0)
		{
			this.DoAutoSave(this.cycle);
		}
	}

	public float GetTimeSinceStartOfReport()
	{
		if (this.IsNighttime())
		{
			return 525f - this.GetTimeSinceStartOfCycle();
		}
		return this.GetTimeSinceStartOfCycle() + 75f;
	}

	public float GetTimeSinceStartOfCycle()
	{
		return this.timeSinceStartOfCycle;
	}

	public float GetCurrentCycleAsPercentage()
	{
		return this.timeSinceStartOfCycle / 600f;
	}

	public float GetTime()
	{
		return this.timeSinceStartOfCycle + (float)this.cycle * 600f;
	}

	public int GetFrame()
	{
		return this.frame;
	}

	public int GetCycle()
	{
		return this.cycle;
	}

	public bool IsNighttime()
	{
		return GameClock.Instance.GetCurrentCycleAsPercentage() >= 0.875f;
	}

	public void SetTime(float new_time)
	{
		float num = Mathf.Max(new_time - this.GetTime(), 0f);
		this.AddTime(num);
	}

	public float GetTimePlayedInSeconds()
	{
		return this.timePlayed;
	}

	private void DoAutoSave(int day)
	{
		if (GenericGameSettings.instance.disableAutosave)
		{
			return;
		}
		day++;
		this.newDayMetric[GameClock.NewCycleKey] = day;
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(this.newDayMetric);
		string text = SaveLoader.GetActiveSaveFilePath();
		if (text == null)
		{
			text = SaveLoader.GetAutosaveFilePath();
		}
		int num = text.LastIndexOf("\\");
		if (num > 0)
		{
			int num2 = text.IndexOf(" Cycle ", num);
			if (num2 > 0)
			{
				text = text.Substring(0, num2);
			}
		}
		text = text.Replace(".sav", string.Empty);
		text = text + " Cycle " + day.ToString();
		text = SaveScreen.GetValidSaveFilename(text);
		string autoSavePrefix = SaveLoader.GetAutoSavePrefix();
		text = Path.Combine(autoSavePrefix, Path.GetFileName(text));
		string text2 = text;
		int num3 = 1;
		while (File.Exists(text))
		{
			text = text2.Replace(".sav", string.Empty);
			text = SaveScreen.GetValidSaveFilename(string.Concat(new object[] { text2, " (", num3, ")" }));
			num3++;
		}
		Game.Instance.StartDelayedSave(text, true, false);
	}

	public static GameClock Instance;

	[Serialize]
	private int frame;

	[Serialize]
	private float time;

	[Serialize]
	private float timeSinceStartOfCycle;

	[Serialize]
	private int cycle;

	[Serialize]
	private float timePlayed;

	[Serialize]
	private bool isNight;

	public static readonly string NewCycleKey = "NewCycle";

	private Dictionary<string, object> newDayMetric = new Dictionary<string, object> { 
	{
		GameClock.NewCycleKey,
		null
	} };
}
