using System;
using System.Text;

namespace System.Xml.Xsl.XsltOld
{
	internal class BuilderInfo
	{
		internal BuilderInfo()
		{
			this.Initialize(string.Empty, string.Empty, string.Empty);
		}

		internal void Initialize(string prefix, string name, string nspace)
		{
			this.prefix = prefix;
			this.localName = name;
			this.namespaceURI = nspace;
			this.name = null;
			this.htmlProps = null;
			this.htmlAttrProps = null;
			this.TextInfoCount = 0;
		}

		internal void Initialize(BuilderInfo src)
		{
			this.prefix = src.Prefix;
			this.localName = src.LocalName;
			this.namespaceURI = src.NamespaceURI;
			this.name = null;
			this.depth = src.Depth;
			this.nodeType = src.NodeType;
			this.htmlProps = src.htmlProps;
			this.htmlAttrProps = src.htmlAttrProps;
			this.TextInfoCount = 0;
			this.EnsureTextInfoSize(src.TextInfoCount);
			src.TextInfo.CopyTo(this.TextInfo, 0);
			this.TextInfoCount = src.TextInfoCount;
		}

		private void EnsureTextInfoSize(int newSize)
		{
			if (this.TextInfo.Length < newSize)
			{
				string[] array = new string[newSize * 2];
				Array.Copy(this.TextInfo, array, this.TextInfoCount);
				this.TextInfo = array;
			}
		}

		internal BuilderInfo Clone()
		{
			BuilderInfo builderInfo = new BuilderInfo();
			builderInfo.Initialize(this);
			return builderInfo;
		}

		internal string Name
		{
			get
			{
				if (this.name == null)
				{
					string text = this.Prefix;
					string text2 = this.LocalName;
					if (text != null && 0 < text.Length)
					{
						if (text2.Length > 0)
						{
							this.name = text + ":" + text2;
						}
						else
						{
							this.name = text;
						}
					}
					else
					{
						this.name = text2;
					}
				}
				return this.name;
			}
		}

		internal string LocalName
		{
			get
			{
				return this.localName;
			}
			set
			{
				this.localName = value;
			}
		}

		internal string NamespaceURI
		{
			get
			{
				return this.namespaceURI;
			}
			set
			{
				this.namespaceURI = value;
			}
		}

		internal string Prefix
		{
			get
			{
				return this.prefix;
			}
			set
			{
				this.prefix = value;
			}
		}

		internal string Value
		{
			get
			{
				int textInfoCount = this.TextInfoCount;
				if (textInfoCount == 0)
				{
					return string.Empty;
				}
				if (textInfoCount != 1)
				{
					int num = 0;
					for (int i = 0; i < this.TextInfoCount; i++)
					{
						string text = this.TextInfo[i];
						if (text != null)
						{
							num += text.Length;
						}
					}
					StringBuilder stringBuilder = new StringBuilder(num);
					for (int j = 0; j < this.TextInfoCount; j++)
					{
						string text2 = this.TextInfo[j];
						if (text2 != null)
						{
							stringBuilder.Append(text2);
						}
					}
					return stringBuilder.ToString();
				}
				return this.TextInfo[0];
			}
			set
			{
				this.TextInfoCount = 0;
				this.ValueAppend(value, false);
			}
		}

		internal void ValueAppend(string s, bool disableEscaping)
		{
			if (s == null || s.Length == 0)
			{
				return;
			}
			this.EnsureTextInfoSize(this.TextInfoCount + (disableEscaping ? 2 : 1));
			int num;
			if (disableEscaping)
			{
				string[] textInfo = this.TextInfo;
				num = this.TextInfoCount;
				this.TextInfoCount = num + 1;
				textInfo[num] = null;
			}
			string[] textInfo2 = this.TextInfo;
			num = this.TextInfoCount;
			this.TextInfoCount = num + 1;
			textInfo2[num] = s;
		}

		internal XmlNodeType NodeType
		{
			get
			{
				return this.nodeType;
			}
			set
			{
				this.nodeType = value;
			}
		}

		internal int Depth
		{
			get
			{
				return this.depth;
			}
			set
			{
				this.depth = value;
			}
		}

		internal bool IsEmptyTag
		{
			get
			{
				return this.isEmptyTag;
			}
			set
			{
				this.isEmptyTag = value;
			}
		}

		private string name;

		private string localName;

		private string namespaceURI;

		private string prefix;

		private XmlNodeType nodeType;

		private int depth;

		private bool isEmptyTag;

		internal string[] TextInfo = new string[4];

		internal int TextInfoCount;

		internal bool search;

		internal HtmlElementProps htmlProps;

		internal HtmlAttributeProps htmlAttrProps;
	}
}
