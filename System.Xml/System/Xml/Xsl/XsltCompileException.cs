using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	[Serializable]
	public class XsltCompileException : XsltException
	{
		public XsltCompileException()
		{
		}

		public XsltCompileException(string message)
			: base(message)
		{
		}

		public XsltCompileException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected XsltCompileException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public XsltCompileException(Exception inner, string sourceUri, int lineNumber, int linePosition)
			: base((lineNumber == 0) ? "{0}." : "{0} at {1}({2},{3}). See InnerException for details.", "XSLT compile error", inner, lineNumber, linePosition, sourceUri)
		{
		}

		internal XsltCompileException(string message, Exception innerException, XPathNavigator nav)
			: base(message, innerException, nav)
		{
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
