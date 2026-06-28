using System;
using UnityEngine;

public class AnimTileable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		Building component = base.GetComponent<Building>();
		Extents extents = component.GetExtents();
		extents = new Extents(extents.x - 1, extents.y, extents.width + 2, 1);
		int mask = GameScenePartitioner.Instance.objectLayerMasks[3].mask;
		this.partitionerEntry = GameScenePartitioner.Instance.Add("AnimTileable.OnSpawn", base.gameObject, extents, mask, new Action<object>(this.OnNeighbourCellsUpdated));
		this.UpdateEndCaps();
	}

	protected override void OnCleanUp()
	{
		this.partitionerEntry.Release();
	}

	private void UpdateEndCaps()
	{
		Vector2I vector2I;
		Grid.PosToXY(this.transform.position, out vector2I);
		KPrefabID component = base.GetComponent<KPrefabID>();
		bool flag = true;
		bool flag2 = true;
		if (vector2I.x > 0)
		{
			int num = vector2I.y * Grid.WidthInCells + vector2I.x - 1;
			flag = !this.HasTileableNeighbour(component.PrefabTag, num);
		}
		if (vector2I.x < Grid.WidthInCells - 1)
		{
			int num2 = vector2I.y * Grid.WidthInCells + vector2I.x + 1;
			flag2 = !this.HasTileableNeighbour(component.PrefabTag, num2);
		}
		KBatchedAnimController[] componentsInChildren = base.GetComponentsInChildren<KBatchedAnimController>();
		foreach (KBatchedAnimController kbatchedAnimController in componentsInChildren)
		{
			foreach (KAnimHashedString kanimHashedString in AnimTileable.leftSymbols)
			{
				kbatchedAnimController.HideSymbol(!flag, kanimHashedString);
			}
			foreach (KAnimHashedString kanimHashedString2 in AnimTileable.rightSymbols)
			{
				kbatchedAnimController.HideSymbol(!flag2, kanimHashedString2);
			}
		}
	}

	private bool HasTileableNeighbour(Tag expected_tag, int neighbour_cell)
	{
		bool flag = false;
		GameObject gameObject = Grid.Objects[neighbour_cell, 3];
		if (gameObject != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (component != null && component.PrefabTag == expected_tag)
			{
				flag = true;
			}
		}
		return flag;
	}

	private void OnNeighbourCellsUpdated(object data)
	{
		this.UpdateEndCaps();
	}

	private GameScenePartitionerEntry partitionerEntry;

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
}
