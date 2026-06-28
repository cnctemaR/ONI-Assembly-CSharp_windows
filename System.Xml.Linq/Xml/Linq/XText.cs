using System;

namespace System.Xml.Linq
{
	public class XText : XNode
	{
		public XText(string value)
		{
			this.value = value;
		}

		public XText(XText other)
		{
			this.value = other.value;
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
				return this.value;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.value = value;
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteString(this.value);
		}

		private string value;
	}
}
