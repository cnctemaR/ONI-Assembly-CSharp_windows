using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeVariableReferenceExpression : CodeExpression
	{
		public CodeVariableReferenceExpression()
		{
		}

		public CodeVariableReferenceExpression(string variableName)
		{
			this._variableName = variableName;
		}

		public string VariableName
		{
			get
			{
				return this._variableName ?? string.Empty;
			}
			set
			{
				this._variableName = value;
			}
		}

		private string _variableName;
	}
}
