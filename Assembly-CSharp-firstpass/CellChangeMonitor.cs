using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Pool;

public class CellChangeMonitor : Singleton<CellChangeMonitor>
{
	private static ulong Join(int front, uint back)
	{
		return new CellChangeMonitor.IDJoiner
		{
			front = front,
			back = back
		}.full;
	}

	private static void Split(ulong id, out int front, out uint back)
	{
		CellChangeMonitor.IDJoiner idjoiner = new CellChangeMonitor.IDJoiner
		{
			full = id
		};
		front = idjoiner.front;
		back = idjoiner.back;
	}

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

	public ulong RegisterMovementStateChanged(Transform transform, Action<Transform, bool, object> handler, object context)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry;
		if (!this.movementStateChangedHandlers.TryGetValue(instanceID, out movementStateChangedEntry))
		{
			movementStateChangedEntry = CellChangeMonitor.MovementStateChangedEntry.Pool.Get();
			movementStateChangedEntry.Setup(transform);
			this.movementStateChangedHandlers[instanceID] = movementStateChangedEntry;
		}
		ulong num = CellChangeMonitor.Join(instanceID, this.nextHandlerID);
		this.nextHandlerID += 1U;
		movementStateChangedEntry.AddHandler(handler, context, num);
		return num;
	}

	public void UnregisterMovementStateChanged(ref ulong handlerid)
	{
		int num;
		uint num2;
		CellChangeMonitor.Split(handlerid, out num, out num2);
		CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry;
		if (this.movementStateChangedHandlers.TryGetValue(num, out movementStateChangedEntry))
		{
			movementStateChangedEntry.RemoveHandler(handlerid);
			handlerid = 0UL;
			if (movementStateChangedEntry.Empty)
			{
				CellChangeMonitor.MovementStateChangedEntry.Pool.Release(movementStateChangedEntry);
				this.movementStateChangedHandlers.Remove(num);
			}
		}
	}

	public ulong RegisterCellChangedHandler(Transform transform, Action<object> callback, object context = null, string debug_name = null)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.CellChangedEntry cellChangedEntry = null;
		if (!this.cellChangedHandlers.TryGetValue(instanceID, out cellChangedEntry))
		{
			cellChangedEntry = CellChangeMonitor.CellChangedEntry.Pool.Get();
			this.cellChangedHandlers[instanceID] = cellChangedEntry;
		}
		cellChangedEntry.UpdateTransform(transform);
		ulong num = CellChangeMonitor.Join(instanceID, this.nextHandlerID);
		this.nextHandlerID += 1U;
		cellChangedEntry.AddHandler(callback, context, num, debug_name);
		return num;
	}

	public void UnregisterCellChangedHandler(ref ulong handlerID)
	{
		int num;
		uint num2;
		CellChangeMonitor.Split(handlerID, out num, out num2);
		CellChangeMonitor.CellChangedEntry cellChangedEntry;
		if (this.cellChangedHandlers.TryGetValue(num, out cellChangedEntry))
		{
			cellChangedEntry.RemoveHandler(handlerID);
			if (cellChangedEntry.Empty)
			{
				CellChangeMonitor.CellChangedEntry.Pool.Release(cellChangedEntry);
				this.cellChangedHandlers.Remove(num);
			}
			handlerID = 0UL;
		}
	}

	public void ClearLastKnownCell(Transform transform)
	{
		int instanceID = transform.GetInstanceID();
		CellChangeMonitor.CellChangedEntry cellChangedEntry;
		if (this.cellChangedHandlers.TryGetValue(instanceID, out cellChangedEntry))
		{
			cellChangedEntry.SetCell(-1);
		}
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
		this.pendingDirtyTransforms.Swap(this.dirtyTransforms);
		this.pendingDirtyTransforms.Clear();
		this.previouslyMovingTransforms.Clear();
		this.previouslyMovingTransforms.Swap(this.movingTransforms);
		int i = 0;
		while (i < this.dirtyTransforms.Count)
		{
			int num = this.dirtyTransforms[i];
			CellChangeMonitor.CellChangedEntry cellChangedEntry;
			if (!this.cellChangedHandlers.TryGetValue(num, out cellChangedEntry))
			{
				goto IL_006F;
			}
			if (cellChangedEntry.Valid)
			{
				if (cellChangedEntry.SetCell())
				{
					cellChangedEntry.TriggerUpdate();
					goto IL_006F;
				}
				goto IL_006F;
			}
			IL_00A1:
			i++;
			continue;
			IL_006F:
			this.movingTransforms.Add(num);
			CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry;
			if (!this.previouslyMovingTransforms.Contains(num) && this.movementStateChangedHandlers.TryGetValue(num, out movementStateChangedEntry))
			{
				movementStateChangedEntry.TriggerUpdate(true);
				goto IL_00A1;
			}
			goto IL_00A1;
		}
		for (int j = 0; j < this.previouslyMovingTransforms.Count; j++)
		{
			int num2 = this.previouslyMovingTransforms[j];
			CellChangeMonitor.MovementStateChangedEntry movementStateChangedEntry2;
			if (!this.movingTransforms.Contains(num2) && this.movementStateChangedHandlers.TryGetValue(num2, out movementStateChangedEntry2))
			{
				movementStateChangedEntry2.TriggerUpdate(false);
			}
		}
		this.dirtyTransforms.Clear();
	}

	private void Validate()
	{
	}

	private uint nextHandlerID = 1U;

	private Dictionary<int, CellChangeMonitor.CellChangedEntry> cellChangedHandlers = new Dictionary<int, CellChangeMonitor.CellChangedEntry>();

	private Dictionary<int, CellChangeMonitor.MovementStateChangedEntry> movementStateChangedHandlers = new Dictionary<int, CellChangeMonitor.MovementStateChangedEntry>();

	private HybridListHashSet<int> pendingDirtyTransforms = new HybridListHashSet<int>();

	private HybridListHashSet<int> dirtyTransforms = new HybridListHashSet<int>();

	private HybridListHashSet<int> movingTransforms = new HybridListHashSet<int>();

	private HybridListHashSet<int> previouslyMovingTransforms = new HybridListHashSet<int>();

	private int gridWidth;

	[StructLayout(LayoutKind.Explicit, Size = 8)]
	private struct IDJoiner
	{
		[FieldOffset(0)]
		public int front;

		[FieldOffset(4)]
		public uint back;

		[FieldOffset(0)]
		public ulong full;
	}

	private class CellChangedEntry
	{
		private void Reset()
		{
			this.handlers.Clear();
			this.lastCell = -1;
		}

		public bool Empty
		{
			get
			{
				return this.handlers.Count == 0;
			}
		}

		public bool Valid
		{
			get
			{
				return this.transform != null;
			}
		}

		public void UpdateTransform(Transform transform)
		{
			this.transform = transform;
		}

		public bool SetCell()
		{
			return this.SetCell(Singleton<CellChangeMonitor>.Instance.PosToCell(this.transform.GetPosition()));
		}

		public bool SetCell(int cell)
		{
			if (this.lastCell == cell)
			{
				return false;
			}
			this.lastCell = cell;
			return true;
		}

		public void TriggerUpdate()
		{
			this.inUpdate = true;
			this.pendingRemoval = CollectionPool<List<ulong>, ulong>.Get();
			int count = this.handlers.Count;
			for (int i = 0; i < count; i++)
			{
				if (!this.pendingRemoval.Contains(this.handlers[i].handlerID))
				{
					this.handlers[i].Invoke();
				}
			}
			this.inUpdate = false;
			foreach (ulong num in this.pendingRemoval)
			{
				this.RemoveHandler(num);
			}
			CollectionPool<List<ulong>, ulong>.Release(this.pendingRemoval);
			this.pendingRemoval = null;
		}

		public void AddHandler(Action<object> cb, object context, ulong id, string name = null)
		{
			this.handlers.Add(new CellChangeMonitor.CellChangedEntry.Handler
			{
				callback = cb,
				context = context,
				handlerID = id
			});
		}

		public void RemoveHandler(ulong id)
		{
			if (this.inUpdate)
			{
				this.pendingRemoval.Add(id);
				return;
			}
			for (int i = 0; i < this.handlers.Count; i++)
			{
				if (this.handlers[i].handlerID == id)
				{
					this.handlers.RemoveAtSwap<CellChangeMonitor.CellChangedEntry.Handler>(i);
					return;
				}
			}
		}

		public static ObjectPool<CellChangeMonitor.CellChangedEntry> Pool = new ObjectPool<CellChangeMonitor.CellChangedEntry>(() => new CellChangeMonitor.CellChangedEntry(), null, delegate(CellChangeMonitor.CellChangedEntry entry)
		{
			entry.Reset();
		}, null, false, 10, 256);

		private Transform transform;

		private List<CellChangeMonitor.CellChangedEntry.Handler> handlers = new List<CellChangeMonitor.CellChangedEntry.Handler>();

		private bool inUpdate;

		private List<ulong> pendingRemoval;

		private int lastCell = -1;

		private struct Handler
		{
			public string name
			{
				get
				{
					return null;
				}
			}

			public void Invoke()
			{
				this.callback(this.context);
			}

			public Action<object> callback;

			public object context;

			public ulong handlerID;
		}
	}

	private class MovementStateChangedEntry
	{
		public bool Empty
		{
			get
			{
				return this.handlers == null || this.handlers.Count == 0;
			}
		}

		public void Setup(Transform transform)
		{
			this.transform = transform;
		}

		public void Clear()
		{
			this.transform = null;
			this.handlers.Clear();
		}

		public void TriggerUpdate(bool value)
		{
			this.inUpdate = true;
			this.pendingRemovals = CollectionPool<List<ulong>, ulong>.Get();
			int count = this.handlers.Count;
			for (int i = 0; i < count; i++)
			{
				if (!this.pendingRemovals.Contains(this.handlers[i].handlerID))
				{
					this.handlers[i].Invoke(this.transform, value);
				}
			}
			this.inUpdate = false;
			foreach (ulong num in this.pendingRemovals)
			{
				this.RemoveHandler(num);
			}
			CollectionPool<List<ulong>, ulong>.Release(this.pendingRemovals);
			this.pendingRemovals = null;
		}

		public void AddHandler(Action<Transform, bool, object> cb, object context, ulong id)
		{
			this.handlers.Add(new CellChangeMonitor.MovementStateChangedEntry.Handler
			{
				handler = cb,
				context = context,
				handlerID = id
			});
		}

		public void RemoveHandler(ulong id)
		{
			if (this.inUpdate)
			{
				this.pendingRemovals.Add(id);
				return;
			}
			for (int i = 0; i < this.handlers.Count; i++)
			{
				if (this.handlers[i].handlerID == id)
				{
					this.handlers.RemoveAtSwap<CellChangeMonitor.MovementStateChangedEntry.Handler>(i);
					return;
				}
			}
		}

		private Transform transform;

		private List<CellChangeMonitor.MovementStateChangedEntry.Handler> handlers = new List<CellChangeMonitor.MovementStateChangedEntry.Handler>();

		private bool inUpdate;

		private List<ulong> pendingRemovals;

		public static ObjectPool<CellChangeMonitor.MovementStateChangedEntry> Pool = new ObjectPool<CellChangeMonitor.MovementStateChangedEntry>(() => new CellChangeMonitor.MovementStateChangedEntry(), null, delegate(CellChangeMonitor.MovementStateChangedEntry entry)
		{
			entry.Clear();
		}, null, false, 10, 256);

		private struct Handler
		{
			public void Invoke(Transform t, bool b)
			{
				this.handler(t, b, this.context);
			}

			public ulong handlerID;

			public Action<Transform, bool, object> handler;

			public object context;
		}
	}
}
