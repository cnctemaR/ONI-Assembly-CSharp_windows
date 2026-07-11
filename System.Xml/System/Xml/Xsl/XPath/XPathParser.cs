using System;
using System.Collections.Generic;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;

namespace System.Xml.Xsl.XPath
{
	internal class XPathParser<Node>
	{
		public Node Parse(XPathScanner scanner, IXPathBuilder<Node> builder, LexKind endLex)
		{
			Node node = default(Node);
			this.scanner = scanner;
			this.builder = builder;
			this.posInfo.Clear();
			try
			{
				builder.StartBuild();
				node = this.ParseExpr();
				scanner.CheckToken(endLex);
			}
			catch (XPathCompileException ex)
			{
				if (ex.queryString == null)
				{
					ex.queryString = scanner.Source;
					this.PopPosInfo(out ex.startChar, out ex.endChar);
				}
				throw;
			}
			finally
			{
				node = builder.EndBuild(node);
			}
			return node;
		}

		internal static bool IsStep(LexKind lexKind)
		{
			return lexKind == LexKind.Dot || lexKind == LexKind.DotDot || lexKind == LexKind.At || lexKind == LexKind.Axis || lexKind == LexKind.Star || lexKind == LexKind.Name;
		}

		private Node ParseLocationPath()
		{
			if (this.scanner.Kind == LexKind.Slash)
			{
				this.scanner.NextLex();
				Node node = this.builder.Axis(XPathAxis.Root, XPathNodeType.All, null, null);
				if (XPathParser<Node>.IsStep(this.scanner.Kind))
				{
					node = this.builder.JoinStep(node, this.ParseRelativeLocationPath());
				}
				return node;
			}
			if (this.scanner.Kind == LexKind.SlashSlash)
			{
				this.scanner.NextLex();
				return this.builder.JoinStep(this.builder.Axis(XPathAxis.Root, XPathNodeType.All, null, null), this.builder.JoinStep(this.builder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativeLocationPath()));
			}
			return this.ParseRelativeLocationPath();
		}

		private Node ParseRelativeLocationPath()
		{
			int num = this.parseRelativePath + 1;
			this.parseRelativePath = num;
			if (num > 1024 && XsltConfigSection.LimitXPathComplexity)
			{
				throw this.scanner.CreateException("The stylesheet is too complex.", Array.Empty<string>());
			}
			Node node = this.ParseStep();
			if (this.scanner.Kind == LexKind.Slash)
			{
				this.scanner.NextLex();
				node = this.builder.JoinStep(node, this.ParseRelativeLocationPath());
			}
			else if (this.scanner.Kind == LexKind.SlashSlash)
			{
				this.scanner.NextLex();
				node = this.builder.JoinStep(node, this.builder.JoinStep(this.builder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativeLocationPath()));
			}
			this.parseRelativePath--;
			return node;
		}

		private Node ParseStep()
		{
			Node node;
			if (LexKind.Dot == this.scanner.Kind)
			{
				this.scanner.NextLex();
				node = this.builder.Axis(XPathAxis.Self, XPathNodeType.All, null, null);
				if (LexKind.LBracket == this.scanner.Kind)
				{
					throw this.scanner.CreateException("Abbreviated step '.' cannot be followed by a predicate. Use the full form 'self::node()[predicate]' instead.", Array.Empty<string>());
				}
			}
			else if (LexKind.DotDot == this.scanner.Kind)
			{
				this.scanner.NextLex();
				node = this.builder.Axis(XPathAxis.Parent, XPathNodeType.All, null, null);
				if (LexKind.LBracket == this.scanner.Kind)
				{
					throw this.scanner.CreateException("Abbreviated step '..' cannot be followed by a predicate. Use the full form 'parent::node()[predicate]' instead.", Array.Empty<string>());
				}
			}
			else
			{
				LexKind kind = this.scanner.Kind;
				XPathAxis xpathAxis;
				if (kind <= LexKind.Name)
				{
					if (kind == LexKind.Axis)
					{
						xpathAxis = this.scanner.Axis;
						this.scanner.NextLex();
						this.scanner.NextLex();
						goto IL_012D;
					}
					if (kind != LexKind.Name)
					{
						goto IL_0108;
					}
				}
				else if (kind != LexKind.Star)
				{
					if (kind != LexKind.At)
					{
						goto IL_0108;
					}
					xpathAxis = XPathAxis.Attribute;
					this.scanner.NextLex();
					goto IL_012D;
				}
				xpathAxis = XPathAxis.Child;
				goto IL_012D;
				IL_0108:
				throw this.scanner.CreateException("Unexpected token '{0}' in the expression.", new string[] { this.scanner.RawValue });
				IL_012D:
				node = this.ParseNodeTest(xpathAxis);
				while (LexKind.LBracket == this.scanner.Kind)
				{
					node = this.builder.Predicate(node, this.ParsePredicate(), XPathParser<Node>.IsReverseAxis(xpathAxis));
				}
			}
			return node;
		}

		private static bool IsReverseAxis(XPathAxis axis)
		{
			return axis == XPathAxis.Ancestor || axis == XPathAxis.Preceding || axis == XPathAxis.AncestorOrSelf || axis == XPathAxis.PrecedingSibling;
		}

