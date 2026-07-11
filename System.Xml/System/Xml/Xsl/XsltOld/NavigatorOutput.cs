using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class NavigatorOutput : RecordOutput
	{
		internal XPathNavigator Navigator
		{
			get
			{
				return ((IXPathNavigable)this.doc).CreateNavigator();
			}
		}

		internal NavigatorOutput(string baseUri)
		{
			this.doc = new XPathDocument();
			this.wr = this.doc.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames, baseUri);
		}

		public Processor.OutputResult RecordDone(RecordBuilder record)
		{
			BuilderInfo mainNode = record.MainNode;
			this.documentIndex++;
			switch (mainNode.NodeType)
			{
			case XmlNodeType.Element:
			{
				this.wr.WriteStartElement(mainNode.Prefix, mainNode.LocalName, mainNode.NamespaceURI);
				for (int i = 0; i < record.AttributeCount; i++)
				{
					this.documentIndex++;
					BuilderInfo builderInfo = (BuilderInfo)record.AttributeList[i];
					if (builderInfo.NamespaceURI == "http://www.w3.org/2000/xmlns/")
					{
						if (builderInfo.Prefix.Length == 0)
						{
							this.wr.WriteNamespaceDeclaration(string.Empty, builderInfo.Value);
						}
						else
						{
							this.wr.WriteNamespaceDeclaration(builderInfo.LocalName, builderInfo.Value);
						}
					}
					else
					{
						this.wr.WriteAttributeString(builderInfo.Prefix, builderInfo.LocalName, builderInfo.NamespaceURI, builderInfo.Value);
					}
				}
				this.wr.StartElementContent();
				if (mainNode.IsEmptyTag)
				{
					this.wr.WriteEndElement(mainNode.Prefix, mainNode.LocalName, mainNode.NamespaceURI);
				}
				break;
			}
			case XmlNodeType.Text:
				this.wr.WriteString(mainNode.Value);
				break;
			case XmlNodeType.ProcessingInstruction:
				this.wr.WriteProcessingInstruction(mainNode.LocalName, mainNode.Value);
				break;
			case XmlNodeType.Comment:
				this.wr.WriteComment(mainNode.Value);
				break;
			case XmlNodeType.SignificantWhitespace:
				this.wr.WriteString(mainNode.Value);
				break;
			case XmlNodeType.EndElement:
				this.wr.WriteEndElement(mainNode.Prefix, mainNode.LocalName, mainNode.NamespaceURI);
				break;
			}
			record.Reset();
			return Processor.OutputResult.Continue;
		}

		public void TheEnd()
		{
			this.wr.Close();
		}

		private XPathDocument doc;

		private int documentIndex;

		private XmlRawWriter wr;
	}
}
