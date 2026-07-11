using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class InputScopeManager
	{
		public InputScopeManager(XPathNavigator navigator, InputScope rootScope)
		{
			this.navigator = navigator;
			this.scopeStack = rootScope;
		}

		internal InputScope CurrentScope
		{
			get
			{
				return this.scopeStack;
			}
		}

		internal InputScope VariableScope
		{
			get
			{
				return this.scopeStack.Parent;
			}
		}

		internal InputScopeManager Clone()
		{
			return new InputScopeManager(this.navigator, null)
			{
				scopeStack = this.scopeStack,
				defaultNS = this.defaultNS
			};
		}

		public XPathNavigator Navigator
		{
			get
			{
				return this.navigator;
			}
		}

		internal InputScope PushScope()
		{
			this.scopeStack = new InputScope(this.scopeStack);
			return this.scopeStack;
		}

		internal void PopScope()
		{
			if (this.scopeStack == null)
			{
				return;
			}
			for (NamespaceDecl namespaceDecl = this.scopeStack.Scopes; namespaceDecl != null; namespaceDecl = namespaceDecl.Next)
			{
				this.defaultNS = namespaceDecl.PrevDefaultNsUri;
			}
			this.scopeStack = this.scopeStack.Parent;
		}

		internal void PushNamespace(string prefix, string nspace)
		{
			this.scopeStack.AddNamespace(prefix, nspace, this.defaultNS);
			if (prefix == null || prefix.Length == 0)
			{
				this.defaultNS = nspace;
			}
		}

		public string DefaultNamespace
		{
			get
			{
				return this.defaultNS;
			}
		}

		private string ResolveNonEmptyPrefix(string prefix)
		{
			if (prefix == "xml")
			{
				return "http://www.w3.org/XML/1998/namespace";
			}
			if (prefix == "xmlns")
			{
				return "http://www.w3.org/2000/xmlns/";
			}
			for (InputScope parent = this.scopeStack; parent != null; parent = parent.Parent)
			{
				string text = parent.ResolveNonAtom(prefix);
				if (text != null)
				{
					return text;
				}
			}
			throw XsltException.Create("Prefix '{0}' is not defined.", new string[] { prefix });
		}

		public string ResolveXmlNamespace(string prefix)
		{
			if (prefix.Length == 0)
			{
				return this.defaultNS;
			}
			return this.ResolveNonEmptyPrefix(prefix);
		}

		public string ResolveXPathNamespace(string prefix)
		{
			if (prefix.Length == 0)
			{
				return string.Empty;
			}
			return this.ResolveNonEmptyPrefix(prefix);
		}

		internal void InsertExtensionNamespaces(string[] nsList)
		{
			for (int i = 0; i < nsList.Length; i++)
			{
				this.scopeStack.InsertExtensionNamespace(nsList[i]);
			}
		}

		internal bool IsExtensionNamespace(string nspace)
		{
			for (InputScope parent = this.scopeStack; parent != null; parent = parent.Parent)
			{
				if (parent.IsExtensionNamespace(nspace))
				{
					return true;
				}
			}
			return false;
		}

		internal void InsertExcludedNamespaces(string[] nsList)
		{
			for (int i = 0; i < nsList.Length; i++)
			{
				this.scopeStack.InsertExcludedNamespace(nsList[i]);
			}
		}

		internal bool IsExcludedNamespace(string nspace)
		{
			for (InputScope parent = this.scopeStack; parent != null; parent = parent.Parent)
			{
				if (parent.IsExcludedNamespace(nspace))
				{
					return true;
				}
			}
			return false;
		}

		private InputScope scopeStack;

		private string defaultNS = string.Empty;

		private XPathNavigator navigator;
	}
}
