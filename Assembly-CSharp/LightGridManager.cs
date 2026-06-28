using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class LightGridManager
{
	public static int GetCellsContainingLightsCount()
	{
		return 0;
	}

	public static int GetCellsEffectedByLightsCount()
	{
		return 0;
	}

	public static void Initialise()
	{
		LightGridManager.activeLightEffectedCells = new List<int>[Grid.CellCount];
		LightGridManager.activeLights = new Dictionary<int, LightGridManager.LightGridEmitter>();
		LightGridManager.lightColour = new Color32[Grid.CellCount];
		LightGridManager.cellsEffectedByLights = new HashSet<int>();
	}

	public static void Shutdown()
	{
		LightGridManager.activeLightEffectedCells = null;
		LightGridManager.activeLights = null;
		LightGridManager.lightColour = null;
		LightGridManager.cellsEffectedByLights = null;
	}

	public static void GetEmitForTemperature(float temperature, ref int emitIntensity, ref float emitDistance, ref Color emitColour)
	{
		emitIntensity = Mathf.Max(emitIntensity, 1);
		emitDistance = Mathf.Max(emitDistance, 3f);
		emitColour = Color.black;
		if (temperature > 1000f)
		{
			temperature /= 100f;
			if (temperature <= 66f)
			{
				emitColour.r = 1f;
			}
			else
			{
				float num = temperature - 60f;
				num = 329.69873f * Mathf.Pow(num, -0.13320476f);
				emitColour.r = num / 255f;
				if (emitColour.r > 1f)
				{
					emitColour.r = 1f;
				}
			}
			if (temperature <= 66f)
			{
				float num = temperature;
				num = 99.4708f * Mathf.Log(num) - 161.11957f;
				emitColour.g = num / 255f;
				if (emitColour.g > 1f)
				{
					emitColour.g = 1f;
				}
			}
			else
			{
				float num = temperature - 60f;
				num = 288.12216f * Mathf.Pow(num, -0.075514846f);
				emitColour.g = num / 255f;
				if (emitColour.g > 1f)
				{
					emitColour.g = 1f;
				}
			}
			if (temperature >= 66f)
			{
				emitColour.b = 1f;
			}
			else if (temperature <= 19f)
			{
				emitColour.b = 0f;
			}
			else
			{
				float num = temperature - 10f;
				num = 138.51773f * Mathf.Log(num) - 305.0448f;
				emitColour.b = num / 255f;
				if (emitColour.b > 1f)
				{
					emitColour.b = 1f;
				}
			}
		}
	}

	public static void SetActiveWindowOnce(Vector2I arStart, Vector2I arEnd)
	{
		LightGridManager.activeRegionStart = Grid.Constrain(arStart);
		LightGridManager.activeRegionEnd = Grid.Constrain(arEnd);
	}

	public static void SetActiveWindow(Vector2I arStart, Vector2I arEnd)
	{
		arStart = Grid.Constrain(arStart);
		arEnd = Grid.Constrain(arEnd);
		LightGridManager.activeRegionStart = Grid.Constrain(LightGridManager.activeRegionStart);
		LightGridManager.activeRegionEnd = Grid.Constrain(LightGridManager.activeRegionEnd);
		if (LightGridManager.activeRegionStart.x != arStart.x || LightGridManager.activeRegionStart.y != arStart.y || LightGridManager.activeRegionEnd.x != arEnd.x || LightGridManager.activeRegionEnd.y != arEnd.y)
		{
			for (int i = LightGridManager.activeRegionStart.x; i < LightGridManager.activeRegionEnd.x; i++)
			{
				if (i < arStart.x || i >= arEnd.x)
				{
					for (int j = LightGridManager.activeRegionStart.y; j < LightGridManager.activeRegionEnd.y; j++)
					{
						if (j < arStart.y || j >= arEnd.y)
						{
							int num = Grid.XYToCell(i, j);
							bool flag = Grid.Element[num].HasTag(GameTags.EmitsLight);
							if (flag)
							{
								LightGridManager.RemoveFromLightGrid(num);
							}
						}
					}
				}
			}
			LightGridManager.activeRegionStart = arStart;
			LightGridManager.activeRegionEnd = arEnd;
			for (int k = LightGridManager.activeRegionStart.x; k < LightGridManager.activeRegionEnd.x; k++)
			{
				for (int l = LightGridManager.activeRegionStart.y; l < LightGridManager.activeRegionEnd.y; l++)
				{
					int num2 = Grid.XYToCell(k, l);
					bool flag2 = Grid.Element[num2].HasTag(GameTags.EmitsLight);
					int emitIntensity = Grid.Element[num2].emitIntensity;
					float emitDistance = Grid.Element[num2].emitDistance;
					Color color = Grid.Element[num2].substance.colour;
					if (flag2)
					{
						LightGridManager.AddToLightGrid(num2, emitIntensity, emitDistance, color, LightShape.Circle);
					}
				}
			}
		}
	}

	public static void GetEmittersForCell(int cell, ref List<LightGridManager.LightGridEmitter> lightsInCell, ref List<LightGridManager.LightGridEmitter> lightsEffectingCell)
	{
	}

	public static void AddToLightGrid(int cell, int intensity, float radius, Color colour, LightShape shape)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		LightGridManager.LightGridEmitter lightGridEmitter = new LightGridManager.LightGridEmitter(cell, intensity, radius, colour, shape);
		LightGridManager.LightGridEmitter lightGridEmitter2 = null;
		if (LightGridManager.activeLights.TryGetValue(cell, out lightGridEmitter2))
		{
			lightGridEmitter2.Remove(LightGridManager.activeLightEffectedCells, LightGridManager.cellsEffectedByLights);
		}
		LightGridManager.activeLights[cell] = lightGridEmitter;
		lightGridEmitter.Recompute(LightGridManager.activeLightEffectedCells, LightGridManager.cellsEffectedByLights);
	}

	public static void RemoveFromLightGrid(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		LightGridManager.LightGridEmitter lightGridEmitter = null;
		if (LightGridManager.activeLights.TryGetValue(cell, out lightGridEmitter))
		{
			lightGridEmitter.Remove(LightGridManager.activeLightEffectedCells, LightGridManager.cellsEffectedByLights);
			LightGridManager.activeLights.Remove(cell);
		}
	}

	public static void DestroyPreview()
	{
		List<int> list = LightGridManager.clearingPreviewLightCells;
		LightGridManager.clearingPreviewLightCells = LightGridManager.previewLightCells;
		LightGridManager.previewLightCells = list;
		LightGridManager.previewLightCells.Clear();
		foreach (int num in LightGridManager.clearingPreviewLightCells)
		{
			LightGridManager.lightColour[num] = LightGridManager.CalculateColorForCell(num);
		}
	}

	public static void CreatePreview(int origin_cell, float radius, LightShape shape)
	{
		LightGridManager.previewLightCells.Clear();
		LightGridManager.previewLightCells.Add(origin_cell);
		DiscreteShadowCaster.GetVisibleCells(origin_cell, LightGridManager.previewLightCells, (int)radius, shape);
		foreach (int num in LightGridManager.previewLightCells)
		{
			LightGridManager.lightColour[num] = LightGridManager.CalculateColorForCell(num);
		}
	}

	public static Color32 CalculateColorForCell(int cell)
	{
		Color32 color = new Color32(0, 0, 0, 0);
		if (LightGridManager.activeLightEffectedCells[cell] != null && LightGridManager.activeLightEffectedCells[cell].Count > 0)
		{
			int count = LightGridManager.activeLightEffectedCells[cell].Count;
			Vector3F vector3F = default(Vector3F);
			for (int i = 0; i < count; i++)
			{
				int num = LightGridManager.activeLightEffectedCells[cell][i];
				LightGridManager.LightGridEmitter lightGridEmitter = LightGridManager.activeLights[num];
				Vector3F vector3F2 = new Vector3F(lightGridEmitter.colour.r, lightGridEmitter.colour.g, lightGridEmitter.colour.b);
				vector3F += vector3F2;
			}
			float num2 = (float)Grid.LightCount[cell] / 14f;
			color = new Color(vector3F.x / (float)count, vector3F.y / (float)count, vector3F.z / (float)count, num2);
		}
		else if (LightGridManager.previewLightCells.Contains(cell))
		{
			color = LIGHT2D.LIGHT_PREVIEW_COLOR;
		}
		return color;
	}

	public static Color32 GetColorForCell(int cell)
	{
		return LightGridManager.lightColour[cell];
	}

	public const float minTemperatureVisible = 1000f;

	private const int maxLightsPerFrame = 500;

	private const int minEmitDistance = 3;

	private static List<int> previewLightCells = new List<int>();

	private static List<int> clearingPreviewLightCells = new List<int>();

	private static List<int>[] activeLightEffectedCells;

	private static Dictionary<int, LightGridManager.LightGridEmitter> activeLights;

	private static Color32[] lightColour;

	private static HashSet<int> cellsEffectedByLights = new HashSet<int>();

	private static Vector2I activeRegionStart = new Vector2I(0, 0);

	private static Vector2I activeRegionEnd = new Vector2I(0, 0);

	public class LightGridEmitter
	{
		public LightGridEmitter(int cell, int intensity, float radius, Color colour, LightShape shape)
		{
			this.cell = cell;
			this.radius = radius;
			this.intensity = intensity;
			this.colour = colour;
			this.shape = shape;
			this.cellsEffected = new List<int>();
		}

		public void Recompute(List<int>[] activeLightEffectedCells, HashSet<int> cellsEffectedByLights)
		{
			if (this.cellsEffected.Count > 0)
			{
				this.Remove(activeLightEffectedCells, cellsEffectedByLights);
			}
			this.cellsEffected.Clear();
			DiscreteShadowCaster.GetVisibleCells(this.cell, this.cellsEffected, (int)this.radius, this.shape);
			this.cellsEffected.Add(this.cell);
			for (int i = 0; i < this.cellsEffected.Count; i++)
			{
				int num = this.cellsEffected[i];
				Grid.LightCount[num] = (byte)Mathf.Max(0, (int)Grid.LightCount[num] + this.intensity);
				if (activeLightEffectedCells[num] == null)
				{
					activeLightEffectedCells[num] = new List<int>();
				}
				activeLightEffectedCells[num].Add(this.cell);
				cellsEffectedByLights.Add(num);
				LightGridManager.lightColour[num] = LightGridManager.CalculateColorForCell(num);
			}
		}

		public void Remove(List<int>[] activeLightEffectedCells, HashSet<int> cellsEffectedByLights)
		{
			for (int i = 0; i < this.cellsEffected.Count; i++)
			{
				int num = this.cellsEffected[i];
				Grid.LightCount[num] = (byte)Mathf.Max(0, (int)Grid.LightCount[num] - this.intensity);
				activeLightEffectedCells[num].Remove(this.cell);
				if (Grid.LightCount[num] == 0)
				{
					cellsEffectedByLights.Remove(num);
					LightGridManager.lightColour[num] = LightGridManager.CalculateColorForCell(num);
				}
			}
			this.cellsEffected.Clear();
		}

		public int cell = -1;

		public float radius = 4f;

		public int intensity = 1;

		public Color colour = Color.white;

		public List<int> cellsEffected;

		public LightShape shape;
	}
}
