using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Xml.Xsl.Runtime;
using Microsoft.VisualBasic;

namespace System.Xml.Xsl.Xslt
{
	internal class Scripts
	{
		public Scripts(Compiler compiler)
		{
			this.compiler = compiler;
		}

		public Dictionary<string, Type> ScriptClasses
		{
			get
			{
				return this.nsToType;
			}
		}

		public XmlExtensionFunction ResolveFunction(string name, string ns, int numArgs, IErrorHelper errorHelper)
		{
			Type type;
			if (this.nsToType.TryGetValue(ns, out type))
			{
				try
				{
					return this.extFuncs.Bind(name, ns, numArgs, type, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
				}
				catch (XslTransformException ex)
				{
					errorHelper.ReportError(ex.Message, Array.Empty<string>());
				}
			}
			return null;
		}

		public ScriptClass GetScriptClass(string ns, string language, IErrorHelper errorHelper)
		{
			CompilerInfo compilerInfo;
			try
			{
				compilerInfo = CodeDomProvider.GetCompilerInfo(language);
			}
			catch (ConfigurationException)
			{
				errorHelper.ReportError("Scripting language '{0}' is not supported.", new string[] { language });
				return null;
			}
			foreach (ScriptClass scriptClass in this.scriptClasses)
			{
				if (ns == scriptClass.ns)
				{
					if (compilerInfo != scriptClass.compilerInfo)
					{
						errorHelper.ReportError("All script blocks implementing the namespace '{0}' must use the same language.", new string[] { ns });
						return null;
					}
					return scriptClass;
				}
			}
			ScriptClass scriptClass2 = new ScriptClass(ns, compilerInfo);
			scriptClass2.typeDecl.TypeAttributes = TypeAttributes.Public;
			this.scriptClasses.Add(scriptClass2);
			return scriptClass2;
		}

		public void CompileScripts()
		{
			List<ScriptClass> list = new List<ScriptClass>();
			for (int i = 0; i < this.scriptClasses.Count; i++)
			{
				if (this.scriptClasses[i] != null)
				{
					CompilerInfo compilerInfo = this.scriptClasses[i].compilerInfo;
					list.Clear();
					for (int j = i; j < this.scriptClasses.Count; j++)
					{
						if (this.scriptClasses[j] != null && this.scriptClasses[j].compilerInfo == compilerInfo)
						{
							list.Add(this.scriptClasses[j]);
							this.scriptClasses[j] = null;
						}
					}
					Assembly assembly = this.CompileAssembly(list);
					if (assembly != null)
					{
						foreach (ScriptClass scriptClass in list)
						{
							Type type = assembly.GetType("System.Xml.Xsl.CompiledQuery" + Type.Delimiter.ToString() + scriptClass.typeDecl.Name);
							if (type != null)
							{
								this.nsToType.Add(scriptClass.ns, type);
							}
						}
					}
				}
			}
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		private Assembly CompileAssembly(List<ScriptClass> scriptsForLang)
		{
			TempFileCollection tempFiles = this.compiler.CompilerResults.TempFiles;
			CompilerErrorCollection errors = this.compiler.CompilerResults.Errors;
			ScriptClass scriptClass = scriptsForLang[scriptsForLang.Count - 1];
			bool flag = false;
			CodeDomProvider codeDomProvider;
			try
			{
				codeDomProvider = scriptClass.compilerInfo.CreateProvider();
			}
			catch (ConfigurationException ex)
			{
				errors.Add(this.compiler.CreateError(scriptClass.EndLineInfo, "Error occurred while compiling the script: {0}", new string[] { ex.Message }));
				return null;
			}
			flag = codeDomProvider is VBCodeProvider;
			CodeCompileUnit[] array = new CodeCompileUnit[scriptsForLang.Count];
			CompilerParameters compilerParameters = scriptClass.compilerInfo.CreateDefaultCompilerParameters();
			compilerParameters.ReferencedAssemblies.Add(typeof(Res).Assembly.Location);
			compilerParameters.ReferencedAssemblies.Add("System.dll");
			if (flag)
			{
				compilerParameters.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
			}
			bool flag2 = false;
			for (int i = 0; i < scriptsForLang.Count; i++)
			{
				ScriptClass scriptClass2 = scriptsForLang[i];
				CodeNamespace codeNamespace = new CodeNamespace("System.Xml.Xsl.CompiledQuery");
				foreach (string text in Scripts.defaultNamespaces)
				{
					codeNamespace.Imports.Add(new CodeNamespaceImport(text));
				}
				if (flag)
				{
					codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.VisualBasic"));
				}
				foreach (string text2 in scriptClass2.nsImports)
				{
					codeNamespace.Imports.Add(new CodeNamespaceImport(text2));
				}
				codeNamespace.Types.Add(scriptClass2.typeDecl);
				CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
				codeCompileUnit.Namespaces.Add(codeNamespace);
				if (flag)
				{
					codeCompileUnit.UserData["AllowLateBound"] = true;
					codeCompileUnit.UserData["RequireVariableDeclaration"] = false;
				}
				if (i == 0)
				{
					codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Security.SecurityTransparentAttribute"));
					codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SecurityRulesAttribute)), new CodeAttributeArgument[]
					{
						new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityRuleSet)), "Level1"))
					}));
				}
				array[i] = codeCompileUnit;
				foreach (string text3 in scriptClass2.refAssemblies)
				{
					compilerParameters.ReferencedAssemblies.Add(text3);
				}
				flag2 |= scriptClass2.refAssembliesByHref;
			}
			XsltSettings settings = this.compiler.Settings;
			compilerParameters.WarningLevel = ((settings.WarningLevel >= 0) ? settings.WarningLevel : compilerParameters.WarningLevel);
			compilerParameters.TreatWarningsAsErrors = settings.TreatWarningsAsErrors;
			compilerParameters.IncludeDebugInformation = this.compiler.IsDebug;
			string text4 = this.compiler.ScriptAssemblyPath;
			if (text4 != null && scriptsForLang.Count < this.scriptClasses.Count)
			{
				text4 = Path.ChangeExtension(text4, "." + this.GetLanguageName(scriptClass.compilerInfo) + Path.GetExtension(text4));
			}
			compilerParameters.OutputAssembly = text4;
			string text5 = ((settings.TempFiles != null) ? settings.TempFiles.TempDir : null);
			compilerParameters.TempFiles = new TempFileCollection(text5);
			bool flag3 = this.compiler.IsDebug && text4 == null;
			flag3 = flag3 && !settings.CheckOnly;
			compilerParameters.TempFiles.KeepFiles = flag3;
			compilerParameters.GenerateInMemory = (text4 == null && !this.compiler.IsDebug && !flag2) || settings.CheckOnly;
			CompilerResults compilerResults;
			try
			{
				compilerResults = codeDomProvider.CompileAssemblyFromDom(compilerParameters, array);
			}
			catch (ExternalException ex2)
			{
				compilerResults = new CompilerResults(compilerParameters.TempFiles);
				compilerResults.Errors.Add(this.compiler.CreateError(scriptClass.EndLineInfo, "Error occurred while compiling the script: {0}", new string[] { ex2.Message }));
			}
			if (!settings.CheckOnly)
			{
				foreach (object obj in compilerResults.TempFiles)
				{
					string text6 = (string)obj;
					tempFiles.AddFile(text6, tempFiles.KeepFiles);
				}
			}
			foreach (object obj2 in compilerResults.Errors)
			{
				CompilerError compilerError = (CompilerError)obj2;
				Scripts.FixErrorPosition(compilerError, scriptsForLang);
				this.compiler.AddModule(compilerError.FileName);
			}
			errors.AddRange(compilerResults.Errors);
			if (!compilerResults.Errors.HasErrors)
			{
				return compilerResults.CompiledAssembly;
			}
			return null;
		}

		private string GetLanguageName(CompilerInfo compilerInfo)
		{
			Regex regex = new Regex("^[0-9a-zA-Z]+$");
			foreach (string text in compilerInfo.GetLanguages())
			{
				if (regex.IsMatch(text))
				{
					return text;
				}
			}
			string text2 = "script";
			int i = this.assemblyCounter + 1;
			this.assemblyCounter = i;
			return text2 + i.ToString(CultureInfo.InvariantCulture);
		}

		private static void FixErrorPosition(CompilerError error, List<ScriptClass> scriptsForLang)
		{
			string text = error.FileName;
			using (List<ScriptClass>.Enumerator enumerator = scriptsForLang.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string text2;
					if (enumerator.Current.scriptUris.TryGetValue(text, out text2))
					{
						error.FileName = text2;
						return;
					}
				}
			}
			ScriptClass scriptClass = scriptsForLang[scriptsForLang.Count - 1];
			text = Path.GetFileNameWithoutExtension(text);
			int num;
			int num2;
			if ((num = text.LastIndexOf('.')) >= 0 && int.TryParse(text.Substring(num + 1), NumberStyles.None, NumberFormatInfo.InvariantInfo, out num2) && (ulong)num2 < (ulong)((long)scriptsForLang.Count))
			{
				scriptClass = scriptsForLang[num2];
			}
			error.FileName = scriptClass.endUri;
			error.Line = scriptClass.endLoc.Line;
			error.Column = scriptClass.endLoc.Pos;
		}

		private const string ScriptClassesNamespace = "System.Xml.Xsl.CompiledQuery";

		private Compiler compiler;

		private List<ScriptClass> scriptClasses = new List<ScriptClass>();

		private Dictionary<string, Type> nsToType = new Dictionary<string, Type>();

		private XmlExtensionFunctionTable extFuncs = new XmlExtensionFunctionTable();

		private static readonly string[] defaultNamespaces = new string[] { "System", "System.Collections", "System.Text", "System.Text.RegularExpressions", "System.Xml", "System.Xml.Xsl", "System.Xml.XPath" };

		private int assemblyCounter;
	}
}
