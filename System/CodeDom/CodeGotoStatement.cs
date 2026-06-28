using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeGotoStatement : CodeStatement
	{
		public CodeGotoStatement()
		{
		}

		public CodeGotoStatement(string label)
		{
			this.Label = label;
		}

		public string Label
		{
			get
			{
				return this.label;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					throw new ArgumentNullException("value");
				}
				this.label = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private string label;
	}
}
