using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace System.Xml.Xsl.Xslt
{
	internal class ScriptClass
	{
		public ScriptClass(string ns, CompilerInfo compilerInfo)
		{
			this.ns = ns;
			this.compilerInfo = compilerInfo;
			this.refAssemblies = new StringCollection();
			this.nsImports = new StringCollection();
			this.typeDecl = new CodeTypeDeclaration(ScriptClass.GenerateUniqueClassName());
			this.refAssembliesByHref = false;
			this.scriptUris = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		private static string GenerateUniqueClassName()
		{
			return "Script" + Interlocked.Increment(ref ScriptClass.scriptClassCounter).ToString();
		}

		public void AddScriptBlock(string source, string uriString, int lineNumber, Location end)
		{
			CodeSnippetTypeMember codeSnippetTypeMember = new CodeSnippetTypeMember(source);
			string fileName = SourceLineInfo.GetFileName(uriString);
			if (lineNumber > 0)
			{
				codeSnippetTypeMember.LinePragma = new CodeLinePragma(fileName, lineNumber);
				this.scriptUris[fileName] = uriString;
			}
			this.typeDecl.Members.Add(codeSnippetTypeMember);
			this.endUri = uriString;
			this.endLoc = end;
		}

		public ISourceLineInfo EndLineInfo
		{
			get
			{
				return new SourceLineInfo(this.endUri, this.endLoc, this.endLoc);
			}
		}

		public string ns;

		public CompilerInfo compilerInfo;

		public StringCollection refAssemblies;

		public StringCollection nsImports;

		public CodeTypeDeclaration typeDecl;

		public bool refAssembliesByHref;

		public Dictionary<string, string> scriptUris;

		public string endUri;

		public Location endLoc;

		private static long scriptClassCounter;
	}
}
