using System;
using System.Diagnostics;
using UnityEngine;

public class LoggerFSSF : Logger<LoggerFSSF.Entry>
{
	public LoggerFSSF(string name)
		: base(name)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string evt, string param, float value)
	{
		LoggerFSSF.Entry entry = default(LoggerFSSF.Entry);
		entry.idx = global::Logger.NextIdx++;
		entry.frame = Time.frameCount;
		entry.evt = evt;
		entry.param = param;
		entry.value = value;
	}

	public struct Entry
	{
		public override string ToString()
		{
			return string.Concat(new object[] { this.frame, "_", this.idx, ": ", this.evt, " ", this.param, " ", this.value });
		}

		public uint idx;

		public int frame;

		public string evt;

		public string param;

		public float value;
	}
}
