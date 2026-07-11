using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

public class Operational : KMonoBehaviour
{
	public bool IsOperational { get; private set; }

	public bool IsFunctional { get; private set; }

	public bool IsActive { get; private set; }

	[OnSerializing]
	private void OnSerializing()
	{
		float num = ((!this.IsActive) ? this.inactiveStartTime : this.activeStartTime);
		List<Operational.TimeEntry> list = ((!this.IsActive) ? this.inactiveTimes : this.activeTimes);
		float time = GameClock.Instance.GetTime();
		this.AddTimeEntry(list, num, time);
		this.activeStartTime = GameClock.Instance.GetTime();
		this.inactiveStartTime = GameClock.Instance.GetTime();
	}

	protected override void OnPrefabInit()
	{
		this.UpdateFunctional();
		this.UpdateOperational();
		base.Subscribe<Operational>(-1661515756, Operational.OnNewBuildingDelegate);
	}

	public void OnNewBuilding(object data)
	{
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.creationTime > 0f)
		{
			this.inactiveStartTime = component.creationTime;
			this.activeStartTime = component.creationTime;
			this.activeTimes.Clear();
			this.inactiveTimes.Clear();
		}
	}

	public bool IsOperationalType(Operational.Flag.Type type)
	{
		if (type == Operational.Flag.Type.Functional)
		{
			return this.IsFunctional;
		}
		return this.IsOperational;
	}

	public void SetFlag(Operational.Flag flag, bool value)
	{
		bool flag2 = false;
		if (this.Flags.TryGetValue(flag, out flag2))
		{
			if (flag2 != value)
			{
				this.Flags[flag] = value;
				base.Trigger(187661686, flag);
			}
		}
		else
		{
			this.Flags[flag] = value;
			base.Trigger(187661686, flag);
		}
		if (flag.FlagType == Operational.Flag.Type.Functional && value != this.IsFunctional)
		{
			this.UpdateFunctional();
		}
		if (value != this.IsOperational)
		{
			this.UpdateOperational();
		}
	}

	public bool GetFlag(Operational.Flag flag)
	{
		bool flag2 = false;
		this.Flags.TryGetValue(flag, out flag2);
		return flag2;
	}

	private void UpdateFunctional()
	{
		bool flag = true;
		foreach (KeyValuePair<Operational.Flag, bool> keyValuePair in this.Flags)
		{
			if (keyValuePair.Key.FlagType == Operational.Flag.Type.Functional && !keyValuePair.Value)
			{
				flag = false;
				break;
			}
		}
		this.IsFunctional = flag;
		base.Trigger(-1852328367, this.IsFunctional);
	}

	private void UpdateOperational()
	{
		Dictionary<Operational.Flag, bool>.Enumerator enumerator = this.Flags.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			KeyValuePair<Operational.Flag, bool> keyValuePair = enumerator.Current;
			if (!keyValuePair.Value)
			{
				flag = false;
				break;
			}
		}
		if (flag != this.IsOperational)
		{
			this.IsOperational = flag;
			if (!this.IsOperational)
			{
				this.SetActive(false, false);
			}
			if (this.IsOperational)
			{
				base.GetComponent<KPrefabID>().AddTag(GameTags.Operational, false);
			}
			else
			{
				base.GetComponent<KPrefabID>().RemoveTag(GameTags.Operational);
			}
			base.Trigger(-592767678, this.IsOperational);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	public void SetActive(bool value, bool force_ignore = false)
	{
		if (this.IsActive != value)
		{
			float num = ((!this.IsActive) ? this.inactiveStartTime : this.activeStartTime);
			List<Operational.TimeEntry> list = ((!this.IsActive) ? this.inactiveTimes : this.activeTimes);
			float time = GameClock.Instance.GetTime();
			this.AddTimeEntry(list, num, time);
			this.IsActive = value;
			if (this.IsActive)
			{
				this.activeStartTime = time;
			}
			else
			{
				this.inactiveStartTime = time;
			}
			base.Trigger(824508782, this);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	private void AddTimeEntry(List<Operational.TimeEntry> timeEntries, float startingTime, float endingTime)
	{
		if (startingTime != endingTime)
		{
			timeEntries.Add(new Operational.TimeEntry(startingTime, endingTime));
		}
	}

	private void ValidateTimeEntries(List<Operational.TimeEntry> timeEntries)
	{
		if (timeEntries.Count > 2)
		{
			for (int i = timeEntries.Count - 1; i > 0; i--)
			{
				Operational.TimeEntry timeEntry = timeEntries[i];
				for (int j = i - 1; j >= 0; j--)
				{
					Operational.TimeEntry timeEntry2 = timeEntries[j];
					if (timeEntry.startTime < timeEntry2.endTime || timeEntry.startTime == timeEntry2.startTime)
					{
						global::Debug.Assert(false, "ENTRY TIMES OVERLAP!");
					}
				}
			}
		}
	}

	public float GetUptimeForTimeSpan(float duration = 600f)
	{
		float num = this.SumTimesForTimeSpawn(this.activeTimes, duration);
		float num2 = this.SumTimesForTimeSpawn(this.inactiveTimes, duration);
		float num3 = GameClock.Instance.GetTime() - duration;
		if (this.IsActive)
		{
			num += GameClock.Instance.GetTime() - Mathf.Max(this.activeStartTime, num3);
		}
		else
		{
			num2 += GameClock.Instance.GetTime() - Mathf.Max(this.inactiveStartTime, num3);
		}
		float num4 = num + num2;
		global::Debug.Assert(num4 <= duration, "totalTime is greater than allowed duration!");
		if (num == 0f || num4 == 0f)
		{
			return 0f;
		}
		if (num2 == 0f)
		{
			return 1f;
		}
		return num / num4;
	}

	private float SumTimesForTimeSpawn(List<Operational.TimeEntry> times, float duration)
	{
		float num = GameClock.Instance.GetTime() - duration;
		float num2 = 0f;
		foreach (Operational.TimeEntry timeEntry in times)
		{
			if (timeEntry.startTime >= num || timeEntry.endTime >= num)
			{
				if (timeEntry.startTime < num && timeEntry.endTime >= num)
				{
					num2 += timeEntry.endTime - num;
				}
				else
				{
					num2 += timeEntry.endTime - timeEntry.startTime;
				}
			}
		}
		return num2;
	}

	[Serialize]
	public float inactiveStartTime;

	[Serialize]
	public float activeStartTime;

	[Serialize]
	private List<Operational.TimeEntry> activeTimes = new List<Operational.TimeEntry>();

	[Serialize]
	private List<Operational.TimeEntry> inactiveTimes = new List<Operational.TimeEntry>();

	public Dictionary<Operational.Flag, bool> Flags = new Dictionary<Operational.Flag, bool>();

	private static readonly EventSystem.IntraObjectHandler<Operational> OnNewBuildingDelegate = new EventSystem.IntraObjectHandler<Operational>(delegate(Operational component, object data)
	{
		component.OnNewBuilding(data);
	});

	public class Flag
	{
		public Flag(string name, Operational.Flag.Type type)
		{
			this.Name = name;
			this.FlagType = type;
		}

		public string Name;

		public Operational.Flag.Type FlagType;

		public enum Type
		{
			Requirement,
			Functional
		}
	}

	public struct TimeEntry
	{
		public TimeEntry(float start, float end)
		{
			this.startTime = start;
			this.endTime = end;
		}

		public float startTime;

		public float endTime;
	}
}
