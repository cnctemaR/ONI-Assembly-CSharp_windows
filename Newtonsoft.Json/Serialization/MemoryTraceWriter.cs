using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Newtonsoft.Json.Serialization
{
	public class MemoryTraceWriter : ITraceWriter
	{
		public TraceLevel LevelFilter { get; set; }

		public MemoryTraceWriter()
		{
			this.LevelFilter = TraceLevel.Verbose;
			this._traceMessages = new Queue<string>();
		}

		public void Trace(TraceLevel level, string message, Exception ex)
		{
			if (this._traceMessages.Count >= 1000)
			{
				this._traceMessages.Dequeue();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff", CultureInfo.InvariantCulture));
			stringBuilder.Append(" ");
			stringBuilder.Append(level.ToString("g"));
			stringBuilder.Append(" ");
			stringBuilder.Append(message);
			this._traceMessages.Enqueue(stringBuilder.ToString());
		}

		public IEnumerable<string> GetTraceMessages()
		{
			return this._traceMessages;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in this._traceMessages)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}

		private readonly Queue<string> _traceMessages;
	}
}
