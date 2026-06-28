using System;
using UnityEngine;

namespace Rendering.World
{
	public class SpaceBorderTileRenderer : TileRenderer
	{
		public void FreeResources()
		{
			if (this.material != null)
			{
				global::UnityEngine.Object.DestroyImmediate(this.material);
			}
			this.Brushes = null;
			SpaceBorderTileRenderer.Instance = null;
		}

		public override void LoadBrushes()
		{
			SpaceBorderTileRenderer.Instance = this;
			this.Brushes = new Brush[this.Masks.Length];
			this.material = new Material(Shader.Find("Klei/SpaceBorder"));
			this.material.SetTexture("_MainTex", this.Atlas.texture);
			for (int i = 0; i < this.Masks.Length; i++)
			{
				int num = i;
				this.Brushes[num] = new Brush(num, i.ToString(), this.material, this.Masks[i], this.ActiveBrushes, this.DirtyBrushes, this.TileGridWidth, null);
			}
		}

		private static bool IsBorder(int cell)
		{
			if (Grid.Element[cell].id == SimHashes.Void)
			{
				return false;
			}
			for (int i = -1; i <= 1; i++)
			{
				int num = Grid.OffsetCell(cell, new CellOffset(i, 0));
				if (Grid.IsValidCell(num) && Grid.Element[num].id == SimHashes.Void)
				{
					return true;
				}
			}
			for (int j = -1; j <= 1; j++)
			{
				int num2 = Grid.OffsetCell(cell, new CellOffset(0, j));
				if (Grid.IsValidCell(num2) && Grid.Element[num2].id == SimHashes.Void)
				{
					return true;
				}
			}
			return false;
		}

		public override void MarkDirty(ref Tile tile, Brush[] brush_array, int[] brush_grid)
		{
			SpaceBorderTileRenderer.isBorder[0] = SpaceBorderTileRenderer.IsBorder(tile.TileCells.Cell0);
			SpaceBorderTileRenderer.isBorder[1] = SpaceBorderTileRenderer.IsBorder(tile.TileCells.Cell1);
			SpaceBorderTileRenderer.isBorder[2] = SpaceBorderTileRenderer.IsBorder(tile.TileCells.Cell2);
			SpaceBorderTileRenderer.isBorder[3] = SpaceBorderTileRenderer.IsBorder(tile.TileCells.Cell3);
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if (SpaceBorderTileRenderer.isBorder[i])
				{
					num += 1 << i;
				}
			}
			if (num != 0)
			{
				Brush brush = brush_array[num];
				brush_grid[tile.Idx * 4] = brush.Id;
				brush.Add(tile.Idx);
			}
		}

		public static SpaceBorderTileRenderer Instance;

		private Material material;

		private static bool[] isBorder = new bool[4];
	}
}
