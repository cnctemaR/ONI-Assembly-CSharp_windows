using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugCellDrawer : KMonoBehaviour
{
	private void Update()
	{
		for (int i = 0; i < this.cells.Count; i++)
		{
			if (this.cells[i] != PathFinder.InvalidCell)
			{
				Vector3 vector = Grid.CellToPosCCF(this.cells[i], Grid.SceneLayer.Background);
				DebugExtension.DebugPoint(vector, 1f, 0f, true);
			}
		}
	}

	public List<int> cells;
}
