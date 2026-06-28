using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeMemberField : CodeTypeMember
	{
		public CodeMemberField()
		{
		}

		public CodeMemberField(CodeTypeReference type, string name)
		{
			this.type = type;
			base.Name = name;
		}

		public CodeMemberField(string type, string name)
		{
			this.type = new CodeTypeReference(type);
			base.Name = name;
		}

		public CodeMemberField(Type type, string name)
		{
			this.type = new CodeTypeReference(type);
			base.Name = name;
		}

		public CodeExpression InitExpression
		{
			get
			{
				return this.initExpression;
			}
			set
			{
				this.initExpression = value;
			}
		}

		public CodeTypeReference Type
		{
			get
			{
				if (this.type == null)
				{
					this.type = new CodeTypeReference(string.Empty);
				}
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeExpression initExpression;

		private CodeTypeReference type;
	}
}
