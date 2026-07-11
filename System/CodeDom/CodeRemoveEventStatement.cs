using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeRemoveEventStatement : CodeStatement
	{
		public CodeRemoveEventStatement()
		{
		}

		public CodeRemoveEventStatement(CodeEventReferenceExpression eventRef, CodeExpression listener)
		{
			this._eventRef = eventRef;
			this.Listener = listener;
		}

		public CodeRemoveEventStatement(CodeExpression targetObject, string eventName, CodeExpression listener)
		{
			this._eventRef = new CodeEventReferenceExpression(targetObject, eventName);
			this.Listener = listener;
		}

		public CodeEventReferenceExpression Event
		{
			get
			{
				CodeEventReferenceExpression codeEventReferenceExpression;
				if ((codeEventReferenceExpression = this._eventRef) == null)
				{
					codeEventReferenceExpression = (this._eventRef = new CodeEventReferenceExpression());
				}
				return codeEventReferenceExpression;
			}
			set
			{
				this._eventRef = value;
			}
		}

		public CodeExpression Listener { get; set; }

		private CodeEventReferenceExpression _eventRef;
	}
}
