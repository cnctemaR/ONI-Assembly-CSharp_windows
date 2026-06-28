using System;
using System.Diagnostics;

public class CellSolidFilterEvent : CellEvent
{
	public CellSolidFilterEvent(string id, bool enable_logging = true)
		: base(id, "filtered", false, enable_logging)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(int cell, bool solid)
	{
		if (!this.enableLogging)
		{
			return;
		}
		CellEventInstance cellEventInstance = new CellEventInstance(cell, (!solid) ? 0 : 1, 0, this);
		CellEventLogger.Instance.Add(cellEventInstance);
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		CellEventInstance cellEventInstance = ev as CellEventInstance;
		return base.GetMessagePrefix() + "Filtered Solid Event solid=" + cellEventInstance.data.ToString();
	}
}
