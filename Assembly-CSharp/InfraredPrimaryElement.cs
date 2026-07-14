using System;
using UnityEngine;

public class InfraredPrimaryElement : TemperatureOverlayInfraredVisualizerBase
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.primaryElement = base.GetComponent<PrimaryElement>();
		this.controller = base.GetComponent<KBatchedAnimController>();
	}

	protected override void TemperatureOverlayInfraredUpdate(Infrared.TemperatureOverlayInfraredData data)
	{
		if (this.primaryElement != null && this.controller != null)
		{
			float temperature = this.primaryElement.Temperature;
			Color32 color = SimDebugView.Instance.NormalizedTemperature(temperature);
			this.controller.OverlayColour = color;
		}
	}

	protected override void TemperatureOverlayInfraredClear()
	{
		if (this.controller != null)
		{
			this.controller.OverlayColour = Color.black;
		}
	}

	private PrimaryElement primaryElement;

	private KBatchedAnimController controller;
}
