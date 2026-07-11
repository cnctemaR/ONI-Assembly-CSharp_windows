using System;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class XslNodeEx : XslNode
	{
		public XslNodeEx(XslNodeType t, QilName name, object arg, XsltInput.ContextInfo ctxInfo, XslVersion xslVer)
			: base(t, name, arg, xslVer)
		{
			this.ElemNameLi = ctxInfo.elemNameLi;
			this.EndTagLi = ctxInfo.endTagLi;
		}

		public XslNodeEx(XslNodeType t, QilName name, object arg, XslVersion xslVer)
			: base(t, name, arg, xslVer)
		{
		}

		public readonly ISourceLineInfo ElemNameLi;

		public readonly ISourceLineInfo EndTagLi;
	}
}
