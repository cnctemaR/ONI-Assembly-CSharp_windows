using System;
using System.Diagnostics;

public struct LoggerFIO
{
	public LoggerFIO(string name, int max_entries = 35)
	{
	}

	public string GetName()
	{
		return string.Empty;
	}

	public void SetName(string name)
	{
	}

	[Conditional("ENABLE_LOGGER")]
	public void Log(int evt, object obj)
	{
	}
}
