using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal abstract class ScriptCompilerInfo
	{
		public virtual string CompilerCommand
		{
			get
			{
				return this.compilerCommand;
			}
			set
			{
				this.compilerCommand = value;
			}
		}

		public virtual string DefaultCompilerOptions
		{
			get
			{
				return this.defaultCompilerOptions;
			}
			set
			{
				this.defaultCompilerOptions = value;
			}
		}

		public abstract CodeDomProvider CodeDomProvider { get; }

		public abstract string Extension { get; }

		public abstract string SourceTemplate { get; }

		public abstract string FormatSource(IXmlLineInfo li, string file, string code);

		public virtual string GetCompilerArguments(string targetFileName)
		{
			return this.DefaultCompilerOptions + " " + targetFileName;
		}

		public virtual Type GetScriptClass(string code, string classSuffix, XPathNavigator scriptNode, Evidence evidence)
		{
			PermissionSet permissionSet = SecurityManager.ResolvePolicy(evidence);
			if (permissionSet != null)
			{
				permissionSet.Demand();
			}
			string text = "Script" + classSuffix;
			string text2 = "GeneratedAssembly." + text;
			try
			{
				Type type = Type.GetType(text2);
				if (type != null)
				{
					return type;
				}
			}
			catch
			{
			}
			try
			{
				Type type2 = Assembly.LoadFrom(text + ".dll").GetType(text2);
				if (type2 != null)
				{
					return type2;
				}
			}
			catch
			{
			}
			ICodeCompiler codeCompiler = this.CodeDomProvider.CreateCompiler();
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.CompilerOptions = this.DefaultCompilerOptions;
			string text3 = string.Empty;
			try
			{
				if (scriptNode.BaseURI != string.Empty)
				{
					text3 = new Uri(scriptNode.BaseURI).LocalPath;
				}
			}
			catch (FormatException)
			{
			}
			if (text3 == string.Empty)
			{
				text3 = "__baseURI_not_supplied__";
			}
			IXmlLineInfo xmlLineInfo = scriptNode as IXmlLineInfo;
			string text4 = this.SourceTemplate.Replace("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture)).Replace("{1}", classSuffix).Replace("{2}", code);
			text4 = this.FormatSource(xmlLineInfo, text3, text4);
			CompilerResults compilerResults = codeCompiler.CompileAssemblyFromSource(compilerParameters, text4);
			foreach (object obj in compilerResults.Errors)
			{
				CompilerError compilerError = (CompilerError)obj;
				if (!compilerError.IsWarning)
				{
					throw new XsltException("Stylesheet script compile error: \n" + this.FormatErrorMessage(compilerResults), null, scriptNode);
				}
			}
			if (compilerResults.CompiledAssembly == null)
			{
				throw new XsltCompileException("Cannot compile stylesheet script", null, scriptNode);
			}
			return compilerResults.CompiledAssembly.GetType(text2);
		}

		private string FormatErrorMessage(CompilerResults res)
		{
			string text = string.Empty;
			foreach (object obj in res.Errors)
			{
				CompilerError compilerError = (CompilerError)obj;
				object[] array = new object[]
				{
					"\n",
					compilerError.FileName,
					(compilerError.Line <= 0) ? string.Empty : (" line " + compilerError.Line),
					(!compilerError.IsWarning) ? " ERROR: " : " WARNING: ",
					compilerError.ErrorNumber,
					": ",
					compilerError.ErrorText
				};
				text += string.Concat(array);
			}
			return text;
		}

		private string compilerCommand;

		private string defaultCompilerOptions;
	}
}
