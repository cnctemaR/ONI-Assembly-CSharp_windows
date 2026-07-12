using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SkyVisibilityVisualizer")]
public class SkyVisibilityVisualizer : KMonoBehaviour
{
	private static bool HasSkyVisibility(int cell)
	{
		return Grid.ExposedToSunlight[cell] >= 1;
	}

	public Vector2I OriginOffset = new Vector2I(0, 0);

	public bool TwoWideOrgin;

	public int RangeMin;

	public int RangeMax;

	public int ScanVerticalStep;

	public bool SkipOnModuleInteriors;

	public bool AllOrNothingVisibility;

	public Func<int, bool> SkyVisibilityCb = new Func<int, bool>(SkyVisibilityVisualizer.HasSkyVisibility);
}
