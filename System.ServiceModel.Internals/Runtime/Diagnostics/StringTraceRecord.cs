using System;
using System.Xml;

namespace System.Runtime.Diagnostics
{
	internal class StringTraceRecord : TraceRecord
	{
		internal StringTraceRecord(string elementName, string content)
		{
			this.elementName = elementName;
			this.content = content;
		}

		internal override string EventId
		{
			get
			{
				return base.BuildEventId("String");
			}
		}

		internal override void WriteTo(XmlWriter writer)
		{
			writer.WriteElementString(this.elementName, this.content);
		}

		private string elementName;

		private string content;
	}
}
