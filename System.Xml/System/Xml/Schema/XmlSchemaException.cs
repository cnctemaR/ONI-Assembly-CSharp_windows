using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.Schema
{
	[Serializable]
	public class XmlSchemaException : SystemException
	{
		public XmlSchemaException()
			: this("A schema error occured.", null)
		{
		}

		public XmlSchemaException(string message)
			: this(message, null)
		{
		}

		protected XmlSchemaException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.hasLineInfo = info.GetBoolean("hasLineInfo");
			this.lineNumber = info.GetInt32("lineNumber");
			this.linePosition = info.GetInt32("linePosition");
			this.sourceUri = info.GetString("sourceUri");
			this.sourceObj = info.GetValue("sourceObj", typeof(XmlSchemaObject)) as XmlSchemaObject;
		}

		public XmlSchemaException(string message, Exception innerException, int lineNumber, int linePosition)
			: this(message, lineNumber, linePosition, null, null, innerException)
		{
		}

		internal XmlSchemaException(string message, int lineNumber, int linePosition, XmlSchemaObject sourceObject, string sourceUri, Exception innerException)
			: base(XmlSchemaException.GetMessage(message, sourceUri, lineNumber, linePosition, sourceObject), innerException)
		{
			this.hasLineInfo = true;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
			this.sourceObj = sourceObject;
			this.sourceUri = sourceUri;
		}

		internal XmlSchemaException(string message, object sender, string sourceUri, XmlSchemaObject sourceObject, Exception innerException)
			: base(XmlSchemaException.GetMessage(message, sourceUri, sender, sourceObject), innerException)
		{
			IXmlLineInfo xmlLineInfo = sender as IXmlLineInfo;
			if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
			{
				this.hasLineInfo = true;
				this.lineNumber = xmlLineInfo.LineNumber;
				this.linePosition = xmlLineInfo.LinePosition;
			}
			this.sourceObj = sourceObject;
		}

		internal XmlSchemaException(string message, XmlSchemaObject sourceObject, Exception innerException)
			: base(XmlSchemaException.GetMessage(message, null, 0, 0, sourceObject), innerException)
		{
			this.hasLineInfo = true;
			this.lineNumber = sourceObject.LineNumber;
			this.linePosition = sourceObject.LinePosition;
			this.sourceObj = sourceObject;
			this.sourceUri = sourceObject.SourceUri;
		}

		public XmlSchemaException(string message, Exception innerException)
			: base(XmlSchemaException.GetMessage(message, null, 0, 0, null), innerException)
		{
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

		public XmlSchemaObject SourceSchemaObject
		{
			get
			{
				return this.sourceObj;
			}
		}

		public string SourceUri
		{
			get
			{
				return this.sourceUri;
			}
		}

		private static string GetMessage(string message, string sourceUri, object sender, XmlSchemaObject sourceObj)
		{
			IXmlLineInfo xmlLineInfo = sender as IXmlLineInfo;
			if (xmlLineInfo == null)
			{
				return XmlSchemaException.GetMessage(message, sourceUri, 0, 0, sourceObj);
			}
			return XmlSchemaException.GetMessage(message, sourceUri, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition, sourceObj);
		}

		private static string GetMessage(string message, string sourceUri, int lineNumber, int linePosition, XmlSchemaObject sourceObj)
		{
			string text = "XmlSchema error: " + message;
			if (lineNumber > 0)
			{
				text += string.Format(CultureInfo.InvariantCulture, " XML {0} Line {1}, Position {2}.", new object[]
				{
					(sourceUri == null || !(sourceUri != string.Empty)) ? string.Empty : ("URI: " + sourceUri + " ."),
					lineNumber,
					linePosition
				});
			}
			if (sourceObj != null)
			{
				text += string.Format(CultureInfo.InvariantCulture, " Related schema item SourceUri: {0}, Line {1}, Position {2}.", new object[] { sourceObj.SourceUri, sourceObj.LineNumber, sourceObj.LinePosition });
			}
			return text;
		}

		public override string Message
		{
			get
			{
				return base.Message;
			}
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hasLineInfo", this.hasLineInfo);
			info.AddValue("lineNumber", this.lineNumber);
			info.AddValue("linePosition", this.linePosition);
			info.AddValue("sourceUri", this.sourceUri);
			info.AddValue("sourceObj", this.sourceObj);
		}

		private bool hasLineInfo;

		private int lineNumber;

		private int linePosition;

		private XmlSchemaObject sourceObj;

		private string sourceUri;
	}
}
