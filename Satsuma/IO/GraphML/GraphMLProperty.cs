using System;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public abstract class GraphMLProperty
	{
		public string Name { get; set; }

		public PropertyDomain Domain { get; set; }

		public string Id { get; set; }

		protected GraphMLProperty()
		{
			this.Domain = PropertyDomain.All;
		}

		protected static string DomainToGraphML(PropertyDomain domain)
		{
			switch (domain)
			{
			case PropertyDomain.Node:
				return "node";
			case PropertyDomain.Arc:
				return "arc";
			case PropertyDomain.Graph:
				return "graph";
			default:
				return "all";
			}
		}

		protected static PropertyDomain ParseDomain(string s)
		{
			if (s != null)
			{
				if (s == "node")
				{
					return PropertyDomain.Node;
				}
				if (s == "edge")
				{
					return PropertyDomain.Arc;
				}
				if (s == "graph")
				{
					return PropertyDomain.Graph;
				}
			}
			return PropertyDomain.All;
		}

		protected virtual void LoadFromKeyElement(XElement xKey)
		{
			XAttribute xattribute = xKey.Attribute("attr.name");
			this.Name = ((xattribute == null) ? null : xattribute.Value);
			this.Domain = GraphMLProperty.ParseDomain(xKey.Attribute("for").Value);
			this.Id = xKey.Attribute("id").Value;
			XElement xelement = Utils.ElementLocal(xKey, "default");
			this.ReadData(xelement, null);
		}

		public virtual XElement GetKeyElement()
		{
			XElement xelement = new XElement(GraphMLFormat.xmlns + "key");
			xelement.SetAttributeValue("attr.name", this.Name);
			xelement.SetAttributeValue("for", GraphMLProperty.DomainToGraphML(this.Domain));
			xelement.SetAttributeValue("id", this.Id);
			XElement xelement2 = this.WriteData(null);
			if (xelement2 != null)
			{
				xelement2.Name = GraphMLFormat.xmlns + "default";
				xelement.Add(xelement2);
			}
			return xelement;
		}

		public abstract void ReadData(XElement x, object key);

		public abstract XElement WriteData(object key);
	}
}
