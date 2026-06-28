using System;
using System.Diagnostics;
using UnityEngine;

public class LoggerFSS : Logger<LoggerFSS.Entry>
{
	public LoggerFSS(string name)
		: base(name)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string evt, string param)
	{
		LoggerFSS.Entry entry = default(LoggerFSS.Entry);
		entry.idx = global::Logger.NextIdx++;
		entry.frame = Time.frameCount;
		entry.evt = evt;
		entry.param = param;
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
