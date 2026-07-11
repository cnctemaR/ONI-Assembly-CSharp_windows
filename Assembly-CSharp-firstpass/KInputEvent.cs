using System;

public class KInputEvent
{
	public KInputController Controller { get; private set; }

	public InputEventType Type { get; private set; }

	public bool Consumed { get; set; }

	public KInputEvent(KInputController controller, InputEventType event_type)
	{
		this.Controller = controller;
		this.Type = event_type;
		this.Consumed = false;
	}
}
