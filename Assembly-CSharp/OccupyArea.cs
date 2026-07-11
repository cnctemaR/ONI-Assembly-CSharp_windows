using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[SkipSaveFileSerialization]
public class OccupyArea : KMonoBehaviour
{
	public bool ApplyToCells
	{
		get
		{
			return this.applyToCells;
		}
		set
		{
			if (value != this.applyToCells)
			{
				if (value)
				{
					this.UpdateOccupiedArea();
				}
				else
				{
					this.ClearOccupiedArea();
				}
				this.applyToCells = value;
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.applyToCells)
		{
			this.UpdateOccupiedArea();
		}
	}

	private void ValidatePosition()
	{
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			global::Debug.LogWarning(base.name + " is outside the grid! DELETING!", null);
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		this.ValidatePosition();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		this.ValidatePosition();
	}

	public void SetCellOffsets(CellOffset[] cells)
	{
		this.OccupiedCellsOffsets = cells;
	}

	public bool CheckIsOccupying(int checkCell)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (checkCell == num)
		{
			return true;
		}
		foreach (CellOffset cellOffset in this.OccupiedCellsOffsets)
		{
			if (Grid.OffsetCell(num, cellOffset) == checkCell)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.ClearOccupiedArea();
	}

	private void ClearOccupiedArea()
	{
		if (this.occupiedGridCells == null)
		{
			return;
		}
		foreach (ObjectLayer objectLayer in this.objectLayers)
		{
			if (objectLayer != ObjectLayer.NumLayers)
			{
				foreach (int num in this.occupiedGridCells)
				{
					if (Grid.Objects[num, (int)objectLayer] == base.gameObject)
					{
						Grid.Objects[num, (int)objectLayer] = null;
					}
				}
			}
		}
	}

	public void UpdateOccupiedArea()
	{
		if (this.objectLayers.Length == 0)
		{
			return;
		}
		if (this.occupiedGridCells == null)
		{
			this.occupiedGridCells = new int[this.OccupiedCellsOffsets.Length];
		}
		this.ClearOccupiedArea();
		int num = Grid.PosToCell(base.gameObject);
		foreach (ObjectLayer objectLayer in this.objectLayers)
		{
			if (objectLayer != ObjectLayer.NumLayers)
			{
				for (int j = 0; j < this.OccupiedCellsOffsets.Length; j++)
				{
					CellOffset cellOffset = this.OccupiedCellsOffsets[j];
					int num2 = Grid.OffsetCell(num, cellOffset);
					Grid.Objects[num2, (int)objectLayer] = base.gameObject;
					this.occupiedGridCells[j] = num2;
				}
			}
		}
	}

	public int GetWidthInCells()
	{
		int num = int.MaxValue;
		int num2 = int.MinValue;
		foreach (CellOffset cellOffset in this.OccupiedCellsOffsets)
		{
			num = Math.Min(num, cellOffset.x);
			num2 = Math.Max(num2, cellOffset.x);
		}
		return num2 - num + 1;
	}

	public int GetHeightInCells()
	{
		int num = int.MaxValue;
		int num2 = int.MinValue;
		foreach (CellOffset cellOffset in this.OccupiedCellsOffsets)
		{
			num = Math.Min(num, cellOffset.y);
			num2 = Math.Max(num2, cellOffset.y);
		}
		return num2 - num + 1;
	}

	public Extents GetExtents()
	{
		return new Extents(Grid.PosToCell(base.gameObject), this.OccupiedCellsOffsets);
	}

	private void OnDrawGizmosSelected()
	{
		int num = Grid.PosToCell(base.gameObject);
		if (this.OccupiedCellsOffsets != null)
		{
			foreach (CellOffset cellOffset in this.OccupiedCellsOffsets)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(Grid.CellToPos(Grid.OffsetCell(num, cellOffset)) + Vector3.right / 2f + Vector3.up / 2f, Vector3.one);
			}
		}
		if (this.AboveOccupiedCellOffsets != null)
		{
			foreach (CellOffset cellOffset2 in this.AboveOccupiedCellOffsets)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireCube(Grid.CellToPos(Grid.OffsetCell(num, cellOffset2)) + Vector3.right / 2f + Vector3.up / 2f, Vector3.one * 0.9f);
			}
		}
	}

	public bool CanOccupyArea(int rootCell, ObjectLayer layer)
	{
		for (int i = 0; i < this.OccupiedCellsOffsets.Length; i++)
		{
			CellOffset cellOffset = this.OccupiedCellsOffsets[i];
			int num = Grid.OffsetCell(rootCell, cellOffset);
			if (Grid.Objects[num, (int)layer] != null)
			{
				return false;
			}
		}
		return true;
	}

	public bool TestArea(int rootCell, object data, Func<int, object, bool> testDelegate)
	{
		for (int i = 0; i < this.OccupiedCellsOffsets.Length; i++)
		{
			CellOffset cellOffset = this.OccupiedCellsOffsets[i];
			int num = Grid.OffsetCell(rootCell, cellOffset);
			if (!testDelegate(num, data))
			{
				return false;
			}
		}
		return true;
	}

	public bool TestAreaAbove(int rootCell, object data, Func<int, object, bool> testDelegate)
	{
		if (this.AboveOccupiedCellOffsets == null)
		{
			List<CellOffset> list = new List<CellOffset>();
			for (int i = 0; i < this.OccupiedCellsOffsets.Length; i++)
			{
				CellOffset cellOffset = new CellOffset(this.OccupiedCellsOffsets[i].x, this.OccupiedCellsOffsets[i].y + 1);
				if (Array.IndexOf<CellOffset>(this.OccupiedCellsOffsets, cellOffset) == -1)
				{
					list.Add(cellOffset);
				}
			}
			this.AboveOccupiedCellOffsets = list.ToArray();
		}
		for (int j = 0; j < this.AboveOccupiedCellOffsets.Length; j++)
		{
			int num = Grid.OffsetCell(rootCell, this.AboveOccupiedCellOffsets[j]);
			if (!testDelegate(num, data))
			{
				return false;
			}
		}
		return true;
	}

	public CellOffset[] OccupiedCellsOffsets;

	private CellOffset[] AboveOccupiedCellOffsets;

	private int[] occupiedGridCells;

	public ObjectLayer[] objectLayers = new ObjectLayer[0];

	[SerializeField]
	private bool applyToCells = true;
}
