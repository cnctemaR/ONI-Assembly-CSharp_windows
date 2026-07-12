using System;
using System.Collections.Generic;
using UnityEngine;

public static class RadiationGridManager
{
	public static int CalculateFalloff(float falloffRate, int cell, int origin)
	{
		return Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float)Mathf.Max(Grid.GetCellDistance(origin, cell), 1)));
	}

	public static void Initialise()
	{
		RadiationGridManager.emitters = new List<RadiationGridEmitter>();
	}

	public static void Shutdown()
	{
		RadiationGridManager.emitters.Clear();
	}

	public static void Refresh()
	{
		for (int i = 0; i < RadiationGridManager.emitters.Count; i++)
		{
			if (RadiationGridManager.emitters[i].enabled)
			{
				RadiationGridManager.emitters[i].Emit();
			}
		}
	}

	public const float STANDARD_MASS_FALLOFF = 1000000f;

	public const int RADIATION_LINGER_RATE = 4;

	public static List<RadiationGridEmitter> emitters = new List<RadiationGridEmitter>();

	public static List<global::Tuple<int, int>> previewLightCells = new List<global::Tuple<int, int>>();

	public static int[] previewLux;
}
