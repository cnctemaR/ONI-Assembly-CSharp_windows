using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeEntryPointMethod : CodeMemberMethod
	{
		public CodeEntryPointMethod()
		{
			base.Attributes = (MemberAttributes)24579;
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
