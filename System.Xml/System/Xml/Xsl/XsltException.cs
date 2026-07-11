using System;
using System.Globalization;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Utils;

namespace System.Xml.Xsl
{
	[Serializable]
	public class XsltException : SystemException
	{
		protected XsltException(SerializationInfo info, StreamingContext context)
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
				this.message = XsltException.CreateMessage(this.res, this.args, this.sourceUri, this.lineNumber, this.linePosition);
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

		public XsltException()
			: this(string.Empty, null)
		{
		}

		public XsltException(string message)
			: this(message, null)
		{
		}

		public XsltException(string message, Exception innerException)
			: this("{0}", new string[] { message }, null, 0, 0, innerException)
		{
		}

		internal static XsltException Create(string res, params string[] args)
		{
			return new XsltException(res, args, null, 0, 0, null);
		}

		internal static XsltException Create(string res, string[] args, Exception inner)
		{
			return new XsltException(res, args, null, 0, 0, inner);
		}

		internal XsltException(string res, string[] args, string sourceUri, int lineNumber, int linePosition, Exception inner)
			: base(XsltException.CreateMessage(res, args, sourceUri, lineNumber, linePosition), inner)
		{
			base.HResult = -2146231998;
			this.res = res;
			this.sourceUri = sourceUri;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		public virtual string SourceUri
		{
			get
			{
				return this.sourceUri;
			}
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
				if (this.message != null)
				{
					return this.message;
				}
				return base.Message;
			}
		}

		private static string CreateMessage(string res, string[] args, string sourceUri, int lineNumber, int linePosition)
		{
			string text2;
			try
			{
				string text = XsltException.FormatMessage(res, args);
				if (res != "XSLT compile error at {0}({1},{2}). See InnerException for details." && lineNumber != 0)
				{
					text = text + " " + XsltException.FormatMessage("An error occurred at {0}({1},{2}).", new string[]
					{
						sourceUri,
						lineNumber.ToString(CultureInfo.InvariantCulture),
						linePosition.ToString(CultureInfo.InvariantCulture)
					});
				}
				text2 = text;
			}
			catch (MissingManifestResourceException)
			{
				text2 = "UNKNOWN(" + res + ")";
			}
			return text2;
		}

		private static string FormatMessage(string key, params string[] args)
		{
			string text = Res.GetString(key);
			if (text != null && args != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, text, args);
			}
			return text;
		}

		private string res;

		private string[] args;

		private string sourceUri;

		private int lineNumber;

		private int linePosition;

		private string message;
	}
}
