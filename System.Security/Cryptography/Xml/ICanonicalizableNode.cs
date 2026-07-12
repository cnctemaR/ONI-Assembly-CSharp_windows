using System;
using System.Text;

namespace System.Security.Cryptography.Xml
{
	internal interface ICanonicalizableNode
	{
		bool IsInNodeSet { get; set; }

		void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc);

		void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc);
	}
}
