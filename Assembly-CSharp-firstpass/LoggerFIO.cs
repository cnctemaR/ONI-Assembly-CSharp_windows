using System;
using System.Diagnostics;
using UnityEngine;

public class LoggerFIO : Logger<LoggerFIO.Entry>
{
	public LoggerFIO(string name)
		: base(name, 35)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(int evt, object obj)
	{
		LoggerFIO.Entry entry = default(LoggerFIO.Entry);
		entry.idx = global::Logger.NextIdx++;
		entry.frame = Time.frameCount;
		entry.evt = evt;
		entry.obj = obj;
	}

	public struct Entry
	{
		public override string ToString()
		{
			string text = HashCache.Get().Get(this.evt);
			string text2;
			if (this.obj != null)
			{
				text2 = string.Concat(new object[]
				{
					this.frame,
					"_",
					this.idx,
					": ",
					text,
					"(data=",
					this.obj.ToString(),
					")"
				});
			}
			else
			{
				text2 = string.Concat(new object[] { this.frame, "_", this.idx, ": ", text });
			}
			return text2;
		}

		public uint idx;

		public int frame;

		public int evt;

		public object obj;
	}
}
