using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class ConduitThresholdSensor : ConduitSensor
{
	public abstract float CurrentValue { get; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<ConduitThresholdSensor>(-905833192, ConduitThresholdSensor.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		ConduitThresholdSensor component = gameObject.GetComponent<ConduitThresholdSensor>();
		if (component != null)
		{
			this.Threshold = component.Threshold;
			this.ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void ConduitUpdate(float dt)
	{
		float containedMass = this.GetContainedMass();
		if (containedMass <= 0f && !this.dirty)
		{
			return;
		}
		float currentValue = this.CurrentValue;
		this.dirty = false;
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
			this.dirty = true;
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
			this.dirty = true;
		}
	}

	[SerializeField]
	[Serialize]
	protected float threshold;

	[SerializeField]
	[Serialize]
	protected bool activateAboveThreshold = true;

	[Serialize]
	private bool dirty = true;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<ConduitThresholdSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ConduitThresholdSensor>(delegate(ConduitThresholdSensor component, object data)
	{
		component.OnCopySettings(data);
	});
}
