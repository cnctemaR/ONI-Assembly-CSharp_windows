using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeAttributeArgument
	{
		public CodeAttributeArgument()
		{
		}

		public CodeAttributeArgument(CodeExpression value)
		{
			this.value = value;
		}

		public CodeAttributeArgument(string name, CodeExpression value)
		{
			this.name = name;
			this.value = value;
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

		public CodeExpression Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private string name;

		private CodeExpression value;
	}
}
