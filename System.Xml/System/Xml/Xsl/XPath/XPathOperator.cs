using System;

namespace System.Xml.Xsl.XPath
{
	internal enum XPathOperator
	{
		Unknown,
		Or,
		And,
		Eq,
		Ne,
		Lt,
		Le,
		Gt,
		Ge,
		Plus,
		Minus,
		Multiply,
		Divide,
		Modulo,
		UnaryMinus,
		Union,
		LastXPath1Operator = 15,
		UnaryPlus,
		Idiv,
		Is,
		After,
		Before,
		Range,
		Except,
		Intersect,
		ValEq,
		ValNe,
		ValLt,
		ValLe,
		ValGt,
		ValGe
	}
}
