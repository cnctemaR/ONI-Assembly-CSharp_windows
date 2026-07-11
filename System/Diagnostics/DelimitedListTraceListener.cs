using System;
using System.IO;
using System.Text;

namespace System.Diagnostics
{
	public class DelimitedListTraceListener : TextWriterTraceListener
	{
		public DelimitedListTraceListener(string fileName)
			: base(fileName)
		{
		}

		public DelimitedListTraceListener(string fileName, string name)
			: base(fileName, name)
		{
		}

		public DelimitedListTraceListener(Stream stream)
			: base(stream)
		{
		}

		public DelimitedListTraceListener(Stream stream, string name)
			: base(stream, name)
		{
		}

		public DelimitedListTraceListener(TextWriter writer)
			: base(writer)
		{
		}

		public DelimitedListTraceListener(TextWriter writer, string name)
			: base(writer, name)
		{
		}

		public string Delimiter
		{
			get
			{
				return this.delimiter;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.delimiter = value;
			}
		}

		protected internal override string[] GetSupportedAttributes()
		{
			return DelimitedListTraceListener.attributes;
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			this.TraceCore(eventCache, source, eventType, id, null, new object[] { data });
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			this.TraceCore(eventCache, source, eventType, id, null, data);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			this.TraceCore(eventCache, source, eventType, id, message, new object[0]);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			this.TraceCore(eventCache, source, eventType, id, string.Format(format, args), new object[0]);
		}

		private void TraceCore(TraceEventCache c, string source, TraceEventType eventType, int id, string message, params object[] data)
		{
			this.Write(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{12}", new object[]
			{
				this.delimiter,
				(source == null) ? null : ("\"" + source.Replace("\"", "\"\"") + "\""),
				eventType,
				id,
				(message == null) ? null : ("\"" + message.Replace("\"", "\"\"") + "\""),
				this.FormatData(data),
				(!this.IsTarget(c, TraceOptions.ProcessId)) ? null : c.ProcessId.ToString(),
				(!this.IsTarget(c, TraceOptions.LogicalOperationStack)) ? null : TraceListener.FormatArray(c.LogicalOperationStack, ", "),
				(!this.IsTarget(c, TraceOptions.ThreadId)) ? null : c.ThreadId,
				(!this.IsTarget(c, TraceOptions.DateTime)) ? null : c.DateTime.ToString("o"),
				(!this.IsTarget(c, TraceOptions.Timestamp)) ? null : c.Timestamp.ToString(),
				(!this.IsTarget(c, TraceOptions.Callstack)) ? null : c.Callstack,
				Environment.NewLine
			}));
		}

		private bool IsTarget(TraceEventCache c, TraceOptions opt)
		{
			return c != null && (base.TraceOutputOptions & opt) != TraceOptions.None;
		}

		private string FormatData(object[] data)
		{
			if (data == null || data.Length == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] != null)
				{
					stringBuilder.Append('"').Append(data[i].ToString().Replace("\"", "\"\"")).Append('"');
				}
				if (i + 1 < data.Length)
				{
					stringBuilder.Append(',');
				}
			}
			return stringBuilder.ToString();
		}

		private static readonly string[] attributes = new string[] { "delimiter" };

		private string delimiter = ";";
	}
}
