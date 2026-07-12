using System;
using System.Collections.Generic;
using UnityEngine;

public class CellChangeMonitor : Singleton<CellChangeMonitor>
{
	public void MarkDirty(Transform transform)
	{
		if (this.gridWidth == 0)
		{
			return;
		}
		this.pendingDirtyTransforms.Add(transform.GetInstanceID());
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			this.MarkDirty(transform.GetChild(i));
		}
	}

	public bool IsMoving(Transform transform)
	{
		return this.movingTransforms.Contains(transform.GetInstanceID());
	}

	public void RegisterMovementStateChanged(Transform transform, Action<Transform, bool> handler)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry = default(CellChangeMonitor.MovementStateChangedEntry);
		if (!this.movementStateChangedHandlers.TryGetValue(instanceID, out movementStateChangedEntry))
		{
			movementStateChangedEntry = default(CellChangeMonitor.MovementStateChangedEntry);
			movementStateChangedEntry.handlers = new List<Action<Transform, bool>>();
			movementStateChangedEntry.transform = transform;
		}
		movementStateChangedEntry.handlers.Add(handler);
		this.movementStateChangedHandlers[instanceID] = movementStateChangedEntry;
	}

	public void UnregisterMovementStateChanged(int instance_id, Action<Transform, bool> callback)
	{
		CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry = default(CellChangeMonitor.MovementStateChangedEntry);
		if (this.movementStateChangedHandlers.TryGetValue(instance_id, out movementStateChangedEntry))
		{
			movementStateChangedEntry.handlers.Remove(callback);
			if (movementStateChangedEntry.handlers.Count == 0)
			{
				this.movementStateChangedHandlers.Remove(instance_id);
			}
		}
	}

	public void UnregisterMovementStateChanged(Transform transform, Action<Transform, bool> callback)
	{
		this.UnregisterMovementStateChanged(transform.GetInstanceID(), callback);
	}

	public int RegisterCellChangedHandler(Transform transform, global::System.Action callback, string debug_name)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.CellChangedEntry cellChangedEntry = default(CellChangeMonitor.CellChangedEntry);
		if (!this.cellChangedHandlers.TryGetValue(instanceID, out cellChangedEntry))
		{
			cellChangedEntry = default(CellChangeMonitor.CellChangedEntry);
			cellChangedEntry.transform = transform;
			cellChangedEntry.handlers = new List<CellChangeMonitor.CellChangedEntry.Handler>();
		}
		CellChangeMonitor.CellChangedEntry.Handler handler = new CellChangeMonitor.CellChangedEntry.Handler
		{
			name = debug_name,
			callback = callback
		};
		cellChangedEntry.handlers.Add(handler);
		this.cellChangedHandlers[instanceID] = cellChangedEntry;
		return instanceID;
	}

	public void UnregisterCellChangedHandler(int instance_id, global::System.Action callback)
	{
		CellChangeMonitor.CellChangedEntry cellChangedEntry = default(CellChangeMonitor.CellChangedEntry);
		if (this.cellChangedHandlers.TryGetValue(instance_id, out cellChangedEntry))
		{
			for (int i = 0; i < cellChangedEntry.handlers.Count; i++)
			{
				if (!(cellChangedEntry.handlers[i].callback != callback))
				{
					cellChangedEntry.handlers.RemoveAt(i);
					break;
				}
			}
			if (cellChangedEntry.handlers.Count == 0)
			{
				this.cellChangedHandlers.Remove(instance_id);
			}
		}
	}

	public void UnregisterCellChangedHandler(Transform transform, global::System.Action callback)
	{
		this.UnregisterCellChangedHandler(transform.GetInstanceID(), callback);
	}

	public void ClearLastKnownCell(Transform transform)
	{
		int instanceID = transform.GetInstanceID();
		this.transformLastKnownCell.Remove(instanceID);
	}

	public int PosToCell(Vector3 pos)
	{
		float x = pos.x;
		int num = (int)(pos.y + 0.05f);
		int num2 = (int)x;
		return num * this.gridWidth + num2;
	}

	public void SetGridSize(int grid_width, int grid_height)
	{
		this.gridWidth = grid_width;
	}

	public void RenderEveryTick()
	{
		HashSet<int> hashSet = this.pendingDirtyTransforms;
		this.pendingDirtyTransforms = this.dirtyTransforms;
		this.dirtyTransforms = hashSet;
		this.pendingDirtyTransforms.Clear();
		this.previouslyMovingTransforms.Clear();
		hashSet = this.previouslyMovingTransforms;
		this.previouslyMovingTransforms = this.movingTransforms;
		this.movingTransforms = hashSet;
		foreach (int num in this.dirtyTransforms)
		{
			CellChangeMonitor.CellChangedEntry cellChangedEntry = default(CellChangeMonitor.CellChangedEntry);
			if (this.cellChangedHandlers.TryGetValue(num, out cellChangedEntry))
			{
				if (cellChangedEntry.transform == null)
				{
					continue;
				}
				int num2 = -1;
				this.transformLastKnownCell.TryGetValue(num, out num2);
				int num3 = this.PosToCell(cellChangedEntry.transform.GetPosition());
				if (num2 != num3)
				{
					this.cellChangedCallbacksToRun.Clear();
					this.cellChangedCallbacksToRun.AddRange(cellChangedEntry.handlers);
					foreach (CellChangeMonitor.CellChangedEntry.Handler handler in this.cellChangedCallbacksToRun)
					{
						foreach (CellChangeMonitor.CellChangedEntry.Handler handler2 in cellChangedEntry.handlers)
						{
							if (handler2.callback == handler.callback)
							{
								handler2.callback();
								break;
							}
						}
					}
					this.transformLastKnownCell[num] = num3;
				}
			}
			this.movingTransforms.Add(num);
			if (!this.previouslyMovingTransforms.Contains(num))
			{
				this.RunMovementStateChangedCallbacks(num, true);
			}
		}
		foreach (int num4 in this.previouslyMovingTransforms)
		{
			if (!this.movingTransforms.Contains(num4))
			{
				this.RunMovementStateChangedCallbacks(num4, false);
			}
		}
		this.dirtyTransforms.Clear();
	}

	private void RunMovementStateChangedCallbacks(int instance_id, bool state)
	{
		CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry = default(CellChangeMonitor.MovementStateChangedEntry);
		if (this.movementStateChangedHandlers.TryGetValue(instance_id, out movementStateChangedEntry))
		{
			this.moveChangedCallbacksToRun.Clear();
			this.moveChangedCallbacksToRun.AddRange(movementStateChangedEntry.handlers);
			foreach (Action<Transform, bool> action in this.moveChangedCallbacksToRun)
			{
				if (movementStateChangedEntry.handlers.Contains(action))
				{
					action(movementStateChangedEntry.transform, state);
				}
			}
		}
	}

	private void Validate()
	{
	}

	private Dictionary<int, CellChangeMonitor.CellChangedEntry> cellChangedHandlers = new Dictionary<int, CellChangeMonitor.CellChangedEntry>();

	private Dictionary<int, CellChangeMonitor.MovementStateChangedEntry> movementStateChangedHandlers = new Dictionary<int, CellChangeMonitor.MovementStateChangedEntry>();

	private HashSet<int> pendingDirtyTransforms = new HashSet<int>();

	private HashSet<int> dirtyTransforms = new HashSet<int>();

	private HashSet<int> movingTransforms = new HashSet<int>();

	private HashSet<int> previouslyMovingTransforms = new HashSet<int>();

	private Dictionary<int, int> transformLastKnownCell = new Dictionary<int, int>();

	private List<CellChangeMonitor.CellChangedEntry.Handler> cellChangedCallbacksToRun = new List<CellChangeMonitor.CellChangedEntry.Handler>();

	private List<Action<Transform, bool>> moveChangedCallbacksToRun = new List<Action<Transform, bool>>();

	private int gridWidth;

	private struct CellChangedEntry
	{
		public Transform transform;

		public List<CellChangeMonitor.CellChangedEntry.Handler> handlers;

		public struct Handler
		{
			public string name;

			public global::System.Action callback;
		}
	}

	private struct MovementStateChangedEntry
	{
		public Transform transform;

		public List<Action<Transform, bool>> handlers;
	}
}
