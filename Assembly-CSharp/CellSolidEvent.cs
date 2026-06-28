using System;
using System.Diagnostics;

public class CellSolidEvent : CellEvent
{
	public CellSolidEvent(string id, string reason, bool is_send, bool enable_logging = true)
		: base(id, reason, is_send, enable_logging)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(int cell, bool solid)
	{
		if (this.enableLogging)
		{
			CellEventInstance cellEventInstance = new CellEventInstance(cell, (!solid) ? 0 : 1, 0, this);
			CellEventLogger.Instance.Add(cellEventInstance);
		}
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		CellEventInstance cellEventInstance = ev as CellEventInstance;
		string text;
		if (cellEventInstance.data == 1)
		{
			text = base.GetMessagePrefix() + "Solid=true (" + this.reason + ")";
		}
		else
		{
			text = base.GetMessagePrefix() + "Solid=false (" + this.reason + ")";
		}
		return text;
	}
}
