using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Function : AstNode
	{
		public Function(Function.FunctionType ftype, List<AstNode> argumentList)
		{
			this._functionType = ftype;
			this._argumentList = new List<AstNode>(argumentList);
		}

		public Function(string prefix, string name, List<AstNode> argumentList)
		{
			this._functionType = Function.FunctionType.FuncUserDefined;
			this._prefix = prefix;
			this._name = name;
			this._argumentList = new List<AstNode>(argumentList);
		}

		public Function(Function.FunctionType ftype, AstNode arg)
		{
			this._functionType = ftype;
			this._argumentList = new List<AstNode>();
			this._argumentList.Add(arg);
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
				return Function.ReturnTypes[(int)this._functionType];
			}
		}

		public Function.FunctionType TypeOfFunction
		{
			get
			{
				return this._functionType;
			}
		}

		public List<AstNode> ArgumentList
		{
			get
			{
				return this._argumentList;
			}
		}

		public string Prefix
		{
			get
			{
				return this._prefix;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		private Function.FunctionType _functionType;

		private List<AstNode> _argumentList;

		private string _name;

		private string _prefix;

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
