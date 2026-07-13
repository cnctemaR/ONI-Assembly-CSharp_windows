using System;
using UnityEngine;

public class AnimTilableSingleController : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.tagsOfNeightboursThatICanTileWith == null || this.tagsOfNeightboursThatICanTileWith.Length == 0)
		{
			this.tagsOfNeightboursThatICanTileWith = new Tag[] { base.GetComponent<KPrefabID>().PrefabTag };
		}
	}

	protected override void OnSpawn()
	{
		OccupyArea component = base.GetComponent<OccupyArea>();
		if (component != null)
		{
			this.extents = component.GetExtents();
		}
		else
		{
			Building component2 = base.GetComponent<Building>();
			this.extents = component2.GetExtents();
		}
		Extents extents = new Extents(this.extents.x - 1, this.extents.y - 1, this.extents.width + 2, this.extents.height + 2);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("AnimTileableSingleController.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[(int)this.objectLayer], new Action<object>(this.OnNeighbourCellsUpdated));
		base.GetComponent<KBatchedAnimController>();
		this.RefreshAnim();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	private void RefreshAnim()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (this.RefreshAnimCallback == null)
		{
			return;
		}
		int num = Grid.PosToCell(this);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		int num2;
		int num3;
		Grid.CellToXY(num, out num2, out num3);
		CellOffset rotatedCellOffset = new CellOffset(this.extents.x - num2 - 1, 0);
		CellOffset rotatedCellOffset2 = new CellOffset(this.extents.x - num2 + this.extents.width, 0);
		CellOffset rotatedCellOffset3 = new CellOffset(0, this.extents.y - num3 + this.extents.height);
		CellOffset rotatedCellOffset4 = new CellOffset(0, this.extents.y - num3 - 1);
		Rotatable component2 = base.GetComponent<Rotatable>();
		if (component2)
		{
			rotatedCellOffset = component2.GetRotatedCellOffset(rotatedCellOffset);
			rotatedCellOffset2 = component2.GetRotatedCellOffset(rotatedCellOffset2);
			rotatedCellOffset3 = component2.GetRotatedCellOffset(rotatedCellOffset3);
			rotatedCellOffset4 = component2.GetRotatedCellOffset(rotatedCellOffset4);
		}
		int num4 = Grid.OffsetCell(num, rotatedCellOffset);
		int num5 = Grid.OffsetCell(num, rotatedCellOffset2);
		int num6 = Grid.OffsetCell(num, rotatedCellOffset3);
		int num7 = Grid.OffsetCell(num, rotatedCellOffset4);
		if (Grid.IsValidCell(num4))
		{
			flag = this.HasTileableNeighbour(num4);
		}
		if (Grid.IsValidCell(num5))
		{
			flag2 = this.HasTileableNeighbour(num5);
		}
		if (Grid.IsValidCell(num6))
		{
			flag3 = this.HasTileableNeighbour(num6);
		}
		if (Grid.IsValidCell(num7))
		{
			flag4 = this.HasTileableNeighbour(num7);
		}
		this.RefreshAnimCallback(component, flag3, flag2, flag4, flag);
	}

	private bool HasTileableNeighbour(int neighbour_cell)
	{
		bool flag = false;
		GameObject gameObject = Grid.Objects[neighbour_cell, (int)this.objectLayer];
		if (gameObject != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (component != null && component.HasAnyTags(this.tagsOfNeightboursThatICanTileWith))
			{
				flag = true;
			}
		}
		return flag;
	}

	private void OnNeighbourCellsUpdated(object data)
	{
		if (this.partitionerEntry.IsValid())
		{
			this.RefreshAnim();
		}
	}

	private HandleVector<int>.Handle partitionerEntry;

	public ObjectLayer objectLayer = ObjectLayer.Building;

	public Tag[] tagsOfNeightboursThatICanTileWith;

	private Extents extents;

	public Action<KBatchedAnimController, bool, bool, bool, bool> RefreshAnimCallback;
}
