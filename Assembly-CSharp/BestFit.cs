using System;
using System.Collections.Generic;
using ProcGen;
using TUNING;

public class BestFit
{
	public static Vector2I BestFitWorlds(List<WorldPlacement> worldsToArrange, bool ignoreBestFitY = false)
	{
		List<BestFit.Rect> list = new List<BestFit.Rect>();
		Vector2I vector2I = default(Vector2I);
		worldsToArrange.Sort((WorldPlacement a, WorldPlacement b) => b.height.CompareTo(a.height));
		int height = worldsToArrange[0].height;
		foreach (WorldPlacement worldPlacement in worldsToArrange)
		{
			Vector2I vector2I2 = default(Vector2I);
			while (!BestFit.UnoccupiedSpace(new BestFit.Rect(vector2I2.x, vector2I2.y, worldPlacement.width, worldPlacement.height), list))
			{
				if (ignoreBestFitY)
				{
					vector2I2.x++;
				}
				else if (vector2I2.y + worldPlacement.height >= height + 32)
				{
					vector2I2.y = 0;
					vector2I2.x++;
				}
				else
				{
					vector2I2.y++;
				}
			}
			vector2I.x = Math.Max(worldPlacement.width + vector2I2.x, vector2I.x);
			vector2I.y = Math.Max(worldPlacement.height + vector2I2.y, vector2I.y);
			list.Add(new BestFit.Rect(vector2I2.x, vector2I2.y, worldPlacement.width, worldPlacement.height));
			worldPlacement.SetPosition(vector2I2);
		}
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			vector2I.x += 136;
			vector2I.y = Math.Max(vector2I.y, 136);
		}
		return vector2I;
	}

	private static bool UnoccupiedSpace(BestFit.Rect RectA, List<BestFit.Rect> placed)
	{
		foreach (BestFit.Rect rect in placed)
		{
			if (RectA.X1 < rect.X2 && RectA.X2 > rect.X1 && RectA.Y1 < rect.Y2 && RectA.Y2 > rect.Y1)
			{
				return false;
			}
		}
		return true;
	}

	public static Vector2I GetGridOffset(IList<WorldContainer> existingWorlds, Vector2I newWorldSize, out Vector2I newWorldOffset)
	{
		List<BestFit.Rect> list = new List<BestFit.Rect>();
		foreach (WorldContainer worldContainer in existingWorlds)
		{
			list.Add(new BestFit.Rect(worldContainer.WorldOffset.x, worldContainer.WorldOffset.y, worldContainer.WorldSize.x, worldContainer.WorldSize.y));
		}
		Vector2I vector2I = new Vector2I(Grid.WidthInCells, 0);
		int widthInCells = Grid.WidthInCells;
		Vector2I vector2I2 = default(Vector2I);
		while (!BestFit.UnoccupiedSpace(new BestFit.Rect(vector2I2.x, vector2I2.y, newWorldSize.x, newWorldSize.y), list))
		{
			if (vector2I2.x + newWorldSize.x >= widthInCells)
			{
				vector2I2.x = 0;
				vector2I2.y++;
			}
			else
			{
				vector2I2.x++;
			}
		}
		Debug.Assert(vector2I2.x + newWorldSize.x <= Grid.WidthInCells, "BestFit is trying to expand the grid width, this is unsupported and will break the SIM.");
		vector2I.y = Math.Max(newWorldSize.y + vector2I2.y, Grid.HeightInCells);
		newWorldOffset = vector2I2;
		return vector2I;
	}

	public static int CountRocketInteriors(IList<WorldContainer> existingWorlds)
	{
		int num = 0;
		List<BestFit.Rect> list = new List<BestFit.Rect>();
		foreach (WorldContainer worldContainer in existingWorlds)
		{
			list.Add(new BestFit.Rect(worldContainer.WorldOffset.x, worldContainer.WorldOffset.y, worldContainer.WorldSize.x, worldContainer.WorldSize.y));
		}
		Vector2I rocket_INTERIOR_SIZE = ROCKETRY.ROCKET_INTERIOR_SIZE;
		Vector2I vector2I;
		while (BestFit.PlaceWorld(list, rocket_INTERIOR_SIZE, out vector2I))
		{
			num++;
			list.Add(new BestFit.Rect(vector2I.x, vector2I.y, rocket_INTERIOR_SIZE.x, rocket_INTERIOR_SIZE.y));
		}
		return num;
	}

	private static bool PlaceWorld(List<BestFit.Rect> placedWorlds, Vector2I newWorldSize, out Vector2I newWorldOffset)
	{
		Vector2I vector2I = new Vector2I(Grid.WidthInCells, 0);
		int widthInCells = Grid.WidthInCells;
		Vector2I vector2I2 = default(Vector2I);
		while (!BestFit.UnoccupiedSpace(new BestFit.Rect(vector2I2.x, vector2I2.y, newWorldSize.x, newWorldSize.y), placedWorlds))
		{
			if (vector2I2.x + newWorldSize.x >= widthInCells)
			{
				vector2I2.x = 0;
				vector2I2.y++;
			}
			else
			{
				vector2I2.x++;
			}
		}
		vector2I.y = Math.Max(newWorldSize.y + vector2I2.y, Grid.HeightInCells);
		newWorldOffset = vector2I2;
		return vector2I2.x + newWorldSize.x <= Grid.WidthInCells && vector2I2.y + newWorldSize.y <= Grid.HeightInCells;
	}

	private struct Rect
	{
		public int X1
		{
			get
			{
				return this.x;
			}
		}

		public int X2
		{
			get
			{
				return this.x + this.width + 2;
			}
		}

		public int Y1
		{
			get
			{
				return this.y;
			}
		}

		public int Y2
		{
			get
			{
				return this.y + this.height + 2;
			}
		}

		public Rect(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		private int x;

		private int y;

		private int width;

		private int height;
	}
}
