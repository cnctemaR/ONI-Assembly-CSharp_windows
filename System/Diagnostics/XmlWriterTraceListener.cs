using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace System.Diagnostics
{
	public class XmlWriterTraceListener : TextWriterTraceListener
	{
		public XmlWriterTraceListener(string filename)
			: this(filename, XmlWriterTraceListener.default_name)
		{
		}

		public XmlWriterTraceListener(string filename, string name)
			: this(new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)), name)
		{
		}

		public XmlWriterTraceListener(Stream stream)
			: this(stream, XmlWriterTraceListener.default_name)
		{
		}

		public XmlWriterTraceListener(Stream writer, string name)
			: this(new StreamWriter(writer), name)
		{
		}

		public XmlWriterTraceListener(TextWriter writer)
			: this(writer, XmlWriterTraceListener.default_name)
		{
		}

		public XmlWriterTraceListener(TextWriter writer, string name)
			: base(name)
		{
			this.w = XmlWriter.Create(writer, new XmlWriterSettings
			{
				OmitXmlDeclaration = true
			});
		}

		public override void Close()
		{
			this.w.Close();
		}

		public override void Fail(string message, string detailMessage)
		{
			this.TraceEvent(null, null, TraceEventType.Error, 0, message + " " + detailMessage);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			this.TraceCore(eventCache, source, eventType, id, false, Guid.Empty, 2, true, new object[] { data });
		}

		[global::System.MonoLimitation("level is not always correct")]
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			this.TraceCore(eventCache, source, eventType, id, false, Guid.Empty, 2, true, data);
		}

		[global::System.MonoLimitation("level is not always correct")]
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			this.TraceCore(eventCache, source, TraceEventType.Transfer, id, false, Guid.Empty, 2, true, new object[] { message });
		}

		[global::System.MonoLimitation("level is not always correct")]
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			this.TraceCore(eventCache, source, TraceEventType.Transfer, id, false, Guid.Empty, 2, true, new object[] { string.Format(format, args) });
		}

		public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
		{
			this.TraceCore(eventCache, source, TraceEventType.Transfer, id, true, relatedActivityId, 255, true, new object[] { message });
		}

		public override void Write(string message)
		{
			this.WriteLine(message);
		}

		[global::System.MonoLimitation("level is not always correct")]
		public override void WriteLine(string message)
		{
			this.TraceCore(null, "Trace", TraceEventType.Information, 0, false, Guid.Empty, 8, false, new object[] { message });
		}

		private void TraceCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, bool hasRelatedActivity, Guid relatedActivity, int level, bool wrapData, params object[] data)
		{
			Process process = ((eventCache == null) ? Process.GetCurrentProcess() : Process.GetProcessById(eventCache.ProcessId));
			this.w.WriteStartElement("E2ETraceEvent", XmlWriterTraceListener.e2e_ns);
			this.w.WriteStartElement("System", XmlWriterTraceListener.sys_ns);
			this.w.WriteStartElement("EventID", XmlWriterTraceListener.sys_ns);
			this.w.WriteString(XmlConvert.ToString(id));
			this.w.WriteEndElement();
			this.w.WriteStartElement("Type", XmlWriterTraceListener.sys_ns);
			this.w.WriteString("3");
			this.w.WriteEndElement();
			this.w.WriteStartElement("SubType", XmlWriterTraceListener.sys_ns);
			this.w.WriteAttributeString("Name", eventType.ToString());
			this.w.WriteString("0");
			this.w.WriteEndElement();
			this.w.WriteStartElement("Level", XmlWriterTraceListener.sys_ns);
			this.w.WriteString(level.ToString());
			this.w.WriteEndElement();
			this.w.WriteStartElement("TimeCreated", XmlWriterTraceListener.sys_ns);
			this.w.WriteAttributeString("SystemTime", XmlConvert.ToString((eventCache == null) ? DateTime.Now : eventCache.DateTime));
			this.w.WriteEndElement();
			this.w.WriteStartElement("Source", XmlWriterTraceListener.sys_ns);
			this.w.WriteAttributeString("Name", source);
			this.w.WriteEndElement();
			this.w.WriteStartElement("Correlation", XmlWriterTraceListener.sys_ns);
			this.w.WriteAttributeString("ActivityID", "{" + Guid.Empty + "}");
			this.w.WriteEndElement();
			this.w.WriteStartElement("Execution", XmlWriterTraceListener.sys_ns);
			this.w.WriteAttributeString("ProcessName", process.MainModule.ModuleName);
			this.w.WriteAttributeString("ProcessID", process.Id.ToString());
			this.w.WriteAttributeString("ThreadID", (eventCache == null) ? Thread.CurrentThread.ManagedThreadId.ToString() : eventCache.ThreadId);
			this.w.WriteEndElement();
			this.w.WriteStartElement("Channel", XmlWriterTraceListener.sys_ns);
			this.w.WriteEndElement();
			this.w.WriteStartElement("Computer");
			this.w.WriteString(process.MachineName);
			this.w.WriteEndElement();
			this.w.WriteEndElement();
			this.w.WriteStartElement("ApplicationData", XmlWriterTraceListener.e2e_ns);
			foreach (object obj in data)
			{
				if (wrapData)
				{
					this.w.WriteStartElement("TraceData", XmlWriterTraceListener.e2e_ns);
				}
				if (obj != null)
				{
					this.w.WriteString(obj.ToString());
				}
				if (wrapData)
				{
					this.w.WriteEndElement();
				}
			}
			this.w.WriteEndElement();
			this.w.WriteEndElement();
		}

		private static readonly string e2e_ns = "http://schemas.microsoft.com/2004/06/E2ETraceEvent";

		private static readonly string sys_ns = "http://schemas.microsoft.com/2004/06/windows/eventlog/system";

		private static readonly string default_name = "XmlWriter";

		private XmlWriter w;
	}
}
