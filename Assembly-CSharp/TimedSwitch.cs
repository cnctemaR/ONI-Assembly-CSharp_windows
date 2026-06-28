using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class TimedSwitch : Switch, ISaveLoadableJson
{
	private void SimUpdate(float dt)
	{
		this.switchTime -= dt;
		if (this.switchTime <= 0f)
		{
			this.TimedToggle();
		}
	}

	public void TimedToggle()
	{
		this.Toggle();
		this.switchTime = ((!base.IsSwitchedOn) ? this.offTime : this.onTime);
	}

	public void SetState(bool newState)
	{
		if (base.IsSwitchedOn == newState)
		{
			return;
		}
		this.TimedToggle();
	}

	public override List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	[Serialize]
	public float switchTime;

	[Serialize]
	public float onTime = 30f;

	[Serialize]
	public float offTime = 30f;
}
