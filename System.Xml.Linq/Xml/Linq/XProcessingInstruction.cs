using System;

namespace System.Xml.Linq
{
	public class XProcessingInstruction : XNode
	{
		public XProcessingInstruction(string name, string data)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this.name = name;
			this.data = data;
		}

		public XProcessingInstruction(XProcessingInstruction other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.name = other.name;
			this.data = other.data;
		}

		public string Data
		{
			get
			{
				return this.data;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.data = value;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.ProcessingInstruction;
			}
		}

		public string Target
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.name = value;
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteProcessingInstruction(this.name, this.data);
		}

		private string name;

		private string data;
	}
}
