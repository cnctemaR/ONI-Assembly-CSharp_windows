using System;

namespace System.Xml.Schema
{
	public class ValidationEventArgs : EventArgs
	{
		internal ValidationEventArgs(XmlSchemaException ex)
		{
			this.ex = ex;
			this.severity = XmlSeverityType.Error;
		}

		internal ValidationEventArgs(XmlSchemaException ex, XmlSeverityType severity)
		{
			this.ex = ex;
			this.severity = severity;
		}

		public XmlSeverityType Severity
		{
			get
			{
				return this.severity;
			}
		}

		public XmlSchemaException Exception
		{
			get
			{
				return this.ex;
			}
		}

		public string Message
		{
			get
			{
				return this.ex.Message;
			}
		}

		private XmlSchemaException ex;

		private XmlSeverityType severity;
	}
}
