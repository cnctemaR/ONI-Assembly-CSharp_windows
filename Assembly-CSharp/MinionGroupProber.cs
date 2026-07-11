using System;
using System.Collections.Generic;

public class MinionGroupProber : KMonoBehaviour, IGroupProber
{
	public static void DestroyInstance()
	{
		MinionGroupProber.Instance = null;
	}

	public static MinionGroupProber Get()
	{
		return MinionGroupProber.Instance;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MinionGroupProber.Instance = this;
		this.cells = new Dictionary<object, int>[Grid.CellCount];
	}

	public bool IsReachable(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		Dictionary<object, int> dictionary = this.cells[cell];
		if (dictionary == null)
		{
			return false;
		}
		bool flag = false;
		object obj = this.access;
		lock (obj)
		{
			this.pending_removals.Clear();
			foreach (KeyValuePair<object, int> keyValuePair in dictionary)
			{
				object key = keyValuePair.Key;
				int value = keyValuePair.Value;
				KeyValuePair<int, int> keyValuePair2;
				if (this.valid_serial_nos.TryGetValue(key, out keyValuePair2) && (value == keyValuePair2.Key || value == keyValuePair2.Value))
				{
					flag = true;
					break;
				}
				this.pending_removals.Add(key);
			}
			foreach (object obj2 in this.pending_removals)
			{
				dictionary.Remove(obj2);
				if (dictionary.Count == 0)
				{
					this.cells[cell] = null;
				}
			}
		}
		return flag;
	}

	public bool IsReachable(int cell, CellOffset[] offsets)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		foreach (CellOffset cellOffset in offsets)
		{
			if (this.IsReachable(Grid.OffsetCell(cell, cellOffset)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsReachable(Workable workable)
	{
		return this.IsReachable(Grid.PosToCell(workable), workable.GetOffsets());
	}

	public void Occupy(object prober, int serial_no, List<int> cells)
	{
		object obj = this.access;
		lock (obj)
		{
			foreach (int num in cells)
			{
				if (this.cells[num] == null)
				{
					this.cells[num] = new Dictionary<object, int>();
				}
				this.cells[num][prober] = serial_no;
			}
		}
	}

	public void SetValidSerialNos(object prober, int previous_serial_no, int serial_no)
	{
		object obj = this.access;
		lock (obj)
		{
			this.valid_serial_nos[prober] = new KeyValuePair<int, int>(previous_serial_no, serial_no);
		}
	}

	public bool ReleaseProber(object prober)
	{
		object obj = this.access;
		bool flag;
		lock (obj)
		{
			flag = this.valid_serial_nos.Remove(prober);
		}
		return flag;
	}

	private static MinionGroupProber Instance;

	private Dictionary<object, int>[] cells;

	private Dictionary<object, KeyValuePair<int, int>> valid_serial_nos = new Dictionary<object, KeyValuePair<int, int>>();

	private List<object> pending_removals = new List<object>();

	private readonly object access = new object();
}
