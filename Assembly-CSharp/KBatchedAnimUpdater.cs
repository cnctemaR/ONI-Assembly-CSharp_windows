using System;
using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimUpdater : Singleton<KBatchedAnimUpdater>
{
	public void InitializeGrid()
	{
		this.Clear();
		Vector2I visibleSize = this.GetVisibleSize();
		int num = (visibleSize.x + 32 - 1) / 32;
		int num2 = (visibleSize.y + 32 - 1) / 32;
		this.controllerGrid = new List<KBatchedAnimController>[num, num2];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				this.controllerGrid[j, i] = new List<KBatchedAnimController>();
			}
		}
		this.visibleChunks.Clear();
		this.previouslyVisibleChunks.Clear();
		this.previouslyVisibleChunkGrid = new bool[num, num2];
		this.visibleChunkGrid = new bool[num, num2];
	}

	public Vector2I GetVisibleSize()
	{
		return new Vector2I((int)((float)Grid.WidthInCells * KBatchedAnimUpdater.VISIBLE_RANGE_SCALE.x), (int)((float)Grid.HeightInCells * KBatchedAnimUpdater.VISIBLE_RANGE_SCALE.y));
	}

	public void Clear()
	{
		foreach (KBatchedAnimController kbatchedAnimController in this.updateList)
		{
			if (kbatchedAnimController != null)
			{
				global::UnityEngine.Object.DestroyImmediate(kbatchedAnimController);
			}
		}
		this.updateList.Clear();
		foreach (KBatchedAnimController kbatchedAnimController2 in this.alwaysUpdateList)
		{
			if (kbatchedAnimController2 != null)
			{
				global::UnityEngine.Object.DestroyImmediate(kbatchedAnimController2);
			}
		}
		this.alwaysUpdateList.Clear();
		this.queuedRegistrations.Clear();
		this.visibleChunks.Clear();
		this.previouslyVisibleChunks.Clear();
		this.controllerGrid = null;
		this.previouslyVisibleChunkGrid = null;
		this.visibleChunkGrid = null;
	}

	public void UpdateRegister(KBatchedAnimController controller)
	{
		switch (controller.updateRegistrationState)
		{
		case KBatchedAnimUpdater.RegistrationState.Registered:
			break;
		case KBatchedAnimUpdater.RegistrationState.PendingRemoval:
			controller.updateRegistrationState = KBatchedAnimUpdater.RegistrationState.Registered;
			return;
		case KBatchedAnimUpdater.RegistrationState.Unregistered:
			((controller.visibilityType == KAnimControllerBase.VisibilityType.Always) ? this.alwaysUpdateList : this.updateList).AddLast(controller);
			controller.updateRegistrationState = KBatchedAnimUpdater.RegistrationState.Registered;
			break;
		default:
			return;
		}
	}

	public void UpdateUnregister(KBatchedAnimController controller)
	{
		switch (controller.updateRegistrationState)
		{
		case KBatchedAnimUpdater.RegistrationState.Registered:
			controller.updateRegistrationState = KBatchedAnimUpdater.RegistrationState.PendingRemoval;
			break;
		case KBatchedAnimUpdater.RegistrationState.PendingRemoval:
		case KBatchedAnimUpdater.RegistrationState.Unregistered:
			break;
		default:
			return;
		}
	}

	public void VisibilityRegister(KBatchedAnimController controller)
	{
		this.queuedRegistrations.Add(new KBatchedAnimUpdater.RegistrationInfo
		{
			transformId = controller.transform.GetInstanceID(),
			controllerInstanceId = controller.GetInstanceID(),
			controller = controller,
			register = true
		});
	}

	public void VisibilityUnregister(KBatchedAnimController controller)
	{
		if (App.IsExiting)
		{
			return;
		}
		this.queuedRegistrations.Add(new KBatchedAnimUpdater.RegistrationInfo
		{
			transformId = controller.transform.GetInstanceID(),
			controllerInstanceId = controller.GetInstanceID(),
			controller = controller,
			register = false
		});
	}

	private List<KBatchedAnimController> GetControllerList(Vector2I chunk_xy)
	{
		List<KBatchedAnimController> list = null;
		if (this.controllerGrid != null && 0 <= chunk_xy.x && chunk_xy.x < this.controllerGrid.GetLength(0) && 0 <= chunk_xy.y && chunk_xy.y < this.controllerGrid.GetLength(1))
		{
			list = this.controllerGrid[chunk_xy.x, chunk_xy.y];
		}
		return list;
	}

	public void LateUpdate()
	{
		this.ProcessMovingAnims();
		this.UpdateVisibility();
		this.ProcessRegistrations();
		this.CleanUp();
		float num = Time.unscaledDeltaTime;
		int count = this.alwaysUpdateList.Count;
		KBatchedAnimUpdater.UpdateRegisteredAnims(this.alwaysUpdateList, num);
		if (this.DoGridProcessing())
		{
			num = Time.deltaTime;
			if (num > 0f)
			{
				int count2 = this.updateList.Count;
				KBatchedAnimUpdater.UpdateRegisteredAnims(this.updateList, num);
			}
		}
	}

	private static void UpdateRegisteredAnims(LinkedList<KBatchedAnimController> list, float dt)
	{
		LinkedListNode<KBatchedAnimController> next;
		for (LinkedListNode<KBatchedAnimController> linkedListNode = list.First; linkedListNode != null; linkedListNode = next)
		{
			next = linkedListNode.Next;
			KBatchedAnimController value = linkedListNode.Value;
			if (value == null)
			{
				list.Remove(linkedListNode);
			}
			else if (value.updateRegistrationState != KBatchedAnimUpdater.RegistrationState.Registered)
			{
				value.updateRegistrationState = KBatchedAnimUpdater.RegistrationState.Unregistered;
				list.Remove(linkedListNode);
			}
			else
			{
				value.UpdateAnim(dt);
			}
		}
	}

	public bool IsChunkVisible(Vector2I chunk_xy)
	{
		return this.visibleChunkGrid[chunk_xy.x, chunk_xy.y];
	}

	public void GetVisibleArea(out Vector2I vis_chunk_min, out Vector2I vis_chunk_max)
	{
		vis_chunk_min = this.vis_chunk_min;
		vis_chunk_max = this.vis_chunk_max;
	}

	public static Vector2I PosToChunkXY(Vector3 pos)
	{
		return KAnimBatchManager.CellXYToChunkXY(Grid.PosToXY(pos));
	}

	private void UpdateVisibility()
	{
		if (!this.DoGridProcessing())
		{
			return;
		}
		Vector2I vector2I;
		Vector2I vector2I2;
		KBatchedAnimUpdater.GetVisibleCellRange(out vector2I, out vector2I2);
		this.vis_chunk_min = new Vector2I(vector2I.x / 32, vector2I.y / 32);
		this.vis_chunk_max = new Vector2I(vector2I2.x / 32, vector2I2.y / 32);
		this.vis_chunk_max.x = Math.Min(this.vis_chunk_max.x, this.controllerGrid.GetLength(0) - 1);
		this.vis_chunk_max.y = Math.Min(this.vis_chunk_max.y, this.controllerGrid.GetLength(1) - 1);
		bool[,] array = this.previouslyVisibleChunkGrid;
		this.previouslyVisibleChunkGrid = this.visibleChunkGrid;
		this.visibleChunkGrid = array;
		Array.Clear(this.visibleChunkGrid, 0, this.visibleChunkGrid.Length);
		List<Vector2I> list = this.previouslyVisibleChunks;
		this.previouslyVisibleChunks = this.visibleChunks;
		this.visibleChunks = list;
		this.visibleChunks.Clear();
		for (int i = this.vis_chunk_min.y; i <= this.vis_chunk_max.y; i++)
		{
			for (int j = this.vis_chunk_min.x; j <= this.vis_chunk_max.x; j++)
			{
				this.visibleChunkGrid[j, i] = true;
				this.visibleChunks.Add(new Vector2I(j, i));
				if (!this.previouslyVisibleChunkGrid[j, i])
				{
					List<KBatchedAnimController> list2 = this.controllerGrid[j, i];
					for (int k = 0; k < list2.Count; k++)
					{
						KBatchedAnimController kbatchedAnimController = list2[k];
						if (!(kbatchedAnimController == null))
						{
							kbatchedAnimController.SetVisiblity(true);
						}
					}
				}
			}
		}
		for (int l = 0; l < this.previouslyVisibleChunks.Count; l++)
		{
			Vector2I vector2I3 = this.previouslyVisibleChunks[l];
			if (!this.visibleChunkGrid[vector2I3.x, vector2I3.y])
			{
				List<KBatchedAnimController> list3 = this.controllerGrid[vector2I3.x, vector2I3.y];
				for (int m = 0; m < list3.Count; m++)
				{
					KBatchedAnimController kbatchedAnimController2 = list3[m];
					if (!(kbatchedAnimController2 == null))
					{
						kbatchedAnimController2.SetVisiblity(false);
					}
				}
			}
		}
	}

	private void ProcessMovingAnims()
	{
		for (int i = 0; i < this.movingControllerInfos.Count; i++)
		{
			KBatchedAnimUpdater.MovingControllerInfo movingControllerInfo = this.movingControllerInfos[i];
			if (!(movingControllerInfo.controller == null))
			{
				Vector2I vector2I = KBatchedAnimUpdater.PosToChunkXY(movingControllerInfo.controller.PositionIncludingOffset);
				if (movingControllerInfo.chunkXY != vector2I)
				{
					KBatchedAnimUpdater.ControllerChunkInfo controllerChunkInfo = default(KBatchedAnimUpdater.ControllerChunkInfo);
					DebugUtil.Assert(this.controllerChunkInfos.TryGetValue(movingControllerInfo.controllerInstanceId, out controllerChunkInfo));
					DebugUtil.Assert(movingControllerInfo.controller == controllerChunkInfo.controller);
					DebugUtil.Assert(controllerChunkInfo.chunkXY == movingControllerInfo.chunkXY);
					List<KBatchedAnimController> list = this.GetControllerList(controllerChunkInfo.chunkXY);
					if (list != null)
					{
						DebugUtil.Assert(list.Contains(controllerChunkInfo.controller));
						list.Remove(controllerChunkInfo.controller);
					}
					list = this.GetControllerList(vector2I);
					if (list != null)
					{
						DebugUtil.Assert(!list.Contains(controllerChunkInfo.controller));
						list.Add(controllerChunkInfo.controller);
					}
					movingControllerInfo.chunkXY = vector2I;
					this.movingControllerInfos[i] = movingControllerInfo;
					controllerChunkInfo.chunkXY = vector2I;
					this.controllerChunkInfos[movingControllerInfo.controllerInstanceId] = controllerChunkInfo;
					if (list != null)
					{
						controllerChunkInfo.controller.SetVisiblity(this.visibleChunkGrid[vector2I.x, vector2I.y]);
					}
					else
					{
						controllerChunkInfo.controller.SetVisiblity(false);
					}
				}
			}
		}
	}

	private void ProcessRegistrations()
	{
		ListPool<KBatchedAnimController, KBatchedAnimUpdater>.PooledList pooledList = ListPool<KBatchedAnimController, KBatchedAnimUpdater>.Allocate();
		for (int i = 0; i < this.queuedRegistrations.Count; i++)
		{
			KBatchedAnimUpdater.RegistrationInfo info = this.queuedRegistrations[i];
			if (info.register)
			{
				if (!(info.controller == null))
				{
					int instanceID = info.controller.GetInstanceID();
					DebugUtil.Assert(!this.controllerChunkInfos.ContainsKey(instanceID));
					KBatchedAnimUpdater.ControllerChunkInfo controllerChunkInfo = new KBatchedAnimUpdater.ControllerChunkInfo
					{
						controller = info.controller,
						chunkXY = KBatchedAnimUpdater.PosToChunkXY(info.controller.PositionIncludingOffset)
					};
					this.controllerChunkInfos[instanceID] = controllerChunkInfo;
					Singleton<CellChangeMonitor>.Instance.RegisterMovementStateChanged(info.controller.transform, new Action<Transform, bool>(this.OnMovementStateChanged));
					List<KBatchedAnimController> controllerList = this.GetControllerList(controllerChunkInfo.chunkXY);
					if (controllerList != null)
					{
						DebugUtil.Assert(!controllerList.Contains(info.controller));
						controllerList.Add(info.controller);
					}
					if (Singleton<CellChangeMonitor>.Instance.IsMoving(info.controller.transform))
					{
						this.movingControllerInfos.Add(new KBatchedAnimUpdater.MovingControllerInfo
						{
							controllerInstanceId = instanceID,
							controller = info.controller,
							chunkXY = controllerChunkInfo.chunkXY
						});
					}
					if (controllerList != null && this.visibleChunkGrid[controllerChunkInfo.chunkXY.x, controllerChunkInfo.chunkXY.y])
					{
						pooledList.Add(info.controller);
					}
				}
			}
			else
			{
				KBatchedAnimUpdater.ControllerChunkInfo controllerChunkInfo2 = default(KBatchedAnimUpdater.ControllerChunkInfo);
				if (this.controllerChunkInfos.TryGetValue(info.controllerInstanceId, out controllerChunkInfo2))
				{
					if (info.controller != null)
					{
						List<KBatchedAnimController> controllerList2 = this.GetControllerList(controllerChunkInfo2.chunkXY);
						if (controllerList2 != null)
						{
							DebugUtil.Assert(controllerList2.Contains(info.controller));
							controllerList2.Remove(info.controller);
						}
					}
					this.movingControllerInfos.RemoveAll((KBatchedAnimUpdater.MovingControllerInfo x) => x.controllerInstanceId == info.controllerInstanceId);
					Singleton<CellChangeMonitor>.Instance.UnregisterMovementStateChanged(info.transformId, new Action<Transform, bool>(this.OnMovementStateChanged));
					this.controllerChunkInfos.Remove(info.controllerInstanceId);
					pooledList.Remove(info.controller);
				}
			}
		}
		this.queuedRegistrations.Clear();
		foreach (KBatchedAnimController kbatchedAnimController in pooledList)
		{
			if (kbatchedAnimController != null)
			{
				kbatchedAnimController.SetVisiblity(true);
			}
		}
		pooledList.Recycle();
	}

	public void OnMovementStateChanged(Transform transform, bool is_moving)
	{
		if (transform == null)
		{
			return;
		}
		KBatchedAnimController component = transform.GetComponent<KBatchedAnimController>();
		int controller_instance_id = component.GetInstanceID();
		KBatchedAnimUpdater.ControllerChunkInfo controllerChunkInfo = default(KBatchedAnimUpdater.ControllerChunkInfo);
		DebugUtil.Assert(this.controllerChunkInfos.TryGetValue(controller_instance_id, out controllerChunkInfo));
		if (is_moving)
		{
			this.movingControllerInfos.Add(new KBatchedAnimUpdater.MovingControllerInfo
			{
				controllerInstanceId = controller_instance_id,
				controller = component,
				chunkXY = controllerChunkInfo.chunkXY
			});
			return;
		}
		this.movingControllerInfos.RemoveAll((KBatchedAnimUpdater.MovingControllerInfo x) => x.controllerInstanceId == controller_instance_id);
	}

	private void CleanUp()
	{
		if (!this.DoGridProcessing())
		{
			return;
		}
		int length = this.controllerGrid.GetLength(0);
		for (int i = 0; i < 16; i++)
		{
			int num = (this.cleanUpChunkIndex + i) % this.controllerGrid.Length;
			int num2 = num % length;
			int num3 = num / length;
			this.controllerGrid[num2, num3].RemoveAll((KBatchedAnimController item) => item == null);
		}
		this.cleanUpChunkIndex = (this.cleanUpChunkIndex + 16) % this.controllerGrid.Length;
	}

	public static void GetVisibleCellRange(out Vector2I min, out Vector2I max)
	{
		Grid.GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);
		min.x -= 4;
		min.y -= 4;
		min.x = Math.Min((int)((float)Grid.WidthInCells * KBatchedAnimUpdater.VISIBLE_RANGE_SCALE.x) - 1, Math.Max(0, min.x));
		min.y = Math.Min((int)((float)Grid.HeightInCells * KBatchedAnimUpdater.VISIBLE_RANGE_SCALE.y) - 1, Math.Max(0, min.y));
		max.x += 4;
		max.y += 4;
		max.x = Math.Min((int)((float)Grid.WidthInCells * KBatchedAnimUpdater.VISIBLE_RANGE_SCALE.x) - 1, Math.Max(0, max.x));
		max.y = Math.Min((int)((float)Grid.HeightInCells * KBatchedAnimUpdater.VISIBLE_RANGE_SCALE.y) - 1, Math.Max(0, max.y));
	}

	private bool DoGridProcessing()
	{
		return this.controllerGrid != null && Camera.main != null;
	}

	private const int VISIBLE_BORDER = 4;

	public static readonly Vector2I INVALID_CHUNK_ID = Vector2I.minusone;

	private List<KBatchedAnimController>[,] controllerGrid;

	private LinkedList<KBatchedAnimController> updateList = new LinkedList<KBatchedAnimController>();

	private LinkedList<KBatchedAnimController> alwaysUpdateList = new LinkedList<KBatchedAnimController>();

	private bool[,] visibleChunkGrid;

	private bool[,] previouslyVisibleChunkGrid;

	private List<Vector2I> visibleChunks = new List<Vector2I>();

	private List<Vector2I> previouslyVisibleChunks = new List<Vector2I>();

	private Vector2I vis_chunk_min = Vector2I.zero;

	private Vector2I vis_chunk_max = Vector2I.zero;

	private List<KBatchedAnimUpdater.RegistrationInfo> queuedRegistrations = new List<KBatchedAnimUpdater.RegistrationInfo>();

	private Dictionary<int, KBatchedAnimUpdater.ControllerChunkInfo> controllerChunkInfos = new Dictionary<int, KBatchedAnimUpdater.ControllerChunkInfo>();

	private List<KBatchedAnimUpdater.MovingControllerInfo> movingControllerInfos = new List<KBatchedAnimUpdater.MovingControllerInfo>();

	private const int CHUNKS_TO_CLEAN_PER_TICK = 16;

	private int cleanUpChunkIndex;

	private static readonly Vector2 VISIBLE_RANGE_SCALE = new Vector2(1.5f, 1.5f);

	public enum RegistrationState
	{
		Registered,
		PendingRemoval,
		Unregistered
	}

	private struct RegistrationInfo
	{
		public bool register;

		public int transformId;

		public int controllerInstanceId;

		public KBatchedAnimController controller;
	}

	private struct ControllerChunkInfo
	{
		public KBatchedAnimController controller;

		public Vector2I chunkXY;
	}

	private struct MovingControllerInfo
	{
		public int controllerInstanceId;

		public KBatchedAnimController controller;

		public Vector2I chunkXY;
	}
}
