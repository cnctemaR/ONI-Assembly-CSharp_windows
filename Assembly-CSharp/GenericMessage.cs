using System;
using KSerialization;

public class GenericMessage : Message
{
	public GenericMessage(string _title, string _body, string _tooltip)
	{
		this.title = _title;
		this.body = _body;
		this.tooltip = _tooltip;
	}

	public GenericMessage()
	{
	}

	public override string GetSound()
	{
		return null;
	}

	public override string GetMessageBody()
	{
		return this.body;
	}

	public override string GetTooltip()
	{
		return this.tooltip;
	}

	public override string GetTitle()
	{
		return this.title;
	}

	[Serialize]
	private string title;

	[Serialize]
	private string tooltip;

	[Serialize]
	private string body;
}
