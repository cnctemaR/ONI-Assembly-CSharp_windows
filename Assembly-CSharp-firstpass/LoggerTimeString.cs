using System;
using System.Diagnostics;

public class LoggerTimeString : Logger<LoggerTimeString.Entry>
{
	public LoggerTimeString(string name)
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
			return string.Concat(new object[]
			{
				this.time.ToString(),
				"_",
				this.idx,
				": ",
				this.evt
			});
		}

		public uint idx;

		public float time;

		public string evt;
	}
}
