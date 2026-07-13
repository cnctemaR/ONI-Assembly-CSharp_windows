using System;
using System.Collections.Generic;

namespace Unity.VectorGraphics
{
	internal class SVGStyleResolver
	{
		public void PushNode(XmlReaderIterator.Node node)
		{
			SVGStyleResolver.NodeData nodeData = default(SVGStyleResolver.NodeData);
			nodeData.node = node;
			nodeData.name = node.Name;
			string text = node["class"];
			bool flag = text != null;
			if (flag)
			{
				nodeData.classes = new List<string>();
				foreach (string text2 in text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
				{
					string text3 = text2.Trim();
					bool flag2 = !string.IsNullOrEmpty(text3);
					if (flag2)
					{
						nodeData.classes.Add(text3);
					}
				}
			}
			else
			{
				nodeData.classes = new List<string>();
			}
			List<string> list = new List<string>();
			foreach (string text4 in this.SortedClasses(nodeData.classes))
			{
				list.Add(text4);
			}
			nodeData.classes = list;
			nodeData.id = node["id"];
			SVGStyleResolver.StyleLayer styleLayer = new SVGStyleResolver.StyleLayer();
			styleLayer.nodeData = nodeData;
			styleLayer.attributeSheet = node.GetAttributes();
			styleLayer.styleSheet = new SVGStyleSheet();
			string text5 = node["style"];
			bool flag3 = text5 != null;
			if (flag3)
			{
				SVGPropertySheet svgpropertySheet = SVGStyleSheetUtils.ParseInline(text5);
				styleLayer.styleSheet[node.Name] = svgpropertySheet;
			}
			this.PushLayer(styleLayer);
		}

		public void PopNode()
		{
			this.PopLayer();
		}

		public void PushLayer(SVGStyleResolver.StyleLayer layer)
		{
			this.layers.Add(layer);
		}

		public void PopLayer()
		{
			bool flag = this.layers.Count == 0;
			if (flag)
			{
				throw SVGFormatException.StackError;
			}
			this.layers.RemoveAt(this.layers.Count - 1);
		}

		public SVGStyleResolver.StyleLayer PeekLayer()
		{
			bool flag = this.layers.Count == 0;
			SVGStyleResolver.StyleLayer styleLayer;
			if (flag)
			{
				styleLayer = null;
			}
			else
			{
				styleLayer = this.layers[this.layers.Count - 1];
			}
			return styleLayer;
		}

		public void SaveLayerForSceneNode(SceneNode node)
		{
			this.nodeLayers[node] = this.PeekLayer();
		}

		public SVGStyleResolver.StyleLayer GetLayerForScenNode(SceneNode node)
		{
			bool flag = !this.nodeLayers.ContainsKey(node);
			SVGStyleResolver.StyleLayer styleLayer;
			if (flag)
			{
				styleLayer = null;
			}
			else
			{
				styleLayer = this.nodeLayers[node];
			}
			return styleLayer;
		}

		public void SetGlobalStyleSheet(SVGStyleSheet sheet)
		{
			foreach (string text in sheet.selectors)
			{
				this.globalStyleSheet[text] = sheet[text];
			}
		}

		public string Evaluate(string attribName, Inheritance inheritance = Inheritance.None)
		{
			for (int i = this.layers.Count - 1; i >= 0; i--)
			{
				string text = null;
				bool flag = this.LookupStyleOrAttribute(this.layers[i], attribName, inheritance, out text);
				if (flag)
				{
					return text;
				}
				bool flag2 = inheritance == Inheritance.None;
				if (flag2)
				{
					break;
				}
			}
			return null;
		}

		private bool LookupStyleOrAttribute(SVGStyleResolver.StyleLayer layer, string attribName, Inheritance inheritance, out string attrib)
		{
			bool flag = this.LookupProperty(layer.nodeData, attribName, layer.styleSheet, out attrib);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = this.LookupProperty(layer.nodeData, attribName, this.globalStyleSheet, out attrib);
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = layer.attributeSheet.ContainsKey(attribName);
					if (flag4)
					{
						attrib = layer.attributeSheet[attribName];
						flag2 = true;
					}
					else
					{
						flag2 = false;
					}
				}
			}
			return flag2;
		}

