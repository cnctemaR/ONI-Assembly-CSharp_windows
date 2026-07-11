using System;
using System.Collections.Specialized;

namespace System.CodeDom
{
	[Serializable]
	public class CodeCompileUnit : CodeObject
	{
		public CodeNamespaceCollection Namespaces { get; } = new CodeNamespaceCollection();

		public StringCollection ReferencedAssemblies
		{
			get
			{
				StringCollection stringCollection;
				if ((stringCollection = this._assemblies) == null)
				{
					stringCollection = (this._assemblies = new StringCollection());
				}
				return stringCollection;
			}
		}

		public CodeAttributeDeclarationCollection AssemblyCustomAttributes
		{
			get
			{
				CodeAttributeDeclarationCollection codeAttributeDeclarationCollection;
				if ((codeAttributeDeclarationCollection = this._attributes) == null)
				{
					codeAttributeDeclarationCollection = (this._attributes = new CodeAttributeDeclarationCollection());
				}
				return codeAttributeDeclarationCollection;
			}
		}

		public CodeDirectiveCollection StartDirectives
		{
			get
			{
				CodeDirectiveCollection codeDirectiveCollection;
				if ((codeDirectiveCollection = this._startDirectives) == null)
				{
					codeDirectiveCollection = (this._startDirectives = new CodeDirectiveCollection());
				}
				return codeDirectiveCollection;
			}
		}

		public CodeDirectiveCollection EndDirectives
		{
			get
			{
				CodeDirectiveCollection codeDirectiveCollection;
				if ((codeDirectiveCollection = this._endDirectives) == null)
				{
					codeDirectiveCollection = (this._endDirectives = new CodeDirectiveCollection());
				}
				return codeDirectiveCollection;
			}
		}

		private StringCollection _assemblies;

		private CodeAttributeDeclarationCollection _attributes;

		private CodeDirectiveCollection _startDirectives;

		private CodeDirectiveCollection _endDirectives;
	}
}
