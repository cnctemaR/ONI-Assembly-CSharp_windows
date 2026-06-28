using System;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class PowerTransformer : Generator
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.battery = base.GetComponent<Battery>();
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
	}

	public override void ApplyDeltaJoules(float joules_delta, bool can_over_power = false)
	{
		this.battery.ConsumeEnergy(-joules_delta, false);
		base.ApplyDeltaJoules(joules_delta, can_over_power);
	}

	public override float JoulesAvailable
	{
		get
		{
			return Math.Min(this.battery.JoulesAvailable, base.WattageRating * 0.2f);
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (!(bool)data)
		{
			this.battery.ConsumeEnergy(float.MaxValue, false);
			base.ResetJoules();
		}
	}

	private Battery battery;
}
