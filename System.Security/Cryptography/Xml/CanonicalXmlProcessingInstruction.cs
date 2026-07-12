using System;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class CanonicalXmlProcessingInstruction : XmlProcessingInstruction, ICanonicalizableNode
	{
		public CanonicalXmlProcessingInstruction(string target, string data, XmlDocument doc, bool defaultNodeSetInclusionState)
			: base(target, data, doc)
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
			if (!this.IsInNodeSet)
			{
				return;
			}
			if (docPos == DocPosition.AfterRootElement)
			{
				strBuilder.Append('\n');
			}
			strBuilder.Append("<?");
			strBuilder.Append(this.Name);
			if (this.Value != null && this.Value.Length > 0)
			{
				strBuilder.Append(" " + this.Value);
			}
			strBuilder.Append("?>");
			if (docPos == DocPosition.BeforeRootElement)
			{
				strBuilder.Append('\n');
			}
		}

		public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (!this.IsInNodeSet)
			{
				return;
			}
			UTF8Encoding utf8Encoding = new UTF8Encoding(false);
			byte[] array;
			if (docPos == DocPosition.AfterRootElement)
			{
				array = utf8Encoding.GetBytes("(char) 10");
				hash.TransformBlock(array, 0, array.Length, array, 0);
			}
			array = utf8Encoding.GetBytes("<?");
			hash.TransformBlock(array, 0, array.Length, array, 0);
			array = utf8Encoding.GetBytes(this.Name);
			hash.TransformBlock(array, 0, array.Length, array, 0);
			if (this.Value != null && this.Value.Length > 0)
			{
				array = utf8Encoding.GetBytes(" " + this.Value);
				hash.TransformBlock(array, 0, array.Length, array, 0);
			}
			array = utf8Encoding.GetBytes("?>");
			hash.TransformBlock(array, 0, array.Length, array, 0);
			if (docPos == DocPosition.BeforeRootElement)
			{
				array = utf8Encoding.GetBytes("(char) 10");
				hash.TransformBlock(array, 0, array.Length, array, 0);
			}
		}

		private bool _isInNodeSet;
	}
}
