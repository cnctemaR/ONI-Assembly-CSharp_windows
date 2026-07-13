using System;
using System.Collections.Generic;

public class HybridListHashSet<DataType>
{
	public HybridListHashSet()
	{
		this.dict = new Dictionary<DataType, int>();
		this.list = new List<DataType>();
	}

	public HybridListHashSet(int capacity)
	{
		this.dict = new Dictionary<DataType, int>(capacity);
		this.list = new List<DataType>(capacity);
	}

	public bool Add(DataType data)
	{
		if (this.dict.ContainsKey(data))
		{
			return false;
		}
		this.dict[data] = this.list.Count;
		this.list.Add(data);
		return true;
	}

	public int Add(List<DataType> otherlist)
	{
		int num = 0;
		foreach (DataType dataType in otherlist)
		{
			if (this.Add(dataType))
			{
				num++;
			}
		}
		return num;
	}

	public int UnionWith(HybridListHashSet<DataType> other)
	{
		return this.Add(other.list);
	}

	public bool Remove(DataType data)
	{
		int num;
		if (this.dict.TryGetValue(data, out num))
		{
			this.dict.Remove(data);
			this.list.RemoveAtSwap<DataType>(num);
			if (num < this.list.Count)
			{
				this.dict[this.list[num]] = num;
			}
			return true;
		}
		return false;
	}

	public int Remove(List<DataType> exclude)
	{
		int num = 0;
		foreach (DataType dataType in exclude)
		{
			if (this.Remove(dataType))
			{
				num++;
			}
		}
		return num;
	}

	public int Exclude(HybridListHashSet<DataType> other)
	{
		return this.Remove(other.list);
	}

	public bool Contains(DataType data)
	{
		return this.dict.ContainsKey(data);
	}

	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	public DataType this[int index]
	{
		get
		{
			return this.list[index];
		}
	}

	public void Clear()
	{
		this.dict.Clear();
		this.list.Clear();
	}

	public void Swap(HybridListHashSet<DataType> other)
	{
		Util.Swap<Dictionary<DataType, int>>(ref this.dict, ref other.dict);
		Util.Swap<List<DataType>>(ref this.list, ref other.list);
	}

	private Dictionary<DataType, int> dict;

	private List<DataType> list;
}
