using System;
using System.Runtime.Serialization;
using UnityEngine;

public class OccupyArea : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateOccupiedArea();
	}

	private void ValidatePosition()
	{
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			Debug.LogWarning(base.name + " is outside the grid! DELETING!");
			global::UnityEngine.Object.Destroy(base.gameObject);
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
		if (this.objectLayer == ObjectLayer.NumLayers)
		{
			return;
		}
		foreach (int num in this.occupiedGridCells)
		{
			if (Grid.Objects[num, (int)this.objectLayer] == base.gameObject)
			{
				Grid.Objects[num, (int)this.objectLayer] = null;
			}
		}
	}

	private void UpdateOccupiedArea()
	{
		if (this.objectLayer == ObjectLayer.NumLayers)
		{
			return;
		}
		if (this.occupiedGridCells == null)
		{
			this.occupiedGridCells = new int[this.OccupiedCellsOffsets.Length];
		}
		this.ClearOccupiedArea();
		int num = Grid.PosToCell(base.gameObject);
		for (int i = 0; i < this.occupiedGridCells.Length; i++)
		{
			CellOffset cellOffset = this.OccupiedCellsOffsets[i];
			int num2 = Grid.OffsetCell(num, cellOffset);
			Grid.Objects[num2, (int)this.objectLayer] = base.gameObject;
			this.occupiedGridCells[i] = num2;
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

	private void OnDrawGizmosSelected()
	{
		int num = Grid.PosToCell(base.gameObject);
		if (this.OccupiedCellsOffsets != null)
		{
			foreach (CellOffset cellOffset in this.OccupiedCellsOffsets)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(Grid.CellToPos(Grid.OffsetCell(num, cellOffset)) + Vector3.right / 2f + Vector3.up / 2f, Vector3.one);
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube(Grid.CellToPos(Grid.OffsetCell(num, cellOffset)) + Vector3.right / 2f + Vector3.up / 2f, Vector3.one / 0.9f);
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

	public bool TestArea(int rootCell, Func<int, bool> testDelegate)
	{
		for (int i = 0; i < this.OccupiedCellsOffsets.Length; i++)
		{
			CellOffset cellOffset = this.OccupiedCellsOffsets[i];
			int num = Grid.OffsetCell(rootCell, cellOffset);
			if (!testDelegate(num))
			{
				return false;
			}
		}
		return true;
	}

	public CellOffset[] OccupiedCellsOffsets;

	private int[] occupiedGridCells;

	public ObjectLayer objectLayer = ObjectLayer.NumLayers;
}
