using System;

namespace System.Diagnostics
{
	public class SourceFilter : TraceFilter
	{
		public SourceFilter(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.source = source;
		}

		public string Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source == null)
				{
					throw new ArgumentNullException("value");
				}
				this.source = value;
			}
		}

		public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
		{
			return source == this.source;
		}

		private string source;
	}
}
