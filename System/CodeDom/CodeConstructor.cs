using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeConstructor : CodeMemberMethod
	{
		public CodeConstructor()
		{
			base.Name = ".ctor";
		}

		public CodeExpressionCollection BaseConstructorArgs { get; } = new CodeExpressionCollection();

		public CodeExpressionCollection ChainedConstructorArgs { get; } = new CodeExpressionCollection();
	}
}
