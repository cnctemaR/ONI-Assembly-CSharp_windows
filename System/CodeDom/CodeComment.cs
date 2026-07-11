using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeComment : CodeObject
	{
		public CodeComment()
		{
		}

		public CodeComment(string text)
		{
			this.text = text;
		}

		public CodeComment(string text, bool docComment)
		{
			this.text = text;
			this.docComment = docComment;
		}

		public bool DocComment
		{
			get
			{
				return this.docComment;
			}
			set
			{
				this.docComment = value;
			}
		}

		public string Text
		{
			get
			{
				if (this.text == null)
				{
					return string.Empty;
				}
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		private bool docComment;

		private string text;
	}
}
