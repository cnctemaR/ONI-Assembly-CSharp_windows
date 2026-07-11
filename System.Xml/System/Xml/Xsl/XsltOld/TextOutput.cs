using System;
using System.IO;

namespace System.Xml.Xsl.XsltOld
{
	internal class TextOutput : SequentialOutput
	{
		internal TextOutput(Processor processor, Stream stream)
			: base(processor)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.encoding = processor.Output.Encoding;
			this.writer = new StreamWriter(stream, this.encoding);
		}

		internal TextOutput(Processor processor, TextWriter writer)
			: base(processor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.encoding = writer.Encoding;
			this.writer = writer;
		}

		internal override void Write(char outputChar)
		{
			this.writer.Write(outputChar);
		}

		internal override void Write(string outputText)
		{
			this.writer.Write(outputText);
		}

		internal override void Close()
		{
			this.writer.Flush();
			this.writer = null;
		}

		private TextWriter writer;
	}
}
