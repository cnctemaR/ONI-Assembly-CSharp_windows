using System;
using System.Diagnostics;

public class CellCallbackEvent : CellEvent
{
	public CellCallbackEvent(string id, bool is_send, bool enable_logging = true)
		: base(id, "Callback", is_send, enable_logging)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(int cell, int callback_id)
	{
		if (this.enableLogging)
		{
			CellEventInstance cellEventInstance = new CellEventInstance(cell, callback_id, 0, this);
			CellEventLogger.Instance.Add(cellEventInstance);
		}
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		CellEventInstance cellEventInstance = ev as CellEventInstance;
		return base.GetMessagePrefix() + "Callback=" + cellEventInstance.data.ToString();
	}
}
