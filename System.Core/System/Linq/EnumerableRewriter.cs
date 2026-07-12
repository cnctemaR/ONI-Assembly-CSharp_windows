using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq
{
	internal class EnumerableRewriter : ExpressionVisitor
	{
		protected internal override Expression VisitMethodCall(MethodCallExpression m)
		{
			Expression expression = this.Visit(m.Object);
			ReadOnlyCollection<Expression> readOnlyCollection = base.Visit(m.Arguments);
			if (expression == m.Object && readOnlyCollection == m.Arguments)
			{
				return m;
			}
			MethodInfo method = m.Method;
			Type[] array = (method.IsGenericMethod ? method.GetGenericArguments() : null);
			if ((method.IsStatic || method.DeclaringType.IsAssignableFrom(expression.Type)) && EnumerableRewriter.ArgsMatch(method, readOnlyCollection, array))
			{
				return Expression.Call(expression, method, readOnlyCollection);
			}
			if (method.DeclaringType == typeof(Queryable))
			{
				MethodInfo methodInfo = EnumerableRewriter.FindEnumerableMethod(method.Name, readOnlyCollection, array);
				readOnlyCollection = this.FixupQuotedArgs(methodInfo, readOnlyCollection);
				return Expression.Call(expression, methodInfo, readOnlyCollection);
			}
			MethodInfo methodInfo2 = EnumerableRewriter.FindMethod(method.DeclaringType, method.Name, readOnlyCollection, array);
			readOnlyCollection = this.FixupQuotedArgs(methodInfo2, readOnlyCollection);
			return Expression.Call(expression, methodInfo2, readOnlyCollection);
		}

		private ReadOnlyCollection<Expression> FixupQuotedArgs(MethodInfo mi, ReadOnlyCollection<Expression> argList)
		{
			ParameterInfo[] parameters = mi.GetParameters();
			if (parameters.Length != 0)
			{
				List<Expression> list = null;
				int i = 0;
				int num = parameters.Length;
				while (i < num)
				{
					Expression expression = argList[i];
					ParameterInfo parameterInfo = parameters[i];
					expression = this.FixupQuotedExpression(parameterInfo.ParameterType, expression);
					if (list == null && expression != argList[i])
					{
						list = new List<Expression>(argList.Count);
						for (int j = 0; j < i; j++)
						{
							list.Add(argList[j]);
						}
					}
					if (list != null)
					{
						list.Add(expression);
					}
					i++;
				}
				if (list != null)
				{
					argList = list.AsReadOnly();
				}
			}
			return argList;
		}

		private Expression FixupQuotedExpression(Type type, Expression expression)
		{
			Expression expression2 = expression;
			while (!type.IsAssignableFrom(expression2.Type))
			{
				if (expression2.NodeType != ExpressionType.Quote)
				{
					if (!type.IsAssignableFrom(expression2.Type) && type.IsArray && expression2.NodeType == ExpressionType.NewArrayInit)
					{
						Type type2 = EnumerableRewriter.StripExpression(expression2.Type);
						if (type.IsAssignableFrom(type2))
						{
							Type elementType = type.GetElementType();
							NewArrayExpression newArrayExpression = (NewArrayExpression)expression2;
							List<Expression> list = new List<Expression>(newArrayExpression.Expressions.Count);
							int i = 0;
							int count = newArrayExpression.Expressions.Count;
							while (i < count)
							{
								list.Add(this.FixupQuotedExpression(elementType, newArrayExpression.Expressions[i]));
								i++;
							}
							expression = Expression.NewArrayInit(elementType, list);
						}
					}
					return expression;
				}
				expression2 = ((UnaryExpression)expression2).Operand;
			}
			return expression2;
		}

		protected internal override Expression VisitLambda<T>(Expression<T> node)
		{
			return node;
		}

		private static Type GetPublicType(Type t)
		{
			if (t.IsGenericType && t.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IGrouping<, >)))
			{
				return typeof(IGrouping<, >).MakeGenericType(t.GetGenericArguments());
			}
			if (!t.IsNestedPrivate)
			{
				return t;
			}
			foreach (Type type in t.GetInterfaces())
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					return type;
				}
			}
			if (typeof(IEnumerable).IsAssignableFrom(t))
			{
				return typeof(IEnumerable);
			}
			return t;
		}

		private Type GetEquivalentType(Type type)
		{
			if (this._equivalentTypeCache == null)
			{
				this._equivalentTypeCache = new Dictionary<Type, Type>
				{
					{
						typeof(IQueryable),
						typeof(IEnumerable)
					},
					{
						typeof(IEnumerable),
						typeof(IEnumerable)
					}
				};
			}
			Type type2;
			if (!this._equivalentTypeCache.TryGetValue(type, out type2))
			{
				Type publicType = EnumerableRewriter.GetPublicType(type);
				if (publicType.IsInterface && publicType.IsGenericType)
				{
					Type genericTypeDefinition = publicType.GetGenericTypeDefinition();
					if (genericTypeDefinition == typeof(IOrderedEnumerable<>))
					{
						type2 = publicType;
					}
					else if (genericTypeDefinition == typeof(IOrderedQueryable<>))
					{
						type2 = typeof(IOrderedEnumerable<>).MakeGenericType(new Type[] { publicType.GenericTypeArguments[0] });
					}
					else if (genericTypeDefinition == typeof(IEnumerable<>))
					{
						type2 = publicType;
					}
					else if (genericTypeDefinition == typeof(IQueryable<>))
					{
						type2 = typeof(IEnumerable<>).MakeGenericType(new Type[] { publicType.GenericTypeArguments[0] });
					}
				}
				if (type2 == null)
				{
					var anon = (from i in publicType.GetInterfaces().Select<Type, TypeInfo>(new Func<Type, TypeInfo>(IntrospectionExtensions.GetTypeInfo)).ToArray<TypeInfo>()
						where i.IsGenericType && i.GenericTypeArguments.Length == 1
						select new
						{
							Info = i,
							GenType = i.GetGenericTypeDefinition()
						}).ToArray();
					Type type3 = (from i in anon
						where i.GenType == typeof(IOrderedQueryable<>) || i.GenType == typeof(IOrderedEnumerable<>)
						select i.Info.GenericTypeArguments[0]).Distinct<Type>().SingleOrDefault<Type>();
					if (type3 != null)
					{
						type2 = typeof(IOrderedEnumerable<>).MakeGenericType(new Type[] { type3 });
					}
					else
					{
						type3 = (from i in anon
							where i.GenType == typeof(IQueryable<>) || i.GenType == typeof(IEnumerable<>)
							select i.Info.GenericTypeArguments[0]).Distinct<Type>().Single<Type>();
						type2 = typeof(IEnumerable<>).MakeGenericType(new Type[] { type3 });
					}
				}
				this._equivalentTypeCache.Add(type, type2);
			}
			return type2;
		}

		protected internal override Expression VisitConstant(ConstantExpression c)
		{
			EnumerableQuery enumerableQuery = c.Value as EnumerableQuery;
			if (enumerableQuery != null)
			{
				if (enumerableQuery.Enumerable != null)
				{
					Type publicType = EnumerableRewriter.GetPublicType(enumerableQuery.Enumerable.GetType());
					return Expression.Constant(enumerableQuery.Enumerable, publicType);
				}
				Expression expression = enumerableQuery.Expression;
				if (expression != c)
				{
					return this.Visit(expression);
				}
			}
			return c;
		}

		[global::System.Runtime.CompilerServices.PreserveDependency("DefaultIfEmpty`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Count`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Contains`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Concat`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Cast`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Average`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Aggregate`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Append`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Any`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("All`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Aggregate`3", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Aggregate`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("GroupJoin`4", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Average", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Distinct`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Intersect`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Sum", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("SkipWhile`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Except`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Sum`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Take`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("TakeLast`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("TakeWhile`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("ThenBy`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("ThenByDescending`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Union`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Where`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Zip`3", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("GroupBy`4", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("GroupBy`3", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("GroupBy`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("FirstOrDefault`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("ElementAt`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("First`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("SkipLast`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Skip`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("ElementAtOrDefault`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Single`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Join`4", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Last`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("LastOrDefault`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("LongCount`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("SingleOrDefault`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Max`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Min`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Min`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Max`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("OrderBy`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("OrderByDescending`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Prepend`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Reverse`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("Select`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("SequenceEqual`1", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("SelectMany`3", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("SelectMany`2", "System.Linq.Enumerable")]
		[global::System.Runtime.CompilerServices.PreserveDependency("OfType`1", "System.Linq.Enumerable")]
		private static MethodInfo FindEnumerableMethod(string name, ReadOnlyCollection<Expression> args, params Type[] typeArgs)
		{
			if (EnumerableRewriter.s_seqMethods == null)
			{
				EnumerableRewriter.s_seqMethods = typeof(Enumerable).GetStaticMethods().ToLookup<MethodInfo, string>((MethodInfo m) => m.Name);
			}
			MethodInfo methodInfo = EnumerableRewriter.s_seqMethods[name].FirstOrDefault<MethodInfo>((MethodInfo m) => EnumerableRewriter.ArgsMatch(m, args, typeArgs));
			if (typeArgs != null)
			{
				return methodInfo.MakeGenericMethod(typeArgs);
			}
			return methodInfo;
		}

		private static MethodInfo FindMethod(Type type, string name, ReadOnlyCollection<Expression> args, Type[] typeArgs)
		{
			using (IEnumerator<MethodInfo> enumerator = (from m in type.GetStaticMethods()
				where m.Name == name
				select m).GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoMethodOnType(name, type);
				}
				MethodInfo methodInfo;
				for (;;)
				{
					methodInfo = enumerator.Current;
					if (EnumerableRewriter.ArgsMatch(methodInfo, args, typeArgs))
					{
						break;
					}
					if (!enumerator.MoveNext())
					{
						goto Block_6;
					}
				}
				return (typeArgs != null) ? methodInfo.MakeGenericMethod(typeArgs) : methodInfo;
				Block_6:;
			}
			throw Error.NoMethodOnTypeMatchingArguments(name, type);
		}

		private static bool ArgsMatch(MethodInfo m, ReadOnlyCollection<Expression> args, Type[] typeArgs)
		{
			ParameterInfo[] array = m.GetParameters();
			if (array.Length != args.Count)
			{
				return false;
			}
			if (!m.IsGenericMethod && typeArgs != null && typeArgs.Length != 0)
			{
				return false;
			}
			if (!m.IsGenericMethodDefinition && m.IsGenericMethod && m.ContainsGenericParameters)
			{
				m = m.GetGenericMethodDefinition();
			}
			if (m.IsGenericMethodDefinition)
			{
				if (typeArgs == null || typeArgs.Length == 0)
				{
					return false;
				}
				if (m.GetGenericArguments().Length != typeArgs.Length)
				{
					return false;
				}
				m = m.MakeGenericMethod(typeArgs);
				array = m.GetParameters();
			}
			int i = 0;
			int count = args.Count;
			while (i < count)
			{
				Type type = array[i].ParameterType;
				if (type == null)
				{
					return false;
				}
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				Expression expression = args[i];
				if (!type.IsAssignableFrom(expression.Type))
				{
					if (expression.NodeType == ExpressionType.Quote)
					{
						expression = ((UnaryExpression)expression).Operand;
					}
					if (!type.IsAssignableFrom(expression.Type) && !type.IsAssignableFrom(EnumerableRewriter.StripExpression(expression.Type)))
					{
						return false;
					}
				}
				i++;
			}
			return true;
		}

		private static Type StripExpression(Type type)
		{
			bool isArray = type.IsArray;
			Type type2 = (isArray ? type.GetElementType() : type);
			Type type3 = TypeHelper.FindGenericType(typeof(Expression<>), type2);
			if (type3 != null)
			{
				type2 = type3.GetGenericArguments()[0];
			}
			if (!isArray)
			{
				return type;
			}
			int arrayRank = type.GetArrayRank();
			if (arrayRank != 1)
			{
				return type2.MakeArrayType(arrayRank);
			}
			return type2.MakeArrayType();
		}

		protected internal override Expression VisitConditional(ConditionalExpression c)
		{
			Type type = c.Type;
			if (!typeof(IQueryable).IsAssignableFrom(type))
			{
				return base.VisitConditional(c);
			}
			Expression expression = this.Visit(c.Test);
			Expression expression2 = this.Visit(c.IfTrue);
			Expression expression3 = this.Visit(c.IfFalse);
			Type type2 = expression2.Type;
			Type type3 = expression3.Type;
			if (type2.IsAssignableFrom(type3))
			{
				return Expression.Condition(expression, expression2, expression3, type2);
			}
			if (type3.IsAssignableFrom(type2))
			{
				return Expression.Condition(expression, expression2, expression3, type3);
			}
			return Expression.Condition(expression, expression2, expression3, this.GetEquivalentType(type));
		}

		protected internal override Expression VisitBlock(BlockExpression node)
		{
			Type type = node.Type;
			if (!typeof(IQueryable).IsAssignableFrom(type))
			{
				return base.VisitBlock(node);
			}
			ReadOnlyCollection<Expression> readOnlyCollection = base.Visit(node.Expressions);
			ReadOnlyCollection<ParameterExpression> readOnlyCollection2 = base.VisitAndConvert<ParameterExpression>(node.Variables, "EnumerableRewriter.VisitBlock");
			if (type == node.Expressions.Last<Expression>().Type)
			{
				return Expression.Block(readOnlyCollection2, readOnlyCollection);
			}
			return Expression.Block(this.GetEquivalentType(type), readOnlyCollection2, readOnlyCollection);
		}

		protected internal override Expression VisitGoto(GotoExpression node)
		{
			Type type = node.Value.Type;
			if (!typeof(IQueryable).IsAssignableFrom(type))
			{
				return base.VisitGoto(node);
			}
			LabelTarget labelTarget = this.VisitLabelTarget(node.Target);
			Expression expression = this.Visit(node.Value);
			return Expression.MakeGoto(node.Kind, labelTarget, expression, this.GetEquivalentType(typeof(EnumerableQuery).IsAssignableFrom(type) ? expression.Type : type));
		}

		protected override LabelTarget VisitLabelTarget(LabelTarget node)
		{
			LabelTarget labelTarget;
			if (this._targetCache == null)
			{
				this._targetCache = new Dictionary<LabelTarget, LabelTarget>();
			}
			else if (this._targetCache.TryGetValue(node, out labelTarget))
			{
				return labelTarget;
			}
			Type type = node.Type;
			if (!typeof(IQueryable).IsAssignableFrom(type))
			{
				labelTarget = base.VisitLabelTarget(node);
			}
			else
			{
				labelTarget = Expression.Label(this.GetEquivalentType(type), node.Name);
			}
			this._targetCache.Add(node, labelTarget);
			return labelTarget;
		}

		private Dictionary<LabelTarget, LabelTarget> _targetCache;

		private Dictionary<Type, Type> _equivalentTypeCache;

		private static ILookup<string, MethodInfo> s_seqMethods;
	}
}
