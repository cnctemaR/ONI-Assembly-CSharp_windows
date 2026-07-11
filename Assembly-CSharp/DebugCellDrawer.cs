using System;
using System.Collections.Generic;

public class DebugCellDrawer : KMonoBehaviour
{
	private void Update()
	{
		for (int i = 0; i < this.cells.Count; i++)
		{
			if (this.cells[i] != PathFinder.InvalidCell)
			{
				DebugExtension.DebugPoint(Grid.CellToPosCCF(this.cells[i], Grid.SceneLayer.Background), 1f, 0f, true);
			}
		}
	}

	public List<int> cells;
}
