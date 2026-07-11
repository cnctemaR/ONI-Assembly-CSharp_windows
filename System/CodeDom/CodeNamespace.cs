using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeNamespace : CodeObject
	{
		public event EventHandler PopulateComments;

		public event EventHandler PopulateImports;

		public event EventHandler PopulateTypes;

		public CodeNamespace()
		{
		}

		public CodeNamespace(string name)
		{
			this.Name = name;
		}

		public CodeTypeDeclarationCollection Types
		{
			get
			{
				if ((this._populated & 4) == 0)
				{
					this._populated |= 4;
					EventHandler populateTypes = this.PopulateTypes;
					if (populateTypes != null)
					{
						populateTypes(this, EventArgs.Empty);
					}
				}
				return this._classes;
			}
		}

		public CodeNamespaceImportCollection Imports
		{
			get
			{
				if ((this._populated & 1) == 0)
				{
					this._populated |= 1;
					EventHandler populateImports = this.PopulateImports;
					if (populateImports != null)
					{
						populateImports(this, EventArgs.Empty);
					}
				}
				return this._imports;
			}
		}

		public string Name
		{
			get
			{
				return this._name ?? string.Empty;
			}
			set
			{
				this._name = value;
			}
		}

		public CodeCommentStatementCollection Comments
		{
			get
			{
				if ((this._populated & 2) == 0)
				{
					this._populated |= 2;
					EventHandler populateComments = this.PopulateComments;
					if (populateComments != null)
					{
						populateComments(this, EventArgs.Empty);
					}
				}
				return this._comments;
			}
		}

		private string _name;

		private readonly CodeNamespaceImportCollection _imports = new CodeNamespaceImportCollection();

		private readonly CodeCommentStatementCollection _comments = new CodeCommentStatementCollection();

		private readonly CodeTypeDeclarationCollection _classes = new CodeTypeDeclarationCollection();

		private int _populated;

		private const int ImportsCollection = 1;

		private const int CommentsCollection = 2;

		private const int TypesCollection = 4;
	}
}
