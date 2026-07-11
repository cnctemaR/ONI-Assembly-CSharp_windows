using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Xml.Xsl.Qil
{
	internal class QilPatternFactory
	{
		public QilPatternFactory(QilFactory f, bool debug)
		{
			this.f = f;
			this.debug = debug;
		}

		public QilFactory BaseFactory
		{
			get
			{
				return this.f;
			}
		}

		public bool IsDebug
		{
			get
			{
				return this.debug;
			}
		}

		public QilLiteral String(string val)
		{
			return this.f.LiteralString(val);
		}

		public QilLiteral Int32(int val)
		{
			return this.f.LiteralInt32(val);
		}

		public QilLiteral Double(double val)
		{
			return this.f.LiteralDouble(val);
		}

		public QilName QName(string local, string uri, string prefix)
		{
			return this.f.LiteralQName(local, uri, prefix);
		}

		public QilName QName(string local, string uri)
		{
			return this.f.LiteralQName(local, uri, string.Empty);
		}

		public QilName QName(string local)
		{
			return this.f.LiteralQName(local, string.Empty, string.Empty);
		}

		public QilNode Unknown(XmlQueryType t)
		{
			return this.f.Unknown(t);
		}

		public QilExpression QilExpression(QilNode root, QilFactory factory)
		{
			return this.f.QilExpression(root, factory);
		}

		public QilList FunctionList()
		{
			return this.f.FunctionList();
		}

		public QilList GlobalVariableList()
		{
			return this.f.GlobalVariableList();
		}

		public QilList GlobalParameterList()
		{
			return this.f.GlobalParameterList();
		}

		public QilList ActualParameterList()
		{
			return this.f.ActualParameterList();
		}

		public QilList ActualParameterList(QilNode arg1)
		{
			QilList qilList = this.f.ActualParameterList();
			qilList.Add(arg1);
			return qilList;
		}

		public QilList ActualParameterList(QilNode arg1, QilNode arg2)
		{
			QilList qilList = this.f.ActualParameterList();
			qilList.Add(arg1);
			qilList.Add(arg2);
			return qilList;
		}

		public QilList ActualParameterList(params QilNode[] args)
		{
			return this.f.ActualParameterList(args);
		}

		public QilList FormalParameterList()
		{
			return this.f.FormalParameterList();
		}

		public QilList FormalParameterList(QilNode arg1)
		{
			QilList qilList = this.f.FormalParameterList();
			qilList.Add(arg1);
			return qilList;
		}

		public QilList FormalParameterList(QilNode arg1, QilNode arg2)
		{
			QilList qilList = this.f.FormalParameterList();
			qilList.Add(arg1);
			qilList.Add(arg2);
			return qilList;
		}

		public QilList FormalParameterList(params QilNode[] args)
		{
			return this.f.FormalParameterList(args);
		}

		public QilList SortKeyList()
		{
			return this.f.SortKeyList();
		}

		public QilList SortKeyList(QilSortKey key)
		{
			QilList qilList = this.f.SortKeyList();
			qilList.Add(key);
			return qilList;
		}

		public QilList BranchList(params QilNode[] args)
		{
			return this.f.BranchList(args);
		}

		public QilNode OptimizeBarrier(QilNode child)
		{
			return this.f.OptimizeBarrier(child);
		}

		public QilNode DataSource(QilNode name, QilNode baseUri)
		{
			return this.f.DataSource(name, baseUri);
		}

		public QilNode Nop(QilNode child)
		{
			return this.f.Nop(child);
		}

		public QilNode Error(QilNode text)
		{
			return this.f.Error(text);
		}

		public QilNode Warning(QilNode text)
		{
			return this.f.Warning(text);
		}

		public QilIterator For(QilNode binding)
		{
			return this.f.For(binding);
		}

		public QilIterator Let(QilNode binding)
		{
			return this.f.Let(binding);
		}

		public QilParameter Parameter(XmlQueryType t)
		{
			return this.f.Parameter(t);
		}

		public QilParameter Parameter(QilNode defaultValue, QilName name, XmlQueryType t)
		{
			return this.f.Parameter(defaultValue, name, t);
		}

		public QilNode PositionOf(QilIterator expr)
		{
			return this.f.PositionOf(expr);
		}

		public QilNode True()
		{
			return this.f.True();
		}

		public QilNode False()
		{
			return this.f.False();
		}

		public QilNode Boolean(bool b)
		{
			if (!b)
			{
				return this.False();
			}
			return this.True();
		}

		private static void CheckLogicArg(QilNode arg)
		{
		}

		public QilNode And(QilNode left, QilNode right)
		{
			QilPatternFactory.CheckLogicArg(left);
			QilPatternFactory.CheckLogicArg(right);
			if (!this.debug)
			{
				if (left.NodeType == QilNodeType.True || right.NodeType == QilNodeType.False)
				{
					return right;
				}
				if (left.NodeType == QilNodeType.False || right.NodeType == QilNodeType.True)
				{
					return left;
				}
			}
			return this.f.And(left, right);
		}

		public QilNode Or(QilNode left, QilNode right)
		{
			QilPatternFactory.CheckLogicArg(left);
			QilPatternFactory.CheckLogicArg(right);
			if (!this.debug)
			{
				if (left.NodeType == QilNodeType.True || right.NodeType == QilNodeType.False)
				{
					return left;
				}
				if (left.NodeType == QilNodeType.False || right.NodeType == QilNodeType.True)
				{
					return right;
				}
			}
			return this.f.Or(left, right);
		}

		public QilNode Not(QilNode child)
		{
			if (!this.debug)
			{
				QilNodeType nodeType = child.NodeType;
				if (nodeType == QilNodeType.True)
				{
					return this.f.False();
				}
				if (nodeType == QilNodeType.False)
				{
					return this.f.True();
				}
				if (nodeType == QilNodeType.Not)
				{
					return ((QilUnary)child).Child;
				}
			}
			return this.f.Not(child);
		}

		public QilNode Conditional(QilNode condition, QilNode trueBranch, QilNode falseBranch)
		{
			if (!this.debug)
			{
				QilNodeType nodeType = condition.NodeType;
				if (nodeType == QilNodeType.True)
				{
					return trueBranch;
				}
				if (nodeType == QilNodeType.False)
				{
					return falseBranch;
				}
				if (nodeType == QilNodeType.Not)
				{
					return this.Conditional(((QilUnary)condition).Child, falseBranch, trueBranch);
				}
			}
			return this.f.Conditional(condition, trueBranch, falseBranch);
		}

		public QilNode Choice(QilNode expr, QilList branches)
		{
			if (!this.debug)
			{
				int count = branches.Count;
				if (count == 1)
				{
					return this.f.Loop(this.f.Let(expr), branches[0]);
				}
				if (count == 2)
				{
					return this.f.Conditional(this.f.Eq(expr, this.f.LiteralInt32(0)), branches[0], branches[1]);
				}
			}
			return this.f.Choice(expr, branches);
		}

		public QilNode Length(QilNode child)
		{
			return this.f.Length(child);
		}

		public QilNode Sequence()
		{
			return this.f.Sequence();
		}

		public QilNode Sequence(QilNode child)
		{
			if (!this.debug)
			{
				return child;
			}
			QilList qilList = this.f.Sequence();
			qilList.Add(child);
			return qilList;
		}

		public QilNode Sequence(QilNode child1, QilNode child2)
		{
			QilList qilList = this.f.Sequence();
			qilList.Add(child1);
			qilList.Add(child2);
			return qilList;
		}

		public QilNode Sequence(params QilNode[] args)
		{
			if (!this.debug)
			{
				int i = args.Length;
				if (i == 0)
				{
					return this.f.Sequence();
				}
				if (i == 1)
				{
					return args[0];
				}
			}
			QilList qilList = this.f.Sequence();
			foreach (QilNode qilNode in args)
			{
				qilList.Add(qilNode);
			}
			return qilList;
		}

		public QilNode Union(QilNode left, QilNode right)
		{
			return this.f.Union(left, right);
		}

		public QilNode Sum(QilNode collection)
		{
			return this.f.Sum(collection);
		}

		public QilNode Negate(QilNode child)
		{
			return this.f.Negate(child);
		}

		public QilNode Add(QilNode left, QilNode right)
		{
			return this.f.Add(left, right);
		}

		public QilNode Subtract(QilNode left, QilNode right)
		{
			return this.f.Subtract(left, right);
		}

		public QilNode Multiply(QilNode left, QilNode right)
		{
			return this.f.Multiply(left, right);
		}

		public QilNode Divide(QilNode left, QilNode right)
		{
			return this.f.Divide(left, right);
		}

		public QilNode Modulo(QilNode left, QilNode right)
		{
			return this.f.Modulo(left, right);
		}

		public QilNode StrLength(QilNode str)
		{
			return this.f.StrLength(str);
		}

		public QilNode StrConcat(QilNode values)
		{
			if (!this.debug && values.XmlType.IsSingleton)
			{
				return values;
			}
			return this.f.StrConcat(values);
		}

		public QilNode StrConcat(params QilNode[] args)
		{
			return this.StrConcat(args);
		}

		public QilNode StrConcat(IList<QilNode> args)
		{
			if (!this.debug)
			{
				int count = args.Count;
				if (count == 0)
				{
					return this.f.LiteralString(string.Empty);
				}
				if (count == 1)
				{
					return this.StrConcat(args[0]);
				}
			}
			return this.StrConcat(this.f.Sequence(args));
		}

		public QilNode StrParseQName(QilNode str, QilNode ns)
		{
			return this.f.StrParseQName(str, ns);
		}

		public QilNode Ne(QilNode left, QilNode right)
		{
			return this.f.Ne(left, right);
		}

		public QilNode Eq(QilNode left, QilNode right)
		{
			return this.f.Eq(left, right);
		}

		public QilNode Gt(QilNode left, QilNode right)
		{
			return this.f.Gt(left, right);
		}

		public QilNode Ge(QilNode left, QilNode right)
		{
			return this.f.Ge(left, right);
		}

		public QilNode Lt(QilNode left, QilNode right)
		{
			return this.f.Lt(left, right);
		}

		public QilNode Le(QilNode left, QilNode right)
		{
			return this.f.Le(left, right);
		}

		public QilNode Is(QilNode left, QilNode right)
		{
			return this.f.Is(left, right);
		}

		public QilNode After(QilNode left, QilNode right)
		{
			return this.f.After(left, right);
		}

		public QilNode Before(QilNode left, QilNode right)
		{
			return this.f.Before(left, right);
		}

		public QilNode Loop(QilIterator variable, QilNode body)
		{
			if (!this.debug && body == variable.Binding)
			{
				return body;
			}
			return this.f.Loop(variable, body);
		}

		public QilNode Filter(QilIterator variable, QilNode expr)
		{
			if (!this.debug && expr.NodeType == QilNodeType.True)
			{
				return variable.Binding;
			}
			return this.f.Filter(variable, expr);
		}

		public QilNode Sort(QilIterator iter, QilNode keys)
		{
			return this.f.Sort(iter, keys);
		}

		public QilSortKey SortKey(QilNode key, QilNode collation)
		{
			return this.f.SortKey(key, collation);
		}

		public QilNode DocOrderDistinct(QilNode collection)
		{
			if (collection.NodeType == QilNodeType.DocOrderDistinct)
			{
				return collection;
			}
			return this.f.DocOrderDistinct(collection);
		}

		public QilFunction Function(QilList args, QilNode sideEffects, XmlQueryType resultType)
		{
			return this.f.Function(args, sideEffects, resultType);
		}

		public QilFunction Function(QilList args, QilNode defn, QilNode sideEffects)
		{
			return this.f.Function(args, defn, sideEffects, defn.XmlType);
		}

		public QilNode Invoke(QilFunction func, QilList args)
		{
			return this.f.Invoke(func, args);
		}

		public QilNode Content(QilNode context)
		{
			return this.f.Content(context);
		}

		public QilNode Parent(QilNode context)
		{
			return this.f.Parent(context);
		}

		public QilNode Root(QilNode context)
		{
			return this.f.Root(context);
		}

		public QilNode XmlContext()
		{
			return this.f.XmlContext();
		}

		public QilNode Descendant(QilNode expr)
		{
			return this.f.Descendant(expr);
		}

		public QilNode DescendantOrSelf(QilNode context)
		{
			return this.f.DescendantOrSelf(context);
		}

		public QilNode Ancestor(QilNode expr)
		{
			return this.f.Ancestor(expr);
		}

		public QilNode AncestorOrSelf(QilNode expr)
		{
			return this.f.AncestorOrSelf(expr);
		}

		public QilNode Preceding(QilNode expr)
		{
			return this.f.Preceding(expr);
		}

		public QilNode FollowingSibling(QilNode expr)
		{
			return this.f.FollowingSibling(expr);
		}

		public QilNode PrecedingSibling(QilNode expr)
		{
			return this.f.PrecedingSibling(expr);
		}

		public QilNode NodeRange(QilNode left, QilNode right)
		{
			return this.f.NodeRange(left, right);
		}

		public QilBinary Deref(QilNode context, QilNode id)
		{
			return this.f.Deref(context, id);
		}

		public QilNode ElementCtor(QilNode name, QilNode content)
		{
			return this.f.ElementCtor(name, content);
		}

		public QilNode AttributeCtor(QilNode name, QilNode val)
		{
			return this.f.AttributeCtor(name, val);
		}

		public QilNode CommentCtor(QilNode content)
		{
			return this.f.CommentCtor(content);
		}

		public QilNode PICtor(QilNode name, QilNode content)
		{
			return this.f.PICtor(name, content);
		}

		public QilNode TextCtor(QilNode content)
		{
			return this.f.TextCtor(content);
		}

		public QilNode RawTextCtor(QilNode content)
		{
			return this.f.RawTextCtor(content);
		}

		public QilNode DocumentCtor(QilNode child)
		{
			return this.f.DocumentCtor(child);
		}

		public QilNode NamespaceDecl(QilNode prefix, QilNode uri)
		{
			return this.f.NamespaceDecl(prefix, uri);
		}

		public QilNode RtfCtor(QilNode content, QilNode baseUri)
		{
			return this.f.RtfCtor(content, baseUri);
		}

		public QilNode NameOf(QilNode expr)
		{
			return this.f.NameOf(expr);
		}

		public QilNode LocalNameOf(QilNode expr)
		{
			return this.f.LocalNameOf(expr);
		}

		public QilNode NamespaceUriOf(QilNode expr)
		{
			return this.f.NamespaceUriOf(expr);
		}

		public QilNode PrefixOf(QilNode expr)
		{
			return this.f.PrefixOf(expr);
		}

		public QilNode TypeAssert(QilNode expr, XmlQueryType t)
		{
			return this.f.TypeAssert(expr, t);
		}

		public QilNode IsType(QilNode expr, XmlQueryType t)
		{
			return this.f.IsType(expr, t);
		}

		public QilNode IsEmpty(QilNode set)
		{
			return this.f.IsEmpty(set);
		}

		public QilNode XPathNodeValue(QilNode expr)
		{
			return this.f.XPathNodeValue(expr);
		}

		public QilNode XPathFollowing(QilNode expr)
		{
			return this.f.XPathFollowing(expr);
		}

		public QilNode XPathNamespace(QilNode expr)
		{
			return this.f.XPathNamespace(expr);
		}

		public QilNode XPathPreceding(QilNode expr)
		{
			return this.f.XPathPreceding(expr);
		}

		public QilNode XsltGenerateId(QilNode expr)
		{
			return this.f.XsltGenerateId(expr);
		}

		public QilNode XsltInvokeEarlyBound(QilNode name, MethodInfo d, XmlQueryType t, IList<QilNode> args)
		{
			QilList qilList = this.f.ActualParameterList();
			qilList.Add(args);
			return this.f.XsltInvokeEarlyBound(name, this.f.LiteralObject(d), qilList, t);
		}

		public QilNode XsltInvokeLateBound(QilNode name, IList<QilNode> args)
		{
			QilList qilList = this.f.ActualParameterList();
			qilList.Add(args);
			return this.f.XsltInvokeLateBound(name, qilList);
		}

		public QilNode XsltCopy(QilNode expr, QilNode content)
		{
			return this.f.XsltCopy(expr, content);
		}

		public QilNode XsltCopyOf(QilNode expr)
		{
			return this.f.XsltCopyOf(expr);
		}

		public QilNode XsltConvert(QilNode expr, XmlQueryType t)
		{
			return this.f.XsltConvert(expr, t);
		}

		private bool debug;

		private QilFactory f;
	}
}
