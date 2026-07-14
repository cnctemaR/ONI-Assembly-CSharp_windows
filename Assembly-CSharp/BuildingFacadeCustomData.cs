using System;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingFacadeCustomData
{
	public static void ApplyCustomData(Building building, Dictionary<string, string> newData)
	{
		BuildingFacadeCustomData.UpdateLightColor(newData, building);
	}

	private static void UpdateLightColor(Dictionary<string, string> newData, Building building)
	{
		Light2D light2D;
		if (!Assets.GetPrefab(building.PrefabID()).TryGetComponent<Light2D>(out light2D))
		{
			return;
		}
		Color color = light2D.Color;
		Color overlayColour = light2D.overlayColour;
		Light2D light2D2;
		if (building.TryGetComponent<Light2D>(out light2D2))
		{
			if (newData == null)
			{
				light2D2.Color = color;
				light2D2.overlayColour = overlayColour;
				return;
			}
			string text;
			if (newData.TryGetValue("LightColor", out text))
			{
				Color color2 = Util.ColorFromHex(text);
				light2D2.Color = color2;
			}
			else
			{
				light2D2.Color = color;
			}
			string text2;
			if (newData.TryGetValue("LightOverlayColor", out text2))
			{
				Color color3 = Util.ColorFromHex(text2);
				light2D2.overlayColour = color3;
				return;
			}
			light2D2.overlayColour = overlayColour;
		}
	}

	public const string DATA_KEY_LIGHT_COLOR = "LightColor";

	public const string DATA_KEY_LIGHT_OVERLAY_COLOR = "LightOverlayColor";
}