		private Node ParseNodeTest(XPathAxis axis)
		{
			int lexStart = this.scanner.LexStart;
			XPathNodeType xpathNodeType;
			string text;
			string text2;
			XPathParser<Node>.InternalParseNodeTest(this.scanner, axis, out xpathNodeType, out text, out text2);
			this.PushPosInfo(lexStart, this.scanner.PrevLexEnd);
			Node node = this.builder.Axis(axis, xpathNodeType, text, text2);
			this.PopPosInfo();
			return node;
		}

		private static bool IsNodeType(XPathScanner scanner)
		{
			return scanner.Prefix.Length == 0 && (scanner.Name == "node" || scanner.Name == "text" || scanner.Name == "processing-instruction" || scanner.Name == "comment");
		}

		private static XPathNodeType PrincipalNodeType(XPathAxis axis)
		{
			if (axis == XPathAxis.Attribute)
			{
				return XPathNodeType.Attribute;
			}
			if (axis != XPathAxis.Namespace)
			{
				return XPathNodeType.Element;
			}
			return XPathNodeType.Namespace;
		}

		internal static void InternalParseNodeTest(XPathScanner scanner, XPathAxis axis, out XPathNodeType nodeType, out string nodePrefix, out string nodeName)
		{
			LexKind kind = scanner.Kind;
			if (kind != LexKind.Name)
			{
				if (kind != LexKind.Star)
				{
					throw scanner.CreateException("Expected a node test, found '{0}'.", new string[] { scanner.RawValue });
				}
				nodePrefix = null;
				nodeName = null;
				nodeType = XPathParser<Node>.PrincipalNodeType(axis);
				scanner.NextLex();
				return;
			}
			else
			{
				if (scanner.CanBeFunction && XPathParser<Node>.IsNodeType(scanner))
				{
					nodePrefix = null;
					nodeName = null;
					string name = scanner.Name;
					if (!(name == "comment"))
					{
						if (!(name == "text"))
						{
							if (!(name == "node"))
							{
								nodeType = XPathNodeType.ProcessingInstruction;
							}
							else
							{
								nodeType = XPathNodeType.All;
							}
						}
						else
						{
							nodeType = XPathNodeType.Text;
						}
					}
					else
					{
						nodeType = XPathNodeType.Comment;
					}
					scanner.NextLex();
					scanner.PassToken(LexKind.LParens);
					if (nodeType == XPathNodeType.ProcessingInstruction && scanner.Kind != LexKind.RParens)
					{
						scanner.CheckToken(LexKind.String);
						nodePrefix = string.Empty;
						nodeName = scanner.StringValue;
						scanner.NextLex();
					}
					scanner.PassToken(LexKind.RParens);
					return;
				}
				nodePrefix = scanner.Prefix;
				nodeName = scanner.Name;
				nodeType = XPathParser<Node>.PrincipalNodeType(axis);
				scanner.NextLex();
				if (nodeName == "*")
				{
					nodeName = null;
					return;
				}
				return;
			}
		}

		private Node ParsePredicate()
		{
			this.scanner.PassToken(LexKind.LBracket);
			Node node = this.ParseExpr();
			this.scanner.PassToken(LexKind.RBracket);
			return node;
		}

		private Node ParseExpr()
		{
			return this.ParseSubExpr(0);
		}

		private Node ParseSubExpr(int callerPrec)
		{
			int num = this.parseSubExprDepth + 1;
			this.parseSubExprDepth = num;
			if (num > 1024 && XsltConfigSection.LimitXPathComplexity)
			{
				throw this.scanner.CreateException("The stylesheet is too complex.", Array.Empty<string>());
			}
			Node node;
			if (this.scanner.Kind == LexKind.Minus)
			{
				XPathOperator xpathOperator = XPathOperator.UnaryMinus;
				int num2 = XPathParser<Node>.XPathOperatorPrecedence[(int)xpathOperator];
				this.scanner.NextLex();
				node = this.builder.Operator(xpathOperator, this.ParseSubExpr(num2), default(Node));
			}
			else
			{
				node = this.ParseUnionExpr();
			}
			for (;;)
			{
				XPathOperator xpathOperator = (XPathOperator)((this.scanner.Kind <= LexKind.Union) ? this.scanner.Kind : LexKind.Unknown);
				int num3 = XPathParser<Node>.XPathOperatorPrecedence[(int)xpathOperator];
				if (num3 <= callerPrec)
				{
					break;
				}
				this.scanner.NextLex();
				node = this.builder.Operator(xpathOperator, node, this.ParseSubExpr(num3));
			}
			this.parseSubExprDepth--;
			return node;
		}

		private Node ParseUnionExpr()
		{
			int num = this.scanner.LexStart;
			Node node = this.ParsePathExpr();
			if (this.scanner.Kind == LexKind.Union)
			{
				this.PushPosInfo(num, this.scanner.PrevLexEnd);
				node = this.builder.Operator(XPathOperator.Union, default(Node), node);
				this.PopPosInfo();
				while (this.scanner.Kind == LexKind.Union)
				{
					this.scanner.NextLex();
					num = this.scanner.LexStart;
					Node node2 = this.ParsePathExpr();
					this.PushPosInfo(num, this.scanner.PrevLexEnd);
					node = this.builder.Operator(XPathOperator.Union, node, node2);
					this.PopPosInfo();
				}
			}
			return node;
		}

