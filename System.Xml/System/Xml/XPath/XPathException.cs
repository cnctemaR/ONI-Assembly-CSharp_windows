using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.XPath
{
	[Serializable]
	public class XPathException : SystemException
	{
		public XPathException()
			: base(string.Empty)
		{
		}

		protected XPathException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public XPathException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public XPathException(string message)
			: base(message, null)
		{
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
		}
	}
}
