using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CellChangeMonitor
{
	public static CellChangeMonitor Instance
	{
		get
		{
			return Singleton<CellChangeMonitor>.Instance;
		}
	}

	public static void Destroy()
	{
		Singleton<CellChangeMonitor>.Destroy();
	}

	private int GetEntryIdx(List<CellChangeMonitor.Entry> list, Transform transform)
	{
		return list.FindIndex((CellChangeMonitor.Entry x) => object.ReferenceEquals(x.transform, transform));
	}

	public void Add(Component component, Action<int, int> callback, bool trigger_object_movement_wakeup_sleep = false)
	{
		List<CellChangeMonitor.Entry> list = ((!trigger_object_movement_wakeup_sleep) ? this.cellChangeEntries : this.objectMovedEntries);
		Transform transform = component.transform;
		int num = this.GetEntryIdx(list, transform);
		int num2 = Grid.PosToCell(transform.position);
		if (num == -1)
		{
			CellChangeMonitor.Entry entry = new CellChangeMonitor.Entry
			{
				transform = transform,
				previousPosition = transform.position,
				previousCell = num2,
				callbacks = new List<Action<int, int>>(),
				name = component.name,
				prefabId = transform.GetComponent<KMonoBehaviour>(),
				previouslyMoved = false
			};
			list.Add(entry);
			num = list.Count - 1;
		}
		list[num].callbacks.Add(callback);
		if (!trigger_object_movement_wakeup_sleep)
		{
			callback(num2, num2);
		}
	}

	public void Remove(Component component, Action<int, int> callback, bool trigger_object_movement_wakeup_sleep = false)
	{
		List<CellChangeMonitor.Entry> list = ((!trigger_object_movement_wakeup_sleep) ? this.cellChangeEntries : this.objectMovedEntries);
		Transform transform = component.transform;
		int entryIdx = this.GetEntryIdx(list, transform);
		if (entryIdx != -1)
		{
			list[entryIdx].callbacks.Remove(callback);
			if (list[entryIdx].callbacks.Count == 0)
			{
				list.RemoveAt(entryIdx);
			}
		}
	}

	public void Update()
	{
		for (int i = 0; i < this.cellChangeEntries.Count; i++)
		{
			CellChangeMonitor.Entry entry = this.cellChangeEntries[i];
			Vector3 position = entry.transform.position;
			bool flag = position.x != entry.previousPosition.x || position.y != entry.previousPosition.y;
			if (flag)
			{
				int num = Grid.PosToCell(position);
				if (entry.previousCell != num)
				{
					if (!Grid.IsValidCell(num))
					{
						entry.transform.gameObject.DeleteObject();
					}
					else
					{
						int previousCell = entry.previousCell;
						entry.previousPosition = position;
						entry.previousCell = num;
						this.cellChangeEntries[i] = entry;
						for (int j = 0; j < entry.callbacks.Count; j++)
						{
							Action<int, int> action = entry.callbacks[j];
							action(previousCell, num);
						}
						entry.prefabId.Trigger(1088554450, num);
					}
				}
			}
		}
		for (int k = 0; k < this.objectMovedEntries.Count; k++)
		{
			CellChangeMonitor.Entry entry2 = this.objectMovedEntries[k];
			Vector3 position2 = entry2.transform.position;
			bool flag2 = position2.x != entry2.previousPosition.x || position2.y != entry2.previousPosition.y;
			if (flag2)
			{
				int num2 = Grid.PosToCell(position2);
				entry2.previousPosition = position2;
				entry2.previousCell = num2;
			}
			if (flag2 != entry2.previouslyMoved)
			{
				entry2.previouslyMoved = flag2;
				GameHashes gameHashes = ((!flag2) ? GameHashes.ObjectMovementSleep : GameHashes.ObjectMovementWakeUp);
				for (int l = 0; l < entry2.callbacks.Count; l++)
				{
					Action<int, int> action2 = entry2.callbacks[l];
					action2((int)gameHashes, -1);
				}
			}
			this.objectMovedEntries[k] = entry2;
		}
	}

	private List<CellChangeMonitor.Entry> cellChangeEntries = new List<CellChangeMonitor.Entry>();

	private List<CellChangeMonitor.Entry> objectMovedEntries = new List<CellChangeMonitor.Entry>();

	[DebuggerDisplay("{name} {callbacks[0]}")]
	private struct Entry
	{
		public Transform transform;

		public Vector3 previousPosition;

		public int previousCell;

		public List<Action<int, int>> callbacks;

		public string name;

		public KMonoBehaviour prefabId;

		public bool previouslyMoved;
	}
}
