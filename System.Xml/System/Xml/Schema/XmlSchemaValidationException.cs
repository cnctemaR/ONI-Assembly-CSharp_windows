using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.Schema
{
	[Serializable]
	public class XmlSchemaValidationException : XmlSchemaException
	{
		public XmlSchemaValidationException()
		{
		}

		public XmlSchemaValidationException(string message)
			: base(message)
		{
		}

		protected XmlSchemaValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public XmlSchemaValidationException(string message, Exception innerException, int lineNumber, int linePosition)
			: base(message, lineNumber, linePosition, null, null, innerException)
		{
		}

		internal XmlSchemaValidationException(string message, int lineNumber, int linePosition, XmlSchemaObject sourceObject, string sourceUri, Exception innerException)
			: base(message, lineNumber, linePosition, sourceObject, sourceUri, innerException)
		{
		}

		internal XmlSchemaValidationException(string message, object sender, string sourceUri, XmlSchemaObject sourceObject, Exception innerException)
			: base(message, sender, sourceUri, sourceObject, innerException)
		{
		}

		internal XmlSchemaValidationException(string message, XmlSchemaObject sourceObject, Exception innerException)
			: base(message, sourceObject, innerException)
		{
		}

		public XmlSchemaValidationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		protected internal void SetSourceObject(object o)
		{
			this.source_object = o;
		}

		public object SourceObject
		{
			get
			{
				return this.source_object;
			}
		}

		private object source_object;
	}
}
