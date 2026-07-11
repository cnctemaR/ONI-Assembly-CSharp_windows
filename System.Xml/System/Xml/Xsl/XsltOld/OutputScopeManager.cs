using System;
using System.Globalization;

namespace System.Xml.Xsl.XsltOld
{
	internal class OutputScopeManager
	{
		internal string DefaultNamespace
		{
			get
			{
				return this.defaultNS;
			}
		}

		internal OutputScope CurrentElementScope
		{
			get
			{
				return (OutputScope)this.elementScopesStack.Peek();
			}
		}

		internal XmlSpace XmlSpace
		{
			get
			{
				return this.CurrentElementScope.Space;
			}
		}

		internal string XmlLang
		{
			get
			{
				return this.CurrentElementScope.Lang;
			}
		}

		internal OutputScopeManager(XmlNameTable nameTable, OutKeywords atoms)
		{
			this.elementScopesStack = new HWStack(10);
			this.nameTable = nameTable;
			this.atoms = atoms;
			this.defaultNS = this.atoms.Empty;
			OutputScope outputScope = (OutputScope)this.elementScopesStack.Push();
			if (outputScope == null)
			{
				outputScope = new OutputScope();
				this.elementScopesStack.AddToTop(outputScope);
			}
			outputScope.Init(string.Empty, string.Empty, string.Empty, XmlSpace.None, string.Empty, false);
		}

		internal void PushNamespace(string prefix, string nspace)
		{
			this.CurrentElementScope.AddNamespace(prefix, nspace, this.defaultNS);
			if (prefix == null || prefix.Length == 0)
			{
				this.defaultNS = nspace;
			}
		}

		internal void PushScope(string name, string nspace, string prefix)
		{
			OutputScope currentElementScope = this.CurrentElementScope;
			OutputScope outputScope = (OutputScope)this.elementScopesStack.Push();
			if (outputScope == null)
			{
				outputScope = new OutputScope();
				this.elementScopesStack.AddToTop(outputScope);
			}
			outputScope.Init(name, nspace, prefix, currentElementScope.Space, currentElementScope.Lang, currentElementScope.Mixed);
		}

		internal void PopScope()
		{
			for (NamespaceDecl namespaceDecl = ((OutputScope)this.elementScopesStack.Pop()).Scopes; namespaceDecl != null; namespaceDecl = namespaceDecl.Next)
			{
				this.defaultNS = namespaceDecl.PrevDefaultNsUri;
			}
		}

		internal string ResolveNamespace(string prefix)
		{
			bool flag;
			return this.ResolveNamespace(prefix, out flag);
		}

		internal string ResolveNamespace(string prefix, out bool thisScope)
		{
			thisScope = true;
			if (prefix == null || prefix.Length == 0)
			{
				return this.defaultNS;
			}
			if (Ref.Equal(prefix, this.atoms.Xml))
			{
				return this.atoms.XmlNamespace;
			}
			if (Ref.Equal(prefix, this.atoms.Xmlns))
			{
				return this.atoms.XmlnsNamespace;
			}
			for (int i = this.elementScopesStack.Length - 1; i >= 0; i--)
			{
				string text = ((OutputScope)this.elementScopesStack[i]).ResolveAtom(prefix);
				if (text != null)
				{
					thisScope = i == this.elementScopesStack.Length - 1;
					return text;
				}
			}
			return null;
		}

		internal bool FindPrefix(string nspace, out string prefix)
		{
			int num = this.elementScopesStack.Length - 1;
			while (0 <= num)
			{
				OutputScope outputScope = (OutputScope)this.elementScopesStack[num];
				string text = null;
				if (outputScope.FindPrefix(nspace, out text))
				{
					string text2 = this.ResolveNamespace(text);
					if (text2 != null && Ref.Equal(text2, nspace))
					{
						prefix = text;
						return true;
					}
					break;
				}
				else
				{
					num--;
				}
			}
			prefix = null;
			return false;
		}

		internal string GeneratePrefix(string format)
		{
			string text;
			do
			{
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				int num = this.prefixIndex;
				this.prefixIndex = num + 1;
				text = string.Format(invariantCulture, format, num);
			}
			while (this.nameTable.Get(text) != null);
			return this.nameTable.Add(text);
		}

		private const int STACK_INCREMENT = 10;

		private HWStack elementScopesStack;

		private string defaultNS;

		private OutKeywords atoms;

		private XmlNameTable nameTable;

		private int prefixIndex;
	}
}
