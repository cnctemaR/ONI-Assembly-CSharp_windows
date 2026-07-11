using System;
using System.Collections.Generic;
using Klei.AI;

public class Bed : Workable, IEffectDescriptor, IBasicBuilding
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.BasicBuildings.Add(this);
		this.sleepable = base.GetComponent<Sleepable>();
		Sleepable sleepable = this.sleepable;
		sleepable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(sleepable.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkableEvent));
	}

	private void OnWorkableEvent(Workable.WorkableEvent workable_event)
	{
		if (workable_event == Workable.WorkableEvent.WorkStarted)
		{
			this.AddEffects();
			return;
		}
		if (workable_event == Workable.WorkableEvent.WorkStopped)
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
		roomType.TriggerRoomEffects(base.GetComponent<KPrefabID>(), this.targetWorker.GetComponent<Effects>());
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

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.effects != null)
		{
			foreach (string text in this.effects)
			{
				if (text != null && text != "")
				{
					Effect.AddModifierDescriptions(base.gameObject, list, text, false);
				}
			}
		}
		return list;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.BasicBuildings.Remove(this);
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
}
