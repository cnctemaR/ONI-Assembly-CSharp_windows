using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeMemberField : CodeTypeMember
	{
		public CodeMemberField()
		{
		}

		public CodeMemberField(CodeTypeReference type, string name)
		{
			this.Type = type;
			base.Name = name;
		}

		public CodeMemberField(string type, string name)
		{
			this.Type = new CodeTypeReference(type);
			base.Name = name;
		}

		public CodeMemberField(Type type, string name)
		{
			this.Type = new CodeTypeReference(type);
			base.Name = name;
		}

		public CodeTypeReference Type
		{
			get
			{
				CodeTypeReference codeTypeReference;
				if ((codeTypeReference = this._type) == null)
				{
					codeTypeReference = (this._type = new CodeTypeReference(""));
				}
				return codeTypeReference;
			}
			set
			{
				this._type = value;
			}
		}

		public CodeExpression InitExpression { get; set; }

		private CodeTypeReference _type;
	}
}
