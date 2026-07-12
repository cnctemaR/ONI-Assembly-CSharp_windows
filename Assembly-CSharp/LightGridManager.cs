using System;
using System.Collections.Generic;
using UnityEngine;

public static class LightGridManager
{
	public static int ComputeFalloff(float fallOffRate, int cell, int originCell, global::LightShape lightShape, DiscreteShadowCaster.Direction lightDirection)
	{
		int num = originCell;
		if (lightShape == global::LightShape.Quad)
		{
			Vector2I vector2I = Grid.CellToXY(num);
			Vector2I vector2I2 = Grid.CellToXY(cell);
			switch (lightDirection)
			{
			case DiscreteShadowCaster.Direction.North:
			case DiscreteShadowCaster.Direction.South:
			{
				Vector2I vector2I3 = new Vector2I(vector2I2.x, vector2I.y);
				num = Grid.XYToCell(vector2I3.x, vector2I3.y);
				break;
			}
			case DiscreteShadowCaster.Direction.East:
			case DiscreteShadowCaster.Direction.West:
			{
				Vector2I vector2I3 = new Vector2I(vector2I.x, vector2I2.y);
				num = Grid.XYToCell(vector2I3.x, vector2I3.y);
				break;
			}
			}
		}
		return LightGridManager.CalculateFalloff(fallOffRate, cell, num);
	}

	private static int CalculateFalloff(float falloffRate, int cell, int origin)
	{
		return Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float)Mathf.Max(Grid.GetCellDistance(origin, cell), 1)));
	}

	public static void Initialise()
	{
		LightGridManager.previewLux = new int[Grid.CellCount];
	}

	public static void Shutdown()
	{
		LightGridManager.previewLux = null;
		LightGridManager.previewLightCells.Clear();
	}

	public static void DestroyPreview()
	{
		foreach (global::Tuple<int, int> tuple in LightGridManager.previewLightCells)
		{
			LightGridManager.previewLux[tuple.first] = 0;
		}
		LightGridManager.previewLightCells.Clear();
	}

	public static void CreatePreview(int origin_cell, float radius, global::LightShape shape, int lux)
	{
		LightGridManager.CreatePreview(origin_cell, radius, shape, lux, 0, DiscreteShadowCaster.Direction.South);
	}

	public static void CreatePreview(int origin_cell, float radius, global::LightShape shape, int lux, int width, DiscreteShadowCaster.Direction direction)
	{
		LightGridManager.previewLightCells.Clear();
		ListPool<int, LightGridManager.LightGridEmitter>.PooledList pooledList = ListPool<int, LightGridManager.LightGridEmitter>.Allocate();
		pooledList.Add(origin_cell);
		DiscreteShadowCaster.GetVisibleCells(origin_cell, pooledList, (int)radius, width, direction, shape, true);
		foreach (int num in pooledList)
		{
			if (Grid.IsValidCell(num))
			{
				int num2 = lux / LightGridManager.ComputeFalloff(0.5f, num, origin_cell, shape, direction);
				LightGridManager.previewLightCells.Add(new global::Tuple<int, int>(num, num2));
				LightGridManager.previewLux[num] = num2;
			}
		}
		pooledList.Recycle();
	}

	public const float DEFAULT_FALLOFF_RATE = 0.5f;

	public static List<global::Tuple<int, int>> previewLightCells = new List<global::Tuple<int, int>>();

	public static int[] previewLux;

	public class LightGridEmitter
	{
		public void UpdateLitCells()
		{
			DiscreteShadowCaster.GetVisibleCells(this.state.origin, this.litCells, (int)this.state.radius, this.state.width, this.state.direction, this.state.shape, true);
		}

		public void AddToGrid(bool update_lit_cells)
		{
			DebugUtil.DevAssert(!update_lit_cells || this.litCells.Count == 0, "adding an already added emitter", null);
			if (update_lit_cells)
			{
				this.UpdateLitCells();
			}
			foreach (int num in this.litCells)
			{
				if (Grid.IsValidCell(num))
				{
					int num2 = Mathf.Max(0, Grid.LightCount[num] + this.ComputeLux(num));
					Grid.LightCount[num] = num2;
					LightGridManager.previewLux[num] = num2;
				}
			}
		}

		public void RemoveFromGrid()
		{
			foreach (int num in this.litCells)
			{
				if (Grid.IsValidCell(num))
				{
					Grid.LightCount[num] = Mathf.Max(0, Grid.LightCount[num] - this.ComputeLux(num));
					LightGridManager.previewLux[num] = 0;
				}
			}
			this.litCells.Clear();
		}

		public bool Refresh(LightGridManager.LightGridEmitter.State state, bool force = false)
		{
			if (!force && EqualityComparer<LightGridManager.LightGridEmitter.State>.Default.Equals(this.state, state))
			{
				return false;
			}
			this.RemoveFromGrid();
			this.state = state;
			this.AddToGrid(true);
			return true;
		}

		private int ComputeLux(int cell)
		{
			return this.state.intensity / this.ComputeFalloff(cell);
		}

		private int ComputeFalloff(int cell)
		{
			return LightGridManager.ComputeFalloff(this.state.falloffRate, cell, this.state.origin, this.state.shape, this.state.direction);
		}

		private LightGridManager.LightGridEmitter.State state = LightGridManager.LightGridEmitter.State.DEFAULT;

		private List<int> litCells = new List<int>();

		[Serializable]
		public struct State : IEquatable<LightGridManager.LightGridEmitter.State>
		{
			public bool Equals(LightGridManager.LightGridEmitter.State rhs)
			{
				return this.origin == rhs.origin && this.shape == rhs.shape && this.radius == rhs.radius && this.intensity == rhs.intensity && this.falloffRate == rhs.falloffRate && this.colour == rhs.colour && this.width == rhs.width && this.direction == rhs.direction;
			}

			public int origin;

			public global::LightShape shape;

			public int width;

			public DiscreteShadowCaster.Direction direction;

			public float radius;

			public int intensity;

			public float falloffRate;

			public Color colour;

			public static readonly LightGridManager.LightGridEmitter.State DEFAULT = new LightGridManager.LightGridEmitter.State
			{
				origin = Grid.InvalidCell,
				shape = global::LightShape.Circle,
				radius = 4f,
				intensity = 1,
				falloffRate = 0.5f,
				colour = Color.white,
				direction = DiscreteShadowCaster.Direction.South,
				width = 4
			};
		}
	}
}
