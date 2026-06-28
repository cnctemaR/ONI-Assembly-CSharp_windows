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
		bool flag2 = this.usingTank;
		this.usingTank = this.breather.IsUsingOxygenTank;
		this.isBreathable = this.breather.IsBreathableElement;
		if (this.isBreathable != flag || this.usingTank != flag2)
		{
			if (this.isBreathable)
			{
				base.Trigger(99949694, null);
			}
			else
			{
				base.Trigger(-1189351068, null);
			}
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

	private bool usingTank;

	private OxygenBreather breather;
}
