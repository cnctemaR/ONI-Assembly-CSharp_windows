using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class XslNode
	{
		public XslNode(XslNodeType nodeType, QilName name, object arg, XslVersion xslVer)
		{
			this.NodeType = nodeType;
			this.Name = name;
			this.Arg = arg;
			this.XslVersion = xslVer;
		}

		public XslNode(XslNodeType nodeType)
		{
			this.NodeType = nodeType;
			this.XslVersion = XslVersion.Version10;
		}

		public string Select
		{
			get
			{
				return (string)this.Arg;
			}
		}

		public bool ForwardsCompatible
		{
			get
			{
				return this.XslVersion == XslVersion.ForwardsCompatible;
			}
		}

		public IList<XslNode> Content
		{
			get
			{
				IList<XslNode> list = this.content;
				return list ?? XslNode.EmptyList;
			}
		}

		public void SetContent(List<XslNode> content)
		{
			this.content = content;
		}

		public void AddContent(XslNode node)
		{
			if (this.content == null)
			{
				this.content = new List<XslNode>();
			}
			this.content.Add(node);
		}

		public void InsertContent(IEnumerable<XslNode> collection)
		{
			if (this.content == null)
			{
				this.content = new List<XslNode>(collection);
				return;
			}
			this.content.InsertRange(0, collection);
		}

		internal string TraceName
		{
			get
			{
				return null;
			}
		}

		public readonly XslNodeType NodeType;

		public ISourceLineInfo SourceLine;

		public NsDecl Namespaces;

		public readonly QilName Name;

		public readonly object Arg;

		public readonly XslVersion XslVersion;

		public XslFlags Flags;

		private List<XslNode> content;

		private static readonly IList<XslNode> EmptyList = new List<XslNode>().AsReadOnly();
	}
}
