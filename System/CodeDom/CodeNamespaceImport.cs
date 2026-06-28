using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeNamespaceImport : CodeObject
	{
		public CodeNamespaceImport()
		{
		}

		public CodeNamespaceImport(string nameSpace)
		{
			this.nameSpace = nameSpace;
		}

		public CodeLinePragma LinePragma
		{
			get
			{
				return this.linePragma;
			}
			set
			{
				this.linePragma = value;
			}
		}

		public string Namespace
		{
			get
			{
				if (this.nameSpace == null)
				{
					return string.Empty;
				}
				return this.nameSpace;
			}
			set
			{
				this.nameSpace = value;
			}
		}

		private CodeLinePragma linePragma;

		private string nameSpace;
	}
}
