using System;
using System.Xml.XPath;

namespace System.Xml
{
	public abstract class XmlCharacterData : XmlLinkedNode
	{
		protected internal XmlCharacterData(string data, XmlDocument doc)
			: base(doc)
		{
			if (data == null)
			{
				data = string.Empty;
			}
			this.data = data;
		}

		public virtual string Data
		{
			get
			{
				return this.data;
			}
			set
			{
				string text = this.data;
				this.OwnerDocument.onNodeChanging(this, this.ParentNode, text, value);
				this.data = value;
				this.OwnerDocument.onNodeChanged(this, this.ParentNode, text, value);
			}
		}

		public override string InnerText
		{
			get
			{
				return this.data;
			}
			set
			{
				this.Data = value;
			}
		}

		public virtual int Length
		{
			get
			{
				return (this.data == null) ? 0 : this.data.Length;
			}
		}

		public override string Value
		{
			get
			{
				return this.data;
			}
			set
			{
				this.Data = value;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Text;
			}
		}

		public virtual void AppendData(string strData)
		{
			string text = this.data;
			string text2 = (this.data += strData);
			this.OwnerDocument.onNodeChanging(this, this.ParentNode, text, text2);
			this.data = text2;
			this.OwnerDocument.onNodeChanged(this, this.ParentNode, text, text2);
		}

		public virtual void DeleteData(int offset, int count)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be non-negative and must not be greater than the length of this instance.");
			}
			int num = this.data.Length - offset;
			if (offset + count < this.data.Length)
			{
				num = count;
			}
			string text = this.data;
			string text2 = this.data.Remove(offset, num);
			this.OwnerDocument.onNodeChanging(this, this.ParentNode, text, text2);
			this.data = text2;
			this.OwnerDocument.onNodeChanged(this, this.ParentNode, text, text2);
		}

		public virtual void InsertData(int offset, string strData)
		{
			if (offset < 0 || offset > this.data.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be non-negative and must not be greater than the length of this instance.");
			}
			string text = this.data;
			string text2 = this.data.Insert(offset, strData);
			this.OwnerDocument.onNodeChanging(this, this.ParentNode, text, text2);
			this.data = text2;
			this.OwnerDocument.onNodeChanged(this, this.ParentNode, text, text2);
		}

		public virtual void ReplaceData(int offset, int count, string strData)
		{
			if (offset < 0 || offset > this.data.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be non-negative and must not be greater than the length of this instance.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must be non-negative.");
			}
			if (strData == null)
			{
				throw new ArgumentNullException("strData", "Must be non-null.");
			}
			string text = this.data;
			string text2 = this.data.Substring(0, offset) + strData;
			if (offset + count < this.data.Length)
			{
				text2 += this.data.Substring(offset + count);
			}
			this.OwnerDocument.onNodeChanging(this, this.ParentNode, text, text2);
			this.data = text2;
			this.OwnerDocument.onNodeChanged(this, this.ParentNode, text, text2);
		}

		public virtual string Substring(int offset, int count)
		{
			if (this.data.Length < offset + count)
			{
				return this.data.Substring(offset);
			}
			return this.data.Substring(offset, count);
		}

		private string data;
	}
}
