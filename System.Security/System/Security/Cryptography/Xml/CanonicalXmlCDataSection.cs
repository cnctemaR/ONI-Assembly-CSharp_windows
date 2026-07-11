using System;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class CanonicalXmlCDataSection : XmlCDataSection, ICanonicalizableNode
	{
		public CanonicalXmlCDataSection(string data, XmlDocument doc, bool defaultNodeSetInclusionState)
			: base(data, doc)
		{
			this._isInNodeSet = defaultNodeSetInclusionState;
		}

		public bool IsInNodeSet
		{
			get
			{
				return this._isInNodeSet;
			}
			set
			{
				this._isInNodeSet = value;
			}
		}

		public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (this.IsInNodeSet)
			{
				strBuilder.Append(Utils.EscapeCData(this.Data));
			}
		}

		public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (this.IsInNodeSet)
			{
				byte[] bytes = new UTF8Encoding(false).GetBytes(Utils.EscapeCData(this.Data));
				hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			}
		}

		private bool _isInNodeSet;
	}
}
