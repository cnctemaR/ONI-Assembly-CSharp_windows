using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodePropertyReferenceExpression : CodeExpression
	{
		public CodePropertyReferenceExpression()
		{
		}

		public CodePropertyReferenceExpression(CodeExpression targetObject, string propertyName)
		{
			this.TargetObject = targetObject;
			this.PropertyName = propertyName;
		}

		public CodeExpression TargetObject { get; set; }

		public string PropertyName
		{
			get
			{
				return this._propertyName ?? string.Empty;
			}
			set
			{
				this._propertyName = value;
			}
		}

		private string _propertyName;
	}
}
