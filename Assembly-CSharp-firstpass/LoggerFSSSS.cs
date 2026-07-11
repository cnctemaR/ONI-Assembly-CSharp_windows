using System;
using System.Diagnostics;

public struct LoggerFSSSS
{
	public LoggerFSSSS(string name, int max_entries = 35)
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
	public void Log(string evt, string param0 = "", string param1 = "", string param2 = "")
	{
	}
}
