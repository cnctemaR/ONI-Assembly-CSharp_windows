using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml
{
	[Serializable]
	public class XmlException : SystemException
	{
		public XmlException()
		{
			this.res = "Xml_DefaultException";
			this.messages = new string[1];
		}

		public XmlException(string message, Exception innerException)
			: base(message, innerException)
		{
			this.res = "Xml_UserException";
			this.messages = new string[] { message };
		}

		protected XmlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.lineNumber = info.GetInt32("lineNumber");
			this.linePosition = info.GetInt32("linePosition");
			this.res = info.GetString("res");
			this.messages = (string[])info.GetValue("args", typeof(string[]));
			this.sourceUri = info.GetString("sourceUri");
		}

		public XmlException(string message)
			: base(message)
		{
			this.res = "Xml_UserException";
			this.messages = new string[] { message };
		}

		internal XmlException(IXmlLineInfo li, string sourceUri, string message)
			: this(li, null, sourceUri, message)
		{
		}

		internal XmlException(IXmlLineInfo li, Exception innerException, string sourceUri, string message)
			: this(message, innerException)
		{
			if (li != null)
			{
				this.lineNumber = li.LineNumber;
				this.linePosition = li.LinePosition;
			}
			this.sourceUri = sourceUri;
		}

		public XmlException(string message, Exception innerException, int lineNumber, int linePosition)
			: this(message, innerException)
		{
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		public string SourceUri
		{
			get
			{
				return this.sourceUri;
			}
		}

		public override string Message
		{
			get
			{
				if (this.lineNumber == 0)
				{
					return base.Message;
				}
				return string.Format(CultureInfo.InvariantCulture, "{0} {3} Line {1}, position {2}.", new object[] { base.Message, this.lineNumber, this.linePosition, this.sourceUri });
			}
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("lineNumber", this.lineNumber);
			info.AddValue("linePosition", this.linePosition);
			info.AddValue("res", this.res);
			info.AddValue("args", this.messages);
			info.AddValue("sourceUri", this.sourceUri);
		}

		private const string Xml_DefaultException = "Xml_DefaultException";

		private const string Xml_UserException = "Xml_UserException";

		private int lineNumber;

		private int linePosition;

		private string sourceUri;

		private string res;

		private string[] messages;
	}
}
