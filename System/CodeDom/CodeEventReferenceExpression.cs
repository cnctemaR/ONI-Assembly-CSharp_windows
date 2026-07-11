using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeEventReferenceExpression : CodeExpression
	{
		public CodeEventReferenceExpression()
		{
		}

		public CodeEventReferenceExpression(CodeExpression targetObject, string eventName)
		{
			this.targetObject = targetObject;
			this.eventName = eventName;
		}

		public string EventName
		{
			get
			{
				if (this.eventName == null)
				{
					return string.Empty;
				}
				return this.eventName;
			}
			set
			{
				this.eventName = value;
			}
		}

		public CodeExpression TargetObject
		{
			get
			{
				return this.targetObject;
			}
			set
			{
				this.targetObject = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private string eventName;

		private CodeExpression targetObject;
	}
}
