using System;
using Klei.AI;
using UnityEngine;

public struct InfraredVisualizerData
{
	public void Update()
	{
		float num = 0f;
		if (this.temperatureAmount != null)
		{
			num = this.temperatureAmount.value;
		}
		else if (this.structureTemperature.IsValid())
		{
			num = GameComps.StructureTemperatures.GetPayload(this.structureTemperature).Temperature;
		}
		else if (this.primaryElement != null)
		{
			num = this.primaryElement.Temperature;
		}
		else if (this.temperatureVulnerable != null)
		{
			num = this.temperatureVulnerable.InternalTemperature;
		}
		else if (this.critterTemperatureMonitorInstance != null)
		{
			num = this.critterTemperatureMonitorInstance.GetTemperatureInternal();
		}
		if (num < 0f)
		{
			return;
		}
		Color32 color = SimDebugView.Instance.NormalizedTemperature(num);
		this.controller.OverlayColour = color;
	}

	public InfraredVisualizerData(GameObject go)
	{
		this.controller = go.GetComponent<KBatchedAnimController>();
		if (this.controller != null)
		{
			this.temperatureAmount = Db.Get().Amounts.Temperature.Lookup(go);
			this.structureTemperature = GameComps.StructureTemperatures.GetHandle(go);
			this.primaryElement = go.GetComponent<PrimaryElement>();
			this.temperatureVulnerable = go.GetComponent<TemperatureVulnerable>();
			this.critterTemperatureMonitorInstance = go.GetSMI<CritterTemperatureMonitor.Instance>();
			return;
		}
		this.temperatureAmount = null;
		this.structureTemperature = HandleVector<int>.InvalidHandle;
		this.primaryElement = null;
		this.temperatureVulnerable = null;
		this.critterTemperatureMonitorInstance = null;
	}

	public KAnimControllerBase controller;

	public AmountInstance temperatureAmount;

	public HandleVector<int>.Handle structureTemperature;

	public PrimaryElement primaryElement;

	public TemperatureVulnerable temperatureVulnerable;

	public CritterTemperatureMonitor.Instance critterTemperatureMonitorInstance;
}
