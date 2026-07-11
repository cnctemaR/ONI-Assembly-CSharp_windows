using System;
using System.Text;

namespace System.Xml.Linq
{
	public class XText : XNode
	{
		public XText(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.text = value;
		}

		public XText(XText other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.text = other.text;
		}

		internal XText(XmlReader r)
		{
			this.text = r.Value;
			r.Read();
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Text;
			}
		}

		public string Value
		{
			get
			{
				return this.text;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this.text = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (this.parent is XDocument)
			{
				writer.WriteWhitespace(this.text);
				return;
			}
			writer.WriteString(this.text);
		}

		internal override void AppendText(StringBuilder sb)
		{
			sb.Append(this.text);
		}

		internal override XNode CloneNode()
		{
			return new XText(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			return node != null && this.NodeType == node.NodeType && this.text == ((XText)node).text;
		}

		internal override int GetDeepHashCode()
		{
			return this.text.GetHashCode();
		}

		internal string text;
	}
}
