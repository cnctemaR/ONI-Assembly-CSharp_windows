using System;
using System.Collections;

namespace System.Xml.Xsl.XsltOld
{
	internal class InputScope : DocumentScope
	{
		internal InputScope Parent
		{
			get
			{
				return this.parent;
			}
		}

		internal Hashtable Variables
		{
			get
			{
				return this.variables;
			}
		}

		internal bool ForwardCompatibility
		{
			get
			{
				return this.forwardCompatibility;
			}
			set
			{
				this.forwardCompatibility = value;
			}
		}

		internal bool CanHaveApplyImports
		{
			get
			{
				return this.canHaveApplyImports;
			}
			set
			{
				this.canHaveApplyImports = value;
			}
		}

		internal InputScope(InputScope parent)
		{
			this.Init(parent);
		}

		internal void Init(InputScope parent)
		{
			this.scopes = null;
			this.parent = parent;
			if (this.parent != null)
			{
				this.forwardCompatibility = this.parent.forwardCompatibility;
				this.canHaveApplyImports = this.parent.canHaveApplyImports;
			}
		}

		internal void InsertExtensionNamespace(string nspace)
		{
			if (this.extensionNamespaces == null)
			{
				this.extensionNamespaces = new Hashtable();
			}
			this.extensionNamespaces[nspace] = null;
		}

		internal bool IsExtensionNamespace(string nspace)
		{
			return this.extensionNamespaces != null && this.extensionNamespaces.Contains(nspace);
		}

		internal void InsertExcludedNamespace(string nspace)
		{
			if (this.excludedNamespaces == null)
			{
				this.excludedNamespaces = new Hashtable();
			}
			this.excludedNamespaces[nspace] = null;
		}

		internal bool IsExcludedNamespace(string nspace)
		{
			return this.excludedNamespaces != null && this.excludedNamespaces.Contains(nspace);
		}

		internal void InsertVariable(VariableAction variable)
		{
			if (this.variables == null)
			{
				this.variables = new Hashtable();
			}
			this.variables[variable.Name] = variable;
		}

		internal int GetVeriablesCount()
		{
			if (this.variables == null)
			{
				return 0;
			}
			return this.variables.Count;
		}

		public VariableAction ResolveVariable(XmlQualifiedName qname)
		{
			for (InputScope inputScope = this; inputScope != null; inputScope = inputScope.Parent)
			{
				if (inputScope.Variables != null)
				{
					VariableAction variableAction = (VariableAction)inputScope.Variables[qname];
					if (variableAction != null)
					{
						return variableAction;
					}
				}
			}
			return null;
		}

		public VariableAction ResolveGlobalVariable(XmlQualifiedName qname)
		{
			InputScope inputScope = null;
			for (InputScope inputScope2 = this; inputScope2 != null; inputScope2 = inputScope2.Parent)
			{
				inputScope = inputScope2;
			}
			return inputScope.ResolveVariable(qname);
		}

		private InputScope parent;

		private bool forwardCompatibility;

		private bool canHaveApplyImports;

		private Hashtable variables;

		private Hashtable extensionNamespaces;

		private Hashtable excludedNamespaces;
	}
}
