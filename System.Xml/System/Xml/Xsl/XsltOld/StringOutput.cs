using System;
using System.Text;

namespace System.Xml.Xsl.XsltOld
{
	internal class StringOutput : SequentialOutput
	{
		internal string Result
		{
			get
			{
				return this.result;
			}
		}

		internal StringOutput(Processor processor)
			: base(processor)
		{
			this.builder = new StringBuilder();
		}

		internal override void Write(char outputChar)
		{
			this.builder.Append(outputChar);
		}

		internal override void Write(string outputText)
		{
			this.builder.Append(outputText);
		}

		internal override void Close()
		{
			this.result = this.builder.ToString();
		}

		private StringBuilder builder;

		private string result;
	}
}
