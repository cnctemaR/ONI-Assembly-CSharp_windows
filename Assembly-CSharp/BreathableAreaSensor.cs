using System;

public class BreathableAreaSensor : Sensor
{
	public BreathableAreaSensor(Sensors sensors)
		: base(sensors)
	{
	}

	public override void Update()
	{
		if (this.breather == null)
		{
			this.breather = base.GetComponent<OxygenBreather>();
		}
		bool flag = this.isBreathable;
		this.isBreathable = this.breather.IsBreathableElement || this.breather.HasTag(GameTags.InTransitTube);
		if (this.isBreathable != flag)
		{
			if (this.isBreathable)
			{
				base.Trigger(99949694, null);
				return;
			}
			base.Trigger(-1189351068, null);
		}
	}

	public bool IsBreathable()
	{
		return this.isBreathable;
	}

	public bool IsUnderwater()
	{
		return this.breather.IsUnderLiquid;
	}

	private bool isBreathable;

	private OxygenBreather breather;
}
