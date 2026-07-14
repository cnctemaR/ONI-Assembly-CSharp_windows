using System;
using Klei.AI;
using UnityEngine;

public struct InfraredVisualizerData
{
	public void Update()
	{
	}

	public InfraredVisualizerData(GameObject go)
	{
		this.controller = null;
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
