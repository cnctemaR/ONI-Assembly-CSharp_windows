using System;
using System.Text;

namespace System.Xml
{
	public sealed class XmlWriterSettings
	{
		public XmlWriterSettings()
		{
			this.Reset();
		}

		public XmlWriterSettings Clone()
		{
			return (XmlWriterSettings)base.MemberwiseClone();
		}

		public void Reset()
		{
			this.checkCharacters = true;
			this.closeOutput = false;
			this.conformance = ConformanceLevel.Document;
			this.encoding = Encoding.UTF8;
			this.indent = false;
			this.indentChars = "  ";
			this.newLineChars = Environment.NewLine;
			this.newLineOnAttributes = false;
			this.newLineHandling = NewLineHandling.None;
			this.omitXmlDeclaration = false;
			this.outputMethod = XmlOutputMethod.AutoDetect;
		}

		public bool CheckCharacters
		{
			get
			{
				return this.checkCharacters;
			}
			set
			{
				this.checkCharacters = value;
			}
		}

		public bool CloseOutput
		{
			get
			{
				return this.closeOutput;
			}
			set
			{
				this.closeOutput = value;
			}
		}

		public ConformanceLevel ConformanceLevel
		{
			get
			{
				return this.conformance;
			}
			set
			{
				this.conformance = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		public bool Indent
		{
			get
			{
				return this.indent;
			}
			set
			{
				this.indent = value;
			}
		}

		public string IndentChars
		{
			get
			{
				return this.indentChars;
			}
			set
			{
				this.indentChars = value;
			}
		}

		public string NewLineChars
		{
			get
			{
				return this.newLineChars;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.newLineChars = value;
			}
		}

		public bool NewLineOnAttributes
		{
			get
			{
				return this.newLineOnAttributes;
			}
			set
			{
				this.newLineOnAttributes = value;
			}
		}

		public NewLineHandling NewLineHandling
		{
			get
			{
				return this.newLineHandling;
			}
			set
			{
				this.newLineHandling = value;
			}
		}

		public bool OmitXmlDeclaration
		{
			get
			{
				return this.omitXmlDeclaration;
			}
			set
			{
				this.omitXmlDeclaration = value;
			}
		}

		public XmlOutputMethod OutputMethod
		{
			get
			{
				return this.outputMethod;
			}
		}

		internal NamespaceHandling NamespaceHandling { get; set; }

		private bool checkCharacters;

		private bool closeOutput;

		private ConformanceLevel conformance;

		private Encoding encoding;

		private bool indent;

		private string indentChars;

		private string newLineChars;

		private bool newLineOnAttributes;

		private NewLineHandling newLineHandling;

		private bool omitXmlDeclaration;

		private XmlOutputMethod outputMethod;
	}
}
