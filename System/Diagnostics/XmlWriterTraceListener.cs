using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace System.Diagnostics
{
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true)]
	public class XmlWriterTraceListener : TextWriterTraceListener
	{
		public XmlWriterTraceListener(Stream stream)
			: base(stream)
		{
		}

		public XmlWriterTraceListener(Stream stream, string name)
			: base(stream, name)
		{
		}

		public XmlWriterTraceListener(TextWriter writer)
			: base(writer)
		{
		}

		public XmlWriterTraceListener(TextWriter writer, string name)
			: base(writer, name)
		{
		}

		public XmlWriterTraceListener(string filename)
			: base(filename)
		{
		}

		public XmlWriterTraceListener(string filename, string name)
			: base(filename, name)
		{
		}

		public override void Write(string message)
		{
			this.WriteLine(message);
		}

		public override void WriteLine(string message)
		{
			this.TraceEvent(null, global::SR.GetString("Trace"), TraceEventType.Information, 0, message);
		}

		public override void Fail(string message, string detailMessage)
		{
			StringBuilder stringBuilder = new StringBuilder(message);
			if (detailMessage != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(detailMessage);
			}
			this.TraceEvent(null, global::SR.GetString("Trace"), TraceEventType.Error, 0, stringBuilder.ToString());
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, format, args))
			{
				return;
			}
			this.WriteHeader(source, eventType, id, eventCache);
			string text;
			if (args != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, format, args);
			}
			else
			{
				text = format;
			}
			this.WriteEscaped(text);
			this.WriteFooter(eventCache);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, message))
			{
				return;
			}
			this.WriteHeader(source, eventType, id, eventCache);
			this.WriteEscaped(message);
			this.WriteFooter(eventCache);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data))
			{
				return;
			}
			this.WriteHeader(source, eventType, id, eventCache);
			this.InternalWrite("<TraceData>");
			if (data != null)
			{
				this.InternalWrite("<DataItem>");
				this.WriteData(data);
				this.InternalWrite("</DataItem>");
			}
			this.InternalWrite("</TraceData>");
			this.WriteFooter(eventCache);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
			{
				return;
			}
			this.WriteHeader(source, eventType, id, eventCache);
			this.InternalWrite("<TraceData>");
			if (data != null)
			{
				for (int i = 0; i < data.Length; i++)
				{
					this.InternalWrite("<DataItem>");
					if (data[i] != null)
					{
						this.WriteData(data[i]);
					}
					this.InternalWrite("</DataItem>");
				}
			}
			this.InternalWrite("</TraceData>");
			this.WriteFooter(eventCache);
		}

		private void WriteData(object data)
		{
			XPathNavigator xpathNavigator = data as XPathNavigator;
			if (xpathNavigator == null)
			{
				this.WriteEscaped(data.ToString());
				return;
			}
			if (this.strBldr == null)
			{
				this.strBldr = new StringBuilder();
				this.xmlBlobWriter = new XmlTextWriter(new StringWriter(this.strBldr, CultureInfo.CurrentCulture));
			}
			else
			{
				this.strBldr.Length = 0;
			}
			try
			{
				xpathNavigator.MoveToRoot();
				this.xmlBlobWriter.WriteNode(xpathNavigator, false);
				this.InternalWrite(this.strBldr.ToString());
			}
			catch (Exception)
			{
				this.InternalWrite(data.ToString());
			}
		}

		public override void Close()
		{
			base.Close();
			if (this.xmlBlobWriter != null)
			{
				this.xmlBlobWriter.Close();
			}
			this.xmlBlobWriter = null;
			this.strBldr = null;
		}

		public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
		{
			if (this.shouldRespectFilterOnTraceTransfer && base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, TraceEventType.Transfer, id, message))
			{
				return;
			}
			this.WriteHeader(source, TraceEventType.Transfer, id, eventCache, relatedActivityId);
			this.WriteEscaped(message);
			this.WriteFooter(eventCache);
		}

		private void WriteHeader(string source, TraceEventType eventType, int id, TraceEventCache eventCache, Guid relatedActivityId)
		{
			this.WriteStartHeader(source, eventType, id, eventCache);
			this.InternalWrite("\" RelatedActivityID=\"");
			this.InternalWrite(relatedActivityId.ToString("B"));
			this.WriteEndHeader(eventCache);
		}

		private void WriteHeader(string source, TraceEventType eventType, int id, TraceEventCache eventCache)
		{
			this.WriteStartHeader(source, eventType, id, eventCache);
			this.WriteEndHeader(eventCache);
		}

		private void WriteStartHeader(string source, TraceEventType eventType, int id, TraceEventCache eventCache)
		{
			this.InternalWrite("<E2ETraceEvent xmlns=\"http://schemas.microsoft.com/2004/06/E2ETraceEvent\"><System xmlns=\"http://schemas.microsoft.com/2004/06/windows/eventlog/system\">");
			this.InternalWrite("<EventID>");
			uint num = (uint)id;
			this.InternalWrite(num.ToString(CultureInfo.InvariantCulture));
			this.InternalWrite("</EventID>");
			this.InternalWrite("<Type>3</Type>");
			this.InternalWrite("<SubType Name=\"");
			this.InternalWrite(eventType.ToString());
			this.InternalWrite("\">0</SubType>");
			this.InternalWrite("<Level>");
			int num2 = (int)eventType;
			if (num2 > 255)
			{
				num2 = 255;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			this.InternalWrite(num2.ToString(CultureInfo.InvariantCulture));
			this.InternalWrite("</Level>");
			this.InternalWrite("<TimeCreated SystemTime=\"");
			if (eventCache != null)
			{
				this.InternalWrite(eventCache.DateTime.ToString("o", CultureInfo.InvariantCulture));
			}
			else
			{
				this.InternalWrite(DateTime.Now.ToString("o", CultureInfo.InvariantCulture));
			}
			this.InternalWrite("\" />");
			this.InternalWrite("<Source Name=\"");
			this.WriteEscaped(source);
			this.InternalWrite("\" />");
			this.InternalWrite("<Correlation ActivityID=\"");
			if (eventCache != null)
			{
				this.InternalWrite(eventCache.ActivityId.ToString("B"));
				return;
			}
			this.InternalWrite(Guid.Empty.ToString("B"));
		}

		private void WriteEndHeader(TraceEventCache eventCache)
		{
			this.InternalWrite("\" />");
			this.InternalWrite("<Execution ProcessName=\"");
			this.InternalWrite(TraceEventCache.GetProcessName());
			this.InternalWrite("\" ProcessID=\"");
			this.InternalWrite(((uint)TraceEventCache.GetProcessId()).ToString(CultureInfo.InvariantCulture));
			this.InternalWrite("\" ThreadID=\"");
			if (eventCache != null)
			{
				this.WriteEscaped(eventCache.ThreadId.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				this.WriteEscaped(TraceEventCache.GetThreadId().ToString(CultureInfo.InvariantCulture));
			}
			this.InternalWrite("\" />");
			this.InternalWrite("<Channel/>");
			this.InternalWrite("<Computer>");
			this.InternalWrite(this.machineName);
			this.InternalWrite("</Computer>");
			this.InternalWrite("</System>");
			this.InternalWrite("<ApplicationData>");
		}

		private void WriteFooter(TraceEventCache eventCache)
		{
			bool flag = base.IsEnabled(TraceOptions.LogicalOperationStack);
			bool flag2 = base.IsEnabled(TraceOptions.Callstack);
			if (eventCache != null && (flag || flag2))
			{
				this.InternalWrite("<System.Diagnostics xmlns=\"http://schemas.microsoft.com/2004/08/System.Diagnostics\">");
				if (flag)
				{
					this.InternalWrite("<LogicalOperationStack>");
					Stack logicalOperationStack = eventCache.LogicalOperationStack;
					if (logicalOperationStack != null)
					{
						foreach (object obj in logicalOperationStack)
						{
							this.InternalWrite("<LogicalOperation>");
							this.WriteEscaped(obj.ToString());
							this.InternalWrite("</LogicalOperation>");
						}
					}
					this.InternalWrite("</LogicalOperationStack>");
				}
				this.InternalWrite("<Timestamp>");
				this.InternalWrite(eventCache.Timestamp.ToString(CultureInfo.InvariantCulture));
				this.InternalWrite("</Timestamp>");
				if (flag2)
				{
					this.InternalWrite("<Callstack>");
					this.WriteEscaped(eventCache.Callstack);
					this.InternalWrite("</Callstack>");
				}
				this.InternalWrite("</System.Diagnostics>");
			}
			this.InternalWrite("</ApplicationData></E2ETraceEvent>");
		}

		private void WriteEscaped(string str)
		{
			if (str == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (c <= '"')
				{
					if (c != '\n')
					{
						if (c != '\r')
						{
							if (c == '"')
							{
								this.InternalWrite(str.Substring(num, i - num));
								this.InternalWrite("&quot;");
								num = i + 1;
							}
						}
						else
						{
							this.InternalWrite(str.Substring(num, i - num));
							this.InternalWrite("&#xD;");
							num = i + 1;
						}
					}
					else
					{
						this.InternalWrite(str.Substring(num, i - num));
						this.InternalWrite("&#xA;");
						num = i + 1;
					}
				}
				else if (c <= '\'')
				{
					if (c != '&')
					{
						if (c == '\'')
						{
							this.InternalWrite(str.Substring(num, i - num));
							this.InternalWrite("&apos;");
							num = i + 1;
						}
					}
					else
					{
						this.InternalWrite(str.Substring(num, i - num));
						this.InternalWrite("&amp;");
						num = i + 1;
					}
				}
				else if (c != '<')
				{
					if (c == '>')
					{
						this.InternalWrite(str.Substring(num, i - num));
						this.InternalWrite("&gt;");
						num = i + 1;
					}
				}
				else
				{
					this.InternalWrite(str.Substring(num, i - num));
					this.InternalWrite("&lt;");
					num = i + 1;
				}
			}
			this.InternalWrite(str.Substring(num, str.Length - num));
		}

		private void InternalWrite(string message)
		{
			if (!base.EnsureWriter())
			{
				return;
			}
			this.writer.Write(message);
		}

		private const string fixedHeader = "<E2ETraceEvent xmlns=\"http://schemas.microsoft.com/2004/06/E2ETraceEvent\"><System xmlns=\"http://schemas.microsoft.com/2004/06/windows/eventlog/system\">";

		private readonly string machineName = Environment.MachineName;

		private StringBuilder strBldr;

		private XmlTextWriter xmlBlobWriter;

		internal bool shouldRespectFilterOnTraceTransfer;
	}
}
