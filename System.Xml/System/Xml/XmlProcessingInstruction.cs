using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlProcessingInstruction : XmlLinkedNode
	{
		protected internal XmlProcessingInstruction(string target, string data, XmlDocument doc)
			: base(doc)
		{
			XmlConvert.VerifyName(target);
			if (data == null)
			{
				data = string.Empty;
			}
			this.target = target;
			this.data = data;
		}

		public string Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public override string InnerText
		{
			get
			{
				return this.Data;
			}
			set
			{
				this.data = value;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.target;
			}
		}

		public override string Name
		{
			get
			{
				return this.target;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.ProcessingInstruction;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.ProcessingInstruction;
			}
		}

		public string Target
		{
			get
			{
				return this.target;
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
				if (this.IsReadOnly)
				{
					throw new ArgumentException("This node is read-only.");
				}
				this.data = value;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlProcessingInstruction(this.target, this.data, this.OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteProcessingInstruction(this.target, this.data);
		}

		private string target;

		private string data;
	}
}
