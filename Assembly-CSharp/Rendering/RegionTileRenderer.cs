using System;
using System.Collections.Generic;

namespace Rendering
{
	public class RegionTileRenderer : BlockTileRenderer
	{
		public override BlockTileRenderer.Bits GetConnectionBits(int x, int y, int query_layer)
		{
			BlockTileRenderer.Bits bits = (BlockTileRenderer.Bits)0;
			Region region;
			if (y > 0)
			{
				int num = (y - 1) * Grid.WidthInCells + x;
				region = Game.Instance.RegionManager.GetIntersectionRegion(num - 1);
				if (x > 0 && region != null && region.QueryLayer == query_layer)
				{
					bits |= BlockTileRenderer.Bits.DownLeft;
				}
				region = Game.Instance.RegionManager.GetIntersectionRegion(num);
				if (region != null && region.QueryLayer == query_layer)
				{
					bits |= BlockTileRenderer.Bits.Down;
				}
				region = Game.Instance.RegionManager.GetIntersectionRegion(num + 1);
				if (x < Grid.WidthInCells - 1 && region != null && region.QueryLayer == query_layer)
				{
					bits |= BlockTileRenderer.Bits.DownRight;
				}
			}
			int num2 = y * Grid.WidthInCells + x;
			region = Game.Instance.RegionManager.GetIntersectionRegion(num2 - 1);
			if (x > 0 && region != null && region.QueryLayer == query_layer)
			{
				bits |= BlockTileRenderer.Bits.Left;
			}
			region = Game.Instance.RegionManager.GetIntersectionRegion(num2 + 1);
			if (x < Grid.WidthInCells - 1 && region != null && region.QueryLayer == query_layer)
			{
				bits |= BlockTileRenderer.Bits.Right;
			}
			if (y < Grid.HeightInCells - 1)
			{
				int num3 = (y + 1) * Grid.WidthInCells + x;
				region = Game.Instance.RegionManager.GetIntersectionRegion(num3 - 1);
				if (x > 0 && region != null && region.QueryLayer == query_layer)
				{
					bits |= BlockTileRenderer.Bits.UpLeft;
				}
				region = Game.Instance.RegionManager.GetIntersectionRegion(num3);
				if (region != null && region.QueryLayer == query_layer)
				{
					bits |= BlockTileRenderer.Bits.Up;
				}
				region = Game.Instance.RegionManager.GetIntersectionRegion(num3 + 1);
				if (x < Grid.WidthInCells + 1 && region != null && region.QueryLayer == query_layer)
				{
					bits |= BlockTileRenderer.Bits.UpRight;
				}
			}
			return bits;
		}

		public override BlockTileRenderer.Bits GetDecorConnectionBits(int x, int y, int query_layer)
		{
			return (BlockTileRenderer.Bits)0;
		}

		public void AddBlock(int renderLayer, BuildingDef def, int queryLayer, int cell)
		{
			KeyValuePair<BuildingDef, bool> keyValuePair = new KeyValuePair<BuildingDef, bool>(def, true);
			BlockTileRenderer.RenderInfo renderInfo;
			if (!this.renderInfo.TryGetValue(keyValuePair, out renderInfo))
			{
				renderInfo = new BlockTileRenderer.RenderInfo(queryLayer, renderLayer, def, SimHashes.Vacuum, 0f);
				this.renderInfo[keyValuePair] = renderInfo;
			}
			renderInfo.AddCell(cell);
		}
	}
}
