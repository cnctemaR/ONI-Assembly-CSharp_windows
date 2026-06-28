using System;
using Klei.AI;
using UnityEngine;

public class InfraredVisualizer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Components.InfraredVisualizers.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.InfraredVisualizers.Remove(this);
	}

	public void UpdateTemperature()
	{
		float num = 0f;
		AmountInstance amountInstance = Db.Get().Amounts.Temperature.Lookup(this);
		if (amountInstance != null)
		{
			num = amountInstance.value;
		}
		else if (this.structureTemperature != null)
		{
			num = this.structureTemperature.Temperature;
		}
		else if (this.primaryElement != null)
		{
			num = this.primaryElement.Temperature;
		}
		else if (this.temperatureVulnerable != null)
		{
			num = this.temperatureVulnerable.InternalTemperature;
		}
		if (num < 0f)
		{
			return;
		}
		Color color = SimDebugView.Instance.NormalizedTemperature(num);
		if (this.controller != null)
		{
			this.controller.TemperatureColour = color;
		}
	}

	public void Clear()
	{
	}

	[MyCmpGet]
	private WarmBlooded warmBlooded;

	[MyCmpGet]
	private KAnimControllerBase controller;

	[MyCmpGet]
	private StructureTemperature structureTemperature;

	[MyCmpGet]
	private PrimaryElement primaryElement;

	[MyCmpGet]
	private TemperatureVulnerable temperatureVulnerable;
}
