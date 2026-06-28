using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public sealed class GraphMLFormat
	{
		public IGraph Graph { get; set; }

		public IList<GraphMLProperty> Properties { get; private set; }

		public GraphMLFormat()
		{
			this.Properties = new List<GraphMLProperty>();
			List<Func<XElement, GraphMLProperty>> list = new List<Func<XElement, GraphMLProperty>>();
			list.Add((XElement x) => new StandardProperty<bool>(x));
			list.Add((XElement x) => new StandardProperty<double>(x));
			list.Add((XElement x) => new StandardProperty<float>(x));
			list.Add((XElement x) => new StandardProperty<int>(x));
			list.Add((XElement x) => new StandardProperty<long>(x));
			list.Add((XElement x) => new StandardProperty<string>(x));
			list.Add((XElement x) => new NodeGraphicsProperty(x));
			this.PropertyLoaders = list;
		}

		public void RegisterPropertyLoader(Func<XElement, GraphMLProperty> loader)
		{
			this.PropertyLoaders.Add(loader);
		}

		private static void ReadProperties(Dictionary<string, GraphMLProperty> propertyById, XElement x, object obj)
		{
			foreach (XElement xelement in Utils.ElementsLocal(x, "data"))
			{
				GraphMLProperty graphMLProperty;
				if (propertyById.TryGetValue(xelement.Attribute("key").Value, out graphMLProperty))
				{
					graphMLProperty.ReadData(x, obj);
				}
			}
		}

		public void Load(XDocument doc)
		{
			if (this.Graph == null)
			{
				this.Graph = new CustomGraph();
			}
			IBuildableGraph buildableGraph = (IBuildableGraph)this.Graph;
			buildableGraph.Clear();
			XElement root = doc.Root;
			this.Properties.Clear();
			Dictionary<string, GraphMLProperty> dictionary = new Dictionary<string, GraphMLProperty>();
			foreach (XElement xelement in Utils.ElementsLocal(root, "key"))
			{
				foreach (Func<XElement, GraphMLProperty> func in this.PropertyLoaders)
				{
					try
					{
						GraphMLProperty graphMLProperty = func(xelement);
						this.Properties.Add(graphMLProperty);
						dictionary[graphMLProperty.Id] = graphMLProperty;
						break;
					}
					catch (ArgumentException)
					{
					}
				}
			}
			XElement xelement2 = Utils.ElementLocal(root, "graph");
			Directedness directedness = ((xelement2.Attribute("edgedefault").Value == "directed") ? Directedness.Directed : Directedness.Undirected);
			GraphMLFormat.ReadProperties(dictionary, xelement2, this.Graph);
			Dictionary<string, Node> dictionary2 = new Dictionary<string, Node>();
			foreach (XElement xelement3 in Utils.ElementsLocal(xelement2, "node"))
			{
				Node node = buildableGraph.AddNode();
				dictionary2[xelement3.Attribute("id").Value] = node;
				GraphMLFormat.ReadProperties(dictionary, xelement3, node);
			}
			foreach (XElement xelement4 in Utils.ElementsLocal(xelement2, "edge"))
			{
				Node node2 = dictionary2[xelement4.Attribute("source").Value];
				Node node3 = dictionary2[xelement4.Attribute("target").Value];
				Directedness directedness2 = directedness;
				XAttribute xattribute = xelement4.Attribute("directed");
				if (xattribute != null)
				{
					directedness2 = ((xattribute.Value == "true") ? Directedness.Directed : Directedness.Undirected);
				}
				Arc arc = buildableGraph.AddArc(node2, node3, directedness2);
				GraphMLFormat.ReadProperties(dictionary, xelement4, arc);
			}
		}

		public void Load(XmlReader xml)
		{
			XDocument xdocument = XDocument.Load(xml);
			this.Load(xdocument);
		}

		public void Load(TextReader reader)
		{
			using (XmlReader xmlReader = XmlReader.Create(reader))
			{
				this.Load(xmlReader);
			}
		}

		public void Load(string filename)
		{
			using (StreamReader streamReader = new StreamReader(filename))
			{
				this.Load(streamReader);
			}
		}

		private void DefinePropertyValues(XmlWriter xml, object obj)
		{
			foreach (GraphMLProperty graphMLProperty in this.Properties)
			{
				XElement xelement = graphMLProperty.WriteData(obj);
				if (xelement != null)
				{
					xelement.Name = GraphMLFormat.xmlns + "data";
					xelement.SetAttributeValue("key", graphMLProperty.Id);
					xelement.WriteTo(xml);
				}
			}
		}

		private void Save(XmlWriter xml)
		{
			xml.WriteStartDocument();
			xml.WriteStartElement("graphml", GraphMLFormat.xmlns.NamespaceName);
			xml.WriteAttributeString("xmlns", "xsi", null, GraphMLFormat.xmlnsXsi.NamespaceName);
			xml.WriteAttributeString("xmlns", "y", null, GraphMLFormat.xmlnsY.NamespaceName);
			xml.WriteAttributeString("xmlns", "yed", null, GraphMLFormat.xmlnsYed.NamespaceName);
			xml.WriteAttributeString("xsi", "schemaLocation", null, "http://graphml.graphdrawing.org/xmlns\nhttp://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd");
			for (int i = 0; i < this.Properties.Count; i++)
			{
				GraphMLProperty graphMLProperty = this.Properties[i];
				graphMLProperty.Id = "d" + i;
				graphMLProperty.GetKeyElement().WriteTo(xml);
			}
			xml.WriteStartElement("graph", GraphMLFormat.xmlns.NamespaceName);
			xml.WriteAttributeString("id", "G");
			xml.WriteAttributeString("edgedefault", "directed");
			xml.WriteAttributeString("parse.nodes", this.Graph.NodeCount().ToString(CultureInfo.InvariantCulture));
			xml.WriteAttributeString("parse.edges", this.Graph.ArcCount(ArcFilter.All).ToString(CultureInfo.InvariantCulture));
			xml.WriteAttributeString("parse.order", "nodesfirst");
			this.DefinePropertyValues(xml, this.Graph);
			foreach (Node node in this.Graph.Nodes())
			{
				xml.WriteStartElement("node", GraphMLFormat.xmlns.NamespaceName);
				xml.WriteAttributeString("id", node.Id.ToString(CultureInfo.InvariantCulture));
				this.DefinePropertyValues(xml, node);
				xml.WriteEndElement();
			}
			foreach (Arc arc in this.Graph.Arcs(ArcFilter.All))
			{
				xml.WriteStartElement("edge", GraphMLFormat.xmlns.NamespaceName);
				xml.WriteAttributeString("id", arc.Id.ToString(CultureInfo.InvariantCulture));
				if (this.Graph.IsEdge(arc))
				{
					xml.WriteAttributeString("directed", "false");
				}
				xml.WriteAttributeString("source", this.Graph.U(arc).Id.ToString(CultureInfo.InvariantCulture));
				xml.WriteAttributeString("target", this.Graph.V(arc).Id.ToString(CultureInfo.InvariantCulture));
				this.DefinePropertyValues(xml, arc);
				xml.WriteEndElement();
			}
			xml.WriteEndElement();
			xml.WriteEndElement();
		}

		public void Save(TextWriter writer)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(writer))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(string filename)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				this.Save(streamWriter);
			}
		}

		private const string xsiSchemaLocation = "http://graphml.graphdrawing.org/xmlns\nhttp://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd";

		internal static readonly XNamespace xmlns = "http://graphml.graphdrawing.org/xmlns";

		private static readonly XNamespace xmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance";

		internal static readonly XNamespace xmlnsY = "http://www.yworks.com/xml/graphml";

		private static readonly XNamespace xmlnsYed = "http://www.yworks.com/xml/yed/3";

		private readonly List<Func<XElement, GraphMLProperty>> PropertyLoaders;
	}
}
