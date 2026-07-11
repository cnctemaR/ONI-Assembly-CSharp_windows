using System;
using System.Collections;

namespace System.Xml.Xsl.XsltOld
{
	internal class WriterOutput : RecordOutput
	{
		internal WriterOutput(Processor processor, XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.processor = processor;
		}

		public Processor.OutputResult RecordDone(RecordBuilder record)
		{
			BuilderInfo mainNode = record.MainNode;
			switch (mainNode.NodeType)
			{
			case XmlNodeType.Element:
				this.writer.WriteStartElement(mainNode.Prefix, mainNode.LocalName, mainNode.NamespaceURI);
				this.WriteAttributes(record.AttributeList, record.AttributeCount);
				if (mainNode.IsEmptyTag)
				{
					this.writer.WriteEndElement();
				}
				break;
			case XmlNodeType.Text:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.writer.WriteString(mainNode.Value);
				break;
			case XmlNodeType.CDATA:
				this.writer.WriteCData(mainNode.Value);
				break;
			case XmlNodeType.EntityReference:
				this.writer.WriteEntityRef(mainNode.LocalName);
				break;
			case XmlNodeType.ProcessingInstruction:
				this.writer.WriteProcessingInstruction(mainNode.LocalName, mainNode.Value);
				break;
			case XmlNodeType.Comment:
				this.writer.WriteComment(mainNode.Value);
				break;
			case XmlNodeType.DocumentType:
				this.writer.WriteRaw(mainNode.Value);
				break;
			case XmlNodeType.EndElement:
				this.writer.WriteFullEndElement();
				break;
			}
			record.Reset();
			return Processor.OutputResult.Continue;
		}

		public void TheEnd()
		{
			this.writer.Flush();
			this.writer = null;
		}

		private void WriteAttributes(ArrayList list, int count)
		{
			for (int i = 0; i < count; i++)
			{
				BuilderInfo builderInfo = (BuilderInfo)list[i];
				this.writer.WriteAttributeString(builderInfo.Prefix, builderInfo.LocalName, builderInfo.NamespaceURI, builderInfo.Value);
			}
		}

		private XmlWriter writer;

		private Processor processor;
	}
}
