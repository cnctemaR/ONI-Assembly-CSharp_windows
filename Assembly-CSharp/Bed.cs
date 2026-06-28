using System;
using System.Collections.Generic;
using Klei.AI;

public class Bed : Ownable, IEffectDescriptor
{
	private Bed()
	{
		this.showProgressBar = false;
		base.slot = Db.Get().OwnableSlots.Bed;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (this.effects != null)
		{
			foreach (string text in this.effects)
			{
				worker.GetComponent<Effects>().Add(text, false);
			}
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		if (this.effects != null)
		{
			foreach (string text in this.effects)
			{
				worker.GetComponent<Effects>().Remove(text);
			}
		}
		base.OnStopWork(worker);
	}

	private void AddModifierDescriptions(List<Descriptor> descs, string effect_id, bool increase_indent = false)
	{
		Effect effect = Db.Get().effects.Get(effect_id);
		foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
		{
			Descriptor descriptor = new Descriptor(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + attributeModifier.AttributeId.ToUpper() + ".NAME") + ": " + attributeModifier.GetFormattedString(base.gameObject), string.Empty, Descriptor.DescriptorType.Effect, false);
			if (increase_indent)
			{
				descriptor.IncreaseIndent();
			}
			descs.Add(descriptor);
		}
	}

	public new List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> descriptors = base.GetDescriptors(def);
		if (this.effects != null)
		{
			foreach (string text in this.effects)
			{
				if (text != null && text != string.Empty)
				{
					this.AddModifierDescriptions(descriptors, text, false);
				}
			}
		}
		return descriptors;
	}

	public string[] effects;
}
