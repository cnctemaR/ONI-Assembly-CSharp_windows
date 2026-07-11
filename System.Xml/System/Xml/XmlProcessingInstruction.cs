using System;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlProcessingInstruction : XmlLinkedNode
	{
		protected internal XmlProcessingInstruction(string target, string data, XmlDocument doc)
			: base(doc)
		{
			this.target = target;
			this.data = data;
		}

		public override string Name
		{
			get
			{
				if (this.target != null)
				{
					return this.target;
				}
				return string.Empty;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.Name;
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

		public string Target
		{
			get
			{
				return this.target;
			}
		}

		public string Data
		{
			get
			{
				return this.data;
			}
			set
			{
				XmlNode parentNode = this.ParentNode;
				XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(this, parentNode, parentNode, this.data, value, XmlNodeChangedAction.Change);
				if (eventArgs != null)
				{
					this.BeforeEvent(eventArgs);
				}
				this.data = value;
				if (eventArgs != null)
				{
					this.AfterEvent(eventArgs);
				}
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

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.ProcessingInstruction;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			return this.OwnerDocument.CreateProcessingInstruction(this.target, this.data);
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteProcessingInstruction(this.target, this.data);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		internal override string XPLocalName
		{
			get
			{
				return this.Name;
			}
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				return XPathNodeType.ProcessingInstruction;
			}
		}

		private string target;

		private string data;
	}
}
