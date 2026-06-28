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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.sleepable = base.GetComponent<Sleepable>();
		Sleepable sleepable = this.sleepable;
		sleepable.OnWorkStartedCB = (global::System.Action)Delegate.Combine(sleepable.OnWorkStartedCB, new global::System.Action(this.AddEffects));
		Sleepable sleepable2 = this.sleepable;
		sleepable2.OnWorkStoppedCB = (global::System.Action)Delegate.Combine(sleepable2.OnWorkStoppedCB, new global::System.Action(this.RemoveEffects));
	}

	private void AddEffects()
	{
		this.targetWorker = this.sleepable.worker;
		if (this.effects != null)
		{
			foreach (string text in this.effects)
			{
				this.targetWorker.GetComponent<Effects>().Add(text, false);
			}
		}
		string text2 = "";
		Room roomOfBuilding = Game.Instance.roomProber.GetRoomOfBuilding(base.GetComponent<BuildingComplete>());
		if (roomOfBuilding != null)
		{
			text2 = RoomTypes.GetRoomType(roomOfBuilding).id;
		}
		if (text2 == "Barracks")
		{
			this.targetWorker.GetComponent<Effects>().Add("BarracksStamina", false);
		}
		else if (text2 == "PrivateBedroom")
		{
			this.targetWorker.GetComponent<Effects>().Add("BedroomStamina", false);
		}
	}

	private void RemoveEffects()
	{
		if (!(this.targetWorker == null))
		{
			if (this.effects != null)
			{
				foreach (string text in this.effects)
				{
					this.targetWorker.GetComponent<Effects>().Remove(text);
				}
			}
			this.targetWorker.GetComponent<Effects>().Remove("BarracksStamina");
			this.targetWorker.GetComponent<Effects>().Remove("BedroomStamina");
			this.targetWorker = null;
		}
	}

	private void AddModifierDescriptions(List<Descriptor> descs, string effect_id, bool increase_indent = false)
	{
		Effect effect = Db.Get().effects.Get(effect_id);
		foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
		{
			Descriptor descriptor = new Descriptor(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + attributeModifier.AttributeId.ToUpper() + ".NAME") + ": " + attributeModifier.GetFormattedString(base.gameObject), "", Descriptor.DescriptorType.Effect, false);
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
				if (text != null && text != "")
				{
					this.AddModifierDescriptions(descriptors, text, false);
				}
			}
		}
		return descriptors;
	}

	public string[] effects;

	private Sleepable sleepable;

	private Worker targetWorker;
}
