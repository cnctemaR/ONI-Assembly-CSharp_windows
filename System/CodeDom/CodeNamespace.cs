using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeNamespace : CodeObject
	{
		public CodeNamespace()
		{
		}

		public CodeNamespace(string name)
		{
			this.name = name;
		}

		public event EventHandler PopulateComments;

		public event EventHandler PopulateImports;

		public event EventHandler PopulateTypes;

		public CodeCommentStatementCollection Comments
		{
			get
			{
				if (this.comments == null)
				{
					this.comments = new CodeCommentStatementCollection();
					if (this.PopulateComments != null)
					{
						this.PopulateComments(this, EventArgs.Empty);
					}
				}
				return this.comments;
			}
		}

		public CodeNamespaceImportCollection Imports
		{
			get
			{
				if (this.imports == null)
				{
					this.imports = new CodeNamespaceImportCollection();
					if (this.PopulateImports != null)
					{
						this.PopulateImports(this, EventArgs.Empty);
					}
				}
				return this.imports;
			}
		}

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					return string.Empty;
				}
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public CodeTypeDeclarationCollection Types
		{
			get
			{
				if (this.classes == null)
				{
					this.classes = new CodeTypeDeclarationCollection();
					if (this.PopulateTypes != null)
					{
						this.PopulateTypes(this, EventArgs.Empty);
					}
				}
				return this.classes;
			}
		}

		private CodeCommentStatementCollection comments;

		private CodeNamespaceImportCollection imports;

		private CodeNamespaceCollection namespaces;

		private CodeTypeDeclarationCollection classes;

		private string name;

		private int populated;
	}
}
