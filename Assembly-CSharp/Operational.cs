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
		this.AddTimeData(this.IsActive);
		this.activeStartTime = GameClock.Instance.GetTime();
		this.inactiveStartTime = GameClock.Instance.GetTime();
	}

	protected override void OnPrefabInit()
	{
		this.UpdateFunctional();
		this.UpdateOperational();
		base.Subscribe<Operational>(-1661515756, Operational.OnNewBuildingDelegate);
		GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDay));
	}

	public void OnNewBuilding(object data)
	{
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.creationTime > 0f)
		{
			this.inactiveStartTime = component.creationTime;
			this.activeStartTime = component.creationTime;
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
			this.AddTimeData(value);
			base.Trigger(824508782, this);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	private void AddTimeData(bool value)
	{
		float num = ((!this.IsActive) ? this.inactiveStartTime : this.activeStartTime);
		float time = GameClock.Instance.GetTime();
		float num2 = time - num;
		if (this.IsActive)
		{
			this.activeTime += num2;
		}
		else
		{
			this.inactiveTime += num2;
		}
		this.IsActive = value;
		if (this.IsActive)
		{
			this.activeStartTime = time;
		}
		else
		{
			this.inactiveStartTime = time;
		}
	}

	public void OnNewDay(object data)
	{
		this.AddTimeData(this.IsActive);
		this.uptimeData.Add(this.activeTime / 600f);
		while (this.uptimeData.Count > this.MAX_DATA_POINTS)
		{
			this.uptimeData.RemoveAt(0);
		}
		this.activeTime = 0f;
		this.inactiveTime = 0f;
	}

	public float GetCurrentCycleUptime()
	{
		if (this.IsActive)
		{
			float num = ((!this.IsActive) ? this.inactiveStartTime : this.activeStartTime);
			float time = GameClock.Instance.GetTime();
			float num2 = time - num;
			return (this.activeTime + num2) / GameClock.Instance.GetTimeSinceStartOfCycle();
		}
		return this.activeTime / GameClock.Instance.GetTimeSinceStartOfCycle();
	}

	public float GetLastCycleUptime()
	{
		if (this.uptimeData.Count > 0)
		{
			return this.uptimeData[this.uptimeData.Count - 1];
		}
		return 0f;
	}

	public float GetUptimeOverCycles(int num_cycles)
	{
		if (this.uptimeData.Count > 0)
		{
			int num = Mathf.Min(this.uptimeData.Count, num_cycles);
			float num2 = 0f;
			for (int i = num - 1; i >= 0; i--)
			{
				num2 += this.uptimeData[i];
			}
			return num2 / (float)num;
		}
		return 0f;
	}

	[Serialize]
	public float inactiveStartTime;

	[Serialize]
	public float activeStartTime;

	[Serialize]
	private List<float> uptimeData = new List<float>();

	[Serialize]
	private float activeTime;

	[Serialize]
	private float inactiveTime;

	private int MAX_DATA_POINTS = 5;

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
}
