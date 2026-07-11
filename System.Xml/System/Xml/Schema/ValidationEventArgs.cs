using System;

namespace System.Xml.Schema
{
	public class ValidationEventArgs : EventArgs
	{
		private ValidationEventArgs()
		{
		}

		internal ValidationEventArgs(XmlSchemaException ex, string message, XmlSeverityType severity)
		{
			this.exception = ex;
			this.message = message;
			this.severity = severity;
		}

		public XmlSchemaException Exception
		{
			get
			{
				return this.exception;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public XmlSeverityType Severity
		{
			get
			{
				return this.severity;
			}
		}

		private XmlSchemaException exception;

		private string message;

		private XmlSeverityType severity;
	}
}
