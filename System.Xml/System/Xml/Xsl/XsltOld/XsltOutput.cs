using System;
using System.Collections;
using System.Text;

namespace System.Xml.Xsl.XsltOld
{
	internal class XsltOutput : CompiledAction
	{
		internal XsltOutput.OutputMethod Method
		{
			get
			{
				return this.method;
			}
		}

		internal bool OmitXmlDeclaration
		{
			get
			{
				return this.omitXmlDecl;
			}
		}

		internal bool HasStandalone
		{
			get
			{
				return this.standaloneSId != int.MaxValue;
			}
		}

		internal bool Standalone
		{
			get
			{
				return this.standalone;
			}
		}

		internal string DoctypePublic
		{
			get
			{
				return this.doctypePublic;
			}
		}

		internal string DoctypeSystem
		{
			get
			{
				return this.doctypeSystem;
			}
		}

		internal Hashtable CDataElements
		{
			get
			{
				return this.cdataElements;
			}
		}

		internal bool Indent
		{
			get
			{
				return this.indent;
			}
		}

		internal Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
		}

		internal string MediaType
		{
			get
			{
				return this.mediaType;
			}
		}

		internal XsltOutput CreateDerivedOutput(XsltOutput.OutputMethod method)
		{
			XsltOutput xsltOutput = (XsltOutput)base.MemberwiseClone();
			xsltOutput.method = method;
			if (method == XsltOutput.OutputMethod.Html && this.indentSId == 2147483647)
			{
				xsltOutput.indent = true;
			}
			return xsltOutput;
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckEmpty(compiler);
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Method))
			{
				if (compiler.Stylesheetid <= this.methodSId)
				{
					this.method = XsltOutput.ParseOutputMethod(value, compiler);
					this.methodSId = compiler.Stylesheetid;
					if (this.indentSId == 2147483647)
					{
						this.indent = this.method == XsltOutput.OutputMethod.Html;
					}
				}
			}
			else if (Ref.Equal(localName, compiler.Atoms.Version))
			{
				if (compiler.Stylesheetid <= this.versionSId)
				{
					this.version = value;
					this.versionSId = compiler.Stylesheetid;
				}
			}
			else
			{
				if (Ref.Equal(localName, compiler.Atoms.Encoding))
				{
					if (compiler.Stylesheetid > this.encodingSId)
					{
						return true;
					}
					try
					{
						this.encoding = Encoding.GetEncoding(value);
						this.encodingSId = compiler.Stylesheetid;
						return true;
					}
					catch (NotSupportedException)
					{
						return true;
					}
					catch (ArgumentException)
					{
						return true;
					}
				}
				if (Ref.Equal(localName, compiler.Atoms.OmitXmlDeclaration))
				{
					if (compiler.Stylesheetid <= this.omitXmlDeclSId)
					{
						this.omitXmlDecl = compiler.GetYesNo(value);
						this.omitXmlDeclSId = compiler.Stylesheetid;
					}
				}
				else if (Ref.Equal(localName, compiler.Atoms.Standalone))
				{
					if (compiler.Stylesheetid <= this.standaloneSId)
					{
						this.standalone = compiler.GetYesNo(value);
						this.standaloneSId = compiler.Stylesheetid;
					}
				}
				else if (Ref.Equal(localName, compiler.Atoms.DocTypePublic))
				{
					if (compiler.Stylesheetid <= this.doctypePublicSId)
					{
						this.doctypePublic = value;
						this.doctypePublicSId = compiler.Stylesheetid;
					}
				}
				else if (Ref.Equal(localName, compiler.Atoms.DocTypeSystem))
				{
					if (compiler.Stylesheetid <= this.doctypeSystemSId)
					{
						this.doctypeSystem = value;
						this.doctypeSystemSId = compiler.Stylesheetid;
					}
				}
				else if (Ref.Equal(localName, compiler.Atoms.Indent))
				{
					if (compiler.Stylesheetid <= this.indentSId)
					{
						this.indent = compiler.GetYesNo(value);
						this.indentSId = compiler.Stylesheetid;
					}
				}
				else if (Ref.Equal(localName, compiler.Atoms.MediaType))
				{
					if (compiler.Stylesheetid <= this.mediaTypeSId)
					{
						this.mediaType = value;
						this.mediaTypeSId = compiler.Stylesheetid;
					}
				}
				else
				{
					if (!Ref.Equal(localName, compiler.Atoms.CDataSectionElements))
					{
						return false;
					}
					string[] array = XmlConvert.SplitString(value);
					if (this.cdataElements == null)
					{
						this.cdataElements = new Hashtable(array.Length);
					}
					for (int i = 0; i < array.Length; i++)
					{
						XmlQualifiedName xmlQualifiedName = compiler.CreateXmlQName(array[i]);
						this.cdataElements[xmlQualifiedName] = xmlQualifiedName;
					}
				}
			}
			return true;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
		}

		private static XsltOutput.OutputMethod ParseOutputMethod(string value, Compiler compiler)
		{
			XmlQualifiedName xmlQualifiedName = compiler.CreateXPathQName(value);
			if (xmlQualifiedName.Namespace.Length != 0)
			{
				return XsltOutput.OutputMethod.Other;
			}
			string name = xmlQualifiedName.Name;
			if (name == "xml")
			{
				return XsltOutput.OutputMethod.Xml;
			}
			if (name == "html")
			{
				return XsltOutput.OutputMethod.Html;
			}
			if (name == "text")
			{
				return XsltOutput.OutputMethod.Text;
			}
			if (compiler.ForwardCompatibility)
			{
				return XsltOutput.OutputMethod.Unknown;
			}
			throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "method", value });
		}

		private XsltOutput.OutputMethod method = XsltOutput.OutputMethod.Unknown;

		private int methodSId = int.MaxValue;

		private Encoding encoding = Encoding.UTF8;

		private int encodingSId = int.MaxValue;

		private string version;

		private int versionSId = int.MaxValue;

		private bool omitXmlDecl;

		private int omitXmlDeclSId = int.MaxValue;

		private bool standalone;

		private int standaloneSId = int.MaxValue;

		private string doctypePublic;

		private int doctypePublicSId = int.MaxValue;

		private string doctypeSystem;

		private int doctypeSystemSId = int.MaxValue;

		private bool indent;

		private int indentSId = int.MaxValue;

		private string mediaType = "text/html";

		private int mediaTypeSId = int.MaxValue;

		private Hashtable cdataElements;

		internal enum OutputMethod
		{
			Xml,
			Html,
			Text,
			Other,
			Unknown
		}
	}
}
