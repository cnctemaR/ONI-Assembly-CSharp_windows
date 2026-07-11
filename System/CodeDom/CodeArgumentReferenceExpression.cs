using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeArgumentReferenceExpression : CodeExpression
	{
		public CodeArgumentReferenceExpression()
		{
		}

		public CodeArgumentReferenceExpression(string parameterName)
		{
			this._parameterName = parameterName;
		}

		public string ParameterName
		{
			get
			{
				return this._parameterName ?? string.Empty;
			}
			set
			{
				this._parameterName = value;
			}
		}

		private string _parameterName;
	}
}
