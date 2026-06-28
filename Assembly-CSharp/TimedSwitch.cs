using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class TimedSwitch : CircuitSwitch, ISaveLoadable
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

	public new void SetState(bool newState)
	{
		if (base.IsSwitchedOn == newState)
		{
			return;
		}
		this.TimedToggle();
	}

	[Serialize]
	public float switchTime;

	[Serialize]
	public float onTime = 30f;

	[Serialize]
	public float offTime = 30f;
}
