using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public sealed class XNodeEqualityComparer : IEqualityComparer, IEqualityComparer<XNode>
	{
		bool IEqualityComparer.Equals(object n1, object n2)
		{
			return this.Equals((XNode)n1, (XNode)n2);
		}

		int IEqualityComparer.GetHashCode(object node)
		{
			return this.GetHashCode((XNode)node);
		}

		public bool Equals(XNode n1, XNode n2)
		{
			if (n1 == null)
			{
				return n2 == null;
			}
			if (n2 == null)
			{
				return false;
			}
			if (n1.NodeType != n2.NodeType)
			{
				return false;
			}
			switch (n1.NodeType)
			{
			case XmlNodeType.Element:
			{
				XElement xelement = (XElement)n1;
				XElement xelement2 = (XElement)n2;
				if (xelement.Name != xelement2.Name)
				{
					return false;
				}
				IEnumerator<XAttribute> enumerator = xelement2.Attributes().GetEnumerator();
				foreach (XAttribute xattribute in xelement.Attributes())
				{
					if (!enumerator.MoveNext())
					{
						return false;
					}
					if (!this.Equals(xattribute, enumerator.Current))
					{
						return false;
					}
				}
				if (enumerator.MoveNext())
				{
					return false;
				}
				IEnumerator<XNode> enumerator3 = xelement2.Nodes().GetEnumerator();
				foreach (XNode xnode in xelement.Nodes())
				{
					if (!enumerator3.MoveNext())
					{
						return false;
					}
					if (!this.Equals(xnode, enumerator3.Current))
					{
						return false;
					}
				}
				return !enumerator3.MoveNext();
			}
			case XmlNodeType.Text:
				return ((XText)n1).Value == ((XText)n2).Value;
			case XmlNodeType.ProcessingInstruction:
			{
				XProcessingInstruction xprocessingInstruction = (XProcessingInstruction)n1;
				XProcessingInstruction xprocessingInstruction2 = (XProcessingInstruction)n2;
				return xprocessingInstruction.Target == xprocessingInstruction2.Target && xprocessingInstruction.Data == xprocessingInstruction2.Data;
			}
			case XmlNodeType.Comment:
			{
				XComment xcomment = (XComment)n1;
				XComment xcomment2 = (XComment)n2;
				return xcomment.Value == xcomment2.Value;
			}
			case XmlNodeType.Document:
			{
				XDocument xdocument = (XDocument)n1;
				XDocument xdocument2 = (XDocument)n2;
				if (!this.Equals(xdocument.Declaration, xdocument2.Declaration))
				{
					return false;
				}
				IEnumerator<XNode> enumerator5 = xdocument2.Nodes().GetEnumerator();
				foreach (XNode xnode2 in xdocument.Nodes())
				{
					if (!enumerator5.MoveNext())
					{
						return false;
					}
					if (!this.Equals(xnode2, enumerator5.Current))
					{
						return false;
					}
				}
				return !enumerator5.MoveNext();
			}
			case XmlNodeType.DocumentType:
			{
				XDocumentType xdocumentType = (XDocumentType)n1;
				XDocumentType xdocumentType2 = (XDocumentType)n2;
				return xdocumentType.Name == xdocumentType2.Name && xdocumentType.PublicId == xdocumentType2.PublicId && xdocumentType.SystemId == xdocumentType2.SystemId && xdocumentType.InternalSubset == xdocumentType2.InternalSubset;
			}
			}
			throw new Exception("INTERNAL ERROR: should not happen");
		}

		private bool Equals(XAttribute a1, XAttribute a2)
		{
			if (a1 == null)
			{
				return a2 == null;
			}
			return a2 != null && a1.Name == a2.Name && a1.Value == a2.Value;
		}

		private bool Equals(XDeclaration d1, XDeclaration d2)
		{
			if (d1 == null)
			{
				return d2 == null;
			}
			return d2 != null && (d1.Version == d2.Version && d1.Encoding == d2.Encoding) && d1.Standalone == d2.Standalone;
		}

		private int GetHashCode(XDeclaration d)
		{
			if (d == null)
			{
				return 0;
			}
			return (d.Version.GetHashCode() << 7) ^ (d.Encoding.GetHashCode() << 6) ^ d.Standalone.GetHashCode();
		}

		public int GetHashCode(XNode node)
		{
			if (node == null)
			{
				return 0;
			}
			int num = (int)((int)node.NodeType << 6);
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
			{
				XElement xelement = (XElement)node;
				num ^= xelement.Name.GetHashCode() << 3;
				foreach (XAttribute xattribute in xelement.Attributes())
				{
					num ^= xattribute.GetHashCode() << 7;
				}
				foreach (XNode xnode in xelement.Nodes())
				{
					num ^= xnode.GetHashCode() << 6;
				}
				break;
			}
			case XmlNodeType.Text:
				num ^= ((XText)node).GetHashCode();
				break;
			case XmlNodeType.ProcessingInstruction:
			{
				XProcessingInstruction xprocessingInstruction = (XProcessingInstruction)node;
				num ^= (xprocessingInstruction.Target.GetHashCode() << 6) + xprocessingInstruction.Data.GetHashCode();
				break;
			}
			case XmlNodeType.Comment:
				num ^= ((XComment)node).Value.GetHashCode();
				break;
			case XmlNodeType.Document:
			{
				XDocument xdocument = (XDocument)node;
				num ^= this.GetHashCode(xdocument.Declaration);
				foreach (XNode xnode2 in xdocument.Nodes())
				{
					num ^= xnode2.GetHashCode() << 5;
				}
				break;
			}
			case XmlNodeType.DocumentType:
			{
				XDocumentType xdocumentType = (XDocumentType)node;
				num = num ^ (xdocumentType.Name.GetHashCode() << 7) ^ (xdocumentType.PublicId.GetHashCode() << 6) ^ (xdocumentType.SystemId.GetHashCode() << 5) ^ (xdocumentType.InternalSubset.GetHashCode() << 4);
				break;
			}
			}
			return num;
		}
	}
}
