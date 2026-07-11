using System;
using System.Configuration.Internal;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Xml;

namespace System.Configuration
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class ConfigXmlDocument : XmlDocument, IConfigXmlNode, IConfigErrorInfo
	{
		public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceUri)
		{
			return new ConfigXmlDocument.ConfigXmlAttribute(this, prefix, localName, namespaceUri);
		}

		public override XmlCDataSection CreateCDataSection(string data)
		{
			return new ConfigXmlDocument.ConfigXmlCDataSection(this, data);
		}

		public override XmlComment CreateComment(string data)
		{
			return new ConfigXmlDocument.ConfigXmlComment(this, data);
		}

		public override XmlElement CreateElement(string prefix, string localName, string namespaceUri)
		{
			return new ConfigXmlDocument.ConfigXmlElement(this, prefix, localName, namespaceUri);
		}

		public override XmlSignificantWhitespace CreateSignificantWhitespace(string data)
		{
			return base.CreateSignificantWhitespace(data);
		}

		public override XmlText CreateTextNode(string text)
		{
			return new ConfigXmlDocument.ConfigXmlText(this, text);
		}

		public override XmlWhitespace CreateWhitespace(string data)
		{
			return base.CreateWhitespace(data);
		}

		public override void Load(string filename)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(filename);
			try
			{
				xmlTextReader.MoveToContent();
				this.LoadSingleElement(filename, xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		public void LoadSingleElement(string filename, XmlTextReader sourceReader)
		{
			this.fileName = filename;
			this.lineNumber = sourceReader.LineNumber;
			string text = sourceReader.ReadOuterXml();
			this.reader = new XmlTextReader(new StringReader(text), sourceReader.NameTable);
			this.Load(this.reader);
			this.reader.Close();
		}

		public string Filename
		{
			get
			{
				if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
				}
				return this.fileName;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		string IConfigErrorInfo.Filename
		{
			get
			{
				return this.Filename;
			}
		}

		int IConfigErrorInfo.LineNumber
		{
			get
			{
				return this.LineNumber;
			}
		}

		string IConfigXmlNode.Filename
		{
			get
			{
				return this.Filename;
			}
		}

		int IConfigXmlNode.LineNumber
		{
			get
			{
				return this.LineNumber;
			}
		}

		private XmlTextReader reader;

		private string fileName;

		private int lineNumber;

		private class ConfigXmlAttribute : XmlAttribute, IConfigXmlNode, IConfigErrorInfo
		{
			public ConfigXmlAttribute(ConfigXmlDocument document, string prefix, string localName, string namespaceUri)
				: base(prefix, localName, namespaceUri, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}

		private class ConfigXmlCDataSection : XmlCDataSection, IConfigXmlNode, IConfigErrorInfo
		{
			public ConfigXmlCDataSection(ConfigXmlDocument document, string data)
				: base(data, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}

		private class ConfigXmlComment : XmlComment, IConfigXmlNode
		{
			public ConfigXmlComment(ConfigXmlDocument document, string comment)
				: base(comment, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}

		private class ConfigXmlElement : XmlElement, IConfigXmlNode, IConfigErrorInfo
		{
			public ConfigXmlElement(ConfigXmlDocument document, string prefix, string localName, string namespaceUri)
				: base(prefix, localName, namespaceUri, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}

		private class ConfigXmlText : XmlText, IConfigXmlNode, IConfigErrorInfo
		{
			public ConfigXmlText(ConfigXmlDocument document, string data)
				: base(data, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}
	}
}
