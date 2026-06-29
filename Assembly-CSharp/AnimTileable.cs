using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class AnimTileable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.tags == null || this.tags.Length == 0)
		{
			this.tags = new Tag[] { base.GetComponent<KPrefabID>().PrefabTag };
		}
	}

	protected override void OnSpawn()
	{
		OccupyArea component = base.GetComponent<OccupyArea>();
		Extents extents;
		if (component != null)
		{
			extents = component.GetExtents();
		}
		else
		{
			Building component2 = base.GetComponent<Building>();
			extents = component2.GetExtents();
		}
		extents = new Extents(extents.x - 1, extents.y - 1, extents.width + 2, extents.height + 2);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("AnimTileable.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[(int)this.objectLayer], new Action<object>(this.OnNeighbourCellsUpdated));
		this.UpdateEndCaps();
	}

	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		base.OnCleanUp();
	}

	private void UpdateEndCaps()
	{
		int num = Grid.PosToCell(this);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		int num2 = Grid.CellLeft(num);
		int num3 = Grid.CellRight(num);
		int num4 = Grid.CellAbove(num);
		int num5 = Grid.CellBelow(num);
		if (Grid.IsValidCell(num2))
		{
			flag = !this.HasTileableNeighbour(num2);
		}
		if (Grid.IsValidCell(num3))
		{
			flag2 = !this.HasTileableNeighbour(num3);
		}
		if (Grid.IsValidCell(num4))
		{
			flag3 = !this.HasTileableNeighbour(num4);
		}
		if (Grid.IsValidCell(num5))
		{
			flag4 = !this.HasTileableNeighbour(num5);
		}
		KBatchedAnimController[] componentsInChildren = base.GetComponentsInChildren<KBatchedAnimController>();
		foreach (KBatchedAnimController kbatchedAnimController in componentsInChildren)
		{
			foreach (KAnimHashedString kanimHashedString in AnimTileable.leftSymbols)
			{
				kbatchedAnimController.SetSymbolVisiblity(kanimHashedString, flag);
			}
			foreach (KAnimHashedString kanimHashedString2 in AnimTileable.rightSymbols)
			{
				kbatchedAnimController.SetSymbolVisiblity(kanimHashedString2, flag2);
			}
			foreach (KAnimHashedString kanimHashedString3 in AnimTileable.topSymbols)
			{
				kbatchedAnimController.SetSymbolVisiblity(kanimHashedString3, flag3);
			}
			foreach (KAnimHashedString kanimHashedString4 in AnimTileable.bottomSymbols)
			{
				kbatchedAnimController.SetSymbolVisiblity(kanimHashedString4, flag4);
			}
		}
	}

	private bool HasTileableNeighbour(int neighbour_cell)
	{
		bool flag = false;
		GameObject gameObject = Grid.Objects[neighbour_cell, (int)this.objectLayer];
		if (gameObject != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (component != null)
			{
				for (int i = 0; i < this.tags.Length; i++)
				{
					if (component.PrefabTag == this.tags[i])
					{
						flag = true;
						break;
					}
				}
			}
		}
		return flag;
	}

	private void OnNeighbourCellsUpdated(object data)
	{
		if (this == null || base.gameObject == null)
		{
			return;
		}
		if (this.partitionerEntry != null)
		{
			this.UpdateEndCaps();
		}
	}

	private GameScenePartitionerEntry partitionerEntry;

	public ObjectLayer objectLayer = ObjectLayer.Building;

	public Tag[] tags;

	private static readonly KAnimHashedString[] leftSymbols = new KAnimHashedString[]
	{
		new KAnimHashedString("cap_left"),
		new KAnimHashedString("cap_left_fg"),
		new KAnimHashedString("cap_left_place")
	};

	private static readonly KAnimHashedString[] rightSymbols = new KAnimHashedString[]
	{
		new KAnimHashedString("cap_right"),
		new KAnimHashedString("cap_right_fg"),
		new KAnimHashedString("cap_right_place")
	};

	private static readonly KAnimHashedString[] topSymbols = new KAnimHashedString[]
	{
		new KAnimHashedString("cap_top"),
		new KAnimHashedString("cap_top_fg"),
		new KAnimHashedString("cap_top_place")
	};

	private static readonly KAnimHashedString[] bottomSymbols = new KAnimHashedString[]
	{
		new KAnimHashedString("cap_bottom"),
		new KAnimHashedString("cap_bottom_fg"),
		new KAnimHashedString("cap_bottom_place")
	};
}
