using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MinionGroupProber")]
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

	private bool IsReachable_AssumeLock(int cell)
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
		foreach (object obj in this.pending_removals)
		{
			dictionary.Remove(obj);
			if (dictionary.Count == 0)
			{
				this.cells[cell] = null;
			}
		}
		this.pending_removals.Clear();
		return flag;
	}

	public bool IsReachable(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		bool flag = false;
		object obj = this.access;
		lock (obj)
		{
			flag = this.IsReachable_AssumeLock(cell);
		}
		return flag;
	}

	public bool IsReachable(int cell, CellOffset[] offsets)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		bool flag = false;
		object obj = this.access;
		lock (obj)
		{
			foreach (CellOffset cellOffset in offsets)
			{
				if (this.IsReachable_AssumeLock(Grid.OffsetCell(cell, cellOffset)))
				{
					flag = true;
					break;
				}
			}
		}
		return flag;
	}

	public bool IsAllReachable(int cell, CellOffset[] offsets)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		bool flag = false;
		object obj = this.access;
		lock (obj)
		{
			if (this.IsReachable_AssumeLock(cell))
			{
				flag = true;
			}
			else
			{
				foreach (CellOffset cellOffset in offsets)
				{
					if (this.IsReachable_AssumeLock(Grid.OffsetCell(cell, cellOffset)))
					{
						flag = true;
						break;
					}
				}
			}
		}
		return flag;
	}

	public bool IsReachable(Workable workable)
	{
		return this.IsReachable(Grid.PosToCell(workable), workable.GetOffsets());
	}

	public void Occupy(object prober, int serial_no, IEnumerable<int> cells)
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
		bool flag2;
		lock (obj)
		{
			flag2 = this.valid_serial_nos.Remove(prober);
		}
		return flag2;
	}

	private static MinionGroupProber Instance;

	private Dictionary<object, int>[] cells;

	private Dictionary<object, KeyValuePair<int, int>> valid_serial_nos = new Dictionary<object, KeyValuePair<int, int>>();

	private List<object> pending_removals = new List<object>();

	private readonly object access = new object();
}
