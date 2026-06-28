using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class EventInstanceBase : ISaveLoadableJson
{
	public EventInstanceBase(EventBase ev)
	{
		this.frame = GameClock.Instance.GetFrame();
		this.eventHash = ev.hash;
		this.ev = ev;
	}

	public override string ToString()
	{
		string text = "[" + this.frame.ToString() + "] ";
		if (this.ev != null)
		{
			return text + this.ev.GetDescription(this);
		}
		return text + "Unknown event";
	}

	[Serialize]
	public int frame;

	[Serialize]
	public int eventHash;

	public EventBase ev;
}
