using System;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.Schema
{
	[Serializable]
	public class XmlSchemaException : SystemException
	{
		protected XmlSchemaException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.res = (string)info.GetValue("res", typeof(string));
			this.args = (string[])info.GetValue("args", typeof(string[]));
			this.sourceUri = (string)info.GetValue("sourceUri", typeof(string));
			this.lineNumber = (int)info.GetValue("lineNumber", typeof(int));
			this.linePosition = (int)info.GetValue("linePosition", typeof(int));
			string text = null;
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "version")
				{
					text = (string)serializationEntry.Value;
				}
			}
			if (text == null)
			{
				this.message = XmlSchemaException.CreateMessage(this.res, this.args);
				return;
			}
			this.message = null;
		}

		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("res", this.res);
			info.AddValue("args", this.args);
			info.AddValue("sourceUri", this.sourceUri);
			info.AddValue("lineNumber", this.lineNumber);
			info.AddValue("linePosition", this.linePosition);
			info.AddValue("version", "2.0");
		}

		public XmlSchemaException()
			: this(null)
		{
		}

		public XmlSchemaException(string message)
			: this(message, null, 0, 0)
		{
		}

		public XmlSchemaException(string message, Exception innerException)
			: this(message, innerException, 0, 0)
		{
		}

		public XmlSchemaException(string message, Exception innerException, int lineNumber, int linePosition)
			: this((message == null) ? "A schema error occurred." : "{0}", new string[] { message }, innerException, null, lineNumber, linePosition, null)
		{
		}

		internal XmlSchemaException(string res, string[] args)
			: this(res, args, null, null, 0, 0, null)
		{
		}

		internal XmlSchemaException(string res, string arg)
			: this(res, new string[] { arg }, null, null, 0, 0, null)
		{
		}

		internal XmlSchemaException(string res, string arg, string sourceUri, int lineNumber, int linePosition)
			: this(res, new string[] { arg }, null, sourceUri, lineNumber, linePosition, null)
		{
		}

		internal XmlSchemaException(string res, string sourceUri, int lineNumber, int linePosition)
			: this(res, null, null, sourceUri, lineNumber, linePosition, null)
		{
		}

		internal XmlSchemaException(string res, string[] args, string sourceUri, int lineNumber, int linePosition)
			: this(res, args, null, sourceUri, lineNumber, linePosition, null)
		{
		}

		internal XmlSchemaException(string res, XmlSchemaObject source)
			: this(res, null, source)
		{
		}

		internal XmlSchemaException(string res, string arg, XmlSchemaObject source)
			: this(res, new string[] { arg }, source)
		{
		}

		internal XmlSchemaException(string res, string[] args, XmlSchemaObject source)
			: this(res, args, null, source.SourceUri, source.LineNumber, source.LinePosition, source)
		{
		}

		internal XmlSchemaException(string res, string[] args, Exception innerException, string sourceUri, int lineNumber, int linePosition, XmlSchemaObject source)
			: base(XmlSchemaException.CreateMessage(res, args), innerException)
		{
			base.HResult = -2146231999;
			this.res = res;
			this.args = args;
			this.sourceUri = sourceUri;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
			this.sourceSchemaObject = source;
		}

		internal static string CreateMessage(string res, string[] args)
		{
			string text;
			try
			{
				text = Res.GetString(res, args);
			}
			catch (MissingManifestResourceException)
			{
				text = "UNKNOWN(" + res + ")";
			}
			return text;
		}

		internal string GetRes
		{
			get
			{
				return this.res;
			}
		}

		internal string[] Args
		{
			get
			{
				return this.args;
			}
		}

		public string SourceUri
		{
			get
			{
				return this.sourceUri;
			}
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
				return this.sourceSchemaObject;
			}
		}

		internal void SetSource(string sourceUri, int lineNumber, int linePosition)
		{
			this.sourceUri = sourceUri;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		internal void SetSchemaObject(XmlSchemaObject source)
		{
			this.sourceSchemaObject = source;
		}

		internal void SetSource(XmlSchemaObject source)
		{
			this.sourceSchemaObject = source;
			this.sourceUri = source.SourceUri;
			this.lineNumber = source.LineNumber;
			this.linePosition = source.LinePosition;
		}

		internal void SetResourceId(string resourceId)
		{
			this.res = resourceId;
		}

		public override string Message
		{
			get
			{
				if (this.message != null)
				{
					return this.message;
				}
				return base.Message;
			}
		}

		private string res;

		private string[] args;

		private string sourceUri;

		private int lineNumber;

		private int linePosition;

		[NonSerialized]
		private XmlSchemaObject sourceSchemaObject;

		private string message;
	}
}
