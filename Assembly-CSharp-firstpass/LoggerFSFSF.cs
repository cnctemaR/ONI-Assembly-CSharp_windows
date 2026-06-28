using System;
using System.Diagnostics;

public class LoggerFSFSF : Logger<LoggerFSFSF.Entry>
{
	public LoggerFSFSF(string name)
		: base(name, 35)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string name1, float val1, string name2, float val2)
	{
	}

	public struct Entry
	{
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.frame,
				"_",
				this.idx,
				": ",
				this.name1,
				": ",
				this.val1.ToString(),
				", ",
				this.name2,
				": ",
				this.val2.ToString()
			});
		}

		public uint idx;

		public int frame;

		public string name1;

		public float val1;

		public string name2;

		public float val2;
	}
}
