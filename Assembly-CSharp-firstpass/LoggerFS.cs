using System;
using System.Diagnostics;

public class LoggerFS : Logger<LoggerFS.Entry>
{
	public LoggerFS(string name)
		: base(name, 35)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string evt)
	{
	}

	public struct Entry
	{
		public override string ToString()
		{
			return string.Concat(new object[] { this.frame, "_", this.idx, ": ", this.evt });
		}

		public uint idx;

		public int frame;

		public string evt;
	}
}
