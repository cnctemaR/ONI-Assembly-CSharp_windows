using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.Schema
{
	[Serializable]
	public class XmlSchemaInferenceException : XmlSchemaException
	{
		public XmlSchemaInferenceException()
		{
		}

		public XmlSchemaInferenceException(string message)
			: base(message)
		{
		}

		protected XmlSchemaInferenceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public XmlSchemaInferenceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public XmlSchemaInferenceException(string message, Exception innerException, int line, int column)
			: base(message, innerException, line, column)
		{
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
