using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class EventInstanceBase : ISaveLoadable
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
		string text2;
		if (this.ev != null)
		{
			text2 = text + this.ev.GetDescription(this);
		}
		else
		{
			text2 = text + "Unknown event";
		}
		return text2;
	}

	[Serialize]
	public int frame;

	[Serialize]
	public int eventHash;

	public EventBase ev;
}
