using System;
using System.Xml;

namespace Unity.VectorGraphics
{
	internal class XmlReaderIterator
	{
		public XmlReaderIterator(XmlReader reader)
		{
			this.reader = reader;
		}

		public bool GoToRoot(string tagName)
		{
			return this.reader.ReadToFollowing(tagName) && this.reader.Depth == 0;
		}

		public XmlReaderIterator.Node VisitCurrent()
		{
			this.currentElementVisited = true;
			return new XmlReaderIterator.Node(this.reader);
		}

		public bool IsEmptyElement()
		{
			return this.reader.IsEmptyElement;
		}

		public bool GoToNextChild(XmlReaderIterator.Node node)
		{
			bool flag = !this.currentElementVisited;
			bool flag2;
			if (flag)
			{
				flag2 = this.reader.Depth == node.Depth + 1;
			}
			else
			{
				this.reader.Read();
				while (this.reader.NodeType != XmlNodeType.None && this.reader.NodeType != XmlNodeType.Element)
				{
					this.reader.Read();
				}
				bool flag3 = this.reader.NodeType != XmlNodeType.Element;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					this.currentElementVisited = false;
					flag2 = this.reader.Depth == node.Depth + 1;
				}
			}
			return flag2;
		}

		public void SkipCurrentChildTree(XmlReaderIterator.Node node)
		{
			while (this.GoToNextChild(node))
			{
				this.SkipCurrentChildTree(this.VisitCurrent());
			}
		}

		public string ReadTextWithinElement()
		{
			bool isEmptyElement = this.reader.IsEmptyElement;
			string text;
			if (isEmptyElement)
			{
				text = "";
			}
			else
			{
				string text2 = "";
				while (this.reader.Read() && this.reader.NodeType != XmlNodeType.EndElement)
				{
					text2 += this.reader.Value;
				}
				text = text2;
			}
			return text;
		}

		private XmlReader reader;

		private bool currentElementVisited;

		internal class Node
		{
			public Node(XmlReader reader)
			{
				this.reader = reader;
				this.name = reader.Name;
				this.depth = reader.Depth;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string this[string attrib]
			{
				get
				{
					return this.reader.GetAttribute(attrib);
				}
			}

			public SVGPropertySheet GetAttributes()
			{
				SVGPropertySheet svgpropertySheet = new SVGPropertySheet();
				for (int i = 0; i < this.reader.AttributeCount; i++)
				{
					this.reader.MoveToAttribute(i);
					svgpropertySheet[this.reader.Name] = this.reader.Value;
				}
				this.reader.MoveToElement();
				return svgpropertySheet;
			}

			public SVGFormatException GetException(string message)
			{
				return new SVGFormatException(this.reader, message);
			}

			public SVGFormatException GetUnsupportedAttribValException(string attrib)
			{
				return new SVGFormatException(this.reader, string.Concat(new string[]
				{
					"Value '",
					this[attrib],
					"' is invalid for attribute '",
					attrib,
					"'"
				}));
			}

			public int Depth
			{
				get
				{
					return this.depth;
				}
			}

			private XmlReader reader;

			private int depth;

			private string name;
		}
	}
}
