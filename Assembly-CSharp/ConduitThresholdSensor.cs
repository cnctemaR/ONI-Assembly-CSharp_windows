using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class ConduitThresholdSensor : ConduitSensor
{
	public abstract float CurrentValue { get; }

	protected override void ConduitUpdate(float dt)
	{
		float containedMass = this.GetContainedMass();
		if (containedMass <= 0f)
		{
			return;
		}
		float currentValue = this.CurrentValue;
		if (this.activateAboveThreshold)
		{
			if ((currentValue > this.threshold && !base.IsSwitchedOn) || (currentValue <= this.threshold && base.IsSwitchedOn))
			{
				this.Toggle();
			}
		}
		else if ((currentValue > this.threshold && base.IsSwitchedOn) || (currentValue <= this.threshold && !base.IsSwitchedOn))
		{
			this.Toggle();
		}
	}

	private float GetContainedMass()
	{
		int num = Grid.PosToCell(this);
		ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
		return flowManager.GetContents(num).mass;
	}

	public float Threshold
	{
		get
		{
			return this.threshold;
		}
		set
		{
			this.threshold = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return this.activateAboveThreshold;
		}
		set
		{
			this.activateAboveThreshold = value;
		}
	}

	[SerializeField]
	[Serialize]
	protected float threshold;

	[SerializeField]
	[Serialize]
	protected bool activateAboveThreshold = true;
}
