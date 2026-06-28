using System;

public class EventBase : Resource
{
	public EventBase(string id)
		: base(id, id)
	{
		this.hash = Hash.SDBMLower(id);
	}

	public virtual string GetDescription(EventInstanceBase ev)
	{
		return "";
	}

	public int hash;
}
