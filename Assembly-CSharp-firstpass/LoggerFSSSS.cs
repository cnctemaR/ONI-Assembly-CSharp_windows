using System;
using System.Diagnostics;

public class LoggerFSSSS : Logger<LoggerFSSSS.Entry>
{
	public LoggerFSSSS(string name)
		: base(name)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string evt, string param0, string param1, string param2)
	{
	}

	public struct Entry
	{
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.frame, "_", this.idx, ": ", this.evt, " ", this.param0, " ", this.param1, " ",
				this.param2
			});
		}

		public uint idx;

		public int frame;

		public string evt;

		public string param0;

		public string param1;

		public string param2;
	}
}
