using System;

public class CellAddRemoveSubstanceEvent : CellEvent
{
	public CellAddRemoveSubstanceEvent(string id, string reason, bool enable_logging = false)
		: base(id, reason, true, enable_logging)
	{
	}

	public void Log(int cell, SimHashes element, float amount, int callback_id)
	{
		if (!this.enableLogging)
		{
			return;
		}
		CellEventInstance cellEventInstance = new CellEventInstance(cell, (int)element, (int)(amount * 1000f), this);
		CellEventLogger.Instance.Add(cellEventInstance);
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		CellEventInstance cellEventInstance = ev as CellEventInstance;
		SimHashes data = (SimHashes)cellEventInstance.data;
		return string.Concat(new object[]
		{
			base.GetMessagePrefix(),
			"Element=",
			data.ToString(),
			", Mass=",
			(float)cellEventInstance.data2 / 1000f,
			" (",
			this.reason,
			")"
		});
	}
}
