using System;
using Klei.AI;
using UnityEngine;

public class InfraredTemperatureAmount : TemperatureOverlayInfraredVisualizerBase
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.temperatureAmount = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
		this.controller = base.GetComponent<KBatchedAnimController>();
	}

	protected override void TemperatureOverlayInfraredUpdate(Infrared.TemperatureOverlayInfraredData data)
	{
		if (this.temperatureAmount != null && this.controller != null)
		{
			float value = this.temperatureAmount.value;
			Color32 color = SimDebugView.Instance.NormalizedTemperature(value);
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

	private AmountInstance temperatureAmount;

	private KBatchedAnimController controller;
}
