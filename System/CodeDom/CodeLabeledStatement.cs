using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeLabeledStatement : CodeStatement
	{
		public CodeLabeledStatement()
		{
		}

		public CodeLabeledStatement(string label)
		{
			this.label = label;
		}

		public CodeLabeledStatement(string label, CodeStatement statement)
		{
			this.label = label;
			this.statement = statement;
		}

		public string Label
		{
			get
			{
				if (this.label == null)
				{
					return string.Empty;
				}
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}

		public CodeStatement Statement
		{
			get
			{
				return this.statement;
			}
			set
			{
				this.statement = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private string label;

		private CodeStatement statement;
	}
}
