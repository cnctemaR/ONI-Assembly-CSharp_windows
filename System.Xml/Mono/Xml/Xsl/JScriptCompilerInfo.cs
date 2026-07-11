using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Xml;

namespace Mono.Xml.Xsl
{
	internal class JScriptCompilerInfo : ScriptCompilerInfo
	{
		public JScriptCompilerInfo()
		{
			this.CompilerCommand = "mjs";
			this.DefaultCompilerOptions = "/t:library /r:Microsoft.VisualBasic.dll";
		}

		public override CodeDomProvider CodeDomProvider
		{
			get
			{
				if (JScriptCompilerInfo.providerType == null)
				{
					Assembly assembly = Assembly.LoadWithPartialName("Microsoft.JScript", null);
					if (assembly != null)
					{
						JScriptCompilerInfo.providerType = assembly.GetType("Microsoft.JScript.JScriptCodeProvider");
					}
				}
				return (CodeDomProvider)Activator.CreateInstance(JScriptCompilerInfo.providerType);
			}
		}

		public override string Extension
		{
			get
			{
				return ".js";
			}
		}

		public override string SourceTemplate
		{
			get
			{
				return "// This file is automatically created by Mono managed XSLT engine.\n// Created time: {0}\nimport System;\nimport System.Collections;\nimport System.Text;\nimport System.Text.RegularExpressions;\nimport System.Xml;\nimport System.Xml.XPath;\nimport System.Xml.Xsl;\nimport Microsoft.VisualBasic;\n\npackage GeneratedAssembly\n{\nclass Script{1} {\n\t{2}\n}\n}\n";
			}
		}

		public override string FormatSource(IXmlLineInfo li, string file, string source)
		{
			return source;
		}

		private static Type providerType;
	}
}
