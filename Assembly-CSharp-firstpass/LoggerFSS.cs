using System;
using System.Diagnostics;

public class LoggerFSS : Logger<LoggerFSS.Entry>
{
	public LoggerFSS(string name)
		: base(name, 35)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string evt, string param = "")
	{
	}

	public struct Entry
	{
		public override string ToString()
		{
			return string.Concat(new object[] { this.frame, "_", this.idx, ": ", this.evt, " ", this.param });
		}

		public uint idx;

		public int frame;

		public string evt;

		public string param;
	}
}
