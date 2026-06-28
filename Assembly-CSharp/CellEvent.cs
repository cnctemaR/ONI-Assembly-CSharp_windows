using System;

public class CellEvent : EventBase
{
	public CellEvent(string id, string reason, bool is_send, bool enable_logging = true)
		: base(id)
	{
		this.reason = reason;
		this.isSend = is_send;
		this.enableLogging = enable_logging;
	}

	public string GetMessagePrefix()
	{
		if (this.isSend)
		{
			return ">>>: ";
		}
		return "<<<: ";
	}

	public string reason;

	public bool isSend;

	public bool enableLogging;
}
