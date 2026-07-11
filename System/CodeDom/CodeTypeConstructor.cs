using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeTypeConstructor : CodeMemberMethod
	{
		public CodeTypeConstructor()
		{
			base.Name = ".cctor";
		}
	}
}
