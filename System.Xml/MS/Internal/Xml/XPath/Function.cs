using System;
using System.Collections;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Function : AstNode
	{
		public Function(Function.FunctionType ftype, ArrayList argumentList)
		{
			this.functionType = ftype;
			this.argumentList = new ArrayList(argumentList);
		}

		public Function(string prefix, string name, ArrayList argumentList)
		{
			this.functionType = Function.FunctionType.FuncUserDefined;
			this.prefix = prefix;
			this.name = name;
			this.argumentList = new ArrayList(argumentList);
		}

		public Function(Function.FunctionType ftype)
		{
			this.functionType = ftype;
		}

		public Function(Function.FunctionType ftype, AstNode arg)
		{
			this.functionType = ftype;
			this.argumentList = new ArrayList();
			this.argumentList.Add(arg);
		}

		public override AstNode.AstType Type
		{
			get
			{
				return AstNode.AstType.Function;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return Function.ReturnTypes[(int)this.functionType];
			}
		}

		public Function.FunctionType TypeOfFunction
		{
			get
			{
				return this.functionType;
			}
		}

		public ArrayList ArgumentList
		{
			get
			{
				return this.argumentList;
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private Function.FunctionType functionType;

		private ArrayList argumentList;

		private string name;

		private string prefix;

		internal static XPathResultType[] ReturnTypes = new XPathResultType[]
		{
			XPathResultType.Number,
			XPathResultType.Number,
			XPathResultType.Number,
			XPathResultType.NodeSet,
			XPathResultType.String,
			XPathResultType.String,
			XPathResultType.String,
			XPathResultType.String,
			XPathResultType.Boolean,
			XPathResultType.Number,
			XPathResultType.Boolean,
			XPathResultType.Boolean,
			XPathResultType.Boolean,
			XPathResultType.String,
			XPathResultType.Boolean,
			XPathResultType.Boolean,
			XPathResultType.String,
			XPathResultType.String,
			XPathResultType.String,
			XPathResultType.Number,
			XPathResultType.String,
			XPathResultType.String,
			XPathResultType.Boolean,
			XPathResultType.Number,
			XPathResultType.Number,
			XPathResultType.Number,
			XPathResultType.Number,
			XPathResultType.Any
		};

		public enum FunctionType
		{
			FuncLast,
			FuncPosition,
			FuncCount,
			FuncID,
			FuncLocalName,
			FuncNameSpaceUri,
			FuncName,
			FuncString,
			FuncBoolean,
			FuncNumber,
			FuncTrue,
			FuncFalse,
			FuncNot,
			FuncConcat,
			FuncStartsWith,
			FuncContains,
			FuncSubstringBefore,
			FuncSubstringAfter,
			FuncSubstring,
			FuncStringLength,
			FuncNormalize,
			FuncTranslate,
			FuncLang,
			FuncSum,
			FuncFloor,
			FuncCeiling,
			FuncRound,
			FuncUserDefined
		}
	}
}
