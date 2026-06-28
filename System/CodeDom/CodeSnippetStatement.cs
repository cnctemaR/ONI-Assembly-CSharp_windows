using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeSnippetStatement : CodeStatement
	{
		public CodeSnippetStatement()
		{
		}

		public CodeSnippetStatement(string value)
		{
			this.value = value;
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

		private string value;
	}
}
