using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MinionGroupProber")]
public class MinionGroupProber : KMonoBehaviour, IGroupProber, ISim200ms
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
		this.cells = new Dictionary<object, short>[Grid.CellCount];
		for (int i = 0; i < Grid.CellCount; i++)
		{
			this.cells[i] = new Dictionary<object, short>();
		}
		this.cell_cleanup_index = 0;
		this.cell_checks_per_frame = Grid.CellCount / 500;
	}

	public bool IsReachable(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		foreach (KeyValuePair<object, short> keyValuePair in this.cells[cell])
		{
			object key = keyValuePair.Key;
			short value = keyValuePair.Value;
			KeyValuePair<short, short> keyValuePair2;
			if (this.valid_serial_nos.TryGetValue(key, out keyValuePair2) && (value == keyValuePair2.Key || value == keyValuePair2.Value))
			{
				return true;
			}
		}
		return false;
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

	public bool IsAllReachable(int cell, CellOffset[] offsets)
	{
		if (this.IsReachable(cell))
		{
			return true;
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

	public void Occupy(object prober, short serial_no, IEnumerable<int> cells)
	{
		foreach (int num in cells)
		{
			Dictionary<object, short> dictionary = this.cells[num];
			lock (dictionary)
			{
				this.cells[num][prober] = serial_no;
			}
		}
	}

	public void SetValidSerialNos(object prober, short previous_serial_no, short serial_no)
	{
		object obj = this.access;
		lock (obj)
		{
			this.valid_serial_nos[prober] = new KeyValuePair<short, short>(previous_serial_no, serial_no);
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

	public void Sim200ms(float dt)
	{
		int i = 0;
		while (i < this.cell_checks_per_frame)
		{
			this.pending_removals.Clear();
			foreach (KeyValuePair<object, short> keyValuePair in this.cells[this.cell_cleanup_index])
			{
				KeyValuePair<short, short> keyValuePair2;
				if (!this.valid_serial_nos.TryGetValue(keyValuePair.Key, out keyValuePair2) || (keyValuePair2.Key != keyValuePair.Value && keyValuePair2.Value != keyValuePair.Value))
				{
					this.pending_removals.Add(keyValuePair.Key);
				}
			}
			foreach (object obj in this.pending_removals)
			{
				this.cells[this.cell_cleanup_index].Remove(obj);
			}
			i++;
			this.cell_cleanup_index = (this.cell_cleanup_index + 1) % this.cells.Length;
		}
	}

	private static MinionGroupProber Instance;

	private Dictionary<object, short>[] cells;

	private Dictionary<object, KeyValuePair<short, short>> valid_serial_nos = new Dictionary<object, KeyValuePair<short, short>>();

	private List<object> pending_removals = new List<object>();

	private int cell_cleanup_index;

	private int cell_checks_per_frame;

	private readonly object access = new object();
}
