using System;

namespace System.Xml
{
	public abstract class XmlLinkedNode : XmlNode
	{
		internal XmlLinkedNode(XmlDocument doc)
			: base(doc)
		{
		}

		internal bool IsRooted
		{
			get
			{
				for (XmlNode xmlNode = this.ParentNode; xmlNode != null; xmlNode = xmlNode.ParentNode)
				{
					if (xmlNode.NodeType == XmlNodeType.Document)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override XmlNode NextSibling
		{
			get
			{
				return (this.ParentNode != null && this.ParentNode.LastChild != this) ? this.nextSibling : null;
			}
		}

		internal XmlLinkedNode NextLinkedSibling
		{
			get
			{
				return this.nextSibling;
			}
			set
			{
				this.nextSibling = value;
			}
		}

		public override XmlNode PreviousSibling
		{
			get
			{
				if (this.ParentNode != null)
				{
					XmlNode firstChild = this.ParentNode.FirstChild;
					if (firstChild != this)
					{
						while (firstChild.NextSibling != this)
						{
							if ((firstChild = firstChild.NextSibling) == null)
							{
								goto IL_0039;
							}
						}
						return firstChild;
					}
				}
				IL_0039:
				return null;
			}
		}

		private XmlLinkedNode nextSibling;
	}
}
