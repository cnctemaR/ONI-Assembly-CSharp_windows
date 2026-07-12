using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class HighEnergyParticlePort : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public int GetHighEnergyParticleInputPortPosition()
	{
		return this.m_building.GetHighEnergyParticleInputCell();
	}

	public int GetHighEnergyParticleOutputPortPosition()
	{
		return this.m_building.GetHighEnergyParticleOutputCell();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.HighEnergyParticlePorts.Add(this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.HighEnergyParticlePorts.Remove(this);
	}

	public bool InputActive()
	{
		Operational component = base.GetComponent<Operational>();
		return this.particleInputEnabled && component != null && component.IsFunctional && (!this.requireOperational || component.IsOperational);
	}

	public bool AllowCapture(HighEnergyParticle particle)
	{
		return this.onParticleCaptureAllowed == null || this.onParticleCaptureAllowed(particle);
	}

	public void Capture(HighEnergyParticle particle)
	{
		this.currentParticle = particle;
		if (this.onParticleCapture != null)
		{
			this.onParticleCapture(particle);
		}
	}

	public void Uncapture(HighEnergyParticle particle)
	{
		if (this.onParticleUncapture != null)
		{
			this.onParticleUncapture(particle);
		}
		this.currentParticle = null;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.particleInputEnabled)
		{
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.PARTICLE_PORT_INPUT, UI.BUILDINGEFFECTS.TOOLTIPS.PARTICLE_PORT_INPUT, Descriptor.DescriptorType.Requirement, false));
		}
		if (this.particleOutputEnabled)
		{
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.PARTICLE_PORT_OUTPUT, UI.BUILDINGEFFECTS.TOOLTIPS.PARTICLE_PORT_OUTPUT, Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	[MyCmpGet]
	private Building m_building;

	public HighEnergyParticlePort.OnParticleCapture onParticleCapture;

	public HighEnergyParticlePort.OnParticleCaptureAllowed onParticleCaptureAllowed;

	public HighEnergyParticlePort.OnParticleCapture onParticleUncapture;

	public HighEnergyParticle currentParticle;

	public bool requireOperational = true;

	public bool particleInputEnabled;

	public bool particleOutputEnabled;

	public CellOffset particleInputOffset;

	public CellOffset particleOutputOffset;

	public delegate void OnParticleCapture(HighEnergyParticle particle);

	public delegate bool OnParticleCaptureAllowed(HighEnergyParticle particle);
}
