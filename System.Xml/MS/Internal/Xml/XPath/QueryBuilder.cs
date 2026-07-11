using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class QueryBuilder
	{
		private void Reset()
		{
			this.parseDepth = 0;
			this.needContext = false;
		}

		private Query ProcessAxis(Axis root, QueryBuilder.Flags flags, out QueryBuilder.Props props)
		{
			if (root.Prefix.Length > 0)
			{
				this.needContext = true;
			}
			this.firstInput = null;
			Query query2;
			Query query3;
			if (root.Input != null)
			{
				QueryBuilder.Flags flags2 = QueryBuilder.Flags.None;
				if ((flags & QueryBuilder.Flags.PosFilter) == QueryBuilder.Flags.None)
				{
					Axis axis = root.Input as Axis;
					if (axis != null && root.TypeOfAxis == Axis.AxisType.Child && axis.TypeOfAxis == Axis.AxisType.DescendantOrSelf && axis.NodeType == XPathNodeType.All)
					{
						Query query;
						if (axis.Input != null)
						{
							query = this.ProcessNode(axis.Input, QueryBuilder.Flags.SmartDesc, out props);
						}
						else
						{
							query = new ContextQuery();
							props = QueryBuilder.Props.None;
						}
						query2 = new DescendantQuery(query, root.Name, root.Prefix, root.NodeType, false, axis.AbbrAxis);
						if ((props & QueryBuilder.Props.NonFlat) != QueryBuilder.Props.None)
						{
							query2 = new DocumentOrderQuery(query2);
						}
						props |= QueryBuilder.Props.NonFlat;
						return query2;
					}
					if (root.TypeOfAxis == Axis.AxisType.Descendant || root.TypeOfAxis == Axis.AxisType.DescendantOrSelf)
					{
						flags2 |= QueryBuilder.Flags.SmartDesc;
					}
				}
				query3 = this.ProcessNode(root.Input, flags2, out props);
			}
			else
			{
				query3 = new ContextQuery();
				props = QueryBuilder.Props.None;
			}
			switch (root.TypeOfAxis)
			{
			case Axis.AxisType.Ancestor:
				query2 = new XPathAncestorQuery(query3, root.Name, root.Prefix, root.NodeType, false);
				props |= QueryBuilder.Props.NonFlat;
				break;
			case Axis.AxisType.AncestorOrSelf:
				query2 = new XPathAncestorQuery(query3, root.Name, root.Prefix, root.NodeType, true);
				props |= QueryBuilder.Props.NonFlat;
				break;
			case Axis.AxisType.Attribute:
				query2 = new AttributeQuery(query3, root.Name, root.Prefix, root.NodeType);
				break;
			case Axis.AxisType.Child:
				if ((props & QueryBuilder.Props.NonFlat) != QueryBuilder.Props.None)
				{
					query2 = new CacheChildrenQuery(query3, root.Name, root.Prefix, root.NodeType);
				}
				else
				{
					query2 = new ChildrenQuery(query3, root.Name, root.Prefix, root.NodeType);
				}
				break;
			case Axis.AxisType.Descendant:
				if ((flags & QueryBuilder.Flags.SmartDesc) != QueryBuilder.Flags.None)
				{
					query2 = new DescendantOverDescendantQuery(query3, false, root.Name, root.Prefix, root.NodeType, false);
				}
				else
				{
					query2 = new DescendantQuery(query3, root.Name, root.Prefix, root.NodeType, false, false);
					if ((props & QueryBuilder.Props.NonFlat) != QueryBuilder.Props.None)
					{
						query2 = new DocumentOrderQuery(query2);
					}
				}
				props |= QueryBuilder.Props.NonFlat;
				break;
			case Axis.AxisType.DescendantOrSelf:
				if ((flags & QueryBuilder.Flags.SmartDesc) != QueryBuilder.Flags.None)
				{
					query2 = new DescendantOverDescendantQuery(query3, true, root.Name, root.Prefix, root.NodeType, root.AbbrAxis);
				}
				else
				{
					query2 = new DescendantQuery(query3, root.Name, root.Prefix, root.NodeType, true, root.AbbrAxis);
					if ((props & QueryBuilder.Props.NonFlat) != QueryBuilder.Props.None)
					{
						query2 = new DocumentOrderQuery(query2);
					}
				}
				props |= QueryBuilder.Props.NonFlat;
				break;
			case Axis.AxisType.Following:
				query2 = new FollowingQuery(query3, root.Name, root.Prefix, root.NodeType);
				props |= QueryBuilder.Props.NonFlat;
				break;
			case Axis.AxisType.FollowingSibling:
				query2 = new FollSiblingQuery(query3, root.Name, root.Prefix, root.NodeType);
				if ((props & QueryBuilder.Props.NonFlat) != QueryBuilder.Props.None)
				{
					query2 = new DocumentOrderQuery(query2);
				}
				break;
			case Axis.AxisType.Namespace:
				if ((root.NodeType == XPathNodeType.All || root.NodeType == XPathNodeType.Element || root.NodeType == XPathNodeType.Attribute) && root.Prefix.Length == 0)
				{
					query2 = new NamespaceQuery(query3, root.Name, root.Prefix, root.NodeType);
				}
				else
				{
					query2 = new EmptyQuery();
				}
				break;
			case Axis.AxisType.Parent:
				query2 = new ParentQuery(query3, root.Name, root.Prefix, root.NodeType);
				break;
			case Axis.AxisType.Preceding:
				query2 = new PrecedingQuery(query3, root.Name, root.Prefix, root.NodeType);
				props |= QueryBuilder.Props.NonFlat;
				break;
			case Axis.AxisType.PrecedingSibling:
				query2 = new PreSiblingQuery(query3, root.Name, root.Prefix, root.NodeType);
				break;
			case Axis.AxisType.Self:
				query2 = new XPathSelfQuery(query3, root.Name, root.Prefix, root.NodeType);
				break;
			default:
				throw XPathException.Create("The XPath query '{0}' is not supported.", this.query);
			}
			return query2;
		}

		private bool CanBeNumber(Query q)
		{
			return q.StaticType == XPathResultType.Any || q.StaticType == XPathResultType.Number;
		}

		private Query ProcessFilter(Filter root, QueryBuilder.Flags flags, out QueryBuilder.Props props)
		{
			bool flag = (flags & QueryBuilder.Flags.Filter) == QueryBuilder.Flags.None;
			QueryBuilder.Props props2;
			Query query = this.ProcessNode(root.Condition, QueryBuilder.Flags.None, out props2);
			if (this.CanBeNumber(query) || (props2 & (QueryBuilder.Props)6) != QueryBuilder.Props.None)
			{
				props2 |= QueryBuilder.Props.HasPosition;
				flags |= QueryBuilder.Flags.PosFilter;
			}
			flags &= (QueryBuilder.Flags)(-2);
			Query query2 = this.ProcessNode(root.Input, flags | QueryBuilder.Flags.Filter, out props);
			if (root.Input.Type != AstNode.AstType.Filter)
			{
				props &= (QueryBuilder.Props)(-2);
			}
			if ((props2 & QueryBuilder.Props.HasPosition) != QueryBuilder.Props.None)
			{
				props |= QueryBuilder.Props.PosFilter;
			}
			FilterQuery filterQuery = query2 as FilterQuery;
			if (filterQuery != null && (props2 & QueryBuilder.Props.HasPosition) == QueryBuilder.Props.None && filterQuery.Condition.StaticType != XPathResultType.Any)
			{
				Query query3 = filterQuery.Condition;
				if (query3.StaticType == XPathResultType.Number)
				{
					query3 = new LogicalExpr(Operator.Op.EQ, new NodeFunctions(Function.FunctionType.FuncPosition, null), query3);
				}
				query = new BooleanExpr(Operator.Op.AND, query3, query);
				query2 = filterQuery.qyInput;
			}
			if ((props & QueryBuilder.Props.PosFilter) != QueryBuilder.Props.None && query2 is DocumentOrderQuery)
			{
				query2 = ((DocumentOrderQuery)query2).input;
			}
			if (this.firstInput == null)
			{
				this.firstInput = query2 as BaseAxisQuery;
			}
			bool flag2 = (query2.Properties & QueryProps.Merge) > QueryProps.None;
			bool flag3 = (query2.Properties & QueryProps.Reverse) > QueryProps.None;
			if ((props2 & QueryBuilder.Props.HasPosition) != QueryBuilder.Props.None)
			{
				if (flag3)
				{
					query2 = new ReversePositionQuery(query2);
				}
				else if ((props2 & QueryBuilder.Props.HasLast) != QueryBuilder.Props.None)
				{
					query2 = new ForwardPositionQuery(query2);
				}
			}
			if (flag && this.firstInput != null)
			{
				if (flag2 && (props & QueryBuilder.Props.PosFilter) != QueryBuilder.Props.None)
				{
					query2 = new FilterQuery(query2, query, false);
					Query qyInput = this.firstInput.qyInput;
					if (!(qyInput is ContextQuery))
					{
						this.firstInput.qyInput = new ContextQuery();
						this.firstInput = null;
						return new MergeFilterQuery(qyInput, query2);
					}
					this.firstInput = null;
					return query2;
				}
				else
				{
					this.firstInput = null;
				}
			}
			return new FilterQuery(query2, query, (props2 & QueryBuilder.Props.HasPosition) == QueryBuilder.Props.None);
		}

		private Query ProcessOperator(Operator root, out QueryBuilder.Props props)
		{
			QueryBuilder.Props props2;
			Query query = this.ProcessNode(root.Operand1, QueryBuilder.Flags.None, out props2);
			QueryBuilder.Props props3;
			Query query2 = this.ProcessNode(root.Operand2, QueryBuilder.Flags.None, out props3);
			props = props2 | props3;
			switch (root.OperatorType)
			{
			case Operator.Op.OR:
			case Operator.Op.AND:
				return new BooleanExpr(root.OperatorType, query, query2);
			case Operator.Op.EQ:
			case Operator.Op.NE:
			case Operator.Op.LT:
			case Operator.Op.LE:
			case Operator.Op.GT:
			case Operator.Op.GE:
				return new LogicalExpr(root.OperatorType, query, query2);
			case Operator.Op.PLUS:
			case Operator.Op.MINUS:
			case Operator.Op.MUL:
			case Operator.Op.DIV:
			case Operator.Op.MOD:
				return new NumericExpr(root.OperatorType, query, query2);
			case Operator.Op.UNION:
				props |= QueryBuilder.Props.NonFlat;
				return new UnionExpr(query, query2);
			default:
				return null;
			}
		}

		private Query ProcessVariable(Variable root)
		{
			this.needContext = true;
			if (!this.allowVar)
			{
				throw XPathException.Create("'{0}' is an invalid key pattern. It either contains a variable reference or 'key()' function.", this.query);
			}
			return new VariableQuery(root.Localname, root.Prefix);
		}

		private Query ProcessFunction(Function root, out QueryBuilder.Props props)
		{
			props = QueryBuilder.Props.None;
			switch (root.TypeOfFunction)
			{
			case Function.FunctionType.FuncLast:
			{
				Query query = new NodeFunctions(root.TypeOfFunction, null);
				props |= QueryBuilder.Props.HasLast;
				return query;
			}
			case Function.FunctionType.FuncPosition:
			{
				Query query2 = new NodeFunctions(root.TypeOfFunction, null);
				props |= QueryBuilder.Props.HasPosition;
				return query2;
			}
			case Function.FunctionType.FuncCount:
				return new NodeFunctions(Function.FunctionType.FuncCount, this.ProcessNode((AstNode)root.ArgumentList[0], QueryBuilder.Flags.None, out props));
			case Function.FunctionType.FuncID:
			{
				Query query3 = new IDQuery(this.ProcessNode((AstNode)root.ArgumentList[0], QueryBuilder.Flags.None, out props));
				props |= QueryBuilder.Props.NonFlat;
				return query3;
			}
			case Function.FunctionType.FuncLocalName:
			case Function.FunctionType.FuncNameSpaceUri:
			case Function.FunctionType.FuncName:
				if (root.ArgumentList != null && root.ArgumentList.Count > 0)
				{
					return new NodeFunctions(root.TypeOfFunction, this.ProcessNode((AstNode)root.ArgumentList[0], QueryBuilder.Flags.None, out props));
				}
				return new NodeFunctions(root.TypeOfFunction, null);
			case Function.FunctionType.FuncString:
			case Function.FunctionType.FuncConcat:
			case Function.FunctionType.FuncStartsWith:
			case Function.FunctionType.FuncContains:
			case Function.FunctionType.FuncSubstringBefore:
			case Function.FunctionType.FuncSubstringAfter:
			case Function.FunctionType.FuncSubstring:
			case Function.FunctionType.FuncStringLength:
			case Function.FunctionType.FuncNormalize:
			case Function.FunctionType.FuncTranslate:
				return new StringFunctions(root.TypeOfFunction, this.ProcessArguments(root.ArgumentList, out props));
			case Function.FunctionType.FuncBoolean:
			case Function.FunctionType.FuncNot:
			case Function.FunctionType.FuncLang:
				return new BooleanFunctions(root.TypeOfFunction, this.ProcessNode((AstNode)root.ArgumentList[0], QueryBuilder.Flags.None, out props));
			case Function.FunctionType.FuncNumber:
			case Function.FunctionType.FuncSum:
			case Function.FunctionType.FuncFloor:
			case Function.FunctionType.FuncCeiling:
			case Function.FunctionType.FuncRound:
				if (root.ArgumentList != null && root.ArgumentList.Count > 0)
				{
					return new NumberFunctions(root.TypeOfFunction, this.ProcessNode((AstNode)root.ArgumentList[0], QueryBuilder.Flags.None, out props));
				}
				return new NumberFunctions(Function.FunctionType.FuncNumber, null);
			case Function.FunctionType.FuncTrue:
			case Function.FunctionType.FuncFalse:
				return new BooleanFunctions(root.TypeOfFunction, null);
			case Function.FunctionType.FuncUserDefined:
			{
				this.needContext = true;
				if (!this.allowCurrent && root.Name == "current" && root.Prefix.Length == 0)
				{
					throw XPathException.Create("The 'current()' function cannot be used in a pattern.");
				}
				if (!this.allowKey && root.Name == "key" && root.Prefix.Length == 0)
				{
					throw XPathException.Create("'{0}' is an invalid key pattern. It either contains a variable reference or 'key()' function.", this.query);
				}
				Query query4 = new FunctionQuery(root.Prefix, root.Name, this.ProcessArguments(root.ArgumentList, out props));
				props |= QueryBuilder.Props.NonFlat;
				return query4;
			}
			default:
				throw XPathException.Create("The XPath query '{0}' is not supported.", this.query);
			}
		}

		private List<Query> ProcessArguments(ArrayList args, out QueryBuilder.Props props)
		{
			int num = ((args != null) ? args.Count : 0);
			List<Query> list = new List<Query>(num);
			props = QueryBuilder.Props.None;
			for (int i = 0; i < num; i++)
			{
				QueryBuilder.Props props2;
				list.Add(this.ProcessNode((AstNode)args[i], QueryBuilder.Flags.None, out props2));
				props |= props2;
			}
			return list;
		}

		private Query ProcessNode(AstNode root, QueryBuilder.Flags flags, out QueryBuilder.Props props)
		{
			int num = this.parseDepth + 1;
			this.parseDepth = num;
			if (num > 1024)
			{
				throw XPathException.Create("The xpath query is too complex.");
			}
			Query query = null;
			props = QueryBuilder.Props.None;
			switch (root.Type)
			{
			case AstNode.AstType.Axis:
				query = this.ProcessAxis((Axis)root, flags, out props);
				break;
			case AstNode.AstType.Operator:
				query = this.ProcessOperator((Operator)root, out props);
				break;
			case AstNode.AstType.Filter:
				query = this.ProcessFilter((Filter)root, flags, out props);
				break;
			case AstNode.AstType.ConstantOperand:
				query = new OperandQuery(((Operand)root).OperandValue);
				break;
			case AstNode.AstType.Function:
				query = this.ProcessFunction((Function)root, out props);
				break;
			case AstNode.AstType.Group:
				query = new GroupQuery(this.ProcessNode(((Group)root).GroupNode, QueryBuilder.Flags.None, out props));
				break;
			case AstNode.AstType.Root:
				query = new AbsoluteQuery();
				break;
			case AstNode.AstType.Variable:
				query = this.ProcessVariable((Variable)root);
				break;
			}
			this.parseDepth--;
			return query;
		}

		private Query Build(AstNode root, string query)
		{
			this.Reset();
			this.query = query;
			QueryBuilder.Props props;
			return this.ProcessNode(root, QueryBuilder.Flags.None, out props);
		}

		internal Query Build(string query, bool allowVar, bool allowKey)
		{
			this.allowVar = allowVar;
			this.allowKey = allowKey;
			this.allowCurrent = true;
			return this.Build(XPathParser.ParseXPathExpresion(query), query);
		}

		internal Query Build(string query, out bool needContext)
		{
			Query query2 = this.Build(query, true, true);
			needContext = this.needContext;
			return query2;
		}

		internal Query BuildPatternQuery(string query, bool allowVar, bool allowKey)
		{
			this.allowVar = allowVar;
			this.allowKey = allowKey;
			this.allowCurrent = false;
			return this.Build(XPathParser.ParseXPathPattern(query), query);
		}

		internal Query BuildPatternQuery(string query, out bool needContext)
		{
			Query query2 = this.BuildPatternQuery(query, true, true);
			needContext = this.needContext;
			return query2;
		}

		private string query;

		private bool allowVar;

		private bool allowKey;

		private bool allowCurrent;

		private bool needContext;

		private BaseAxisQuery firstInput;

		private int parseDepth;

		private const int MaxParseDepth = 1024;

		private enum Flags
		{
			None,
			SmartDesc,
			PosFilter,
			Filter = 4
		}

		private enum Props
		{
			None,
			PosFilter,
			HasPosition,
			HasLast = 4,
			NonFlat = 8
		}
	}
}
