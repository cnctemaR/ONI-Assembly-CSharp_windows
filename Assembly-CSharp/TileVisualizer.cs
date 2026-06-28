using System;
using UnityEngine;

public class TileVisualizer
{
	private static void RefreshCellInternal(int cell, ObjectLayer tile_layer)
	{
		if (!Game.IsQuitting())
		{
			if (Grid.IsValidCell(cell))
			{
				GameObject gameObject = Grid.Objects[cell, (int)tile_layer];
				if (gameObject != null)
				{
					World.Instance.blockTileRenderer.Rebuild(tile_layer, cell);
					KAnimGraphTileVisualizer componentInChildren = gameObject.GetComponentInChildren<KAnimGraphTileVisualizer>();
					if (componentInChildren != null)
					{
						componentInChildren.Refresh();
					}
				}
			}
		}
	}

	public static void RefreshCell(int cell, ObjectLayer tile_layer)
	{
		TileVisualizer.RefreshCellInternal(cell, tile_layer);
		TileVisualizer.RefreshCellInternal(Grid.CellAbove(cell), tile_layer);
		TileVisualizer.RefreshCellInternal(Grid.CellBelow(cell), tile_layer);
		TileVisualizer.RefreshCellInternal(Grid.CellLeft(cell), tile_layer);
		TileVisualizer.RefreshCellInternal(Grid.CellRight(cell), tile_layer);
	}
}
