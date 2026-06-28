using System;
using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimUpdater
{
	public static KBatchedAnimUpdater instance
	{
		get
		{
			return Singleton<KBatchedAnimUpdater>.Instance;
		}
	}

	public static void Destroy()
	{
		KBatchedAnimUpdater.instance.Clear();
		Singleton<KBatchedAnimUpdater>.Destroy();
	}

	public static void CreateInstance()
	{
		Singleton<KBatchedAnimUpdater>.CreateInstance();
	}

	public void InitializeGrid()
	{
		this.Clear();
		int num = (Grid.WidthInCells + 16 - 1) / 16;
		int num2 = (Grid.HeightInCells + 16 - 1) / 16;
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

	public void Clear()
	{
		for (int i = 0; i < this.updateList.Count; i++)
		{
			if (this.updateList[i] != null)
			{
				global::UnityEngine.Object.DestroyImmediate(this.updateList[i]);
			}
		}
		this.updateList.Clear();
		for (int j = 0; j < this.alwaysUpdateList.Count; j++)
		{
			if (this.alwaysUpdateList[j] != null)
			{
				global::UnityEngine.Object.DestroyImmediate(this.alwaysUpdateList[j]);
			}
		}
		this.alwaysUpdateList.Clear();
		this.queuedRegistrations.Clear();
		this.newlyVisible.Clear();
		this.visibleChunks.Clear();
		this.previouslyVisibleChunks.Clear();
		this.controllerGrid = null;
		this.previouslyVisibleChunkGrid = null;
		this.visibleChunkGrid = null;
	}

	public void UpdateRegister(KBatchedAnimController controller)
	{
		this.queuedRegistrations.Add(new KBatchedAnimUpdater.RegistrationInfo
		{
			controller = controller,
			register = true,
			update = true
		});
	}

	public void UpdateUnregister(KBatchedAnimController controller)
	{
		if (App.IsExiting)
		{
			return;
		}
		this.queuedRegistrations.Add(new KBatchedAnimUpdater.RegistrationInfo
		{
			controller = controller,
			register = false,
			update = true
		});
	}

	public void VisibilityRegister(Vector2I chunk_xy, KBatchedAnimController controller)
	{
		this.queuedRegistrations.Add(new KBatchedAnimUpdater.RegistrationInfo
		{
			chunkXY = chunk_xy,
			controller = controller,
			register = true,
			update = false
		});
	}

	public void VisibilityUnregister(Vector2I chunk_xy, KBatchedAnimController controller)
	{
		if (App.IsExiting)
		{
			return;
		}
		this.queuedRegistrations.Add(new KBatchedAnimUpdater.RegistrationInfo
		{
			chunkXY = chunk_xy,
			controller = controller,
			register = false,
			update = false
		});
	}

	private List<KBatchedAnimController> GetControllerList(Vector2I chunk_xy)
	{
		if (this.controllerGrid == null || chunk_xy.x < 0 || chunk_xy.x >= this.controllerGrid.GetLength(0) || chunk_xy.y < 0 || chunk_xy.y > this.controllerGrid.GetLength(1))
		{
			return null;
		}
		return this.controllerGrid[chunk_xy.x, chunk_xy.y];
	}

	public void LateUpdate()
	{
		this.UpdateVisibility();
		this.ProcessRegistrations();
		this.CleanUp();
		float num = Time.unscaledDeltaTime;
		for (int i = 0; i < this.alwaysUpdateList.Count; i++)
		{
			this.alwaysUpdateList[i].UpdateAnim(num);
		}
		if (this.DoGridProcessing())
		{
			num = Time.deltaTime;
			for (int j = 0; j < this.updateList.Count; j++)
			{
				this.updateList[j].UpdateAnim(num);
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

	private void UpdateVisibility(KBatchedAnimController controller)
	{
		Vector2I vector2I = KBatchedAnimUpdater.PosToChunkXY(controller.transform.position);
		if (this.visibleChunkGrid[vector2I.x, vector2I.y])
		{
			controller.OnBecameVisible();
		}
		else
		{
			controller.OnBecameInvisible();
		}
	}

	public static Vector2I PosToChunkXY(Vector3 pos)
	{
		Vector2I vector2I = Grid.PosToXY(pos);
		return KAnimBatchManager.CellXYToChunkXY(vector2I);
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
		this.vis_chunk_min = new Vector2I(vector2I.x / 16, vector2I.y / 16);
		this.vis_chunk_max = new Vector2I(vector2I2.x / 16, vector2I2.y / 16);
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
						kbatchedAnimController.OnBecameVisible();
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
					list3[m].OnBecameInvisible();
				}
			}
		}
	}

	private void ProcessRegistrations()
	{
		for (int i = 0; i < this.queuedRegistrations.Count; i++)
		{
			KBatchedAnimUpdater.RegistrationInfo registrationInfo = this.queuedRegistrations[i];
			if (!(registrationInfo.controller == null))
			{
				if (registrationInfo.update)
				{
					List<KBatchedAnimController> list = ((registrationInfo.controller.visibilityType != KAnimControllerBase.VisibilityType.Always) ? this.updateList : this.alwaysUpdateList);
					if (registrationInfo.register)
					{
						list.Add(registrationInfo.controller);
					}
					else
					{
						list.Remove(registrationInfo.controller);
					}
				}
				else
				{
					List<KBatchedAnimController> controllerList = this.GetControllerList(registrationInfo.chunkXY);
					if (controllerList != null)
					{
						if (registrationInfo.register)
						{
							controllerList.Add(registrationInfo.controller);
							this.newlyVisible.Add(registrationInfo.controller);
						}
						else
						{
							controllerList.Remove(registrationInfo.controller);
						}
					}
				}
			}
		}
		this.queuedRegistrations.Clear();
		for (int j = 0; j < this.newlyVisible.Count; j++)
		{
			this.UpdateVisibility(this.newlyVisible[j]);
		}
		this.newlyVisible.Clear();
	}

	private void CleanUp()
	{
		this.updateList.RemoveAll((KBatchedAnimController item) => item == null);
		this.alwaysUpdateList.RemoveAll((KBatchedAnimController item) => item == null);
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
			List<KBatchedAnimController> list = this.controllerGrid[num2, num3];
			list.RemoveAll((KBatchedAnimController item) => item == null);
		}
		this.cleanUpChunkIndex = (this.cleanUpChunkIndex + 16) % this.controllerGrid.Length;
	}

	public static void GetVisibleCellRange(out Vector2I min, out Vector2I max)
	{
		Grid.GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);
		min.x -= 4;
		min.y -= 4;
		min.x = Math.Min(Grid.WidthInCells - 1, Math.Max(0, min.x));
		min.y = Math.Min(Grid.HeightInCells - 1, Math.Max(0, min.y));
		max.x += 4;
		max.y += 4;
		max.x = Math.Min(Grid.WidthInCells - 1, Math.Max(0, max.x));
		max.y = Math.Min(Grid.HeightInCells - 1, Math.Max(0, max.y));
	}

	private bool DoGridProcessing()
	{
		return this.controllerGrid != null && Camera.main != null;
	}

	private const int VISIBLE_BORDER = 4;

	private const int CHUNKS_TO_CLEAN_PER_TICK = 16;

	private List<KBatchedAnimController>[,] controllerGrid;

	private List<KBatchedAnimController> updateList = new List<KBatchedAnimController>();

	private List<KBatchedAnimController> alwaysUpdateList = new List<KBatchedAnimController>();

	private bool[,] visibleChunkGrid;

	private bool[,] previouslyVisibleChunkGrid;

	private List<Vector2I> visibleChunks = new List<Vector2I>();

	private List<Vector2I> previouslyVisibleChunks = new List<Vector2I>();

	private Vector2I vis_chunk_min = Vector2I.zero;

	private Vector2I vis_chunk_max = Vector2I.zero;

	private List<KBatchedAnimUpdater.RegistrationInfo> queuedRegistrations = new List<KBatchedAnimUpdater.RegistrationInfo>();

	private List<KBatchedAnimController> newlyVisible = new List<KBatchedAnimController>();

	private int cleanUpChunkIndex;

	private struct RegistrationInfo
	{
		public bool register;

		public bool update;

		public Vector2I chunkXY;

		public KBatchedAnimController controller;
	}
}
