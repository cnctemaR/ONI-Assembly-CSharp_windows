using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class Compiler
	{
		public Compiler(XsltSettings settings, bool debug, string scriptAssemblyPath)
		{
			TempFileCollection tempFileCollection = settings.TempFiles ?? new TempFileCollection();
			this.Settings = settings;
			this.IsDebug = settings.IncludeDebugInformation || debug;
			this.ScriptAssemblyPath = scriptAssemblyPath;
			this.CompilerResults = new CompilerResults(tempFileCollection);
			this.Scripts = new Scripts(this);
		}

		public CompilerResults Compile(object stylesheet, XmlResolver xmlResolver, out QilExpression qil)
		{
			new XsltLoader().Load(this, stylesheet, xmlResolver);
			qil = QilGenerator.CompileStylesheet(this);
			this.SortErrors();
			return this.CompilerResults;
		}

		public Stylesheet CreateStylesheet()
		{
			Stylesheet stylesheet = new Stylesheet(this, this.CurrentPrecedence);
			int currentPrecedence = this.CurrentPrecedence;
			this.CurrentPrecedence = currentPrecedence - 1;
			if (currentPrecedence == 0)
			{
				this.Root = new RootLevel(stylesheet);
			}
			return stylesheet;
		}

		public void AddModule(string baseUri)
		{
			if (!this.moduleOrder.ContainsKey(baseUri))
			{
				this.moduleOrder[baseUri] = this.moduleOrder.Count;
			}
		}

		public void ApplyNsAliases(ref string prefix, ref string nsUri)
		{
			NsAlias nsAlias;
			if (this.NsAliases.TryGetValue(nsUri, out nsAlias))
			{
				nsUri = nsAlias.ResultNsUri;
				prefix = nsAlias.ResultPrefix;
			}
		}

		public bool SetNsAlias(string ssheetNsUri, string resultNsUri, string resultPrefix, int importPrecedence)
		{
			NsAlias nsAlias;
			if (this.NsAliases.TryGetValue(ssheetNsUri, out nsAlias) && (importPrecedence < nsAlias.ImportPrecedence || resultNsUri == nsAlias.ResultNsUri))
			{
				return false;
			}
			this.NsAliases[ssheetNsUri] = new NsAlias(resultNsUri, resultPrefix, importPrecedence);
			return nsAlias != null;
		}

		private void MergeWhitespaceRules(Stylesheet sheet)
		{
			for (int i = 0; i <= 2; i++)
			{
				sheet.WhitespaceRules[i].Reverse();
				this.WhitespaceRules.AddRange(sheet.WhitespaceRules[i]);
			}
			sheet.WhitespaceRules = null;
		}

		private void MergeAttributeSets(Stylesheet sheet)
		{
			foreach (QilName qilName in sheet.AttributeSets.Keys)
			{
				AttributeSet attributeSet;
				if (!this.AttributeSets.TryGetValue(qilName, out attributeSet))
				{
					this.AttributeSets[qilName] = sheet.AttributeSets[qilName];
				}
				else
				{
					attributeSet.MergeContent(sheet.AttributeSets[qilName]);
				}
			}
			sheet.AttributeSets = null;
		}

		private void MergeGlobalVarPars(Stylesheet sheet)
		{
			foreach (XslNode xslNode in sheet.GlobalVarPars)
			{
				VarPar varPar = (VarPar)xslNode;
				if (!this.AllGlobalVarPars.ContainsKey(varPar.Name))
				{
					if (varPar.NodeType == XslNodeType.Variable)
					{
						this.GlobalVars.Add(varPar);
					}
					else
					{
						this.ExternalPars.Add(varPar);
					}
					this.AllGlobalVarPars[varPar.Name] = varPar;
				}
			}
			sheet.GlobalVarPars = null;
		}

		public void MergeWithStylesheet(Stylesheet sheet)
		{
			this.MergeWhitespaceRules(sheet);
			this.MergeAttributeSets(sheet);
			this.MergeGlobalVarPars(sheet);
		}

		public static string ConstructQName(string prefix, string localName)
		{
			if (prefix.Length == 0)
			{
				return localName;
			}
			return prefix + ":" + localName;
		}

		public bool ParseQName(string qname, out string prefix, out string localName, IErrorHelper errorHelper)
		{
			bool flag;
			try
			{
				ValidateNames.ParseQNameThrow(qname, out prefix, out localName);
				flag = true;
			}
			catch (XmlException ex)
			{
				errorHelper.ReportError(ex.Message, null);
				prefix = this.PhantomNCName;
				localName = this.PhantomNCName;
				flag = false;
			}
			return flag;
		}

		public bool ParseNameTest(string nameTest, out string prefix, out string localName, IErrorHelper errorHelper)
		{
			bool flag;
			try
			{
				ValidateNames.ParseNameTestThrow(nameTest, out prefix, out localName);
				flag = true;
			}
			catch (XmlException ex)
			{
				errorHelper.ReportError(ex.Message, null);
				prefix = this.PhantomNCName;
				localName = this.PhantomNCName;
				flag = false;
			}
			return flag;
		}

		public void ValidatePiName(string name, IErrorHelper errorHelper)
		{
			try
			{
				ValidateNames.ValidateNameThrow(string.Empty, name, string.Empty, XPathNodeType.ProcessingInstruction, ValidateNames.Flags.AllExceptPrefixMapping);
			}
			catch (XmlException ex)
			{
				errorHelper.ReportError(ex.Message, null);
			}
		}

		public string CreatePhantomNamespace()
		{
			object obj = "\0namespace";
			int num = this.phantomNsCounter;
			this.phantomNsCounter = num + 1;
			return obj + num;
		}

		public bool IsPhantomNamespace(string namespaceName)
		{
			return namespaceName.Length > 0 && namespaceName[0] == '\0';
		}

		public bool IsPhantomName(QilName qname)
		{
			string namespaceUri = qname.NamespaceUri;
			return namespaceUri.Length > 0 && namespaceUri[0] == '\0';
		}

		private int ErrorCount
		{
			get
			{
				return this.CompilerResults.Errors.Count;
			}
			set
			{
				for (int i = this.ErrorCount - 1; i >= value; i--)
				{
					this.CompilerResults.Errors.RemoveAt(i);
				}
			}
		}

		public void EnterForwardsCompatible()
		{
			this.savedErrorCount = this.ErrorCount;
		}

		public bool ExitForwardsCompatible(bool fwdCompat)
		{
			if (fwdCompat && this.ErrorCount > this.savedErrorCount)
			{
				this.ErrorCount = this.savedErrorCount;
				return false;
			}
			return true;
		}

		public CompilerError CreateError(ISourceLineInfo lineInfo, string res, params string[] args)
		{
			this.AddModule(lineInfo.Uri);
			return new CompilerError(lineInfo.Uri, lineInfo.Start.Line, lineInfo.Start.Pos, string.Empty, XslTransformException.CreateMessage(res, args));
		}

		public void ReportError(ISourceLineInfo lineInfo, string res, params string[] args)
		{
			CompilerError compilerError = this.CreateError(lineInfo, res, args);
			this.CompilerResults.Errors.Add(compilerError);
		}

		public void ReportWarning(ISourceLineInfo lineInfo, string res, params string[] args)
		{
			int num = 1;
			if (0 <= this.Settings.WarningLevel && this.Settings.WarningLevel < num)
			{
				return;
			}
			CompilerError compilerError = this.CreateError(lineInfo, res, args);
			if (this.Settings.TreatWarningsAsErrors)
			{
				compilerError.ErrorText = XslTransformException.CreateMessage("Warning as Error: {0}", new string[] { compilerError.ErrorText });
				this.CompilerResults.Errors.Add(compilerError);
				return;
			}
			compilerError.IsWarning = true;
			this.CompilerResults.Errors.Add(compilerError);
		}

		private void SortErrors()
		{
			CompilerErrorCollection errors = this.CompilerResults.Errors;
			if (errors.Count > 1)
			{
				CompilerError[] array = new CompilerError[errors.Count];
				errors.CopyTo(array, 0);
				Array.Sort<CompilerError>(array, new Compiler.CompilerErrorComparer(this.moduleOrder));
				errors.Clear();
				errors.AddRange(array);
			}
		}

		public XsltSettings Settings;

		public bool IsDebug;

		public string ScriptAssemblyPath;

		public int Version;

		public string inputTypeAnnotations;

		public CompilerResults CompilerResults;

		public int CurrentPrecedence;

		public XslNode StartApplyTemplates;

		public RootLevel Root;

		public Scripts Scripts;

		public Output Output = new Output();

		public List<VarPar> ExternalPars = new List<VarPar>();

		public List<VarPar> GlobalVars = new List<VarPar>();

		public List<WhitespaceRule> WhitespaceRules = new List<WhitespaceRule>();

		public DecimalFormats DecimalFormats = new DecimalFormats();

		public Keys Keys = new Keys();

		public List<ProtoTemplate> AllTemplates = new List<ProtoTemplate>();

		public Dictionary<QilName, VarPar> AllGlobalVarPars = new Dictionary<QilName, VarPar>();

		public Dictionary<QilName, Template> NamedTemplates = new Dictionary<QilName, Template>();

		public Dictionary<QilName, AttributeSet> AttributeSets = new Dictionary<QilName, AttributeSet>();

		public Dictionary<string, NsAlias> NsAliases = new Dictionary<string, NsAlias>();

		private Dictionary<string, int> moduleOrder = new Dictionary<string, int>();

		public readonly string PhantomNCName = "error";

		private int phantomNsCounter;

		private int savedErrorCount = -1;

		private class CompilerErrorComparer : IComparer<CompilerError>
		{
			public CompilerErrorComparer(Dictionary<string, int> moduleOrder)
			{
				this.moduleOrder = moduleOrder;
			}

			public int Compare(CompilerError x, CompilerError y)
			{
				if (x == y)
				{
					return 0;
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				int num = this.moduleOrder[x.FileName].CompareTo(this.moduleOrder[y.FileName]);
				if (num != 0)
				{
					return num;
				}
				num = x.Line.CompareTo(y.Line);
				if (num != 0)
				{
					return num;
				}
				num = x.Column.CompareTo(y.Column);
				if (num != 0)
				{
					return num;
				}
				num = x.IsWarning.CompareTo(y.IsWarning);
				if (num != 0)
				{
					return num;
				}
				num = string.CompareOrdinal(x.ErrorNumber, y.ErrorNumber);
				if (num != 0)
				{
					return num;
				}
				return string.CompareOrdinal(x.ErrorText, y.ErrorText);
			}

			private Dictionary<string, int> moduleOrder;
		}
	}
}
