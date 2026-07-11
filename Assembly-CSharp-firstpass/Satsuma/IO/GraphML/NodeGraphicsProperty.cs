using System;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public sealed class NodeGraphicsProperty : DictionaryProperty<NodeGraphics>
	{
		public NodeGraphicsProperty()
		{
			base.Domain = PropertyDomain.Node;
		}

		internal NodeGraphicsProperty(XElement xKey)
			: this()
		{
			XAttribute xattribute = xKey.Attribute("yfiles.type");
			if (xattribute == null || xattribute.Value != "nodegraphics")
			{
				throw new ArgumentException("Key not compatible with property.");
			}
			this.LoadFromKeyElement(xKey);
		}

		public override XElement GetKeyElement()
		{
			XElement keyElement = base.GetKeyElement();
			keyElement.SetAttributeValue("yfiles.type", "nodegraphics");
			return keyElement;
		}

		protected override NodeGraphics ReadValue(XElement x)
		{
			return new NodeGraphics(x);
		}

		protected override XElement WriteValue(NodeGraphics value)
		{
			return value.ToXml();
		}
	}
}
