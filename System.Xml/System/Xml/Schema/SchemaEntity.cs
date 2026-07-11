using System;

namespace System.Xml.Schema
{
	internal sealed class SchemaEntity : IDtdEntityInfo
	{
		internal SchemaEntity(XmlQualifiedName qname, bool isParameter)
		{
			this.qname = qname;
			this.isParameter = isParameter;
		}

		string IDtdEntityInfo.Name
		{
			get
			{
				return this.Name.Name;
			}
		}

		bool IDtdEntityInfo.IsExternal
		{
			get
			{
				return this.IsExternal;
			}
		}

		bool IDtdEntityInfo.IsDeclaredInExternal
		{
			get
			{
				return this.DeclaredInExternal;
			}
		}

		bool IDtdEntityInfo.IsUnparsedEntity
		{
			get
			{
				return !this.NData.IsEmpty;
			}
		}

		bool IDtdEntityInfo.IsParameterEntity
		{
			get
			{
				return this.isParameter;
			}
		}

		string IDtdEntityInfo.BaseUriString
		{
			get
			{
				return this.BaseURI;
			}
		}

		string IDtdEntityInfo.DeclaredUriString
		{
			get
			{
				return this.DeclaredURI;
			}
		}

		string IDtdEntityInfo.SystemId
		{
			get
			{
				return this.Url;
			}
		}

		string IDtdEntityInfo.PublicId
		{
			get
			{
				return this.Pubid;
			}
		}

		string IDtdEntityInfo.Text
		{
			get
			{
				return this.Text;
			}
		}

		int IDtdEntityInfo.LineNumber
		{
			get
			{
				return this.Line;
			}
		}

		int IDtdEntityInfo.LinePosition
		{
			get
			{
				return this.Pos;
			}
		}

		internal static bool IsPredefinedEntity(string n)
		{
			return n == "lt" || n == "gt" || n == "amp" || n == "apos" || n == "quot";
		}

		internal XmlQualifiedName Name
		{
			get
			{
				return this.qname;
			}
		}

		internal string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				this.url = value;
				this.isExternal = true;
			}
		}

		internal string Pubid
		{
			get
			{
				return this.pubid;
			}
			set
			{
				this.pubid = value;
			}
		}

		internal bool IsExternal
		{
			get
			{
				return this.isExternal;
			}
			set
			{
				this.isExternal = value;
			}
		}

		internal bool DeclaredInExternal
		{
			get
			{
				return this.isDeclaredInExternal;
			}
			set
			{
				this.isDeclaredInExternal = value;
			}
		}

		internal XmlQualifiedName NData
		{
			get
			{
				return this.ndata;
			}
			set
			{
				this.ndata = value;
			}
		}

		internal string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.isExternal = false;
			}
		}

		internal int Line
		{
			get
			{
				return this.lineNumber;
			}
			set
			{
				this.lineNumber = value;
			}
		}

		internal int Pos
		{
			get
			{
				return this.linePosition;
			}
			set
			{
				this.linePosition = value;
			}
		}

		internal string BaseURI
		{
			get
			{
				if (this.baseURI != null)
				{
					return this.baseURI;
				}
				return string.Empty;
			}
			set
			{
				this.baseURI = value;
			}
		}

		internal bool ParsingInProgress
		{
			get
			{
				return this.parsingInProgress;
			}
			set
			{
				this.parsingInProgress = value;
			}
		}

		internal string DeclaredURI
		{
			get
			{
				if (this.declaredURI != null)
				{
					return this.declaredURI;
				}
				return string.Empty;
			}
			set
			{
				this.declaredURI = value;
			}
		}

		private XmlQualifiedName qname;

		private string url;

		private string pubid;

		private string text;

		private XmlQualifiedName ndata = XmlQualifiedName.Empty;

		private int lineNumber;

		private int linePosition;

		private bool isParameter;

		private bool isExternal;

		private bool parsingInProgress;

		private bool isDeclaredInExternal;

		private string baseURI;

		private string declaredURI;
	}
}
