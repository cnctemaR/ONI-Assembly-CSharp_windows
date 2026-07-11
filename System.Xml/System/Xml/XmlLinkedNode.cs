using System;

namespace System.Xml
{
	public abstract class XmlLinkedNode : XmlNode
	{
		internal XmlLinkedNode()
		{
			this.next = null;
		}

		internal XmlLinkedNode(XmlDocument doc)
			: base(doc)
		{
			this.next = null;
		}

		public override XmlNode PreviousSibling
		{
			get
			{
				XmlNode parentNode = this.ParentNode;
				if (parentNode != null)
				{
					XmlNode xmlNode;
					XmlNode nextSibling;
					for (xmlNode = parentNode.FirstChild; xmlNode != null; xmlNode = nextSibling)
					{
						nextSibling = xmlNode.NextSibling;
						if (nextSibling == this)
						{
							break;
						}
					}
					return xmlNode;
				}
				return null;
			}
		}

		public override XmlNode NextSibling
		{
			get
			{
				XmlNode parentNode = this.ParentNode;
				if (parentNode != null && this.next != parentNode.FirstChild)
				{
					return this.next;
				}
				return null;
			}
		}

		internal XmlLinkedNode next;
	}
}
