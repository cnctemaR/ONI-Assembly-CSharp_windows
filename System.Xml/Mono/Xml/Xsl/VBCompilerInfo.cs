using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic;

namespace Mono.Xml.Xsl
{
	internal class VBCompilerInfo : ScriptCompilerInfo
	{
		public VBCompilerInfo()
		{
			this.CompilerCommand = "mbas";
			this.DefaultCompilerOptions = "/t:library";
		}

		public override CodeDomProvider CodeDomProvider
		{
			get
			{
				return new VBCodeProvider();
			}
		}

		public override string Extension
		{
			get
			{
				return ".vb";
			}
		}

		public override string SourceTemplate
		{
			get
			{
				return "' This file is automatically created by Mono managed XSLT engine.\n' Created time: {0}\nimports System\nimports System.Collections\nimports System.Text\nimports System.Text.RegularExpressions\nimports System.Xml\nimports System.Xml.XPath\nimports System.Xml.Xsl\nimports Microsoft.VisualBasic\n\nnamespace GeneratedAssembly\npublic Class Script{1}\n\t{2}\nend Class\nend namespace\n";
			}
		}

		public override string FormatSource(IXmlLineInfo li, string file, string source)
		{
			if (li == null)
			{
				return source;
			}
			return string.Format(CultureInfo.InvariantCulture, "#ExternalSource (\"{1}\", {0})\n{2}\n#end ExternalSource", new object[]
			{
				li.LineNumber,
				new FileInfo(file).Name,
				source
			});
		}
	}
}
