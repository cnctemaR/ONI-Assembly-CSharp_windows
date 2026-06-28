using System;
using System.Collections.Generic;

public class Operational : KMonoBehaviour
{
	public bool IsOperational { get; private set; }

	public bool IsFunctional { get; private set; }

	public bool IsActive { get; private set; }

	protected override void OnPrefabInit()
	{
		this.UpdateFunctional();
		this.UpdateOperational();
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
				this.Trigger(187661686, flag);
			}
		}
		else
		{
			this.Flags[flag] = value;
			this.Trigger(187661686, flag);
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
		this.Trigger(-1852328367, this.IsFunctional);
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
			this.Trigger(-592767678, this.IsOperational);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	public void SetActive(bool value, bool force_ignore = false)
	{
		if (this.IsActive != value)
		{
			this.IsActive = value;
			this.Trigger(824508782, this.IsActive);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	public Dictionary<Operational.Flag, bool> Flags = new Dictionary<Operational.Flag, bool>();

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
