using System;

namespace System.Xml.Xsl.XPath
{
	internal enum LexKind
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
		LastOperator = 15,
		DotDot,
		ColonColon,
		SlashSlash,
		Number,
		Axis,
		Name,
		String,
		Eof,
		FirstStringable = 21,
		LastNonChar = 23,
		LParens = 40,
		RParens,
		LBracket = 91,
		RBracket = 93,
		Dot = 46,
		At = 64,
		Comma = 44,
		Star = 42,
		Slash = 47,
		Dollar = 36,
		RBrace = 125
	}
}
