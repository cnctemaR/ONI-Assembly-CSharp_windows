using System;
using System.Collections.Generic;
using Klei.AI;

public class Bed : Workable, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.sleepable = base.GetComponent<Sleepable>();
		Sleepable sleepable = this.sleepable;
		sleepable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(sleepable.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkableEvent));
	}

	private void OnWorkableEvent(Workable.WorkableEvent workable_event)
	{
		if (workable_event == Workable.WorkableEvent.WorkStarted)
		{
			this.AddEffects();
		}
		else if (workable_event == Workable.WorkableEvent.WorkStopped)
		{
			this.RemoveEffects();
		}
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
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject == null)
		{
			return;
		}
		RoomType roomType = roomOfGameObject.roomType;
		foreach (KeyValuePair<string, string> keyValuePair in Bed.roomSleepingEffects)
		{
			if (keyValuePair.Key == roomType.Id)
			{
				this.targetWorker.GetComponent<Effects>().Add(keyValuePair.Value, false);
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair2 in Bed.roomEffects)
		{
			if (keyValuePair2.Key == roomType.Id)
			{
				this.targetWorker.GetComponent<Effects>().Add(keyValuePair2.Value, true);
			}
			else
			{
				this.targetWorker.GetComponent<Effects>().Remove(keyValuePair2.Value);
			}
		}
	}

	private void RemoveEffects()
	{
		if (this.targetWorker == null)
		{
			return;
		}
		if (this.effects != null)
		{
			foreach (string text in this.effects)
			{
				this.targetWorker.GetComponent<Effects>().Remove(text);
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair in Bed.roomSleepingEffects)
		{
			this.targetWorker.GetComponent<Effects>().Remove(keyValuePair.Value);
		}
		this.targetWorker = null;
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

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.effects != null)
		{
			foreach (string text in this.effects)
			{
				if (text != null && text != string.Empty)
				{
					this.AddModifierDescriptions(list, text, false);
				}
			}
		}
		return list;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.sleepable != null)
		{
			Sleepable sleepable = this.sleepable;
			sleepable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Remove(sleepable.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkableEvent));
		}
	}

	[MyCmpReq]
	private Sleepable sleepable;

	private Worker targetWorker;

	public string[] effects;

	private static Dictionary<string, string> roomSleepingEffects = new Dictionary<string, string>
	{
		{ "Barracks", "BarracksStamina" },
		{ "Bedroom", "BedroomStamina" }
	};

	private static Dictionary<string, string> roomEffects = new Dictionary<string, string>
	{
		{ "Barracks", "RoomBarracks" },
		{ "Bedroom", "RoomBedroom" }
	};
}
