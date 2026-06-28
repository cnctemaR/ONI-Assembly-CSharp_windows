using System;
using System.Collections.Generic;

public class LinearDictionary<Key, Val> where Key : IEquatable<Key>
{
	private int GetIdx(Key key)
	{
		int count = this.keys.Count;
		int num = -1;
		for (int i = 0; i < count; i++)
		{
			Key key2 = this.keys[i];
			if (key2.Equals(key))
			{
				num = i;
				break;
			}
		}
		return num;
	}

	public Val this[Key key]
	{
		get
		{
			Val val = default(Val);
			int idx = this.GetIdx(key);
			if (idx != -1)
			{
				val = this.values[idx];
			}
			return val;
		}
		set
		{
			int idx = this.GetIdx(key);
			if (idx != -1)
			{
				this.values[idx] = value;
			}
			else
			{
				this.keys.Add(key);
				this.values.Add(value);
			}
		}
	}

	private List<Key> keys = new List<Key>();

	private List<Val> values = new List<Val>();
}
