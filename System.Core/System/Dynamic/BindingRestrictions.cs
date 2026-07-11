using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace System.Dynamic
{
	[DebuggerTypeProxy(typeof(BindingRestrictions.BindingRestrictionsProxy))]
	[DebuggerDisplay("{DebugView}")]
	public abstract class BindingRestrictions
	{
		private BindingRestrictions()
		{
		}

		internal abstract Expression GetExpression();

		public BindingRestrictions Merge(BindingRestrictions restrictions)
		{
			ContractUtils.RequiresNotNull(restrictions, "restrictions");
			if (this == BindingRestrictions.Empty)
			{
				return restrictions;
			}
			if (restrictions == BindingRestrictions.Empty)
			{
				return this;
			}
			return new BindingRestrictions.MergedRestriction(this, restrictions);
		}

		public static BindingRestrictions GetTypeRestriction(Expression expression, Type type)
		{
			ContractUtils.RequiresNotNull(expression, "expression");
			ContractUtils.RequiresNotNull(type, "type");
			return new BindingRestrictions.TypeRestriction(expression, type);
		}

		internal static BindingRestrictions GetTypeRestriction(DynamicMetaObject obj)
		{
			if (obj.Value == null && obj.HasValue)
			{
				return BindingRestrictions.GetInstanceRestriction(obj.Expression, null);
			}
			return BindingRestrictions.GetTypeRestriction(obj.Expression, obj.LimitType);
		}

		public static BindingRestrictions GetInstanceRestriction(Expression expression, object instance)
		{
			ContractUtils.RequiresNotNull(expression, "expression");
			return new BindingRestrictions.InstanceRestriction(expression, instance);
		}

		public static BindingRestrictions GetExpressionRestriction(Expression expression)
		{
			ContractUtils.RequiresNotNull(expression, "expression");
			ContractUtils.Requires(expression.Type == typeof(bool), "expression");
			return new BindingRestrictions.CustomRestriction(expression);
		}

		public static BindingRestrictions Combine(IList<DynamicMetaObject> contributingObjects)
		{
			BindingRestrictions bindingRestrictions = BindingRestrictions.Empty;
			if (contributingObjects != null)
			{
				foreach (DynamicMetaObject dynamicMetaObject in contributingObjects)
				{
					if (dynamicMetaObject != null)
					{
						bindingRestrictions = bindingRestrictions.Merge(dynamicMetaObject.Restrictions);
					}
				}
			}
			return bindingRestrictions;
		}

		public Expression ToExpression()
		{
			return this.GetExpression();
		}

		private string DebugView
		{
			get
			{
				return this.ToExpression().ToString();
			}
		}

		public static readonly BindingRestrictions Empty = new BindingRestrictions.CustomRestriction(Utils.Constant(true));

		private const int TypeRestrictionHash = 1227133513;

		private const int InstanceRestrictionHash = -1840700270;

		private const int CustomRestrictionHash = 613566756;

		private sealed class TestBuilder
		{
			internal void Append(BindingRestrictions restrictions)
			{
				if (this._unique.Add(restrictions))
				{
					this.Push(restrictions.GetExpression(), 0);
				}
			}

			internal Expression ToExpression()
			{
				Expression expression = this._tests.Pop().Node;
				while (this._tests.Count > 0)
				{
					expression = Expression.AndAlso(this._tests.Pop().Node, expression);
				}
				return expression;
			}

			private void Push(Expression node, int depth)
			{
				while (this._tests.Count > 0 && this._tests.Peek().Depth == depth)
				{
					node = Expression.AndAlso(this._tests.Pop().Node, node);
					depth++;
				}
				this._tests.Push(new BindingRestrictions.TestBuilder.AndNode
				{
					Node = node,
					Depth = depth
				});
			}

			private readonly HashSet<BindingRestrictions> _unique = new HashSet<BindingRestrictions>();

			private readonly Stack<BindingRestrictions.TestBuilder.AndNode> _tests = new Stack<BindingRestrictions.TestBuilder.AndNode>();

			private struct AndNode
			{
				internal int Depth;

				internal Expression Node;
			}
		}

		private sealed class MergedRestriction : BindingRestrictions
		{
			internal MergedRestriction(BindingRestrictions left, BindingRestrictions right)
			{
				this.Left = left;
				this.Right = right;
			}

			internal override Expression GetExpression()
			{
				BindingRestrictions.TestBuilder testBuilder = new BindingRestrictions.TestBuilder();
				Stack<BindingRestrictions> stack = new Stack<BindingRestrictions>();
				BindingRestrictions bindingRestrictions = this;
				for (;;)
				{
					BindingRestrictions.MergedRestriction mergedRestriction = bindingRestrictions as BindingRestrictions.MergedRestriction;
					if (mergedRestriction != null)
					{
						stack.Push(mergedRestriction.Right);
						bindingRestrictions = mergedRestriction.Left;
					}
					else
					{
						testBuilder.Append(bindingRestrictions);
						if (stack.Count == 0)
						{
							break;
						}
						bindingRestrictions = stack.Pop();
					}
				}
				return testBuilder.ToExpression();
			}

			internal readonly BindingRestrictions Left;

			internal readonly BindingRestrictions Right;
		}

		private sealed class CustomRestriction : BindingRestrictions
		{
			internal CustomRestriction(Expression expression)
			{
				this._expression = expression;
			}

			public override bool Equals(object obj)
			{
				BindingRestrictions.CustomRestriction customRestriction = obj as BindingRestrictions.CustomRestriction;
				return ((customRestriction != null) ? customRestriction._expression : null) == this._expression;
			}

			public override int GetHashCode()
			{
				return 613566756 ^ this._expression.GetHashCode();
			}

			internal override Expression GetExpression()
			{
				return this._expression;
			}

			private readonly Expression _expression;
		}

		private sealed class TypeRestriction : BindingRestrictions
		{
			internal TypeRestriction(Expression parameter, Type type)
			{
				this._expression = parameter;
				this._type = type;
			}

			public override bool Equals(object obj)
			{
				BindingRestrictions.TypeRestriction typeRestriction = obj as BindingRestrictions.TypeRestriction;
				return ((typeRestriction != null) ? typeRestriction._expression : null) == this._expression && TypeUtils.AreEquivalent(typeRestriction._type, this._type);
			}

			public override int GetHashCode()
			{
				return 1227133513 ^ this._expression.GetHashCode() ^ this._type.GetHashCode();
			}

			internal override Expression GetExpression()
			{
				return Expression.TypeEqual(this._expression, this._type);
			}

			private readonly Expression _expression;

			private readonly Type _type;
		}

		private sealed class InstanceRestriction : BindingRestrictions
		{
			internal InstanceRestriction(Expression parameter, object instance)
			{
				this._expression = parameter;
				this._instance = instance;
			}

			public override bool Equals(object obj)
			{
				BindingRestrictions.InstanceRestriction instanceRestriction = obj as BindingRestrictions.InstanceRestriction;
				return ((instanceRestriction != null) ? instanceRestriction._expression : null) == this._expression && instanceRestriction._instance == this._instance;
			}

			public override int GetHashCode()
			{
				return -1840700270 ^ RuntimeHelpers.GetHashCode(this._instance) ^ this._expression.GetHashCode();
			}

			internal override Expression GetExpression()
			{
				if (this._instance == null)
				{
					return Expression.Equal(Expression.Convert(this._expression, typeof(object)), Utils.Null);
				}
				ParameterExpression parameterExpression = Expression.Parameter(typeof(object), null);
				return Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(new ParameterExpression[] { parameterExpression }), new TrueReadOnlyCollection<Expression>(new Expression[]
				{
					Expression.Assign(parameterExpression, Expression.Constant(this._instance, typeof(object))),
					Expression.AndAlso(Expression.NotEqual(parameterExpression, Utils.Null), Expression.Equal(Expression.Convert(this._expression, typeof(object)), parameterExpression))
				}));
			}

			private readonly Expression _expression;

			private readonly object _instance;
		}

		private sealed class BindingRestrictionsProxy
		{
			public BindingRestrictionsProxy(BindingRestrictions node)
			{
				ContractUtils.RequiresNotNull(node, "node");
				this._node = node;
			}

			public bool IsEmpty
			{
				get
				{
					return this._node == BindingRestrictions.Empty;
				}
			}

			public Expression Test
			{
				get
				{
					return this._node.ToExpression();
				}
			}

			public BindingRestrictions[] Restrictions
			{
				get
				{
					List<BindingRestrictions> list = new List<BindingRestrictions>();
					Stack<BindingRestrictions> stack = new Stack<BindingRestrictions>();
					BindingRestrictions bindingRestrictions = this._node;
					for (;;)
					{
						BindingRestrictions.MergedRestriction mergedRestriction = bindingRestrictions as BindingRestrictions.MergedRestriction;
						if (mergedRestriction != null)
						{
							stack.Push(mergedRestriction.Right);
							bindingRestrictions = mergedRestriction.Left;
						}
						else
						{
							list.Add(bindingRestrictions);
							if (stack.Count == 0)
							{
								break;
							}
							bindingRestrictions = stack.Pop();
						}
					}
					return list.ToArray();
				}
			}

			public override string ToString()
			{
				return this._node.DebugView;
			}

			private readonly BindingRestrictions _node;
		}
	}
}
