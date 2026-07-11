using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeFieldReferenceExpression : CodeExpression
	{
		public CodeFieldReferenceExpression()
		{
		}

		public CodeFieldReferenceExpression(CodeExpression targetObject, string fieldName)
		{
			this.TargetObject = targetObject;
			this.FieldName = fieldName;
		}

		public CodeExpression TargetObject { get; set; }

		public string FieldName
		{
			get
			{
				return this._fieldName ?? string.Empty;
			}
			set
			{
				this._fieldName = value;
			}
		}

		private string _fieldName;
	}
}
