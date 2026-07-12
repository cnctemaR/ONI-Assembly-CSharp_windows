using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AntiCluster")]
public class MoverLayerOccupier : KMonoBehaviour, ISim200ms
{
	private void RefreshCellOccupy()
	{
		int num = Grid.PosToCell(this);
		foreach (CellOffset cellOffset in this.cellOffsets)
		{
			int num2 = Grid.OffsetCell(num, cellOffset);
			if (this.previousCell != Grid.InvalidCell)
			{
				int num3 = Grid.OffsetCell(this.previousCell, cellOffset);
				this.UpdateCell(num3, num2);
			}
			else
			{
				this.UpdateCell(this.previousCell, num2);
			}
		}
		this.previousCell = num;
	}

	public void Sim200ms(float dt)
	{
		this.RefreshCellOccupy();
	}

	private void UpdateCell(int previous_cell, int current_cell)
	{
		foreach (ObjectLayer objectLayer in this.objectLayers)
		{
			if (previous_cell != Grid.InvalidCell && previous_cell != current_cell && Grid.Objects[previous_cell, (int)objectLayer] == base.gameObject)
			{
				Grid.Objects[previous_cell, (int)objectLayer] = null;
			}
			GameObject gameObject = Grid.Objects[current_cell, (int)objectLayer];
			if (gameObject == null)
			{
				Grid.Objects[current_cell, (int)objectLayer] = base.gameObject;
			}
			else
			{
				KPrefabID component = base.GetComponent<KPrefabID>();
				KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
				if (component.InstanceID > component2.InstanceID)
				{
					Grid.Objects[current_cell, (int)objectLayer] = base.gameObject;
				}
			}
		}
	}

	private void CleanUpOccupiedCells()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		foreach (CellOffset cellOffset in this.cellOffsets)
		{
			int num2 = Grid.OffsetCell(num, cellOffset);
			foreach (ObjectLayer objectLayer in this.objectLayers)
			{
				if (Grid.Objects[num2, (int)objectLayer] == base.gameObject)
				{
					Grid.Objects[num2, (int)objectLayer] = null;
				}
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.RefreshCellOccupy();
	}

	protected override void OnCleanUp()
	{
		this.CleanUpOccupiedCells();
		base.OnCleanUp();
	}

	private int previousCell = Grid.InvalidCell;

	public ObjectLayer[] objectLayers;

	public CellOffset[] cellOffsets;
}