		private Node ParsePathExpr()
		{
			if (this.IsPrimaryExpr())
			{
				int lexStart = this.scanner.LexStart;
				Node node = this.ParseFilterExpr();
				int prevLexEnd = this.scanner.PrevLexEnd;
				if (this.scanner.Kind == LexKind.Slash)
				{
					this.scanner.NextLex();
					this.PushPosInfo(lexStart, prevLexEnd);
					node = this.builder.JoinStep(node, this.ParseRelativeLocationPath());
					this.PopPosInfo();
				}
				else if (this.scanner.Kind == LexKind.SlashSlash)
				{
					this.scanner.NextLex();
					this.PushPosInfo(lexStart, prevLexEnd);
					node = this.builder.JoinStep(node, this.builder.JoinStep(this.builder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativeLocationPath()));
					this.PopPosInfo();
				}
				return node;
			}
			return this.ParseLocationPath();
		}

		private Node ParseFilterExpr()
		{
			int lexStart = this.scanner.LexStart;
			Node node = this.ParsePrimaryExpr();
			int prevLexEnd = this.scanner.PrevLexEnd;
			while (this.scanner.Kind == LexKind.LBracket)
			{
				this.PushPosInfo(lexStart, prevLexEnd);
				node = this.builder.Predicate(node, this.ParsePredicate(), false);
				this.PopPosInfo();
			}
			return node;
		}

		private bool IsPrimaryExpr()
		{
			return this.scanner.Kind == LexKind.String || this.scanner.Kind == LexKind.Number || this.scanner.Kind == LexKind.Dollar || this.scanner.Kind == LexKind.LParens || (this.scanner.Kind == LexKind.Name && this.scanner.CanBeFunction && !XPathParser<Node>.IsNodeType(this.scanner));
		}

		private Node ParsePrimaryExpr()
		{
			LexKind kind = this.scanner.Kind;
			Node node;
			if (kind <= LexKind.String)
			{
				if (kind == LexKind.Number)
				{
					node = this.builder.Number(XPathConvert.StringToDouble(this.scanner.RawValue));
					this.scanner.NextLex();
					return node;
				}
				if (kind == LexKind.String)
				{
					node = this.builder.String(this.scanner.StringValue);
					this.scanner.NextLex();
					return node;
				}
			}
			else
			{
				if (kind == LexKind.Dollar)
				{
					int lexStart = this.scanner.LexStart;
					this.scanner.NextLex();
					this.scanner.CheckToken(LexKind.Name);
					this.PushPosInfo(lexStart, this.scanner.LexStart + this.scanner.LexSize);
					node = this.builder.Variable(this.scanner.Prefix, this.scanner.Name);
					this.PopPosInfo();
					this.scanner.NextLex();
					return node;
				}
				if (kind == LexKind.LParens)
				{
					this.scanner.NextLex();
					node = this.ParseExpr();
					this.scanner.PassToken(LexKind.RParens);
					return node;
				}
			}
			node = this.ParseFunctionCall();
			return node;
		}

		private Node ParseFunctionCall()
		{
			List<Node> list = new List<Node>();
			string name = this.scanner.Name;
			string prefix = this.scanner.Prefix;
			int lexStart = this.scanner.LexStart;
			this.scanner.PassToken(LexKind.Name);
			this.scanner.PassToken(LexKind.LParens);
			if (this.scanner.Kind != LexKind.RParens)
			{
				for (;;)
				{
					list.Add(this.ParseExpr());
					if (this.scanner.Kind != LexKind.Comma)
					{
						break;
					}
					this.scanner.NextLex();
				}
				this.scanner.CheckToken(LexKind.RParens);
			}
			this.scanner.NextLex();
			this.PushPosInfo(lexStart, this.scanner.PrevLexEnd);
			Node node = this.builder.Function(prefix, name, list);
			this.PopPosInfo();
			return node;
		}

		private void PushPosInfo(int startChar, int endChar)
		{
			this.posInfo.Push(startChar);
			this.posInfo.Push(endChar);
		}

		private void PopPosInfo()
		{
			this.posInfo.Pop();
			this.posInfo.Pop();
		}

		private void PopPosInfo(out int startChar, out int endChar)
		{
			endChar = this.posInfo.Pop();
			startChar = this.posInfo.Pop();
		}

		private XPathScanner scanner;

		private IXPathBuilder<Node> builder;

		private Stack<int> posInfo = new Stack<int>();

		private const int MaxParseRelativePathDepth = 1024;

		private int parseRelativePath;

		private const int MaxParseSubExprDepth = 1024;

		private int parseSubExprDepth;

		private static int[] XPathOperatorPrecedence = new int[]
		{
			0, 1, 2, 3, 3, 4, 4, 4, 4, 5,
			5, 6, 6, 6, 7, 8
		};
	}
}
