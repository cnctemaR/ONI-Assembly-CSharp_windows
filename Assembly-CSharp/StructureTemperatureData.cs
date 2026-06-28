using System;
using System.Collections.Generic;
using UnityEngine;

public struct StructureTemperatureData
{
	public StructureTemperatureData(GameObject go)
	{
		this.dirty = false;
		this.isActiveBuilding = false;
		this.enabled = true;
		this.overrideExtents = false;
		this.overriddenExtents = default(Extents);
		this.simHandle = -1;
		this.primaryElement = go.GetComponent<PrimaryElement>();
		this.selectable = go.GetComponent<KSelectable>();
		this.building = go.GetComponent<Building>();
		this.operational = go.GetComponent<Operational>();
		this.pendingEnergyModifications = 0f;
		this.maxTemperature = 10000f;
		this.energySourcesKW = null;
	}

	public float TotalEnergyProducedKW
	{
		get
		{
			if (this.energySourcesKW == null || this.energySourcesKW.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < this.energySourcesKW.Count; i++)
			{
				num += this.energySourcesKW[i].value;
			}
			return num;
		}
	}

	public void ModifyEnergy(float delta_kilojoules)
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			SimMessages.ModifyBuildingEnergy(this.simHandle, delta_kilojoules);
		}
		else
		{
			this.pendingEnergyModifications += delta_kilojoules;
			this.dirty = true;
		}
	}

	public void OverrideExtents(Extents newExtents)
	{
		this.overrideExtents = true;
		this.overriddenExtents = newExtents;
	}

	public Extents GetExtents()
	{
		return (!this.overrideExtents) ? this.building.GetExtents() : this.overriddenExtents;
	}

	public void ApplyPendingEnergyModifications()
	{
		if (this.pendingEnergyModifications != 0f)
		{
			SimMessages.ModifyBuildingEnergy(this.simHandle, this.pendingEnergyModifications);
			this.pendingEnergyModifications = 0f;
		}
	}

	public float Temperature
	{
		get
		{
			return this.primaryElement.Temperature;
		}
	}

	public float ExhaustKilowatts
	{
		get
		{
			return this.building.Def.ExhaustKilowattsWhenActive;
		}
	}

	public float OperatingKilowatts
	{
		get
		{
			return (!(this.operational != null) || !this.operational.IsActive) ? 0f : this.building.Def.SelfHeatKilowattsWhenActive;
		}
	}

	public bool dirty;

	public bool isActiveBuilding;

	public bool enabled;

	public int simHandle;

	public PrimaryElement primaryElement;

	public KSelectable selectable;

	public Building building;

	public Operational operational;

	public float pendingEnergyModifications;

	public List<StructureTemperatureData.EnergySource> energySourcesKW;

	public float maxTemperature;

	public bool overrideExtents;

	public Extents overriddenExtents;

	public class EnergySource
	{
		public EnergySource(float kj, string source)
		{
			this.source = source;
			int num = Mathf.RoundToInt(186f);
			this.kw_accumulator = new RunningAverage(float.MinValue, float.MaxValue, num, true);
		}

		public float value
		{
			get
			{
				return this.kw_accumulator.AverageValue;
			}
		}

		public void Accumulate(float value)
		{
			this.kw_accumulator.AddSample(value);
		}

		public string source;

		public RunningAverage kw_accumulator;
	}
}
