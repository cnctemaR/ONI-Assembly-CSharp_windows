using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class TemperatureControlledSwitch : Switch, ISaveLoadableJson
{
	public float StructureTemperature
	{
		get
		{
			return this.structureTemperature.Temperature;
		}
	}

	private void SimUpdate(float dt)
	{
		if (this.simUpdateCounter < 8)
		{
			this.simUpdateCounter++;
			return;
		}
		this.simUpdateCounter = 0;
		float temperature = this.structureTemperature.Temperature;
		if (this.activateOnWarmerThan)
		{
			if ((temperature > this.thresholdTemperature && !base.IsSwitchedOn) || (temperature < this.thresholdTemperature && base.IsSwitchedOn))
			{
				this.Toggle();
			}
		}
		else if ((temperature > this.thresholdTemperature && base.IsSwitchedOn) || (temperature < this.thresholdTemperature && !base.IsSwitchedOn))
		{
			this.Toggle();
		}
	}

	public override List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private StructureTemperature structureTemperature;

	private int simUpdateCounter;

	public float thresholdTemperature = 280f;

	public bool activateOnWarmerThan;
}
