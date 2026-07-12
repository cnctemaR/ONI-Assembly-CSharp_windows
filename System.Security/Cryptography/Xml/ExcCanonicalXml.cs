using System;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class ExcCanonicalXml
	{
		internal ExcCanonicalXml(Stream inputStream, bool includeComments, string inclusiveNamespacesPrefixList, XmlResolver resolver, string strBaseUri)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			this._c14nDoc = new CanonicalXmlDocument(true, includeComments);
			this._c14nDoc.XmlResolver = resolver;
			this._c14nDoc.Load(Utils.PreProcessStreamInput(inputStream, resolver, strBaseUri));
			this._ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
		}

		internal ExcCanonicalXml(XmlDocument document, bool includeComments, string inclusiveNamespacesPrefixList, XmlResolver resolver)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this._c14nDoc = new CanonicalXmlDocument(true, includeComments);
			this._c14nDoc.XmlResolver = resolver;
			this._c14nDoc.Load(new XmlNodeReader(document));
			this._ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
		}

		internal ExcCanonicalXml(XmlNodeList nodeList, bool includeComments, string inclusiveNamespacesPrefixList, XmlResolver resolver)
		{
			if (nodeList == null)
			{
				throw new ArgumentNullException("nodeList");
			}
			XmlDocument ownerDocument = Utils.GetOwnerDocument(nodeList);
			if (ownerDocument == null)
			{
				throw new ArgumentException("nodeList");
			}
			this._c14nDoc = new CanonicalXmlDocument(false, includeComments);
			this._c14nDoc.XmlResolver = resolver;
			this._c14nDoc.Load(new XmlNodeReader(ownerDocument));
			this._ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
			ExcCanonicalXml.MarkInclusionStateForNodes(nodeList, ownerDocument, this._c14nDoc);
		}

		internal byte[] GetBytes()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this._c14nDoc.Write(stringBuilder, DocPosition.BeforeRootElement, this._ancMgr);
			return new UTF8Encoding(false).GetBytes(stringBuilder.ToString());
		}

		internal byte[] GetDigestedBytes(HashAlgorithm hash)
		{
			this._c14nDoc.WriteHash(hash, DocPosition.BeforeRootElement, this._ancMgr);
			hash.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
			byte[] array = (byte[])hash.Hash.Clone();
			hash.Initialize();
			return array;
		}

		private static void MarkInclusionStateForNodes(XmlNodeList nodeList, XmlDocument inputRoot, XmlDocument root)
		{
			CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
			CanonicalXmlNodeList canonicalXmlNodeList2 = new CanonicalXmlNodeList();
			canonicalXmlNodeList.Add(inputRoot);
			canonicalXmlNodeList2.Add(root);
			int num = 0;
			do
			{
				XmlNode xmlNode = canonicalXmlNodeList[num];
				XmlNode xmlNode2 = canonicalXmlNodeList2[num];
				XmlNodeList childNodes = xmlNode.ChildNodes;
				XmlNodeList childNodes2 = xmlNode2.ChildNodes;
				for (int i = 0; i < childNodes.Count; i++)
				{
					canonicalXmlNodeList.Add(childNodes[i]);
					canonicalXmlNodeList2.Add(childNodes2[i]);
					if (Utils.NodeInList(childNodes[i], nodeList))
					{
						ExcCanonicalXml.MarkNodeAsIncluded(childNodes2[i]);
					}
					XmlAttributeCollection attributes = childNodes[i].Attributes;
					if (attributes != null)
					{
						for (int j = 0; j < attributes.Count; j++)
						{
							if (Utils.NodeInList(attributes[j], nodeList))
							{
								ExcCanonicalXml.MarkNodeAsIncluded(childNodes2[i].Attributes.Item(j));
							}
						}
					}
				}
				num++;
			}
			while (num < canonicalXmlNodeList.Count);
		}

		private static void MarkNodeAsIncluded(XmlNode node)
		{
			if (node is ICanonicalizableNode)
			{
				((ICanonicalizableNode)node).IsInNodeSet = true;
			}
		}

		private CanonicalXmlDocument _c14nDoc;

		private ExcAncestralNamespaceContextManager _ancMgr;
	}
}
