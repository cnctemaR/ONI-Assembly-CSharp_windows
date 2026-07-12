using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Xml.XPath;
using System.Xml.Xsl.Runtime;
using System.Xml.Xsl.Xslt;
using System.Xml.Xsl.XsltOld.Debugger;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class Compiler
	{
		internal KeywordsTable Atoms
		{
			get
			{
				return this.atoms;
			}
		}

		internal int Stylesheetid
		{
			get
			{
				return this.stylesheetid;
			}
			set
			{
				this.stylesheetid = value;
			}
		}

		internal NavigatorInput Document
		{
			get
			{
				return this.input;
			}
		}

		internal NavigatorInput Input
		{
			get
			{
				return this.input;
			}
		}

		internal bool Advance()
		{
			return this.Document.Advance();
		}

		internal bool Recurse()
		{
			return this.Document.Recurse();
		}

		internal bool ToParent()
		{
			return this.Document.ToParent();
		}

		internal Stylesheet CompiledStylesheet
		{
			get
			{
				return this.stylesheet;
			}
		}

		internal RootAction RootAction
		{
			get
			{
				return this.rootAction;
			}
			set
			{
				this.rootAction = value;
				this.currentTemplate = this.rootAction;
			}
		}

		internal List<TheQuery> QueryStore
		{
			get
			{
				return this.queryStore;
			}
		}

		public virtual IXsltDebugger Debugger
		{
			get
			{
				return null;
			}
		}

		internal string GetUnicRtfId()
		{
			this.rtfCount++;
			return this.rtfCount.ToString(CultureInfo.InvariantCulture);
		}

		internal void Compile(NavigatorInput input, XmlResolver xmlResolver, Evidence evidence)
		{
			evidence = null;
			this.xmlResolver = xmlResolver;
			this.PushInputDocument(input);
			this.rootScope = this.scopeManager.PushScope();
			this.queryStore = new List<TheQuery>();
			try
			{
				this.rootStylesheet = new Stylesheet();
				this.PushStylesheet(this.rootStylesheet);
				try
				{
					this.CreateRootAction();
				}
				catch (XsltCompileException)
				{
					throw;
				}
				catch (Exception ex)
				{
					throw new XsltCompileException(ex, this.Input.BaseURI, this.Input.LineNumber, this.Input.LinePosition);
				}
				this.stylesheet.ProcessTemplates();
				this.rootAction.PorcessAttributeSets(this.rootStylesheet);
				this.stylesheet.SortWhiteSpace();
				this.CompileScript(evidence);
				if (evidence != null)
				{
					this.rootAction.permissions = SecurityManager.GetStandardSandbox(evidence);
				}
				if (this.globalNamespaceAliasTable != null)
				{
					this.stylesheet.ReplaceNamespaceAlias(this);
					this.rootAction.ReplaceNamespaceAlias(this);
				}
			}
			finally
			{
				this.PopInputDocument();
			}
		}

		internal bool ForwardCompatibility
		{
			get
			{
				return this.scopeManager.CurrentScope.ForwardCompatibility;
			}
			set
			{
				this.scopeManager.CurrentScope.ForwardCompatibility = value;
			}
		}

		internal bool CanHaveApplyImports
		{
			get
			{
				return this.scopeManager.CurrentScope.CanHaveApplyImports;
			}
			set
			{
				this.scopeManager.CurrentScope.CanHaveApplyImports = value;
			}
		}

		internal void InsertExtensionNamespace(string value)
		{
			string[] array = this.ResolvePrefixes(value);
			if (array != null)
			{
				this.scopeManager.InsertExtensionNamespaces(array);
			}
		}

		internal void InsertExcludedNamespace(string value)
		{
			string[] array = this.ResolvePrefixes(value);
			if (array != null)
			{
				this.scopeManager.InsertExcludedNamespaces(array);
			}
		}

		internal void InsertExtensionNamespace()
		{
			this.InsertExtensionNamespace(this.Input.Navigator.GetAttribute(this.Input.Atoms.ExtensionElementPrefixes, this.Input.Atoms.UriXsl));
		}

		internal void InsertExcludedNamespace()
		{
			this.InsertExcludedNamespace(this.Input.Navigator.GetAttribute(this.Input.Atoms.ExcludeResultPrefixes, this.Input.Atoms.UriXsl));
		}

		internal bool IsExtensionNamespace(string nspace)
		{
			return this.scopeManager.IsExtensionNamespace(nspace);
		}

		internal bool IsExcludedNamespace(string nspace)
		{
			return this.scopeManager.IsExcludedNamespace(nspace);
		}

		internal void PushLiteralScope()
		{
			this.PushNamespaceScope();
			string attribute = this.Input.Navigator.GetAttribute(this.Atoms.Version, this.Atoms.UriXsl);
			if (attribute.Length != 0)
			{
				this.ForwardCompatibility = attribute != "1.0";
			}
		}

		internal void PushNamespaceScope()
		{
			this.scopeManager.PushScope();
			NavigatorInput navigatorInput = this.Input;
			if (navigatorInput.MoveToFirstNamespace())
			{
				do
				{
					this.scopeManager.PushNamespace(navigatorInput.LocalName, navigatorInput.Value);
				}
				while (navigatorInput.MoveToNextNamespace());
				navigatorInput.ToParent();
			}
		}

		protected InputScopeManager ScopeManager
		{
			get
			{
				return this.scopeManager;
			}
		}

		internal virtual void PopScope()
		{
			this.currentTemplate.ReleaseVariableSlots(this.scopeManager.CurrentScope.GetVeriablesCount());
			this.scopeManager.PopScope();
		}

		internal InputScopeManager CloneScopeManager()
		{
			return this.scopeManager.Clone();
		}

		internal int InsertVariable(VariableAction variable)
		{
			InputScope variableScope;
			if (variable.IsGlobal)
			{
				variableScope = this.rootScope;
			}
			else
			{
				variableScope = this.scopeManager.VariableScope;
			}
			VariableAction variableAction = variableScope.ResolveVariable(variable.Name);
			if (variableAction != null)
			{
				if (!variableAction.IsGlobal)
				{
					throw XsltException.Create("Variable or parameter '{0}' was duplicated within the same scope.", new string[] { variable.NameStr });
				}
				if (variable.IsGlobal)
				{
					if (variable.Stylesheetid == variableAction.Stylesheetid)
					{
						throw XsltException.Create("Variable or parameter '{0}' was duplicated within the same scope.", new string[] { variable.NameStr });
					}
					if (variable.Stylesheetid < variableAction.Stylesheetid)
					{
						variableScope.InsertVariable(variable);
						return variableAction.VarKey;
					}
					return -1;
				}
			}
			variableScope.InsertVariable(variable);
			return this.currentTemplate.AllocateVariableSlot();
		}

		internal void AddNamespaceAlias(string StylesheetURI, NamespaceInfo AliasInfo)
		{
			if (this.globalNamespaceAliasTable == null)
			{
				this.globalNamespaceAliasTable = new Hashtable();
			}
			NamespaceInfo namespaceInfo = this.globalNamespaceAliasTable[StylesheetURI] as NamespaceInfo;
			if (namespaceInfo == null || AliasInfo.stylesheetId <= namespaceInfo.stylesheetId)
			{
				this.globalNamespaceAliasTable[StylesheetURI] = AliasInfo;
			}
		}

		internal bool IsNamespaceAlias(string StylesheetURI)
		{
			return this.globalNamespaceAliasTable != null && this.globalNamespaceAliasTable.Contains(StylesheetURI);
		}

		internal NamespaceInfo FindNamespaceAlias(string StylesheetURI)
		{
			if (this.globalNamespaceAliasTable != null)
			{
				return (NamespaceInfo)this.globalNamespaceAliasTable[StylesheetURI];
			}
			return null;
		}

		internal string ResolveXmlNamespace(string prefix)
		{
			return this.scopeManager.ResolveXmlNamespace(prefix);
		}

		internal string ResolveXPathNamespace(string prefix)
		{
			return this.scopeManager.ResolveXPathNamespace(prefix);
		}

		internal string DefaultNamespace
		{
			get
			{
				return this.scopeManager.DefaultNamespace;
			}
		}

		internal void InsertKey(XmlQualifiedName name, int MatchKey, int UseKey)
		{
			this.rootAction.InsertKey(name, MatchKey, UseKey);
		}

		internal void AddDecimalFormat(XmlQualifiedName name, DecimalFormat formatinfo)
		{
			this.rootAction.AddDecimalFormat(name, formatinfo);
		}

		private string[] ResolvePrefixes(string tokens)
		{
			if (tokens == null || tokens.Length == 0)
			{
				return null;
			}
			string[] array = XmlConvert.SplitString(tokens);
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					array[i] = this.scopeManager.ResolveXmlNamespace((text == "#default") ? string.Empty : text);
				}
			}
			catch (XsltException)
			{
				if (!this.ForwardCompatibility)
				{
					throw;
				}
				return null;
			}
			return array;
		}

		internal bool GetYesNo(string value)
		{
			if (value == "yes")
			{
				return true;
			}
			if (value == "no")
			{
				return false;
			}
			throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[]
			{
				this.Input.LocalName,
				value
			});
		}

		internal string GetSingleAttribute(string attributeAtom)
		{
			NavigatorInput navigatorInput = this.Input;
			string localName = navigatorInput.LocalName;
			string text = null;
			if (navigatorInput.MoveToFirstAttribute())
			{
				string localName2;
				for (;;)
				{
					string namespaceURI = navigatorInput.NamespaceURI;
					localName2 = navigatorInput.LocalName;
					if (namespaceURI.Length == 0)
					{
						if (Ref.Equal(localName2, attributeAtom))
						{
							text = navigatorInput.Value;
						}
						else if (!this.ForwardCompatibility)
						{
							break;
						}
					}
					if (!navigatorInput.MoveToNextAttribute())
					{
						goto Block_4;
					}
				}
				throw XsltException.Create("'{0}' is an invalid attribute for the '{1}' element.", new string[] { localName2, localName });
				Block_4:
				navigatorInput.ToParent();
			}
			if (text == null)
			{
				throw XsltException.Create("Missing mandatory attribute '{0}'.", new string[] { attributeAtom });
			}
			return text;
		}

		internal XmlQualifiedName CreateXPathQName(string qname)
		{
			string text;
			string text2;
			PrefixQName.ParseQualifiedName(qname, out text, out text2);
			return new XmlQualifiedName(text2, this.scopeManager.ResolveXPathNamespace(text));
		}

		internal XmlQualifiedName CreateXmlQName(string qname)
		{
			string text;
			string text2;
			PrefixQName.ParseQualifiedName(qname, out text, out text2);
			return new XmlQualifiedName(text2, this.scopeManager.ResolveXmlNamespace(text));
		}

		internal static XPathDocument LoadDocument(XmlTextReaderImpl reader)
		{
			reader.EntityHandling = EntityHandling.ExpandEntities;
			reader.XmlValidatingReaderCompatibilityMode = true;
			XPathDocument xpathDocument;
			try
			{
				xpathDocument = new XPathDocument(reader, XmlSpace.Preserve);
			}
			finally
			{
				reader.Close();
			}
			return xpathDocument;
		}

		private void AddDocumentURI(string href)
		{
			this.documentURIs.Add(href, null);
		}

		private void RemoveDocumentURI(string href)
		{
			this.documentURIs.Remove(href);
		}

		internal bool IsCircularReference(string href)
		{
			return this.documentURIs.Contains(href);
		}

		internal Uri ResolveUri(string relativeUri)
		{
			string baseURI = this.Input.BaseURI;
			Uri uri = this.xmlResolver.ResolveUri((baseURI.Length != 0) ? this.xmlResolver.ResolveUri(null, baseURI) : null, relativeUri);
			if (uri == null)
			{
				throw XsltException.Create("Cannot resolve the referenced document '{0}'.", new string[] { relativeUri });
			}
			return uri;
		}

		internal NavigatorInput ResolveDocument(Uri absoluteUri)
		{
			object entity = this.xmlResolver.GetEntity(absoluteUri, null, null);
			string text = absoluteUri.ToString();
			if (entity is Stream)
			{
				return new NavigatorInput(Compiler.LoadDocument(new XmlTextReaderImpl(text, (Stream)entity)
				{
					XmlResolver = this.xmlResolver
				}).CreateNavigator(), text, this.rootScope);
			}
			if (entity is XPathNavigator)
			{
				return new NavigatorInput((XPathNavigator)entity, text, this.rootScope);
			}
			throw XsltException.Create("Cannot resolve the referenced document '{0}'.", new string[] { text });
		}

		internal void PushInputDocument(NavigatorInput newInput)
		{
			string href = newInput.Href;
			this.AddDocumentURI(href);
			newInput.Next = this.input;
			this.input = newInput;
			this.atoms = this.input.Atoms;
			this.scopeManager = this.input.InputScopeManager;
		}

		internal void PopInputDocument()
		{
			NavigatorInput navigatorInput = this.input;
			this.input = navigatorInput.Next;
			navigatorInput.Next = null;
			if (this.input != null)
			{
				this.atoms = this.input.Atoms;
				this.scopeManager = this.input.InputScopeManager;
			}
			else
			{
				this.atoms = null;
				this.scopeManager = null;
			}
			this.RemoveDocumentURI(navigatorInput.Href);
			navigatorInput.Close();
		}

		internal void PushStylesheet(Stylesheet stylesheet)
		{
			if (this.stylesheets == null)
			{
				this.stylesheets = new Stack();
			}
			this.stylesheets.Push(stylesheet);
			this.stylesheet = stylesheet;
		}

		internal Stylesheet PopStylesheet()
		{
			Stylesheet stylesheet = (Stylesheet)this.stylesheets.Pop();
			this.stylesheet = (Stylesheet)this.stylesheets.Peek();
			return stylesheet;
		}

		internal void AddAttributeSet(AttributeSetAction attributeSet)
		{
			this.stylesheet.AddAttributeSet(attributeSet);
		}

		internal void AddTemplate(TemplateAction template)
		{
			this.stylesheet.AddTemplate(template);
		}

		internal void BeginTemplate(TemplateAction template)
		{
			this.currentTemplate = template;
			this.currentMode = template.Mode;
			this.CanHaveApplyImports = template.MatchKey != -1;
		}

		internal void EndTemplate()
		{
			this.currentTemplate = this.rootAction;
		}

		internal XmlQualifiedName CurrentMode
		{
			get
			{
				return this.currentMode;
			}
		}

		internal int AddQuery(string xpathQuery)
		{
			return this.AddQuery(xpathQuery, true, true, false);
		}

		internal int AddQuery(string xpathQuery, bool allowVar, bool allowKey, bool isPattern)
		{
			CompiledXpathExpr compiledXpathExpr;
			try
			{
				compiledXpathExpr = new CompiledXpathExpr(isPattern ? this.queryBuilder.BuildPatternQuery(xpathQuery, allowVar, allowKey) : this.queryBuilder.Build(xpathQuery, allowVar, allowKey), xpathQuery, false);
			}
			catch (XPathException ex)
			{
				if (!this.ForwardCompatibility)
				{
					throw XsltException.Create("'{0}' is an invalid XPath expression.", new string[] { xpathQuery }, ex);
				}
				compiledXpathExpr = new Compiler.ErrorXPathExpression(xpathQuery, this.Input.BaseURI, this.Input.LineNumber, this.Input.LinePosition);
			}
			this.queryStore.Add(new TheQuery(compiledXpathExpr, this.scopeManager));
			return this.queryStore.Count - 1;
		}

		internal int AddStringQuery(string xpathQuery)
		{
			string text = (XmlCharType.Instance.IsOnlyWhitespace(xpathQuery) ? xpathQuery : ("string(" + xpathQuery + ")"));
			return this.AddQuery(text);
		}

		internal int AddBooleanQuery(string xpathQuery)
		{
			string text = (XmlCharType.Instance.IsOnlyWhitespace(xpathQuery) ? xpathQuery : ("boolean(" + xpathQuery + ")"));
			return this.AddQuery(text);
		}

		private static string GenerateUniqueClassName()
		{
			return "ScriptClass_" + Interlocked.Increment(ref Compiler.scriptClassCounter).ToString();
		}

		internal void AddScript(string source, ScriptingLanguage lang, string ns, string fileName, int lineNumber)
		{
			Compiler.ValidateExtensionNamespace(ns);
			for (ScriptingLanguage scriptingLanguage = ScriptingLanguage.JScript; scriptingLanguage <= ScriptingLanguage.CSharp; scriptingLanguage++)
			{
				Hashtable hashtable = this._typeDeclsByLang[(int)scriptingLanguage];
				if (lang == scriptingLanguage)
				{
					CodeTypeDeclaration codeTypeDeclaration = (CodeTypeDeclaration)hashtable[ns];
					if (codeTypeDeclaration == null)
					{
						codeTypeDeclaration = new CodeTypeDeclaration(Compiler.GenerateUniqueClassName());
						codeTypeDeclaration.TypeAttributes = TypeAttributes.Public;
						hashtable.Add(ns, codeTypeDeclaration);
					}
					CodeSnippetTypeMember codeSnippetTypeMember = new CodeSnippetTypeMember(source);
					if (lineNumber > 0)
					{
						codeSnippetTypeMember.LinePragma = new CodeLinePragma(fileName, lineNumber);
						this.scriptFiles.Add(fileName);
					}
					codeTypeDeclaration.Members.Add(codeSnippetTypeMember);
				}
				else if (hashtable.Contains(ns))
				{
					throw XsltException.Create("All script blocks implementing the namespace '{0}' must use the same language.", new string[] { ns });
				}
			}
		}

		private static void ValidateExtensionNamespace(string nsUri)
		{
			if (nsUri.Length == 0 || nsUri == "http://www.w3.org/1999/XSL/Transform")
			{
				throw XsltException.Create("Extension namespace cannot be 'null' or an XSLT namespace URI.", Array.Empty<string>());
			}
			XmlConvert.ToUri(nsUri);
		}

		private void FixCompilerError(CompilerError e)
		{
			foreach (object obj in this.scriptFiles)
			{
				string text = (string)obj;
				if (e.FileName == text)
				{
					return;
				}
			}
			e.FileName = string.Empty;
		}

		private CodeDomProvider ChooseCodeDomProvider(ScriptingLanguage lang)
		{
			if (lang == ScriptingLanguage.JScript)
			{
				return (CodeDomProvider)Activator.CreateInstance(Type.GetType("Microsoft.JScript.JScriptCodeProvider, Microsoft.JScript, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
			}
			if (lang != ScriptingLanguage.VisualBasic)
			{
				return new CSharpCodeProvider();
			}
			return new VBCodeProvider();
		}

		private void CompileScript(Evidence evidence)
		{
			for (ScriptingLanguage scriptingLanguage = ScriptingLanguage.JScript; scriptingLanguage <= ScriptingLanguage.CSharp; scriptingLanguage++)
			{
				int num = (int)scriptingLanguage;
				if (this._typeDeclsByLang[num].Count > 0)
				{
					this.CompileAssembly(scriptingLanguage, this._typeDeclsByLang[num], scriptingLanguage.ToString(), evidence);
				}
			}
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		private void CompileAssembly(ScriptingLanguage lang, Hashtable typeDecls, string nsName, Evidence evidence)
		{
			nsName = "Microsoft.Xslt.CompiledScripts." + nsName;
			CodeNamespace codeNamespace = new CodeNamespace(nsName);
			foreach (string text in Compiler._defaultNamespaces)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(text));
			}
			if (lang == ScriptingLanguage.VisualBasic)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.VisualBasic"));
			}
			foreach (object obj in typeDecls.Values)
			{
				CodeTypeDeclaration codeTypeDeclaration = (CodeTypeDeclaration)obj;
				codeNamespace.Types.Add(codeTypeDeclaration);
			}
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(codeNamespace);
			codeCompileUnit.UserData["AllowLateBound"] = true;
			codeCompileUnit.UserData["RequireVariableDeclaration"] = false;
			codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SecurityRulesAttribute)), new CodeAttributeArgument[]
			{
				new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityRuleSet)), "Level1"))
			}));
			CompilerParameters compilerParameters = new CompilerParameters();
			try
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
				try
				{
					compilerParameters.GenerateInMemory = true;
					compilerParameters.Evidence = evidence;
					compilerParameters.ReferencedAssemblies.Add(typeof(XPathNavigator).Module.FullyQualifiedName);
					compilerParameters.ReferencedAssemblies.Add("System.dll");
					if (lang == ScriptingLanguage.VisualBasic)
					{
						compilerParameters.ReferencedAssemblies.Add("microsoft.visualbasic.dll");
					}
				}
				finally
				{
					CodeAccessPermission.RevertAssert();
				}
			}
			catch
			{
				throw;
			}
			CompilerResults compilerResults = this.ChooseCodeDomProvider(lang).CompileAssemblyFromDom(compilerParameters, new CodeCompileUnit[] { codeCompileUnit });
			if (compilerResults.Errors.HasErrors)
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				foreach (object obj2 in compilerResults.Errors)
				{
					CompilerError compilerError = (CompilerError)obj2;
					this.FixCompilerError(compilerError);
					stringWriter.WriteLine(compilerError.ToString());
				}
				throw XsltException.Create("Script compile errors:\n{0}", new string[] { stringWriter.ToString() });
			}
			Assembly compiledAssembly = compilerResults.CompiledAssembly;
			foreach (object obj3 in typeDecls)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
				string text2 = (string)dictionaryEntry.Key;
				CodeTypeDeclaration codeTypeDeclaration2 = (CodeTypeDeclaration)dictionaryEntry.Value;
				this.stylesheet.ScriptObjectTypes.Add(text2, compiledAssembly.GetType(nsName + "." + codeTypeDeclaration2.Name));
			}
		}

		public string GetNsAlias(ref string prefix)
		{
			if (prefix == "#default")
			{
				prefix = string.Empty;
				return this.DefaultNamespace;
			}
			if (!PrefixQName.ValidatePrefix(prefix))
			{
				throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[]
				{
					this.input.LocalName,
					prefix
				});
			}
			return this.ResolveXPathNamespace(prefix);
		}

		private static void getTextLex(string avt, ref int start, StringBuilder lex)
		{
			int length = avt.Length;
			int i;
			for (i = start; i < length; i++)
			{
				char c = avt[i];
				if (c == '{')
				{
					if (i + 1 >= length || avt[i + 1] != '{')
					{
						break;
					}
					i++;
				}
				else if (c == '}')
				{
					if (i + 1 >= length || avt[i + 1] != '}')
					{
						throw XsltException.Create("Right curly brace in the attribute value template '{0}' must be doubled.", new string[] { avt });
					}
					i++;
				}
				lex.Append(c);
			}
			start = i;
		}

		private static void getXPathLex(string avt, ref int start, StringBuilder lex)
		{
			int length = avt.Length;
			int num = 0;
			for (int i = start + 1; i < length; i++)
			{
				char c = avt[i];
				switch (num)
				{
				case 0:
					if (c <= '\'')
					{
						if (c != '"')
						{
							if (c == '\'')
							{
								num = 1;
							}
						}
						else
						{
							num = 2;
						}
					}
					else
					{
						if (c == '{')
						{
							throw XsltException.Create("AVT cannot be nested in AVT '{0}'.", new string[] { avt });
						}
						if (c == '}')
						{
							i++;
							if (i == start + 2)
							{
								throw XsltException.Create("XPath Expression in AVT cannot be empty: '{0}'.", new string[] { avt });
							}
							lex.Append(avt, start + 1, i - start - 2);
							start = i;
							return;
						}
					}
					break;
				case 1:
					if (c == '\'')
					{
						num = 0;
					}
					break;
				case 2:
					if (c == '"')
					{
						num = 0;
					}
					break;
				}
			}
			throw XsltException.Create((num == 0) ? "The braces are not closed in AVT expression '{0}'." : "The literal in AVT expression is not correctly closed '{0}'.", new string[] { avt });
		}

		private static bool GetNextAvtLex(string avt, ref int start, StringBuilder lex, out bool isAvt)
		{
			isAvt = false;
			if (start == avt.Length)
			{
				return false;
			}
			lex.Length = 0;
			Compiler.getTextLex(avt, ref start, lex);
			if (lex.Length == 0)
			{
				isAvt = true;
				Compiler.getXPathLex(avt, ref start, lex);
			}
			return true;
		}

		internal ArrayList CompileAvt(string avtText, out bool constant)
		{
			ArrayList arrayList = new ArrayList();
			constant = true;
			int num = 0;
			bool flag;
			while (Compiler.GetNextAvtLex(avtText, ref num, this.AvtStringBuilder, out flag))
			{
				string text = this.AvtStringBuilder.ToString();
				if (flag)
				{
					arrayList.Add(new AvtEvent(this.AddStringQuery(text)));
					constant = false;
				}
				else
				{
					arrayList.Add(new TextEvent(text));
				}
			}
			return arrayList;
		}

		internal ArrayList CompileAvt(string avtText)
		{
			bool flag;
			return this.CompileAvt(avtText, out flag);
		}

		public virtual ApplyImportsAction CreateApplyImportsAction()
		{
			ApplyImportsAction applyImportsAction = new ApplyImportsAction();
			applyImportsAction.Compile(this);
			return applyImportsAction;
		}

		public virtual ApplyTemplatesAction CreateApplyTemplatesAction()
		{
			ApplyTemplatesAction applyTemplatesAction = new ApplyTemplatesAction();
			applyTemplatesAction.Compile(this);
			return applyTemplatesAction;
		}

		public virtual AttributeAction CreateAttributeAction()
		{
			AttributeAction attributeAction = new AttributeAction();
			attributeAction.Compile(this);
			return attributeAction;
		}

		public virtual AttributeSetAction CreateAttributeSetAction()
		{
			AttributeSetAction attributeSetAction = new AttributeSetAction();
			attributeSetAction.Compile(this);
			return attributeSetAction;
		}

		public virtual CallTemplateAction CreateCallTemplateAction()
		{
			CallTemplateAction callTemplateAction = new CallTemplateAction();
			callTemplateAction.Compile(this);
			return callTemplateAction;
		}

		public virtual ChooseAction CreateChooseAction()
		{
			ChooseAction chooseAction = new ChooseAction();
			chooseAction.Compile(this);
			return chooseAction;
		}

		public virtual CommentAction CreateCommentAction()
		{
			CommentAction commentAction = new CommentAction();
			commentAction.Compile(this);
			return commentAction;
		}

		public virtual CopyAction CreateCopyAction()
		{
			CopyAction copyAction = new CopyAction();
			copyAction.Compile(this);
			return copyAction;
		}

		public virtual CopyOfAction CreateCopyOfAction()
		{
			CopyOfAction copyOfAction = new CopyOfAction();
			copyOfAction.Compile(this);
			return copyOfAction;
		}

		public virtual ElementAction CreateElementAction()
		{
			ElementAction elementAction = new ElementAction();
			elementAction.Compile(this);
			return elementAction;
		}

		public virtual ForEachAction CreateForEachAction()
		{
			ForEachAction forEachAction = new ForEachAction();
			forEachAction.Compile(this);
			return forEachAction;
		}

		public virtual IfAction CreateIfAction(IfAction.ConditionType type)
		{
			IfAction ifAction = new IfAction(type);
			ifAction.Compile(this);
			return ifAction;
		}

		public virtual MessageAction CreateMessageAction()
		{
			MessageAction messageAction = new MessageAction();
			messageAction.Compile(this);
			return messageAction;
		}

		public virtual NewInstructionAction CreateNewInstructionAction()
		{
			NewInstructionAction newInstructionAction = new NewInstructionAction();
			newInstructionAction.Compile(this);
			return newInstructionAction;
		}

		public virtual NumberAction CreateNumberAction()
		{
			NumberAction numberAction = new NumberAction();
			numberAction.Compile(this);
			return numberAction;
		}

		public virtual ProcessingInstructionAction CreateProcessingInstructionAction()
		{
			ProcessingInstructionAction processingInstructionAction = new ProcessingInstructionAction();
			processingInstructionAction.Compile(this);
			return processingInstructionAction;
		}

		public virtual void CreateRootAction()
		{
			this.RootAction = new RootAction();
			this.RootAction.Compile(this);
		}

		public virtual SortAction CreateSortAction()
		{
			SortAction sortAction = new SortAction();
			sortAction.Compile(this);
			return sortAction;
		}

		public virtual TemplateAction CreateTemplateAction()
		{
			TemplateAction templateAction = new TemplateAction();
			templateAction.Compile(this);
			return templateAction;
		}

		public virtual TemplateAction CreateSingleTemplateAction()
		{
			TemplateAction templateAction = new TemplateAction();
			templateAction.CompileSingle(this);
			return templateAction;
		}

		public virtual TextAction CreateTextAction()
		{
			TextAction textAction = new TextAction();
			textAction.Compile(this);
			return textAction;
		}

		public virtual UseAttributeSetsAction CreateUseAttributeSetsAction()
		{
			UseAttributeSetsAction useAttributeSetsAction = new UseAttributeSetsAction();
			useAttributeSetsAction.Compile(this);
			return useAttributeSetsAction;
		}

		public virtual ValueOfAction CreateValueOfAction()
		{
			ValueOfAction valueOfAction = new ValueOfAction();
			valueOfAction.Compile(this);
			return valueOfAction;
		}

		public virtual VariableAction CreateVariableAction(VariableType type)
		{
			VariableAction variableAction = new VariableAction(type);
			variableAction.Compile(this);
			if (variableAction.VarKey != -1)
			{
				return variableAction;
			}
			return null;
		}

		public virtual WithParamAction CreateWithParamAction()
		{
			WithParamAction withParamAction = new WithParamAction();
			withParamAction.Compile(this);
			return withParamAction;
		}

		public virtual BeginEvent CreateBeginEvent()
		{
			return new BeginEvent(this);
		}

		public virtual TextEvent CreateTextEvent()
		{
			return new TextEvent(this);
		}

		public XsltException UnexpectedKeyword()
		{
			XPathNavigator xpathNavigator = this.Input.Navigator.Clone();
			string name = xpathNavigator.Name;
			xpathNavigator.MoveToParent();
			string name2 = xpathNavigator.Name;
			return XsltException.Create("'{0}' cannot be a child of the '{1}' element.", new string[] { name, name2 });
		}

		internal const int InvalidQueryKey = -1;

		internal const double RootPriority = 0.5;

		internal StringBuilder AvtStringBuilder = new StringBuilder();

		private int stylesheetid;

		private InputScope rootScope;

		private XmlResolver xmlResolver;

		private TemplateBaseAction currentTemplate;

		private XmlQualifiedName currentMode;

		private Hashtable globalNamespaceAliasTable;

		private Stack stylesheets;

		private HybridDictionary documentURIs = new HybridDictionary();

		private NavigatorInput input;

		private KeywordsTable atoms;

		private InputScopeManager scopeManager;

		internal Stylesheet stylesheet;

		internal Stylesheet rootStylesheet;

		private RootAction rootAction;

		private List<TheQuery> queryStore;

		private QueryBuilder queryBuilder = new QueryBuilder();

		private int rtfCount;

		public bool AllowBuiltInMode;

		public static XmlQualifiedName BuiltInMode = new XmlQualifiedName("*", string.Empty);

		private Hashtable[] _typeDeclsByLang = new Hashtable[]
		{
			new Hashtable(),
			new Hashtable(),
			new Hashtable()
		};

		private ArrayList scriptFiles = new ArrayList();

		private static string[] _defaultNamespaces = new string[] { "System", "System.Collections", "System.Text", "System.Text.RegularExpressions", "System.Xml", "System.Xml.Xsl", "System.Xml.XPath" };

		private static int scriptClassCounter = 0;

		internal class ErrorXPathExpression : CompiledXpathExpr
		{
			public ErrorXPathExpression(string expression, string baseUri, int lineNumber, int linePosition)
				: base(null, expression, false)
			{
				this.baseUri = baseUri;
				this.lineNumber = lineNumber;
				this.linePosition = linePosition;
			}

			public override XPathExpression Clone()
			{
				return this;
			}

			public override void CheckErrors()
			{
				throw new XsltException("'{0}' is an invalid XPath expression.", new string[] { this.Expression }, this.baseUri, this.linePosition, this.lineNumber, null);
			}

			private string baseUri;

			private int lineNumber;

			private int linePosition;
		}
	}
}
