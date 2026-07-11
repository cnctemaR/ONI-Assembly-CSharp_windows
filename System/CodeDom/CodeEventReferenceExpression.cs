using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeEventReferenceExpression : CodeExpression
	{
		public CodeEventReferenceExpression()
		{
		}

		public CodeEventReferenceExpression(CodeExpression targetObject, string eventName)
		{
			this.TargetObject = targetObject;
			this._eventName = eventName;
		}

		public CodeExpression TargetObject { get; set; }

		public string EventName
		{
			get
			{
				return this._eventName ?? string.Empty;
			}
			set
			{
				this._eventName = value;
			}
		}

		private string _eventName;
	}
}
