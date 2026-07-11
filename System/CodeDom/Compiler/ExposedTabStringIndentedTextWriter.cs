using System;
using System.IO;

namespace System.CodeDom.Compiler
{
	internal sealed class ExposedTabStringIndentedTextWriter : IndentedTextWriter
	{
		public ExposedTabStringIndentedTextWriter(TextWriter writer, string tabString)
			: base(writer, tabString)
		{
			this.TabString = tabString ?? "    ";
		}

		internal void InternalOutputTabs()
		{
			TextWriter innerWriter = base.InnerWriter;
			for (int i = 0; i < base.Indent; i++)
			{
				innerWriter.Write(this.TabString);
			}
		}

		internal string TabString { get; }
	}
}
