using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Xml.Xsl.Qil
{
	internal class QilPatternFactory
	{
		public QilPatternFactory(QilFactory f, bool debug)
		{
			this._f = f;
			this._debug = debug;
		}

		public QilFactory BaseFactory
		{
			get
			{
				return this._f;
			}
		}

		public bool IsDebug
		{
			get
			{
				return this._debug;
			}
		}

		public QilLiteral String(string val)
		{
			return this._f.LiteralString(val);
		}

		public QilLiteral Int32(int val)
		{
			return this._f.LiteralInt32(val);
		}

		public QilLiteral Double(double val)
		{
			return this._f.LiteralDouble(val);
		}

		public QilName QName(string local, string uri, string prefix)
		{
			return this._f.LiteralQName(local, uri, prefix);
		}

		public QilName QName(string local, string uri)
		{
			return this._f.LiteralQName(local, uri, string.Empty);
		}

		public QilName QName(string local)
		{
			return this._f.LiteralQName(local, string.Empty, string.Empty);
		}

		public QilNode Unknown(XmlQueryType t)
		{
			return this._f.Unknown(t);
		}

		public QilExpression QilExpression(QilNode root, QilFactory factory)
		{
			return this._f.QilExpression(root, factory);
		}

		public QilList FunctionList()
		{
			return this._f.FunctionList();
		}

		public QilList GlobalVariableList()
		{
			return this._f.GlobalVariableList();
		}

		public QilList GlobalParameterList()
		{
			return this._f.GlobalParameterList();
		}

		public QilList ActualParameterList()
		{
			return this._f.ActualParameterList();
		}

		public QilList ActualParameterList(QilNode arg1, QilNode arg2)
		{
			QilList qilList = this._f.ActualParameterList();
			qilList.Add(arg1);
			qilList.Add(arg2);
			return qilList;
		}

		public QilList ActualParameterList(params QilNode[] args)
		{
			return this._f.ActualParameterList(args);
		}

		public QilList FormalParameterList()
		{
			return this._f.FormalParameterList();
		}

		public QilList FormalParameterList(QilNode arg1, QilNode arg2)
		{
			QilList qilList = this._f.FormalParameterList();
			qilList.Add(arg1);
			qilList.Add(arg2);
			return qilList;
		}

		public QilList FormalParameterList(params QilNode[] args)
		{
			return this._f.FormalParameterList(args);
		}

		public QilList BranchList(params QilNode[] args)
		{
			return this._f.BranchList(args);
		}

		public QilNode OptimizeBarrier(QilNode child)
		{
			return this._f.OptimizeBarrier(child);
		}

		public QilNode DataSource(QilNode name, QilNode baseUri)
		{
			return this._f.DataSource(name, baseUri);
		}

		public QilNode Nop(QilNode child)
		{
			return this._f.Nop(child);
		}

		public QilNode Error(QilNode text)
		{
			return this._f.Error(text);
		}

		public QilNode Warning(QilNode text)
		{
			return this._f.Warning(text);
		}

		public QilIterator For(QilNode binding)
		{
			return this._f.For(binding);
		}

		public QilIterator Let(QilNode binding)
		{
			return this._f.Let(binding);
		}

		public QilParameter Parameter(XmlQueryType t)
		{
			return this._f.Parameter(t);
		}

		public QilParameter Parameter(QilNode defaultValue, QilName name, XmlQueryType t)
		{
			return this._f.Parameter(defaultValue, name, t);
		}

		public QilNode PositionOf(QilIterator expr)
		{
			return this._f.PositionOf(expr);
		}

		public QilNode True()
		{
			return this._f.True();
		}

		public QilNode False()
		{
			return this._f.False();
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
			if (!this._debug)
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
			return this._f.And(left, right);
		}

		public QilNode Or(QilNode left, QilNode right)
		{
			QilPatternFactory.CheckLogicArg(left);
			QilPatternFactory.CheckLogicArg(right);
			if (!this._debug)
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
			return this._f.Or(left, right);
		}

		public QilNode Not(QilNode child)
		{
			if (!this._debug)
			{
				QilNodeType nodeType = child.NodeType;
				if (nodeType == QilNodeType.True)
				{
					return this._f.False();
				}
				if (nodeType == QilNodeType.False)
				{
					return this._f.True();
				}
				if (nodeType == QilNodeType.Not)
				{
					return ((QilUnary)child).Child;
				}
			}
			return this._f.Not(child);
		}

		public QilNode Conditional(QilNode condition, QilNode trueBranch, QilNode falseBranch)
		{
			if (!this._debug)
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
			return this._f.Conditional(condition, trueBranch, falseBranch);
		}

		public QilNode Choice(QilNode expr, QilList branches)
		{
			if (!this._debug)
			{
				int count = branches.Count;
				if (count == 1)
				{
					return this._f.Loop(this._f.Let(expr), branches[0]);
				}
				if (count == 2)
				{
					return this._f.Conditional(this._f.Eq(expr, this._f.LiteralInt32(0)), branches[0], branches[1]);
				}
			}
			return this._f.Choice(expr, branches);
		}

		public QilNode Length(QilNode child)
		{
			return this._f.Length(child);
		}

		public QilNode Sequence()
		{
			return this._f.Sequence();
		}

		public QilNode Sequence(QilNode child)
		{
			if (!this._debug)
			{
				return child;
			}
			QilList qilList = this._f.Sequence();
			qilList.Add(child);
			return qilList;
		}

		public QilNode Sequence(QilNode child1, QilNode child2)
		{
			QilList qilList = this._f.Sequence();
			qilList.Add(child1);
			qilList.Add(child2);
			return qilList;
		}

		public QilNode Sequence(params QilNode[] args)
		{
			if (!this._debug)
			{
				int i = args.Length;
				if (i == 0)
				{
					return this._f.Sequence();
				}
				if (i == 1)
				{
					return args[0];
				}
			}
			QilList qilList = this._f.Sequence();
			foreach (QilNode qilNode in args)
			{
				qilList.Add(qilNode);
			}
			return qilList;
		}

		public QilNode Union(QilNode left, QilNode right)
		{
			return this._f.Union(left, right);
		}

		public QilNode Sum(QilNode collection)
		{
			return this._f.Sum(collection);
		}

		public QilNode Negate(QilNode child)
		{
			return this._f.Negate(child);
		}

		public QilNode Add(QilNode left, QilNode right)
		{
			return this._f.Add(left, right);
		}

		public QilNode Subtract(QilNode left, QilNode right)
		{
			return this._f.Subtract(left, right);
		}

		public QilNode Multiply(QilNode left, QilNode right)
		{
			return this._f.Multiply(left, right);
		}

		public QilNode Divide(QilNode left, QilNode right)
		{
			return this._f.Divide(left, right);
		}

		public QilNode Modulo(QilNode left, QilNode right)
		{
			return this._f.Modulo(left, right);
		}

		public QilNode StrLength(QilNode str)
		{
			return this._f.StrLength(str);
		}

		public QilNode StrConcat(QilNode values)
		{
			if (!this._debug && values.XmlType.IsSingleton)
			{
				return values;
			}
			return this._f.StrConcat(values);
		}

		public QilNode StrConcat(params QilNode[] args)
		{
			return this.StrConcat(args);
		}

		public QilNode StrConcat(IList<QilNode> args)
		{
			if (!this._debug)
			{
				int count = args.Count;
				if (count == 0)
				{
					return this._f.LiteralString(string.Empty);
				}
				if (count == 1)
				{
					return this.StrConcat(args[0]);
				}
			}
			return this.StrConcat(this._f.Sequence(args));
		}

		public QilNode StrParseQName(QilNode str, QilNode ns)
		{
			return this._f.StrParseQName(str, ns);
		}

		public QilNode Ne(QilNode left, QilNode right)
		{
			return this._f.Ne(left, right);
		}

		public QilNode Eq(QilNode left, QilNode right)
		{
			return this._f.Eq(left, right);
		}

		public QilNode Gt(QilNode left, QilNode right)
		{
			return this._f.Gt(left, right);
		}

		public QilNode Ge(QilNode left, QilNode right)
		{
			return this._f.Ge(left, right);
		}

		public QilNode Lt(QilNode left, QilNode right)
		{
			return this._f.Lt(left, right);
		}

		public QilNode Le(QilNode left, QilNode right)
		{
			return this._f.Le(left, right);
		}

		public QilNode Is(QilNode left, QilNode right)
		{
			return this._f.Is(left, right);
		}

		public QilNode Before(QilNode left, QilNode right)
		{
			return this._f.Before(left, right);
		}

		public QilNode Loop(QilIterator variable, QilNode body)
		{
			if (!this._debug && body == variable.Binding)
			{
				return body;
			}
			return this._f.Loop(variable, body);
		}

		public QilNode Filter(QilIterator variable, QilNode expr)
		{
			if (!this._debug && expr.NodeType == QilNodeType.True)
			{
				return variable.Binding;
			}
			return this._f.Filter(variable, expr);
		}

		public QilNode Sort(QilIterator iter, QilNode keys)
		{
			return this._f.Sort(iter, keys);
		}

		public QilSortKey SortKey(QilNode key, QilNode collation)
		{
			return this._f.SortKey(key, collation);
		}

		public QilNode DocOrderDistinct(QilNode collection)
		{
			if (collection.NodeType == QilNodeType.DocOrderDistinct)
			{
				return collection;
			}
			return this._f.DocOrderDistinct(collection);
		}

		public QilFunction Function(QilList args, QilNode sideEffects, XmlQueryType resultType)
		{
			return this._f.Function(args, sideEffects, resultType);
		}

		public QilFunction Function(QilList args, QilNode defn, QilNode sideEffects)
		{
			return this._f.Function(args, defn, sideEffects, defn.XmlType);
		}

		public QilNode Invoke(QilFunction func, QilList args)
		{
			return this._f.Invoke(func, args);
		}

		public QilNode Content(QilNode context)
		{
			return this._f.Content(context);
		}

		public QilNode Parent(QilNode context)
		{
			return this._f.Parent(context);
		}

		public QilNode Root(QilNode context)
		{
			return this._f.Root(context);
		}

		public QilNode XmlContext()
		{
			return this._f.XmlContext();
		}

		public QilNode Descendant(QilNode expr)
		{
			return this._f.Descendant(expr);
		}

		public QilNode DescendantOrSelf(QilNode context)
		{
			return this._f.DescendantOrSelf(context);
		}

		public QilNode Ancestor(QilNode expr)
		{
			return this._f.Ancestor(expr);
		}

		public QilNode AncestorOrSelf(QilNode expr)
		{
			return this._f.AncestorOrSelf(expr);
		}

		public QilNode Preceding(QilNode expr)
		{
			return this._f.Preceding(expr);
		}

		public QilNode FollowingSibling(QilNode expr)
		{
			return this._f.FollowingSibling(expr);
		}

		public QilNode PrecedingSibling(QilNode expr)
		{
			return this._f.PrecedingSibling(expr);
		}

		public QilNode NodeRange(QilNode left, QilNode right)
		{
			return this._f.NodeRange(left, right);
		}

		public QilBinary Deref(QilNode context, QilNode id)
		{
			return this._f.Deref(context, id);
		}

		public QilNode ElementCtor(QilNode name, QilNode content)
		{
			return this._f.ElementCtor(name, content);
		}

		public QilNode AttributeCtor(QilNode name, QilNode val)
		{
			return this._f.AttributeCtor(name, val);
		}

		public QilNode CommentCtor(QilNode content)
		{
			return this._f.CommentCtor(content);
		}

		public QilNode PICtor(QilNode name, QilNode content)
		{
			return this._f.PICtor(name, content);
		}

		public QilNode TextCtor(QilNode content)
		{
			return this._f.TextCtor(content);
		}

		public QilNode RawTextCtor(QilNode content)
		{
			return this._f.RawTextCtor(content);
		}

		public QilNode DocumentCtor(QilNode child)
		{
			return this._f.DocumentCtor(child);
		}

		public QilNode NamespaceDecl(QilNode prefix, QilNode uri)
		{
			return this._f.NamespaceDecl(prefix, uri);
		}

		public QilNode RtfCtor(QilNode content, QilNode baseUri)
		{
			return this._f.RtfCtor(content, baseUri);
		}

		public QilNode NameOf(QilNode expr)
		{
			return this._f.NameOf(expr);
		}

		public QilNode LocalNameOf(QilNode expr)
		{
			return this._f.LocalNameOf(expr);
		}

		public QilNode NamespaceUriOf(QilNode expr)
		{
			return this._f.NamespaceUriOf(expr);
		}

		public QilNode PrefixOf(QilNode expr)
		{
			return this._f.PrefixOf(expr);
		}

		public QilNode TypeAssert(QilNode expr, XmlQueryType t)
		{
			return this._f.TypeAssert(expr, t);
		}

		public QilNode IsType(QilNode expr, XmlQueryType t)
		{
			return this._f.IsType(expr, t);
		}

		public QilNode IsEmpty(QilNode set)
		{
			return this._f.IsEmpty(set);
		}

		public QilNode XPathNodeValue(QilNode expr)
		{
			return this._f.XPathNodeValue(expr);
		}

		public QilNode XPathFollowing(QilNode expr)
		{
			return this._f.XPathFollowing(expr);
		}

		public QilNode XPathNamespace(QilNode expr)
		{
			return this._f.XPathNamespace(expr);
		}

		public QilNode XPathPreceding(QilNode expr)
		{
			return this._f.XPathPreceding(expr);
		}

		public QilNode XsltGenerateId(QilNode expr)
		{
			return this._f.XsltGenerateId(expr);
		}

		public QilNode XsltInvokeEarlyBound(QilNode name, MethodInfo d, XmlQueryType t, IList<QilNode> args)
		{
			QilList qilList = this._f.ActualParameterList();
			qilList.Add(args);
			return this._f.XsltInvokeEarlyBound(name, this._f.LiteralObject(d), qilList, t);
		}

		public QilNode XsltInvokeLateBound(QilNode name, IList<QilNode> args)
		{
			QilList qilList = this._f.ActualParameterList();
			qilList.Add(args);
			return this._f.XsltInvokeLateBound(name, qilList);
		}

		public QilNode XsltCopy(QilNode expr, QilNode content)
		{
			return this._f.XsltCopy(expr, content);
		}

		public QilNode XsltCopyOf(QilNode expr)
		{
			return this._f.XsltCopyOf(expr);
		}

		public QilNode XsltConvert(QilNode expr, XmlQueryType t)
		{
			return this._f.XsltConvert(expr, t);
		}

		private bool _debug;

		private QilFactory _f;
	}
}
