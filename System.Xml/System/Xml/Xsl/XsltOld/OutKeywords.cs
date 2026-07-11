using System;
using System.Diagnostics;

namespace System.Xml.Xsl.XsltOld
{
	internal class OutKeywords
	{
		internal OutKeywords(XmlNameTable nameTable)
		{
			this._AtomEmpty = nameTable.Add(string.Empty);
			this._AtomLang = nameTable.Add("lang");
			this._AtomSpace = nameTable.Add("space");
			this._AtomXmlns = nameTable.Add("xmlns");
			this._AtomXml = nameTable.Add("xml");
			this._AtomXmlNamespace = nameTable.Add("http://www.w3.org/XML/1998/namespace");
			this._AtomXmlnsNamespace = nameTable.Add("http://www.w3.org/2000/xmlns/");
		}

		internal string Empty
		{
			get
			{
				return this._AtomEmpty;
			}
		}

		internal string Lang
		{
			get
			{
				return this._AtomLang;
			}
		}

		internal string Space
		{
			get
			{
				return this._AtomSpace;
			}
		}

		internal string Xmlns
		{
			get
			{
				return this._AtomXmlns;
			}
		}

		internal string Xml
		{
			get
			{
				return this._AtomXml;
			}
		}

		internal string XmlNamespace
		{
			get
			{
				return this._AtomXmlNamespace;
			}
		}

		internal string XmlnsNamespace
		{
			get
			{
				return this._AtomXmlnsNamespace;
			}
		}

		[Conditional("DEBUG")]
		private void CheckKeyword(string keyword)
		{
		}

		private string _AtomEmpty;

		private string _AtomLang;

		private string _AtomSpace;

		private string _AtomXmlns;

		private string _AtomXml;

		private string _AtomXmlNamespace;

		private string _AtomXmlnsNamespace;
	}
}
