using System;
using System.Collections;
using System.Collections.Generic;
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

	public void MarkDirty(Transform transform)
	{
		if (this.gridWidth == 0)
		{
			return;
		}
		this.pendingDirtyTransforms.Add(transform.GetInstanceID());
		IEnumerator enumerator = transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform2 = (Transform)obj;
				this.MarkDirty(transform2);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public bool IsMoving(Transform transform)
	{
		return this.movingTransforms.Contains(transform.GetInstanceID());
	}

	public void RegisterMovementStateChanged(Transform transform, Action<bool> handler)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry = default(CellChangeMonitor.MovementStateChangedEntry);
		if (!this.movementStateChangedHandlers.TryGetValue(instanceID, out movementStateChangedEntry))
		{
			movementStateChangedEntry = default(CellChangeMonitor.MovementStateChangedEntry);
			movementStateChangedEntry.handlers = new List<Action<bool>>();
			movementStateChangedEntry.transform = transform;
		}
		movementStateChangedEntry.handlers.Add(handler);
		this.movementStateChangedHandlers[instanceID] = movementStateChangedEntry;
	}

	public void UnregisterMovementStateChanged(Transform transform, Action<bool> callback)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry = default(CellChangeMonitor.MovementStateChangedEntry);
		if (this.movementStateChangedHandlers.TryGetValue(instanceID, out movementStateChangedEntry))
		{
			movementStateChangedEntry.handlers.Remove(callback);
			if (movementStateChangedEntry.handlers.Count == 0)
			{
				this.movementStateChangedHandlers.Remove(instanceID);
			}
		}
	}

	public void RegisterCellChangedHandler(Transform transform, global::System.Action handler)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.CellChangedEntry cellChangedEntry = default(CellChangeMonitor.CellChangedEntry);
		if (!this.cellChangedHandlers.TryGetValue(instanceID, out cellChangedEntry))
		{
			cellChangedEntry = default(CellChangeMonitor.CellChangedEntry);
			cellChangedEntry.transform = transform;
			cellChangedEntry.handlers = new List<global::System.Action>();
		}
		cellChangedEntry.handlers.Add(handler);
		this.cellChangedHandlers[instanceID] = cellChangedEntry;
	}

	public void UnregisterCellChangedHandler(Transform transform, global::System.Action callback)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.CellChangedEntry cellChangedEntry = default(CellChangeMonitor.CellChangedEntry);
		if (this.cellChangedHandlers.TryGetValue(instanceID, out cellChangedEntry))
		{
			cellChangedEntry.handlers.Remove(callback);
			if (cellChangedEntry.handlers.Count == 0)
			{
				this.cellChangedHandlers.Remove(instanceID);
			}
		}
	}

	public int PosToCell(Vector3 pos)
	{
		float x = pos.x;
		float num = pos.y + 0.05f;
		int num2 = (int)num;
		int num3 = (int)x;
		return num2 * this.gridWidth + num3;
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
				int num2 = -1;
				this.transformLastKnownCell.TryGetValue(num, out num2);
				int num3 = this.PosToCell(cellChangedEntry.transform.GetPosition());
				if (num2 != num3)
				{
					this.cellChangedCallbacksToRun.Clear();
					this.cellChangedCallbacksToRun.AddRange(cellChangedEntry.handlers);
					foreach (global::System.Action action in this.cellChangedCallbacksToRun)
					{
						if (cellChangedEntry.handlers.Contains(action))
						{
							action();
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
			foreach (Action<bool> action in this.moveChangedCallbacksToRun)
			{
				if (movementStateChangedEntry.handlers.Contains(action))
				{
					action(state);
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

	private List<global::System.Action> cellChangedCallbacksToRun = new List<global::System.Action>();

	private List<Action<bool>> moveChangedCallbacksToRun = new List<Action<bool>>();

	private int gridWidth;

	private struct CellChangedEntry
	{
		public Transform transform;

		public List<global::System.Action> handlers;
	}

	private struct MovementStateChangedEntry
	{
		public Transform transform;

		public List<Action<bool>> handlers;
	}
}
