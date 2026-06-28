using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	[Serializable]
	public class XsltException : SystemException
	{
		public XsltException()
			: this(string.Empty, null)
		{
		}

		public XsltException(string message)
			: this(message, null)
		{
		}

		public XsltException(string message, Exception innerException)
			: this("{0}", message, innerException, 0, 0, null)
		{
		}

		protected XsltException(SerializationInfo info, StreamingContext context)
		{
			this.lineNumber = info.GetInt32("lineNumber");
			this.linePosition = info.GetInt32("linePosition");
			this.sourceUri = info.GetString("sourceUri");
			this.templateFrames = info.GetString("templateFrames");
		}

		internal XsltException(string msgFormat, string message, Exception innerException, int lineNumber, int linePosition, string sourceUri)
			: base(XsltException.CreateMessage(msgFormat, message, lineNumber, linePosition, sourceUri), innerException)
		{
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
			this.sourceUri = sourceUri;
		}

		internal XsltException(string message, Exception innerException, XPathNavigator nav)
			: base(XsltException.CreateMessage(message, nav), innerException)
		{
			IXmlLineInfo xmlLineInfo = nav as IXmlLineInfo;
			this.lineNumber = ((xmlLineInfo == null) ? 0 : xmlLineInfo.LineNumber);
			this.linePosition = ((xmlLineInfo == null) ? 0 : xmlLineInfo.LinePosition);
			this.sourceUri = ((nav == null) ? string.Empty : nav.BaseURI);
		}

		private static string CreateMessage(string message, XPathNavigator nav)
		{
			IXmlLineInfo xmlLineInfo = nav as IXmlLineInfo;
			int num = ((xmlLineInfo == null) ? 0 : xmlLineInfo.LineNumber);
			int num2 = ((xmlLineInfo == null) ? 0 : xmlLineInfo.LinePosition);
			string text = ((nav == null) ? string.Empty : nav.BaseURI);
			if (num != 0)
			{
				return XsltException.CreateMessage("{0} at {1}({2},{3}).", message, num, num2, text);
			}
			return XsltException.CreateMessage("{0}.", message, num, num2, text);
		}

		private static string CreateMessage(string msgFormat, string message, int lineNumber, int linePosition, string sourceUri)
		{
			return string.Format(CultureInfo.InvariantCulture, msgFormat, new object[]
			{
				message,
				sourceUri,
				lineNumber.ToString(CultureInfo.InvariantCulture),
				linePosition.ToString(CultureInfo.InvariantCulture)
			});
		}

		public virtual int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public virtual int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		public override string Message
		{
			get
			{
				return (this.templateFrames == null) ? base.Message : (base.Message + this.templateFrames);
			}
		}

		public virtual string SourceUri
		{
			get
			{
				return this.sourceUri;
			}
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("lineNumber", this.lineNumber);
			info.AddValue("linePosition", this.linePosition);
			info.AddValue("sourceUri", this.sourceUri);
			info.AddValue("templateFrames", this.templateFrames);
		}

		internal void AddTemplateFrame(string frame)
		{
			this.templateFrames += frame;
		}

		private int lineNumber;

		private int linePosition;

		private string sourceUri;

		private string templateFrames;
	}
}
