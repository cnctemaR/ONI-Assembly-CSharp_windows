using System;
using System.Diagnostics;

public class LoggerFSSS : Logger<LoggerFSSS.Entry>
{
	public LoggerFSSS(string name)
		: base(name, 35)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string evt, string param0, string param1)
	{
	}

	public struct Entry
	{
		public override string ToString()
		{
			return string.Concat(new object[] { this.frame, "_", this.idx, ": ", this.evt, " ", this.param0, " ", this.param1 });
		}

		public uint idx;

		public int frame;

		public string evt;

		public string param0;

		public string param1;
	}
}
