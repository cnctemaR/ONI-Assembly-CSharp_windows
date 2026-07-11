using System;
using System.Text;

namespace System.Xml
{
	public class XmlParserContext
	{
		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string xmlLang, XmlSpace xmlSpace)
			: this(nt, nsMgr, null, null, null, null, string.Empty, xmlLang, xmlSpace)
		{
		}

		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string xmlLang, XmlSpace xmlSpace, Encoding enc)
			: this(nt, nsMgr, null, null, null, null, string.Empty, xmlLang, xmlSpace, enc)
		{
		}

		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string docTypeName, string pubId, string sysId, string internalSubset, string baseURI, string xmlLang, XmlSpace xmlSpace)
			: this(nt, nsMgr, docTypeName, pubId, sysId, internalSubset, baseURI, xmlLang, xmlSpace, null)
		{
		}

		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string docTypeName, string pubId, string sysId, string internalSubset, string baseURI, string xmlLang, XmlSpace xmlSpace, Encoding enc)
		{
			if (nsMgr != null)
			{
				if (nt == null)
				{
					this._nt = nsMgr.NameTable;
				}
				else
				{
					if (nt != nsMgr.NameTable)
					{
						throw new XmlException("Not the same name table.", string.Empty);
					}
					this._nt = nt;
				}
			}
			else
			{
				this._nt = nt;
			}
			this._nsMgr = nsMgr;
			this._docTypeName = ((docTypeName == null) ? string.Empty : docTypeName);
			this._pubId = ((pubId == null) ? string.Empty : pubId);
			this._sysId = ((sysId == null) ? string.Empty : sysId);
			this._internalSubset = ((internalSubset == null) ? string.Empty : internalSubset);
			this._baseURI = ((baseURI == null) ? string.Empty : baseURI);
			this._xmlLang = ((xmlLang == null) ? string.Empty : xmlLang);
			this._xmlSpace = xmlSpace;
			this._encoding = enc;
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this._nt;
			}
			set
			{
				this._nt = value;
			}
		}

		public XmlNamespaceManager NamespaceManager
		{
			get
			{
				return this._nsMgr;
			}
			set
			{
				this._nsMgr = value;
			}
		}

		public string DocTypeName
		{
			get
			{
				return this._docTypeName;
			}
			set
			{
				this._docTypeName = ((value == null) ? string.Empty : value);
			}
		}

		public string PublicId
		{
			get
			{
				return this._pubId;
			}
			set
			{
				this._pubId = ((value == null) ? string.Empty : value);
			}
		}

		public string SystemId
		{
			get
			{
				return this._sysId;
			}
			set
			{
				this._sysId = ((value == null) ? string.Empty : value);
			}
		}

		public string BaseURI
		{
			get
			{
				return this._baseURI;
			}
			set
			{
				this._baseURI = ((value == null) ? string.Empty : value);
			}
		}

		public string InternalSubset
		{
			get
			{
				return this._internalSubset;
			}
			set
			{
				this._internalSubset = ((value == null) ? string.Empty : value);
			}
		}

		public string XmlLang
		{
			get
			{
				return this._xmlLang;
			}
			set
			{
				this._xmlLang = ((value == null) ? string.Empty : value);
			}
		}

		public XmlSpace XmlSpace
		{
			get
			{
				return this._xmlSpace;
			}
			set
			{
				this._xmlSpace = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				this._encoding = value;
			}
		}

		internal bool HasDtdInfo
		{
			get
			{
				return this._internalSubset != string.Empty || this._pubId != string.Empty || this._sysId != string.Empty;
			}
		}

		private XmlNameTable _nt;

		private XmlNamespaceManager _nsMgr;

		private string _docTypeName = string.Empty;

		private string _pubId = string.Empty;

		private string _sysId = string.Empty;

		private string _internalSubset = string.Empty;

		private string _xmlLang = string.Empty;

		private XmlSpace _xmlSpace;

		private string _baseURI = string.Empty;

		private Encoding _encoding;
	}
}
