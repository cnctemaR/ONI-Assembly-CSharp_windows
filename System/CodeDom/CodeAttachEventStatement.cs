using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeAttachEventStatement : CodeStatement
	{
		public CodeAttachEventStatement()
		{
		}

		public CodeAttachEventStatement(CodeEventReferenceExpression eventRef, CodeExpression listener)
		{
			this._eventRef = eventRef;
			this.Listener = listener;
		}

		public CodeAttachEventStatement(CodeExpression targetObject, string eventName, CodeExpression listener)
			: this(new CodeEventReferenceExpression(targetObject, eventName), listener)
		{
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
