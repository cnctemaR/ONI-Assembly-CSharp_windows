using System;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class CanonicalizationDispatcher
	{
		private CanonicalizationDispatcher()
		{
		}

		public static void Write(XmlNode node, StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (node is ICanonicalizableNode)
			{
				((ICanonicalizableNode)node).Write(strBuilder, docPos, anc);
				return;
			}
			CanonicalizationDispatcher.WriteGenericNode(node, strBuilder, docPos, anc);
		}

		public static void WriteGenericNode(XmlNode node, StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			foreach (object obj in node.ChildNodes)
			{
				CanonicalizationDispatcher.Write((XmlNode)obj, strBuilder, docPos, anc);
			}
		}

		public static void WriteHash(XmlNode node, HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (node is ICanonicalizableNode)
			{
				((ICanonicalizableNode)node).WriteHash(hash, docPos, anc);
				return;
			}
			CanonicalizationDispatcher.WriteHashGenericNode(node, hash, docPos, anc);
		}

		public static void WriteHashGenericNode(XmlNode node, HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			foreach (object obj in node.ChildNodes)
			{
				CanonicalizationDispatcher.WriteHash((XmlNode)obj, hash, docPos, anc);
			}
		}
	}
}
