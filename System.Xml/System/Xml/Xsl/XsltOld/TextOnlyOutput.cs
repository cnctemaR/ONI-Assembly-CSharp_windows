using System;
using System.IO;

namespace System.Xml.Xsl.XsltOld
{
	internal class TextOnlyOutput : RecordOutput
	{
		internal XsltOutput Output
		{
			get
			{
				return this.processor.Output;
			}
		}

		public TextWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		internal TextOnlyOutput(Processor processor, Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.processor = processor;
			this.writer = new StreamWriter(stream, this.Output.Encoding);
		}

		internal TextOnlyOutput(Processor processor, TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.processor = processor;
			this.writer = writer;
		}

		public Processor.OutputResult RecordDone(RecordBuilder record)
		{
			BuilderInfo mainNode = record.MainNode;
			XmlNodeType nodeType = mainNode.NodeType;
			if (nodeType == XmlNodeType.Text || nodeType - XmlNodeType.Whitespace <= 1)
			{
				this.writer.Write(mainNode.Value);
			}
			record.Reset();
			return Processor.OutputResult.Continue;
		}

		public void TheEnd()
		{
			this.writer.Flush();
		}

		private Processor processor;

		private TextWriter writer;
	}
}
