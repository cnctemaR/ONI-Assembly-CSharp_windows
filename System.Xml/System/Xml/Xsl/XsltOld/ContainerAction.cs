using System;
using System.Collections;
using System.Globalization;
using System.Xml.XPath;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.XsltOld
{
	internal class ContainerAction : CompiledAction
	{
		internal override void Compile(Compiler compiler)
		{
			throw new NotImplementedException();
		}

		internal void CompileStylesheetAttributes(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			string localName = input.LocalName;
			string text = null;
			string text2 = null;
			if (input.MoveToFirstAttribute())
			{
				for (;;)
				{
					string namespaceURI = input.NamespaceURI;
					string localName2 = input.LocalName;
					if (namespaceURI.Length == 0)
					{
						if (Ref.Equal(localName2, input.Atoms.Version))
						{
							text2 = input.Value;
							if (1.0 <= XmlConvert.ToXPathDouble(text2))
							{
								compiler.ForwardCompatibility = text2 != "1.0";
							}
							else if (!compiler.ForwardCompatibility)
							{
								break;
							}
						}
						else if (Ref.Equal(localName2, input.Atoms.ExtensionElementPrefixes))
						{
							compiler.InsertExtensionNamespace(input.Value);
						}
						else if (Ref.Equal(localName2, input.Atoms.ExcludeResultPrefixes))
						{
							compiler.InsertExcludedNamespace(input.Value);
						}
						else if (!Ref.Equal(localName2, input.Atoms.Id))
						{
							text = localName2;
						}
					}
					if (!input.MoveToNextAttribute())
					{
						goto Block_8;
					}
				}
				throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "version", text2 });
				Block_8:
				input.ToParent();
			}
			if (text2 == null)
			{
				throw XsltException.Create("Missing mandatory attribute '{0}'.", new string[] { "version" });
			}
			if (text != null && !compiler.ForwardCompatibility)
			{
				throw XsltException.Create("'{0}' is an invalid attribute for the '{1}' element.", new string[] { text, localName });
			}
		}

		internal void CompileSingleTemplate(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			string text = null;
			if (input.MoveToFirstAttribute())
			{
				do
				{
					string namespaceURI = input.NamespaceURI;
					string localName = input.LocalName;
					if (Ref.Equal(namespaceURI, input.Atoms.UriXsl) && Ref.Equal(localName, input.Atoms.Version))
					{
						text = input.Value;
					}
				}
				while (input.MoveToNextAttribute());
				input.ToParent();
			}
			if (text != null)
			{
				compiler.AddTemplate(compiler.CreateSingleTemplateAction());
				return;
			}
			if (Ref.Equal(input.LocalName, input.Atoms.Stylesheet) && input.NamespaceURI == "http://www.w3.org/TR/WD-xsl")
			{
				throw XsltException.Create("The 'http://www.w3.org/TR/WD-xsl' namespace is no longer supported.", Array.Empty<string>());
			}
			throw XsltException.Create("Stylesheet must start either with an 'xsl:stylesheet' or an 'xsl:transform' element, or with a literal result element that has an 'xsl:version' attribute, where prefix 'xsl' denotes the 'http://www.w3.org/1999/XSL/Transform' namespace.", Array.Empty<string>());
		}

		protected void CompileDocument(Compiler compiler, bool inInclude)
		{
			NavigatorInput input = compiler.Input;
			while (input.NodeType != XPathNodeType.Element)
			{
				if (!compiler.Advance())
				{
					throw XsltException.Create("Stylesheet must start either with an 'xsl:stylesheet' or an 'xsl:transform' element, or with a literal result element that has an 'xsl:version' attribute, where prefix 'xsl' denotes the 'http://www.w3.org/1999/XSL/Transform' namespace.", Array.Empty<string>());
				}
			}
			if (Ref.Equal(input.NamespaceURI, input.Atoms.UriXsl))
			{
				if (!Ref.Equal(input.LocalName, input.Atoms.Stylesheet) && !Ref.Equal(input.LocalName, input.Atoms.Transform))
				{
					throw XsltException.Create("Stylesheet must start either with an 'xsl:stylesheet' or an 'xsl:transform' element, or with a literal result element that has an 'xsl:version' attribute, where prefix 'xsl' denotes the 'http://www.w3.org/1999/XSL/Transform' namespace.", Array.Empty<string>());
				}
				compiler.PushNamespaceScope();
				this.CompileStylesheetAttributes(compiler);
				this.CompileTopLevelElements(compiler);
				if (!inInclude)
				{
					this.CompileImports(compiler);
				}
			}
			else
			{
				compiler.PushLiteralScope();
				this.CompileSingleTemplate(compiler);
			}
			compiler.PopScope();
		}

		internal Stylesheet CompileImport(Compiler compiler, Uri uri, int id)
		{
			NavigatorInput navigatorInput = compiler.ResolveDocument(uri);
			compiler.PushInputDocument(navigatorInput);
			try
			{
				compiler.PushStylesheet(new Stylesheet());
				compiler.Stylesheetid = id;
				this.CompileDocument(compiler, false);
			}
			catch (XsltCompileException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new XsltCompileException(ex, navigatorInput.BaseURI, navigatorInput.LineNumber, navigatorInput.LinePosition);
			}
			finally
			{
				compiler.PopInputDocument();
			}
			return compiler.PopStylesheet();
		}

		private void CompileImports(Compiler compiler)
		{
			ArrayList imports = compiler.CompiledStylesheet.Imports;
			int stylesheetid = compiler.Stylesheetid;
			int num = imports.Count - 1;
			while (0 <= num)
			{
				Uri uri = imports[num] as Uri;
				ArrayList arrayList = imports;
				int num2 = num;
				Uri uri2 = uri;
				int num3 = this.maxid + 1;
				this.maxid = num3;
				arrayList[num2] = this.CompileImport(compiler, uri2, num3);
				num--;
			}
			compiler.Stylesheetid = stylesheetid;
		}

		private void CompileInclude(Compiler compiler)
		{
			Uri uri = compiler.ResolveUri(compiler.GetSingleAttribute(compiler.Input.Atoms.Href));
			string text = uri.ToString();
			if (compiler.IsCircularReference(text))
			{
				throw XsltException.Create("Stylesheet '{0}' cannot directly or indirectly include or import itself.", new string[] { text });
			}
			NavigatorInput navigatorInput = compiler.ResolveDocument(uri);
			compiler.PushInputDocument(navigatorInput);
			try
			{
				this.CompileDocument(compiler, true);
			}
			catch (XsltCompileException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new XsltCompileException(ex, navigatorInput.BaseURI, navigatorInput.LineNumber, navigatorInput.LinePosition);
			}
			finally
			{
				compiler.PopInputDocument();
			}
			base.CheckEmpty(compiler);
		}

		internal void CompileNamespaceAlias(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			string localName = input.LocalName;
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			if (input.MoveToFirstAttribute())
			{
				string localName2;
				for (;;)
				{
					string namespaceURI = input.NamespaceURI;
					localName2 = input.LocalName;
					if (namespaceURI.Length == 0)
					{
						if (Ref.Equal(localName2, input.Atoms.StylesheetPrefix))
						{
							text3 = input.Value;
							text = compiler.GetNsAlias(ref text3);
						}
						else if (Ref.Equal(localName2, input.Atoms.ResultPrefix))
						{
							text4 = input.Value;
							text2 = compiler.GetNsAlias(ref text4);
						}
						else if (!compiler.ForwardCompatibility)
						{
							break;
						}
					}
					if (!input.MoveToNextAttribute())
					{
						goto Block_5;
					}
				}
				throw XsltException.Create("'{0}' is an invalid attribute for the '{1}' element.", new string[] { localName2, localName });
				Block_5:
				input.ToParent();
			}
			base.CheckRequiredAttribute(compiler, text, "stylesheet-prefix");
			base.CheckRequiredAttribute(compiler, text2, "result-prefix");
			base.CheckEmpty(compiler);
			compiler.AddNamespaceAlias(text, new NamespaceInfo(text4, text2, compiler.Stylesheetid));
		}

		internal void CompileKey(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			string localName = input.LocalName;
			int num = -1;
			int num2 = -1;
			XmlQualifiedName xmlQualifiedName = null;
			if (input.MoveToFirstAttribute())
			{
				string localName2;
				for (;;)
				{
					string namespaceURI = input.NamespaceURI;
					localName2 = input.LocalName;
					string value = input.Value;
					if (namespaceURI.Length == 0)
					{
						if (Ref.Equal(localName2, input.Atoms.Name))
						{
							xmlQualifiedName = compiler.CreateXPathQName(value);
						}
						else if (Ref.Equal(localName2, input.Atoms.Match))
						{
							num = compiler.AddQuery(value, false, false, true);
						}
						else if (Ref.Equal(localName2, input.Atoms.Use))
						{
							num2 = compiler.AddQuery(value, false, false, false);
						}
						else if (!compiler.ForwardCompatibility)
						{
							break;
						}
					}
					if (!input.MoveToNextAttribute())
					{
						goto Block_6;
					}
				}
				throw XsltException.Create("'{0}' is an invalid attribute for the '{1}' element.", new string[] { localName2, localName });
				Block_6:
				input.ToParent();
			}
			base.CheckRequiredAttribute(compiler, num != -1, "match");
			base.CheckRequiredAttribute(compiler, num2 != -1, "use");
			base.CheckRequiredAttribute(compiler, xmlQualifiedName != null, "name");
			compiler.InsertKey(xmlQualifiedName, num, num2);
		}

		protected void CompileDecimalFormat(Compiler compiler)
		{
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			DecimalFormat decimalFormat = new DecimalFormat(numberFormatInfo, '#', '0', ';');
			XmlQualifiedName xmlQualifiedName = null;
			NavigatorInput input = compiler.Input;
			if (input.MoveToFirstAttribute())
			{
				do
				{
					if (input.Prefix.Length == 0)
					{
						string localName = input.LocalName;
						string value = input.Value;
						if (Ref.Equal(localName, input.Atoms.Name))
						{
							xmlQualifiedName = compiler.CreateXPathQName(value);
						}
						else if (Ref.Equal(localName, input.Atoms.DecimalSeparator))
						{
							numberFormatInfo.NumberDecimalSeparator = value;
						}
						else if (Ref.Equal(localName, input.Atoms.GroupingSeparator))
						{
							numberFormatInfo.NumberGroupSeparator = value;
						}
						else if (Ref.Equal(localName, input.Atoms.Infinity))
						{
							numberFormatInfo.PositiveInfinitySymbol = value;
						}
						else if (Ref.Equal(localName, input.Atoms.MinusSign))
						{
							numberFormatInfo.NegativeSign = value;
						}
						else if (Ref.Equal(localName, input.Atoms.NaN))
						{
							numberFormatInfo.NaNSymbol = value;
						}
						else if (Ref.Equal(localName, input.Atoms.Percent))
						{
							numberFormatInfo.PercentSymbol = value;
						}
						else if (Ref.Equal(localName, input.Atoms.PerMille))
						{
							numberFormatInfo.PerMilleSymbol = value;
						}
						else if (Ref.Equal(localName, input.Atoms.Digit))
						{
							if (this.CheckAttribute(value.Length == 1, compiler))
							{
								decimalFormat.digit = value[0];
							}
						}
						else if (Ref.Equal(localName, input.Atoms.ZeroDigit))
						{
							if (this.CheckAttribute(value.Length == 1, compiler))
							{
								decimalFormat.zeroDigit = value[0];
							}
						}
						else if (Ref.Equal(localName, input.Atoms.PatternSeparator) && this.CheckAttribute(value.Length == 1, compiler))
						{
							decimalFormat.patternSeparator = value[0];
						}
					}
				}
				while (input.MoveToNextAttribute());
				input.ToParent();
			}
			numberFormatInfo.NegativeInfinitySymbol = numberFormatInfo.NegativeSign + numberFormatInfo.PositiveInfinitySymbol;
			if (xmlQualifiedName == null)
			{
				xmlQualifiedName = new XmlQualifiedName();
			}
			compiler.AddDecimalFormat(xmlQualifiedName, decimalFormat);
			base.CheckEmpty(compiler);
		}

		internal bool CheckAttribute(bool valid, Compiler compiler)
		{
			if (valid)
			{
				return true;
			}
			if (!compiler.ForwardCompatibility)
			{
				throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[]
				{
					compiler.Input.LocalName,
					compiler.Input.Value
				});
			}
			return false;
		}

		protected void CompileSpace(Compiler compiler, bool preserve)
		{
			string[] array = XmlConvert.SplitString(compiler.GetSingleAttribute(compiler.Input.Atoms.Elements));
			for (int i = 0; i < array.Length; i++)
			{
				double num = this.NameTest(array[i]);
				compiler.CompiledStylesheet.AddSpace(compiler, array[i], num, preserve);
			}
			base.CheckEmpty(compiler);
		}

		private double NameTest(string name)
		{
			if (name == "*")
			{
				return -0.5;
			}
			int num = name.Length - 2;
			if (0 > num || name[num] != ':' || name[num + 1] != '*')
			{
				string text;
				string text2;
				PrefixQName.ParseQualifiedName(name, out text, out text2);
				return 0.0;
			}
			if (!PrefixQName.ValidatePrefix(name.Substring(0, num)))
			{
				throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "elements", name });
			}
			return -0.25;
		}

		protected void CompileTopLevelElements(Compiler compiler)
		{
			if (!compiler.Recurse())
			{
				return;
			}
			NavigatorInput input = compiler.Input;
			bool flag = false;
			string text;
			for (;;)
			{
				XPathNodeType nodeType = input.NodeType;
				if (nodeType != XPathNodeType.Element)
				{
					if (nodeType - XPathNodeType.SignificantWhitespace > 3)
					{
						break;
					}
				}
				else
				{
					string localName = input.LocalName;
					string namespaceURI = input.NamespaceURI;
					if (Ref.Equal(namespaceURI, input.Atoms.UriXsl))
					{
						if (Ref.Equal(localName, input.Atoms.Import))
						{
							if (flag)
							{
								goto Block_6;
							}
							Uri uri = compiler.ResolveUri(compiler.GetSingleAttribute(compiler.Input.Atoms.Href));
							text = uri.ToString();
							if (compiler.IsCircularReference(text))
							{
								goto Block_7;
							}
							compiler.CompiledStylesheet.Imports.Add(uri);
							base.CheckEmpty(compiler);
						}
						else if (Ref.Equal(localName, input.Atoms.Include))
						{
							flag = true;
							this.CompileInclude(compiler);
						}
						else
						{
							flag = true;
							compiler.PushNamespaceScope();
							if (Ref.Equal(localName, input.Atoms.StripSpace))
							{
								this.CompileSpace(compiler, false);
							}
							else if (Ref.Equal(localName, input.Atoms.PreserveSpace))
							{
								this.CompileSpace(compiler, true);
							}
							else if (Ref.Equal(localName, input.Atoms.Output))
							{
								this.CompileOutput(compiler);
							}
							else if (Ref.Equal(localName, input.Atoms.Key))
							{
								this.CompileKey(compiler);
							}
							else if (Ref.Equal(localName, input.Atoms.DecimalFormat))
							{
								this.CompileDecimalFormat(compiler);
							}
							else if (Ref.Equal(localName, input.Atoms.NamespaceAlias))
							{
								this.CompileNamespaceAlias(compiler);
							}
							else if (Ref.Equal(localName, input.Atoms.AttributeSet))
							{
								compiler.AddAttributeSet(compiler.CreateAttributeSetAction());
							}
							else if (Ref.Equal(localName, input.Atoms.Variable))
							{
								VariableAction variableAction = compiler.CreateVariableAction(VariableType.GlobalVariable);
								if (variableAction != null)
								{
									this.AddAction(variableAction);
								}
							}
							else if (Ref.Equal(localName, input.Atoms.Param))
							{
								VariableAction variableAction2 = compiler.CreateVariableAction(VariableType.GlobalParameter);
								if (variableAction2 != null)
								{
									this.AddAction(variableAction2);
								}
							}
							else if (Ref.Equal(localName, input.Atoms.Template))
							{
								compiler.AddTemplate(compiler.CreateTemplateAction());
							}
							else if (!compiler.ForwardCompatibility)
							{
								goto Block_21;
							}
							compiler.PopScope();
						}
					}
					else if (namespaceURI == input.Atoms.UrnMsxsl && localName == input.Atoms.Script)
					{
						this.AddScript(compiler);
					}
					else if (namespaceURI.Length == 0)
					{
						goto Block_24;
					}
				}
				if (!compiler.Advance())
				{
					goto Block_25;
				}
			}
			throw XsltException.Create("The contents of '{0}' are invalid.", new string[] { "stylesheet" });
			Block_6:
			throw XsltException.Create("'xsl:import' instructions must precede all other element children of an 'xsl:stylesheet' element.", Array.Empty<string>());
			Block_7:
			throw XsltException.Create("Stylesheet '{0}' cannot directly or indirectly include or import itself.", new string[] { text });
			Block_21:
			throw compiler.UnexpectedKeyword();
			Block_24:
			throw XsltException.Create("Top-level element '{0}' may not have a null namespace URI.", new string[] { input.Name });
			Block_25:
			compiler.ToParent();
		}

		protected void CompileTemplate(Compiler compiler)
		{
			do
			{
				this.CompileOnceTemplate(compiler);
			}
			while (compiler.Advance());
		}

		protected void CompileOnceTemplate(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			if (input.NodeType != XPathNodeType.Element)
			{
				this.CompileLiteral(compiler);
				return;
			}
			string namespaceURI = input.NamespaceURI;
			if (Ref.Equal(namespaceURI, input.Atoms.UriXsl))
			{
				compiler.PushNamespaceScope();
				this.CompileInstruction(compiler);
				compiler.PopScope();
				return;
			}
			compiler.PushLiteralScope();
			compiler.InsertExtensionNamespace();
			if (compiler.IsExtensionNamespace(namespaceURI))
			{
				this.AddAction(compiler.CreateNewInstructionAction());
			}
			else
			{
				this.CompileLiteral(compiler);
			}
			compiler.PopScope();
		}

		private void CompileInstruction(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			string localName = input.LocalName;
			CompiledAction compiledAction;
			if (Ref.Equal(localName, input.Atoms.ApplyImports))
			{
				compiledAction = compiler.CreateApplyImportsAction();
			}
			else if (Ref.Equal(localName, input.Atoms.ApplyTemplates))
			{
				compiledAction = compiler.CreateApplyTemplatesAction();
			}
			else if (Ref.Equal(localName, input.Atoms.Attribute))
			{
				compiledAction = compiler.CreateAttributeAction();
			}
			else if (Ref.Equal(localName, input.Atoms.CallTemplate))
			{
				compiledAction = compiler.CreateCallTemplateAction();
			}
			else if (Ref.Equal(localName, input.Atoms.Choose))
			{
				compiledAction = compiler.CreateChooseAction();
			}
			else if (Ref.Equal(localName, input.Atoms.Comment))
			{
				compiledAction = compiler.CreateCommentAction();
			}
			else if (Ref.Equal(localName, input.Atoms.Copy))
			{
				compiledAction = compiler.CreateCopyAction();
			}
			else if (Ref.Equal(localName, input.Atoms.CopyOf))
			{
				compiledAction = compiler.CreateCopyOfAction();
			}
			else if (Ref.Equal(localName, input.Atoms.Element))
			{
				compiledAction = compiler.CreateElementAction();
			}
			else
			{
				if (Ref.Equal(localName, input.Atoms.Fallback))
				{
					return;
				}
				if (Ref.Equal(localName, input.Atoms.ForEach))
				{
					compiledAction = compiler.CreateForEachAction();
				}
				else if (Ref.Equal(localName, input.Atoms.If))
				{
					compiledAction = compiler.CreateIfAction(IfAction.ConditionType.ConditionIf);
				}
				else if (Ref.Equal(localName, input.Atoms.Message))
				{
					compiledAction = compiler.CreateMessageAction();
				}
				else if (Ref.Equal(localName, input.Atoms.Number))
				{
					compiledAction = compiler.CreateNumberAction();
				}
				else if (Ref.Equal(localName, input.Atoms.ProcessingInstruction))
				{
					compiledAction = compiler.CreateProcessingInstructionAction();
				}
				else if (Ref.Equal(localName, input.Atoms.Text))
				{
					compiledAction = compiler.CreateTextAction();
				}
				else if (Ref.Equal(localName, input.Atoms.ValueOf))
				{
					compiledAction = compiler.CreateValueOfAction();
				}
				else if (Ref.Equal(localName, input.Atoms.Variable))
				{
					compiledAction = compiler.CreateVariableAction(VariableType.LocalVariable);
				}
				else
				{
					if (!compiler.ForwardCompatibility)
					{
						throw compiler.UnexpectedKeyword();
					}
					compiledAction = compiler.CreateNewInstructionAction();
				}
			}
			this.AddAction(compiledAction);
		}

		private void CompileLiteral(Compiler compiler)
		{
			switch (compiler.Input.NodeType)
			{
			case XPathNodeType.Element:
				this.AddEvent(compiler.CreateBeginEvent());
				this.CompileLiteralAttributesAndNamespaces(compiler);
				if (compiler.Recurse())
				{
					this.CompileTemplate(compiler);
					compiler.ToParent();
				}
				this.AddEvent(new EndEvent(XPathNodeType.Element));
				return;
			case XPathNodeType.Attribute:
			case XPathNodeType.Namespace:
			case XPathNodeType.Whitespace:
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
				break;
			case XPathNodeType.Text:
			case XPathNodeType.SignificantWhitespace:
				this.AddEvent(compiler.CreateTextEvent());
				break;
			default:
				return;
			}
		}

		private void CompileLiteralAttributesAndNamespaces(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			if (input.Navigator.MoveToAttribute("use-attribute-sets", input.Atoms.UriXsl))
			{
				this.AddAction(compiler.CreateUseAttributeSetsAction());
				input.Navigator.MoveToParent();
			}
			compiler.InsertExcludedNamespace();
			if (input.MoveToFirstNamespace())
			{
				do
				{
					string value = input.Value;
					if (!(value == "http://www.w3.org/1999/XSL/Transform") && !compiler.IsExcludedNamespace(value) && !compiler.IsExtensionNamespace(value) && !compiler.IsNamespaceAlias(value))
					{
						this.AddEvent(new NamespaceEvent(input));
					}
				}
				while (input.MoveToNextNamespace());
				input.ToParent();
			}
			if (input.MoveToFirstAttribute())
			{
				do
				{
					if (!Ref.Equal(input.NamespaceURI, input.Atoms.UriXsl))
					{
						this.AddEvent(compiler.CreateBeginEvent());
						this.AddEvents(compiler.CompileAvt(input.Value));
						this.AddEvent(new EndEvent(XPathNodeType.Attribute));
					}
				}
				while (input.MoveToNextAttribute());
				input.ToParent();
			}
		}

		private void CompileOutput(Compiler compiler)
		{
			compiler.RootAction.Output.Compile(compiler);
		}

		internal void AddAction(Action action)
		{
			if (this.containedActions == null)
			{
				this.containedActions = new ArrayList();
			}
			this.containedActions.Add(action);
			this.lastCopyCodeAction = null;
		}

		private void EnsureCopyCodeAction()
		{
			if (this.lastCopyCodeAction == null)
			{
				CopyCodeAction copyCodeAction = new CopyCodeAction();
				this.AddAction(copyCodeAction);
				this.lastCopyCodeAction = copyCodeAction;
			}
		}

		protected void AddEvent(Event copyEvent)
		{
			this.EnsureCopyCodeAction();
			this.lastCopyCodeAction.AddEvent(copyEvent);
		}

		protected void AddEvents(ArrayList copyEvents)
		{
			this.EnsureCopyCodeAction();
			this.lastCopyCodeAction.AddEvents(copyEvents);
		}

		private void AddScript(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			ScriptingLanguage scriptingLanguage = ScriptingLanguage.JScript;
			string text = null;
			if (input.MoveToFirstAttribute())
			{
				string value;
				for (;;)
				{
					if (input.LocalName == input.Atoms.Language)
					{
						value = input.Value;
						if (string.Compare(value, "jscript", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "javascript", StringComparison.OrdinalIgnoreCase) == 0)
						{
							scriptingLanguage = ScriptingLanguage.JScript;
						}
						else if (string.Compare(value, "c#", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "csharp", StringComparison.OrdinalIgnoreCase) == 0)
						{
							scriptingLanguage = ScriptingLanguage.CSharp;
						}
						else
						{
							if (string.Compare(value, "vb", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(value, "visualbasic", StringComparison.OrdinalIgnoreCase) != 0)
							{
								break;
							}
							scriptingLanguage = ScriptingLanguage.VisualBasic;
						}
					}
					else if (input.LocalName == input.Atoms.ImplementsPrefix)
					{
						if (!PrefixQName.ValidatePrefix(input.Value))
						{
							goto Block_6;
						}
						text = compiler.ResolveXmlNamespace(input.Value);
					}
					if (!input.MoveToNextAttribute())
					{
						goto Block_7;
					}
				}
				throw XsltException.Create("Scripting language '{0}' is not supported.", new string[] { value });
				Block_6:
				throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { input.LocalName, input.Value });
				Block_7:
				input.ToParent();
			}
			if (text == null)
			{
				throw XsltException.Create("Missing mandatory attribute '{0}'.", new string[] { input.Atoms.ImplementsPrefix });
			}
			if (!input.Recurse() || input.NodeType != XPathNodeType.Text)
			{
				throw XsltException.Create("The 'msxsl:script' element cannot be empty.", Array.Empty<string>());
			}
			compiler.AddScript(input.Value, scriptingLanguage, text, input.BaseURI, input.LineNumber);
			input.ToParent();
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state != 0)
			{
				if (state != 1)
				{
					return;
				}
				frame.Finished();
				return;
			}
			else
			{
				if (this.containedActions != null && this.containedActions.Count > 0)
				{
					processor.PushActionFrame(frame);
					frame.State = 1;
					return;
				}
				frame.Finished();
				return;
			}
		}

		internal Action GetAction(int actionIndex)
		{
			if (this.containedActions != null && actionIndex < this.containedActions.Count)
			{
				return (Action)this.containedActions[actionIndex];
			}
			return null;
		}

		internal void CheckDuplicateParams(XmlQualifiedName name)
		{
			if (this.containedActions != null)
			{
				foreach (object obj in this.containedActions)
				{
					WithParamAction withParamAction = ((CompiledAction)obj) as WithParamAction;
					if (withParamAction != null && withParamAction.Name == name)
					{
						throw XsltException.Create("Value of parameter '{0}' cannot be specified more than once within a single 'xsl:call-template' or 'xsl:apply-templates' element.", new string[] { name.ToString() });
					}
				}
			}
		}

		internal override void ReplaceNamespaceAlias(Compiler compiler)
		{
			if (this.containedActions == null)
			{
				return;
			}
			int count = this.containedActions.Count;
			for (int i = 0; i < this.containedActions.Count; i++)
			{
				((Action)this.containedActions[i]).ReplaceNamespaceAlias(compiler);
			}
		}

		internal ArrayList containedActions;

		internal CopyCodeAction lastCopyCodeAction;

		private int maxid;

		protected const int ProcessingChildren = 1;
	}
}
