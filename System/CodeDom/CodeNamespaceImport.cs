using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeNamespaceImport : CodeObject
	{
		public CodeNamespaceImport()
		{
		}

		public CodeNamespaceImport(string nameSpace)
		{
			this.Namespace = nameSpace;
		}

		public CodeLinePragma LinePragma { get; set; }

		public string Namespace
		{
			get
			{
				return this._nameSpace ?? string.Empty;
			}
			set
			{
				this._nameSpace = value;
			}
		}

		private string _nameSpace;
	}
}
