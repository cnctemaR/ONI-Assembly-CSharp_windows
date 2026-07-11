using System;
using System.Collections;
using System.Configuration.Internal;
using System.Runtime.Serialization;
using System.Xml;

namespace System.Configuration
{
	[Serializable]
	public class ConfigurationErrorsException : ConfigurationException
	{
		public ConfigurationErrorsException()
		{
		}

		public ConfigurationErrorsException(string message)
			: base(message)
		{
		}

		protected ConfigurationErrorsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.filename = info.GetString("ConfigurationErrors_Filename");
			this.line = info.GetInt32("ConfigurationErrors_Line");
		}

		public ConfigurationErrorsException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public ConfigurationErrorsException(string message, XmlNode node)
			: this(message, null, ConfigurationErrorsException.GetFilename(node), ConfigurationErrorsException.GetLineNumber(node))
		{
		}

		public ConfigurationErrorsException(string message, Exception inner, XmlNode node)
			: this(message, inner, ConfigurationErrorsException.GetFilename(node), ConfigurationErrorsException.GetLineNumber(node))
		{
		}

		public ConfigurationErrorsException(string message, XmlReader reader)
			: this(message, null, ConfigurationErrorsException.GetFilename(reader), ConfigurationErrorsException.GetLineNumber(reader))
		{
		}

		public ConfigurationErrorsException(string message, Exception inner, XmlReader reader)
			: this(message, inner, ConfigurationErrorsException.GetFilename(reader), ConfigurationErrorsException.GetLineNumber(reader))
		{
		}

		public ConfigurationErrorsException(string message, string filename, int line)
			: this(message, null, filename, line)
		{
		}

		public ConfigurationErrorsException(string message, Exception inner, string filename, int line)
			: base(message, inner)
		{
			this.filename = filename;
			this.line = line;
		}

		public override string BareMessage
		{
			get
			{
				return base.BareMessage;
			}
		}

		public ICollection Errors
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string Filename
		{
			get
			{
				return this.filename;
			}
		}

		public override int Line
		{
			get
			{
				return this.line;
			}
		}

		public override string Message
		{
			get
			{
				string text;
				if (!string.IsNullOrEmpty(this.filename))
				{
					if (this.line != 0)
					{
						text = string.Concat(new object[] { this.BareMessage, " (", this.filename, " line ", this.line, ")" });
					}
					else
					{
						text = this.BareMessage + " (" + this.filename + ")";
					}
				}
				else if (this.line != 0)
				{
					text = string.Concat(new object[] { this.BareMessage, " (line ", this.line, ")" });
				}
				else
				{
					text = this.BareMessage;
				}
				return text;
			}
		}

		public static string GetFilename(XmlReader reader)
		{
			if (reader is IConfigErrorInfo)
			{
				return ((IConfigErrorInfo)reader).Filename;
			}
			if (reader == null)
			{
				return null;
			}
			return reader.BaseURI;
		}

		public static int GetLineNumber(XmlReader reader)
		{
			if (reader is IConfigErrorInfo)
			{
				return ((IConfigErrorInfo)reader).LineNumber;
			}
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			if (xmlLineInfo == null)
			{
				return 0;
			}
			return xmlLineInfo.LineNumber;
		}

		public static string GetFilename(XmlNode node)
		{
			if (!(node is IConfigErrorInfo))
			{
				return null;
			}
			return ((IConfigErrorInfo)node).Filename;
		}

		public static int GetLineNumber(XmlNode node)
		{
			if (!(node is IConfigErrorInfo))
			{
				return 0;
			}
			return ((IConfigErrorInfo)node).LineNumber;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ConfigurationErrors_Filename", this.filename);
			info.AddValue("ConfigurationErrors_Line", this.line);
		}

		private readonly string filename;

		private readonly int line;
	}
}
