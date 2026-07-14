using System;
using UnityEngine;

public class InfraredVisualizerComponents : KGameObjectComponentManager<InfraredVisualizerData>
{
	public HandleVector<int>.Handle Add(GameObject go)
	{
		return base.Add(go, new InfraredVisualizerData(go));
	}

	public void UpdateTemperature()
	{
	}

	public void ClearOverlayColour()
	{
	}

	public static void ClearOverlayColour(KBatchedAnimController controller)
	{
	}
}
