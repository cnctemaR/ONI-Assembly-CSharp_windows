using System;
using System.Diagnostics;

public class CellDigEvent : CellEvent
{
	public CellDigEvent(bool enable_logging = true)
		: base("Dig", "Dig", true, enable_logging)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(int cell, int callback_id)
	{
		if (!this.enableLogging)
		{
			return;
		}
		CellEventInstance cellEventInstance = new CellEventInstance(cell, 0, 0, this);
		CellEventLogger.Instance.Add(cellEventInstance);
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		return base.GetMessagePrefix() + "Dig=true";
	}
}
