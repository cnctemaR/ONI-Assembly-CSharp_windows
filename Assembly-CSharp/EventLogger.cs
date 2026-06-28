using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class EventLogger<EventInstanceType, EventType> : KMonoBehaviour, ISaveLoadable where EventInstanceType : EventInstanceBase where EventType : EventBase
{
	public IEnumerator<EventInstanceType> GetEnumerator()
	{
		return this.EventInstances.GetEnumerator();
	}

	public EventType AddEvent(EventType ev)
	{
		for (int i = 0; i < this.Events.Count; i++)
		{
			if (this.Events[i].hash == ev.hash)
			{
				this.Events[i] = ev;
				return this.Events[i];
			}
		}
		this.Events.Add(ev);
		return ev;
	}

	public EventInstanceType Add(EventInstanceType ev)
	{
		if (this.EventInstances.Count > 10000)
		{
			this.EventInstances.RemoveAt(0);
		}
		this.EventInstances.Add(ev);
		return ev;
	}

	[OnDeserialized]
	protected internal void OnDeserialized()
	{
		if (this.EventInstances.Count > 10000)
		{
			this.EventInstances.RemoveRange(0, this.EventInstances.Count - 10000);
		}
		for (int i = 0; i < this.EventInstances.Count; i++)
		{
			for (int j = 0; j < this.Events.Count; j++)
			{
				if (this.Events[j].hash == this.EventInstances[i].eventHash)
				{
					this.EventInstances[i].ev = this.Events[j];
					break;
				}
			}
		}
	}

	public void Clear()
	{
		this.EventInstances.Clear();
	}

	private const int MAX_NUM_EVENTS = 10000;

	private List<EventType> Events = new List<EventType>();

	[Serialize]
	private List<EventInstanceType> EventInstances = new List<EventInstanceType>();
}
