using System;
using System.Diagnostics;

public struct LoggerFSSF
{
	public LoggerFSSF(string name)
	{
	}

	public string GetName()
	{
		return "";
	}

	public void SetName(string name)
	{
	}

	[Conditional("ENABLE_LOGGER")]
	public void Log(string evt, string param, float value)
	{
	}
}
