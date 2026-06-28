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
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		for (int i = 0; i < this.data.Count; i++)
		{
			KAnimControllerBase controller = this.data[i].controller;
			if (controller != null)
			{
				Vector3 position = controller.transform.position;
				if (visibleArea.Min <= position && position <= visibleArea.Max)
				{
					this.data[i].Update();
				}
			}
		}
	}
}
