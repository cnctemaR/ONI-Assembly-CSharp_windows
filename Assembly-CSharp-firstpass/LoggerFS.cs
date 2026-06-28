using System;
using System.Diagnostics;
using UnityEngine;

public class LoggerFS : Logger<LoggerFS.Entry>
{
	public LoggerFS(string name)
		: base(name)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string evt)
	{
		LoggerFS.Entry entry = default(LoggerFS.Entry);
		entry.idx = global::Logger.NextIdx++;
		entry.frame = Time.frameCount;
		entry.evt = evt;
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
