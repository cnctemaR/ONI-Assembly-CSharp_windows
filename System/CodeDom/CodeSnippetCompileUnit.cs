using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeSnippetCompileUnit : CodeCompileUnit
	{
		public CodeSnippetCompileUnit()
		{
		}

		public CodeSnippetCompileUnit(string value)
		{
			this.value = value;
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

		public string Value
		{
			get
			{
				if (this.value == null)
				{
					return string.Empty;
				}
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private CodeLinePragma linePragma;

		private string value;
	}
}
