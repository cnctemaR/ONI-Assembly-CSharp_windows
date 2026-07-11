using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ResourceTreeLoader<T> : ResourceLoader<T> where T : ResourceTreeNode, new()
{
	public ResourceTreeLoader(TextAsset file)
		: base(file)
	{
	}

	public override void Load(TextAsset file)
	{
		Dictionary<string, ResourceTreeNode> dictionary = new Dictionary<string, ResourceTreeNode>();
		using (XmlReader xmlReader = XmlReader.Create(new StringReader(file.text)))
		{
			while (xmlReader.ReadToFollowing("node"))
			{
				xmlReader.MoveToFirstAttribute();
				string value = xmlReader.Value;
				float num = 0f;
				float num2 = 0f;
				float num3 = 40f;
				float num4 = 20f;
				if (xmlReader.ReadToFollowing("Geometry"))
				{
					xmlReader.MoveToAttribute("x");
					num = float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("y");
					num2 = -float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("width");
					num3 = float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("height");
					num4 = float.Parse(xmlReader.Value);
				}
				global::Debug.Assert(num3 != 0f && num4 != 0f, "Error parsing GRAPHML");
				if (xmlReader.ReadToFollowing("NodeLabel"))
				{
					string text = xmlReader.ReadString();
					T t = new T();
					t.Id = text;
					t.Name = text;
					t.nodeX = num;
					t.nodeY = num2;
					t.width = num3;
					t.height = num4;
					dictionary[value] = t;
					this.resources.Add(t);
				}
			}
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(file.text);
		foreach (object obj in xmlDocument.DocumentElement.SelectNodes("/graphml/graph/edge"))
		{
			XmlNode xmlNode = (XmlNode)obj;
			ResourceTreeNode resourceTreeNode = null;
			dictionary.TryGetValue(xmlNode.Attributes["source"].Value, out resourceTreeNode);
			ResourceTreeNode resourceTreeNode2 = null;
			dictionary.TryGetValue(xmlNode.Attributes["target"].Value, out resourceTreeNode2);
			if (resourceTreeNode != null && resourceTreeNode2 != null)
			{
				resourceTreeNode.references.Add(resourceTreeNode2);
				ResourceTreeNode.Edge edge = null;
				XmlNode xmlNode2 = null;
				foreach (object obj2 in xmlNode.ChildNodes)
				{
					XmlNode xmlNode3 = (XmlNode)obj2;
					if (xmlNode3.HasChildNodes)
					{
						xmlNode2 = xmlNode3.FirstChild;
						break;
					}
				}
				string name = xmlNode2.Name;
				ResourceTreeNode.Edge.EdgeType edgeType = (ResourceTreeNode.Edge.EdgeType)Enum.Parse(typeof(ResourceTreeNode.Edge.EdgeType), name);
				edge = new ResourceTreeNode.Edge(resourceTreeNode, resourceTreeNode2, edgeType);
				foreach (object obj3 in xmlNode2.ChildNodes)
				{
					XmlNode xmlNode4 = (XmlNode)obj3;
					if (!(xmlNode4.Name != "Path"))
					{
						edge.sourceOffset = new Vector2f(float.Parse(xmlNode4.Attributes["sx"].Value), -float.Parse(xmlNode4.Attributes["sy"].Value));
						edge.targetOffset = new Vector2f(float.Parse(xmlNode4.Attributes["tx"].Value), -float.Parse(xmlNode4.Attributes["ty"].Value));
						using (IEnumerator enumerator3 = xmlNode4.ChildNodes.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								object obj4 = enumerator3.Current;
								XmlNode xmlNode5 = (XmlNode)obj4;
								Vector2f vector2f = new Vector2f(float.Parse(xmlNode5.Attributes["x"].Value), -float.Parse(xmlNode5.Attributes["y"].Value));
								edge.AddToPath(vector2f);
							}
							break;
						}
					}
				}
				resourceTreeNode.edges.Add(edge);
			}
		}
	}
}