		private bool LookupProperty(SVGStyleResolver.NodeData nodeData, string attribName, SVGStyleSheet sheet, out string val)
		{
			string text = (string.IsNullOrEmpty(nodeData.id) ? null : ("#" + nodeData.id));
			string text2 = (string.IsNullOrEmpty(nodeData.name) ? null : nodeData.name);
			bool flag = this.LookupPropertyInSheet(sheet, attribName, text, out val);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				foreach (string text3 in nodeData.classes)
				{
					string text4 = "." + text3;
					bool flag3 = this.LookupPropertyInSheet(sheet, attribName, text4, out val);
					if (flag3)
					{
						return true;
					}
				}
				bool flag4 = this.LookupPropertyInSheet(sheet, attribName, text2, out val);
				if (flag4)
				{
					flag2 = true;
				}
				else
				{
					bool flag5 = this.LookupPropertyInSheet(sheet, attribName, "*", out val);
					if (flag5)
					{
						flag2 = true;
					}
					else
					{
						val = null;
						flag2 = false;
					}
				}
			}
			return flag2;
		}

		private bool LookupPropertyInSheet(SVGStyleSheet sheet, string attribName, string selector, out string val)
		{
			bool flag = selector == null;
			bool flag2;
			if (flag)
			{
				val = null;
				flag2 = false;
			}
			else
			{
				string text = "";
				foreach (string text2 in sheet.selectors)
				{
					bool flag3 = false;
					string[] array = text2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string text3 in array)
					{
						bool flag4 = text3 == selector;
						if (flag4)
						{
							flag3 = true;
							break;
						}
					}
					bool flag5 = flag3;
					if (flag5)
					{
						bool flag6 = array.Length == 1;
						if (flag6)
						{
							text = array[0];
							break;
						}
						bool flag7 = array.Length > 1;
						if (flag7)
						{
							bool flag8 = this.MatchesDescendants(array, array.Length - 1, -1);
							if (flag8)
							{
								text = text2;
								break;
							}
						}
					}
				}
				bool flag9 = !string.IsNullOrEmpty(text);
				if (flag9)
				{
					SVGPropertySheet svgpropertySheet = sheet[text];
					bool flag10 = svgpropertySheet.ContainsKey(attribName);
					if (flag10)
					{
						val = svgpropertySheet[attribName];
						return true;
					}
				}
				val = null;
				flag2 = false;
			}
			return flag2;
		}

		private bool MatchesDescendants(string[] selectorParts, int partIndexToMatch, int layerIndex = -1)
		{
			bool flag = selectorParts.Length == 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = partIndexToMatch < 0;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = layerIndex < 0;
					if (flag4)
					{
						layerIndex = this.layers.Count - 1;
					}
					string text = selectorParts[partIndexToMatch];
					for (int i = layerIndex; i >= 0; i--)
					{
						SVGStyleResolver.StyleLayer styleLayer = this.layers[i];
						SVGStyleResolver.NodeData nodeData = styleLayer.nodeData;
						bool flag5 = text == nodeData.name;
						bool flag6 = text == "#" + nodeData.id;
						bool flag7 = nodeData.classes != null && nodeData.classes.Contains(text.StartsWith(".") ? text.Substring(1) : text);
						bool flag8 = flag5 || flag6 || flag7;
						if (flag8)
						{
							return this.MatchesDescendants(selectorParts, partIndexToMatch - 1, i - 1);
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		private IEnumerable<string> SortedClasses(List<string> classes)
		{
			int selectorCount = 0;
			foreach (string s in this.globalStyleSheet.selectors)
			{
				int num = selectorCount + 1;
				selectorCount = num;
			}
			IEnumerator<string> enumerator = null;
			bool flag = selectorCount == 0;
			if (flag)
			{
				foreach (string klass in classes)
				{
					yield return klass;
					klass = null;
				}
				List<string>.Enumerator enumerator2 = default(List<string>.Enumerator);
			}
			List<string> reversedSelectors = new List<string>(this.globalStyleSheet.selectors);
			reversedSelectors.Reverse();
			foreach (string sel in reversedSelectors)
			{
				string[] parts = sel.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string part in parts)
				{
					bool flag2 = part[0] != '.';
					if (!flag2)
					{
						string klass2 = part.Substring(1);
						bool flag3 = classes.Contains(klass2);
						if (flag3)
						{
							yield return klass2;
						}
						klass2 = null;
						part = null;
					}
				}
				string[] array = null;
				parts = null;
				sel = null;
			}
			List<string>.Enumerator enumerator3 = default(List<string>.Enumerator);
			yield break;
			yield break;
		}

		private List<SVGStyleResolver.StyleLayer> layers = new List<SVGStyleResolver.StyleLayer>();

		private SVGStyleSheet globalStyleSheet = new SVGStyleSheet();

		private Dictionary<SceneNode, SVGStyleResolver.StyleLayer> nodeLayers = new Dictionary<SceneNode, SVGStyleResolver.StyleLayer>();

		public struct NodeData
		{
			public XmlReaderIterator.Node node;

			public string name;

			public List<string> classes;

			public string id;
		}

		public class StyleLayer
		{
			public SVGStyleSheet styleSheet;

			public SVGPropertySheet attributeSheet;

			public SVGStyleResolver.NodeData nodeData;
		}
	}
}
