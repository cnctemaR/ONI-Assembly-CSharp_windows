using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeTypeConstructor : CodeMemberMethod
	{
		public CodeTypeConstructor()
		{
			base.Name = ".cctor";
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
