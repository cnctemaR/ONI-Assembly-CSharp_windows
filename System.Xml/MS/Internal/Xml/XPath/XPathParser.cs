using System;
using System.Collections;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class XPathParser
	{
		private XPathParser(XPathScanner scanner)
		{
			this.scanner = scanner;
		}

		public static AstNode ParseXPathExpresion(string xpathExpresion)
		{
			XPathScanner xpathScanner = new XPathScanner(xpathExpresion);
			AstNode astNode = new XPathParser(xpathScanner).ParseExpresion(null);
			if (xpathScanner.Kind != XPathScanner.LexKind.Eof)
			{
				throw XPathException.Create("'{0}' has an invalid token.", xpathScanner.SourceText);
			}
			return astNode;
		}

		public static AstNode ParseXPathPattern(string xpathPattern)
		{
			XPathScanner xpathScanner = new XPathScanner(xpathPattern);
			AstNode astNode = new XPathParser(xpathScanner).ParsePattern(null);
			if (xpathScanner.Kind != XPathScanner.LexKind.Eof)
			{
				throw XPathException.Create("'{0}' has an invalid token.", xpathScanner.SourceText);
			}
			return astNode;
		}

		private AstNode ParseExpresion(AstNode qyInput)
		{
			int num = this.parseDepth + 1;
			this.parseDepth = num;
			if (num > 200)
			{
				throw XPathException.Create("The xpath query is too complex.");
			}
			AstNode astNode = this.ParseOrExpr(qyInput);
			this.parseDepth--;
			return astNode;
		}

		private AstNode ParseOrExpr(AstNode qyInput)
		{
			AstNode astNode = this.ParseAndExpr(qyInput);
			while (this.TestOp("or"))
			{
				this.NextLex();
				astNode = new Operator(Operator.Op.OR, astNode, this.ParseAndExpr(qyInput));
			}
			return astNode;
		}

		private AstNode ParseAndExpr(AstNode qyInput)
		{
			AstNode astNode = this.ParseEqualityExpr(qyInput);
			while (this.TestOp("and"))
			{
				this.NextLex();
				astNode = new Operator(Operator.Op.AND, astNode, this.ParseEqualityExpr(qyInput));
			}
			return astNode;
		}

		private AstNode ParseEqualityExpr(AstNode qyInput)
		{
			AstNode astNode = this.ParseRelationalExpr(qyInput);
			for (;;)
			{
				Operator.Op op = ((this.scanner.Kind == XPathScanner.LexKind.Eq) ? Operator.Op.EQ : ((this.scanner.Kind == XPathScanner.LexKind.Ne) ? Operator.Op.NE : Operator.Op.INVALID));
				if (op == Operator.Op.INVALID)
				{
					break;
				}
				this.NextLex();
				astNode = new Operator(op, astNode, this.ParseRelationalExpr(qyInput));
			}
			return astNode;
		}

		private AstNode ParseRelationalExpr(AstNode qyInput)
		{
			AstNode astNode = this.ParseAdditiveExpr(qyInput);
			for (;;)
			{
				Operator.Op op = ((this.scanner.Kind == XPathScanner.LexKind.Lt) ? Operator.Op.LT : ((this.scanner.Kind == XPathScanner.LexKind.Le) ? Operator.Op.LE : ((this.scanner.Kind == XPathScanner.LexKind.Gt) ? Operator.Op.GT : ((this.scanner.Kind == XPathScanner.LexKind.Ge) ? Operator.Op.GE : Operator.Op.INVALID))));
				if (op == Operator.Op.INVALID)
				{
					break;
				}
				this.NextLex();
				astNode = new Operator(op, astNode, this.ParseAdditiveExpr(qyInput));
			}
			return astNode;
		}

		private AstNode ParseAdditiveExpr(AstNode qyInput)
		{
			AstNode astNode = this.ParseMultiplicativeExpr(qyInput);
			for (;;)
			{
				Operator.Op op = ((this.scanner.Kind == XPathScanner.LexKind.Plus) ? Operator.Op.PLUS : ((this.scanner.Kind == XPathScanner.LexKind.Minus) ? Operator.Op.MINUS : Operator.Op.INVALID));
				if (op == Operator.Op.INVALID)
				{
					break;
				}
				this.NextLex();
				astNode = new Operator(op, astNode, this.ParseMultiplicativeExpr(qyInput));
			}
			return astNode;
		}

		private AstNode ParseMultiplicativeExpr(AstNode qyInput)
		{
			AstNode astNode = this.ParseUnaryExpr(qyInput);
			for (;;)
			{
				Operator.Op op = ((this.scanner.Kind == XPathScanner.LexKind.Star) ? Operator.Op.MUL : (this.TestOp("div") ? Operator.Op.DIV : (this.TestOp("mod") ? Operator.Op.MOD : Operator.Op.INVALID)));
				if (op == Operator.Op.INVALID)
				{
					break;
				}
				this.NextLex();
				astNode = new Operator(op, astNode, this.ParseUnaryExpr(qyInput));
			}
			return astNode;
		}

		private AstNode ParseUnaryExpr(AstNode qyInput)
		{
			bool flag = false;
			while (this.scanner.Kind == XPathScanner.LexKind.Minus)
			{
				this.NextLex();
				flag = !flag;
			}
			if (flag)
			{
				return new Operator(Operator.Op.MUL, this.ParseUnionExpr(qyInput), new Operand(-1.0));
			}
			return this.ParseUnionExpr(qyInput);
		}

		private AstNode ParseUnionExpr(AstNode qyInput)
		{
			AstNode astNode = this.ParsePathExpr(qyInput);
			while (this.scanner.Kind == XPathScanner.LexKind.Union)
			{
				this.NextLex();
				AstNode astNode2 = this.ParsePathExpr(qyInput);
				this.CheckNodeSet(astNode.ReturnType);
				this.CheckNodeSet(astNode2.ReturnType);
				astNode = new Operator(Operator.Op.UNION, astNode, astNode2);
			}
			return astNode;
		}

		private static bool IsNodeType(XPathScanner scaner)
		{
			return scaner.Prefix.Length == 0 && (scaner.Name == "node" || scaner.Name == "text" || scaner.Name == "processing-instruction" || scaner.Name == "comment");
		}

		private AstNode ParsePathExpr(AstNode qyInput)
		{
			AstNode astNode;
			if (XPathParser.IsPrimaryExpr(this.scanner))
			{
				astNode = this.ParseFilterExpr(qyInput);
				if (this.scanner.Kind == XPathScanner.LexKind.Slash)
				{
					this.NextLex();
					astNode = this.ParseRelativeLocationPath(astNode);
				}
				else if (this.scanner.Kind == XPathScanner.LexKind.SlashSlash)
				{
					this.NextLex();
					astNode = this.ParseRelativeLocationPath(new Axis(Axis.AxisType.DescendantOrSelf, astNode));
				}
			}
			else
			{
				astNode = this.ParseLocationPath(null);
			}
			return astNode;
		}

		private AstNode ParseFilterExpr(AstNode qyInput)
		{
			AstNode astNode = this.ParsePrimaryExpr(qyInput);
			while (this.scanner.Kind == XPathScanner.LexKind.LBracket)
			{
				astNode = new Filter(astNode, this.ParsePredicate(astNode));
			}
			return astNode;
		}

		private AstNode ParsePredicate(AstNode qyInput)
		{
			this.CheckNodeSet(qyInput.ReturnType);
			this.PassToken(XPathScanner.LexKind.LBracket);
			AstNode astNode = this.ParseExpresion(qyInput);
			this.PassToken(XPathScanner.LexKind.RBracket);
			return astNode;
		}

		private AstNode ParseLocationPath(AstNode qyInput)
		{
			if (this.scanner.Kind == XPathScanner.LexKind.Slash)
			{
				this.NextLex();
				AstNode astNode = new Root();
				if (XPathParser.IsStep(this.scanner.Kind))
				{
					astNode = this.ParseRelativeLocationPath(astNode);
				}
				return astNode;
			}
			if (this.scanner.Kind == XPathScanner.LexKind.SlashSlash)
			{
				this.NextLex();
				return this.ParseRelativeLocationPath(new Axis(Axis.AxisType.DescendantOrSelf, new Root()));
			}
			return this.ParseRelativeLocationPath(qyInput);
		}

		private AstNode ParseRelativeLocationPath(AstNode qyInput)
		{
			AstNode astNode = qyInput;
			for (;;)
			{
				astNode = this.ParseStep(astNode);
				if (XPathScanner.LexKind.SlashSlash == this.scanner.Kind)
				{
					this.NextLex();
					astNode = new Axis(Axis.AxisType.DescendantOrSelf, astNode);
				}
				else
				{
					if (XPathScanner.LexKind.Slash != this.scanner.Kind)
					{
						break;
					}
					this.NextLex();
				}
			}
			return astNode;
		}

		private static bool IsStep(XPathScanner.LexKind lexKind)
		{
			return lexKind == XPathScanner.LexKind.Dot || lexKind == XPathScanner.LexKind.DotDot || lexKind == XPathScanner.LexKind.At || lexKind == XPathScanner.LexKind.Axe || lexKind == XPathScanner.LexKind.Star || lexKind == XPathScanner.LexKind.Name;
		}

		private AstNode ParseStep(AstNode qyInput)
		{
			AstNode astNode;
			if (XPathScanner.LexKind.Dot == this.scanner.Kind)
			{
				this.NextLex();
				astNode = new Axis(Axis.AxisType.Self, qyInput);
			}
			else if (XPathScanner.LexKind.DotDot == this.scanner.Kind)
			{
				this.NextLex();
				astNode = new Axis(Axis.AxisType.Parent, qyInput);
			}
			else
			{
				Axis.AxisType axisType = Axis.AxisType.Child;
				XPathScanner.LexKind kind = this.scanner.Kind;
				if (kind != XPathScanner.LexKind.At)
				{
					if (kind == XPathScanner.LexKind.Axe)
					{
						axisType = this.GetAxis(this.scanner);
						this.NextLex();
					}
				}
				else
				{
					axisType = Axis.AxisType.Attribute;
					this.NextLex();
				}
				XPathNodeType xpathNodeType = ((axisType == Axis.AxisType.Attribute) ? XPathNodeType.Attribute : XPathNodeType.Element);
				astNode = this.ParseNodeTest(qyInput, axisType, xpathNodeType);
				while (XPathScanner.LexKind.LBracket == this.scanner.Kind)
				{
					astNode = new Filter(astNode, this.ParsePredicate(astNode));
				}
			}
			return astNode;
		}

		private AstNode ParseNodeTest(AstNode qyInput, Axis.AxisType axisType, XPathNodeType nodeType)
		{
			XPathScanner.LexKind kind = this.scanner.Kind;
			string text;
			string text2;
			if (kind != XPathScanner.LexKind.Star)
			{
				if (kind != XPathScanner.LexKind.Name)
				{
					throw XPathException.Create("Expression must evaluate to a node-set.", this.scanner.SourceText);
				}
				if (this.scanner.CanBeFunction && XPathParser.IsNodeType(this.scanner))
				{
					text = string.Empty;
					text2 = string.Empty;
					nodeType = ((this.scanner.Name == "comment") ? XPathNodeType.Comment : ((this.scanner.Name == "text") ? XPathNodeType.Text : ((this.scanner.Name == "node") ? XPathNodeType.All : ((this.scanner.Name == "processing-instruction") ? XPathNodeType.ProcessingInstruction : XPathNodeType.Root))));
					this.NextLex();
					this.PassToken(XPathScanner.LexKind.LParens);
					if (nodeType == XPathNodeType.ProcessingInstruction && this.scanner.Kind != XPathScanner.LexKind.RParens)
					{
						this.CheckToken(XPathScanner.LexKind.String);
						text2 = this.scanner.StringValue;
						this.NextLex();
					}
					this.PassToken(XPathScanner.LexKind.RParens);
				}
				else
				{
					text = this.scanner.Prefix;
					text2 = this.scanner.Name;
					this.NextLex();
					if (text2 == "*")
					{
						text2 = string.Empty;
					}
				}
			}
			else
			{
				text = string.Empty;
				text2 = string.Empty;
				this.NextLex();
			}
			return new Axis(axisType, qyInput, text, text2, nodeType);
		}

		private static bool IsPrimaryExpr(XPathScanner scanner)
		{
			return scanner.Kind == XPathScanner.LexKind.String || scanner.Kind == XPathScanner.LexKind.Number || scanner.Kind == XPathScanner.LexKind.Dollar || scanner.Kind == XPathScanner.LexKind.LParens || (scanner.Kind == XPathScanner.LexKind.Name && scanner.CanBeFunction && !XPathParser.IsNodeType(scanner));
		}

		private AstNode ParsePrimaryExpr(AstNode qyInput)
		{
			AstNode astNode = null;
			XPathScanner.LexKind kind = this.scanner.Kind;
			if (kind <= XPathScanner.LexKind.LParens)
			{
				if (kind != XPathScanner.LexKind.Dollar)
				{
					if (kind == XPathScanner.LexKind.LParens)
					{
						this.NextLex();
						astNode = this.ParseExpresion(qyInput);
						if (astNode.Type != AstNode.AstType.ConstantOperand)
						{
							astNode = new Group(astNode);
						}
						this.PassToken(XPathScanner.LexKind.RParens);
					}
				}
				else
				{
					this.NextLex();
					this.CheckToken(XPathScanner.LexKind.Name);
					astNode = new Variable(this.scanner.Name, this.scanner.Prefix);
					this.NextLex();
				}
			}
			else if (kind != XPathScanner.LexKind.Number)
			{
				if (kind != XPathScanner.LexKind.Name)
				{
					if (kind == XPathScanner.LexKind.String)
					{
						astNode = new Operand(this.scanner.StringValue);
						this.NextLex();
					}
				}
				else if (this.scanner.CanBeFunction && !XPathParser.IsNodeType(this.scanner))
				{
					astNode = this.ParseMethod(null);
				}
			}
			else
			{
				astNode = new Operand(this.scanner.NumberValue);
				this.NextLex();
			}
			return astNode;
		}

		private AstNode ParseMethod(AstNode qyInput)
		{
			ArrayList arrayList = new ArrayList();
			string name = this.scanner.Name;
			string prefix = this.scanner.Prefix;
			this.PassToken(XPathScanner.LexKind.Name);
			this.PassToken(XPathScanner.LexKind.LParens);
			if (this.scanner.Kind != XPathScanner.LexKind.RParens)
			{
				for (;;)
				{
					arrayList.Add(this.ParseExpresion(qyInput));
					if (this.scanner.Kind == XPathScanner.LexKind.RParens)
					{
						break;
					}
					this.PassToken(XPathScanner.LexKind.Comma);
				}
			}
			this.PassToken(XPathScanner.LexKind.RParens);
			if (prefix.Length == 0)
			{
				XPathParser.ParamInfo paramInfo = (XPathParser.ParamInfo)XPathParser.functionTable[name];
				if (paramInfo != null)
				{
					int num = arrayList.Count;
					if (num < paramInfo.Minargs)
					{
						throw XPathException.Create("Function '{0}' in '{1}' has an invalid number of arguments.", name, this.scanner.SourceText);
					}
					if (paramInfo.FType == Function.FunctionType.FuncConcat)
					{
						for (int i = 0; i < num; i++)
						{
							AstNode astNode = (AstNode)arrayList[i];
							if (astNode.ReturnType != XPathResultType.String)
							{
								astNode = new Function(Function.FunctionType.FuncString, astNode);
							}
							arrayList[i] = astNode;
						}
					}
					else
					{
						if (paramInfo.Maxargs < num)
						{
							throw XPathException.Create("Function '{0}' in '{1}' has an invalid number of arguments.", name, this.scanner.SourceText);
						}
						if (paramInfo.ArgTypes.Length < num)
						{
							num = paramInfo.ArgTypes.Length;
						}
						for (int j = 0; j < num; j++)
						{
							AstNode astNode2 = (AstNode)arrayList[j];
							if (paramInfo.ArgTypes[j] != XPathResultType.Any && paramInfo.ArgTypes[j] != astNode2.ReturnType)
							{
								switch (paramInfo.ArgTypes[j])
								{
								case XPathResultType.Number:
									astNode2 = new Function(Function.FunctionType.FuncNumber, astNode2);
									break;
								case XPathResultType.String:
									astNode2 = new Function(Function.FunctionType.FuncString, astNode2);
									break;
								case XPathResultType.Boolean:
									astNode2 = new Function(Function.FunctionType.FuncBoolean, astNode2);
									break;
								case XPathResultType.NodeSet:
									if (!(astNode2 is Variable) && (!(astNode2 is Function) || astNode2.ReturnType != XPathResultType.Any))
									{
										throw XPathException.Create("The argument to function '{0}' in '{1}' cannot be converted to a node-set.", name, this.scanner.SourceText);
									}
									break;
								}
								arrayList[j] = astNode2;
							}
						}
					}
					return new Function(paramInfo.FType, arrayList);
				}
			}
			return new Function(prefix, name, arrayList);
		}

		private AstNode ParsePattern(AstNode qyInput)
		{
			AstNode astNode = this.ParseLocationPathPattern(qyInput);
			while (this.scanner.Kind == XPathScanner.LexKind.Union)
			{
				this.NextLex();
				astNode = new Operator(Operator.Op.UNION, astNode, this.ParseLocationPathPattern(qyInput));
			}
			return astNode;
		}

		private AstNode ParseLocationPathPattern(AstNode qyInput)
		{
			AstNode astNode = null;
			XPathScanner.LexKind lexKind = this.scanner.Kind;
			if (lexKind != XPathScanner.LexKind.Slash)
			{
				if (lexKind != XPathScanner.LexKind.SlashSlash)
				{
					if (lexKind == XPathScanner.LexKind.Name)
					{
						if (this.scanner.CanBeFunction)
						{
							astNode = this.ParseIdKeyPattern(qyInput);
							if (astNode != null)
							{
								lexKind = this.scanner.Kind;
								if (lexKind != XPathScanner.LexKind.Slash)
								{
									if (lexKind != XPathScanner.LexKind.SlashSlash)
									{
										return astNode;
									}
									this.NextLex();
									astNode = new Axis(Axis.AxisType.DescendantOrSelf, astNode);
								}
								else
								{
									this.NextLex();
								}
							}
						}
					}
				}
				else
				{
					this.NextLex();
					astNode = new Axis(Axis.AxisType.DescendantOrSelf, new Root());
				}
			}
			else
			{
				this.NextLex();
				astNode = new Root();
				if (this.scanner.Kind == XPathScanner.LexKind.Eof || this.scanner.Kind == XPathScanner.LexKind.Union)
				{
					return astNode;
				}
			}
			return this.ParseRelativePathPattern(astNode);
		}

		private AstNode ParseIdKeyPattern(AstNode qyInput)
		{
			ArrayList arrayList = new ArrayList();
			if (this.scanner.Prefix.Length == 0)
			{
				if (this.scanner.Name == "id")
				{
					XPathParser.ParamInfo paramInfo = (XPathParser.ParamInfo)XPathParser.functionTable["id"];
					this.NextLex();
					this.PassToken(XPathScanner.LexKind.LParens);
					this.CheckToken(XPathScanner.LexKind.String);
					arrayList.Add(new Operand(this.scanner.StringValue));
					this.NextLex();
					this.PassToken(XPathScanner.LexKind.RParens);
					return new Function(paramInfo.FType, arrayList);
				}
				if (this.scanner.Name == "key")
				{
					this.NextLex();
					this.PassToken(XPathScanner.LexKind.LParens);
					this.CheckToken(XPathScanner.LexKind.String);
					arrayList.Add(new Operand(this.scanner.StringValue));
					this.NextLex();
					this.PassToken(XPathScanner.LexKind.Comma);
					this.CheckToken(XPathScanner.LexKind.String);
					arrayList.Add(new Operand(this.scanner.StringValue));
					this.NextLex();
					this.PassToken(XPathScanner.LexKind.RParens);
					return new Function("", "key", arrayList);
				}
			}
			return null;
		}

		private AstNode ParseRelativePathPattern(AstNode qyInput)
		{
			AstNode astNode = this.ParseStepPattern(qyInput);
			if (XPathScanner.LexKind.SlashSlash == this.scanner.Kind)
			{
				this.NextLex();
				astNode = this.ParseRelativePathPattern(new Axis(Axis.AxisType.DescendantOrSelf, astNode));
			}
			else if (XPathScanner.LexKind.Slash == this.scanner.Kind)
			{
				this.NextLex();
				astNode = this.ParseRelativePathPattern(astNode);
			}
			return astNode;
		}

		private AstNode ParseStepPattern(AstNode qyInput)
		{
			Axis.AxisType axisType = Axis.AxisType.Child;
			XPathScanner.LexKind kind = this.scanner.Kind;
			if (kind != XPathScanner.LexKind.At)
			{
				if (kind == XPathScanner.LexKind.Axe)
				{
					axisType = this.GetAxis(this.scanner);
					if (axisType != Axis.AxisType.Child && axisType != Axis.AxisType.Attribute)
					{
						throw XPathException.Create("'{0}' has an invalid token.", this.scanner.SourceText);
					}
					this.NextLex();
				}
			}
			else
			{
				axisType = Axis.AxisType.Attribute;
				this.NextLex();
			}
			XPathNodeType xpathNodeType = ((axisType == Axis.AxisType.Attribute) ? XPathNodeType.Attribute : XPathNodeType.Element);
			AstNode astNode = this.ParseNodeTest(qyInput, axisType, xpathNodeType);
			while (XPathScanner.LexKind.LBracket == this.scanner.Kind)
			{
				astNode = new Filter(astNode, this.ParsePredicate(astNode));
			}
			return astNode;
		}

		private void CheckToken(XPathScanner.LexKind t)
		{
			if (this.scanner.Kind != t)
			{
				throw XPathException.Create("'{0}' has an invalid token.", this.scanner.SourceText);
			}
		}

		private void PassToken(XPathScanner.LexKind t)
		{
			this.CheckToken(t);
			this.NextLex();
		}

		private void NextLex()
		{
			this.scanner.NextLex();
		}

		private bool TestOp(string op)
		{
			return this.scanner.Kind == XPathScanner.LexKind.Name && this.scanner.Prefix.Length == 0 && this.scanner.Name.Equals(op);
		}

		private void CheckNodeSet(XPathResultType t)
		{
			if (t != XPathResultType.NodeSet && t != XPathResultType.Any)
			{
				throw XPathException.Create("Expression must evaluate to a node-set.", this.scanner.SourceText);
			}
		}

		private static Hashtable CreateFunctionTable()
		{
			return new Hashtable(36)
			{
				{
					"last",
					new XPathParser.ParamInfo(Function.FunctionType.FuncLast, 0, 0, XPathParser.temparray1)
				},
				{
					"position",
					new XPathParser.ParamInfo(Function.FunctionType.FuncPosition, 0, 0, XPathParser.temparray1)
				},
				{
					"name",
					new XPathParser.ParamInfo(Function.FunctionType.FuncName, 0, 1, XPathParser.temparray2)
				},
				{
					"namespace-uri",
					new XPathParser.ParamInfo(Function.FunctionType.FuncNameSpaceUri, 0, 1, XPathParser.temparray2)
				},
				{
					"local-name",
					new XPathParser.ParamInfo(Function.FunctionType.FuncLocalName, 0, 1, XPathParser.temparray2)
				},
				{
					"count",
					new XPathParser.ParamInfo(Function.FunctionType.FuncCount, 1, 1, XPathParser.temparray2)
				},
				{
					"id",
					new XPathParser.ParamInfo(Function.FunctionType.FuncID, 1, 1, XPathParser.temparray3)
				},
				{
					"string",
					new XPathParser.ParamInfo(Function.FunctionType.FuncString, 0, 1, XPathParser.temparray3)
				},
				{
					"concat",
					new XPathParser.ParamInfo(Function.FunctionType.FuncConcat, 2, 100, XPathParser.temparray4)
				},
				{
					"starts-with",
					new XPathParser.ParamInfo(Function.FunctionType.FuncStartsWith, 2, 2, XPathParser.temparray5)
				},
				{
					"contains",
					new XPathParser.ParamInfo(Function.FunctionType.FuncContains, 2, 2, XPathParser.temparray5)
				},
				{
					"substring-before",
					new XPathParser.ParamInfo(Function.FunctionType.FuncSubstringBefore, 2, 2, XPathParser.temparray5)
				},
				{
					"substring-after",
					new XPathParser.ParamInfo(Function.FunctionType.FuncSubstringAfter, 2, 2, XPathParser.temparray5)
				},
				{
					"substring",
					new XPathParser.ParamInfo(Function.FunctionType.FuncSubstring, 2, 3, XPathParser.temparray6)
				},
				{
					"string-length",
					new XPathParser.ParamInfo(Function.FunctionType.FuncStringLength, 0, 1, XPathParser.temparray4)
				},
				{
					"normalize-space",
					new XPathParser.ParamInfo(Function.FunctionType.FuncNormalize, 0, 1, XPathParser.temparray4)
				},
				{
					"translate",
					new XPathParser.ParamInfo(Function.FunctionType.FuncTranslate, 3, 3, XPathParser.temparray7)
				},
				{
					"boolean",
					new XPathParser.ParamInfo(Function.FunctionType.FuncBoolean, 1, 1, XPathParser.temparray3)
				},
				{
					"not",
					new XPathParser.ParamInfo(Function.FunctionType.FuncNot, 1, 1, XPathParser.temparray8)
				},
				{
					"true",
					new XPathParser.ParamInfo(Function.FunctionType.FuncTrue, 0, 0, XPathParser.temparray8)
				},
				{
					"false",
					new XPathParser.ParamInfo(Function.FunctionType.FuncFalse, 0, 0, XPathParser.temparray8)
				},
				{
					"lang",
					new XPathParser.ParamInfo(Function.FunctionType.FuncLang, 1, 1, XPathParser.temparray4)
				},
				{
					"number",
					new XPathParser.ParamInfo(Function.FunctionType.FuncNumber, 0, 1, XPathParser.temparray3)
				},
				{
					"sum",
					new XPathParser.ParamInfo(Function.FunctionType.FuncSum, 1, 1, XPathParser.temparray2)
				},
				{
					"floor",
					new XPathParser.ParamInfo(Function.FunctionType.FuncFloor, 1, 1, XPathParser.temparray9)
				},
				{
					"ceiling",
					new XPathParser.ParamInfo(Function.FunctionType.FuncCeiling, 1, 1, XPathParser.temparray9)
				},
				{
					"round",
					new XPathParser.ParamInfo(Function.FunctionType.FuncRound, 1, 1, XPathParser.temparray9)
				}
			};
		}

		private static Hashtable CreateAxesTable()
		{
			return new Hashtable(13)
			{
				{
					"ancestor",
					Axis.AxisType.Ancestor
				},
				{
					"ancestor-or-self",
					Axis.AxisType.AncestorOrSelf
				},
				{
					"attribute",
					Axis.AxisType.Attribute
				},
				{
					"child",
					Axis.AxisType.Child
				},
				{
					"descendant",
					Axis.AxisType.Descendant
				},
				{
					"descendant-or-self",
					Axis.AxisType.DescendantOrSelf
				},
				{
					"following",
					Axis.AxisType.Following
				},
				{
					"following-sibling",
					Axis.AxisType.FollowingSibling
				},
				{
					"namespace",
					Axis.AxisType.Namespace
				},
				{
					"parent",
					Axis.AxisType.Parent
				},
				{
					"preceding",
					Axis.AxisType.Preceding
				},
				{
					"preceding-sibling",
					Axis.AxisType.PrecedingSibling
				},
				{
					"self",
					Axis.AxisType.Self
				}
			};
		}

		private Axis.AxisType GetAxis(XPathScanner scaner)
		{
			object obj = XPathParser.AxesTable[scaner.Name];
			if (obj == null)
			{
				throw XPathException.Create("'{0}' has an invalid token.", this.scanner.SourceText);
			}
			return (Axis.AxisType)obj;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static XPathParser()
		{
			XPathResultType[] array = new XPathResultType[3];
			array[0] = XPathResultType.String;
			XPathParser.temparray6 = array;
			XPathParser.temparray7 = new XPathResultType[]
			{
				XPathResultType.String,
				XPathResultType.String,
				XPathResultType.String
			};
			XPathParser.temparray8 = new XPathResultType[] { XPathResultType.Boolean };
			XPathParser.temparray9 = new XPathResultType[1];
			XPathParser.functionTable = XPathParser.CreateFunctionTable();
			XPathParser.AxesTable = XPathParser.CreateAxesTable();
		}

		private XPathScanner scanner;

		private int parseDepth;

		private const int MaxParseDepth = 200;

		private static readonly XPathResultType[] temparray1 = new XPathResultType[0];

		private static readonly XPathResultType[] temparray2 = new XPathResultType[] { XPathResultType.NodeSet };

		private static readonly XPathResultType[] temparray3 = new XPathResultType[] { XPathResultType.Any };

		private static readonly XPathResultType[] temparray4 = new XPathResultType[] { XPathResultType.String };

		private static readonly XPathResultType[] temparray5 = new XPathResultType[]
		{
			XPathResultType.String,
			XPathResultType.String
		};

		private static readonly XPathResultType[] temparray6;

		private static readonly XPathResultType[] temparray7;

		private static readonly XPathResultType[] temparray8;

		private static readonly XPathResultType[] temparray9;

		private static Hashtable functionTable;

		private static Hashtable AxesTable;

		private class ParamInfo
		{
			public Function.FunctionType FType
			{
				get
				{
					return this.ftype;
				}
			}

			public int Minargs
			{
				get
				{
					return this.minargs;
				}
			}

			public int Maxargs
			{
				get
				{
					return this.maxargs;
				}
			}

			public XPathResultType[] ArgTypes
			{
				get
				{
					return this.argTypes;
				}
			}

			internal ParamInfo(Function.FunctionType ftype, int minargs, int maxargs, XPathResultType[] argTypes)
			{
				this.ftype = ftype;
				this.minargs = minargs;
				this.maxargs = maxargs;
				this.argTypes = argTypes;
			}

			private Function.FunctionType ftype;

			private int minargs;

			private int maxargs;

			private XPathResultType[] argTypes;
		}
	}
}
