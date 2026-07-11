using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeAttributeArgument
	{
		public CodeAttributeArgument()
		{
		}

		public CodeAttributeArgument(CodeExpression value)
		{
			this.Value = value;
		}

		public CodeAttributeArgument(string name, CodeExpression value)
		{
			this.Name = name;
			this.Value = value;
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

		public CodeExpression Value { get; set; }

		private string _name;
	}
}
