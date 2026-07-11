using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodePropertySetValueReferenceExpression : CodeExpression
	{
		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
