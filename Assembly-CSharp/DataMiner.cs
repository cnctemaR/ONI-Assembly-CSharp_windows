using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ResearchCenter")]
public class DataMiner : ComplexFabricator
{
	public float OperatingTemp
	{
		get
		{
			return this.pe.Temperature;
		}
	}

	public float TemperatureScaleFactor
	{
		get
		{
			return 1f - DataMinerConfig.TEMPERATURE_SCALING_RANGE.LerpFactorClamped(this.OperatingTemp);
		}
	}

	public float EfficiencyRate
	{
		get
		{
			return DataMinerConfig.PRODUCTION_RATE_SCALE.Lerp(this.TemperatureScaleFactor);
		}
	}

	protected override float ComputeWorkProgress(float dt, ComplexRecipe recipe)
	{
		float efficiencyRate = this.EfficiencyRate;
		this.minEfficiency = Mathf.Min(this.minEfficiency, efficiencyRate);
		return base.ComputeWorkProgress(dt, recipe) * efficiencyRate;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(this, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.DataMinerEfficiency, this);
	}

	public override void CompleteWorkingOrder()
	{
		if (this.minEfficiency == DataMinerConfig.PRODUCTION_RATE_SCALE.max)
		{
			SaveGame.Instance.ColonyAchievementTracker.efficientlyGatheredData = true;
		}
		this.minEfficiency = DataMinerConfig.PRODUCTION_RATE_SCALE.max;
		base.CompleteWorkingOrder();
	}

	public override void Sim1000ms(float dt)
	{
		base.Sim1000ms(dt);
		this.meter.SetPositionPercent(this.TemperatureScaleFactor);
	}

	[MyCmpReq]
	private PrimaryElement pe;

	[Serialize]
	private float minEfficiency = DataMinerConfig.PRODUCTION_RATE_SCALE.max;

	private MeterController meter;
}
