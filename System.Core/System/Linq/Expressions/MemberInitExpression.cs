using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.MemberInitExpressionProxy))]
	public sealed class MemberInitExpression : Expression
	{
		internal MemberInitExpression(NewExpression newExpression, ReadOnlyCollection<MemberBinding> bindings)
		{
			this.NewExpression = newExpression;
			this.Bindings = bindings;
		}

		public sealed override Type Type
		{
			get
			{
				return this.NewExpression.Type;
			}
		}

		public override bool CanReduce
		{
			get
			{
				return true;
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.MemberInit;
			}
		}

		public NewExpression NewExpression { get; }

		public ReadOnlyCollection<MemberBinding> Bindings { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitMemberInit(this);
		}

		public override Expression Reduce()
		{
			return MemberInitExpression.ReduceMemberInit(this.NewExpression, this.Bindings, true);
		}

		private static Expression ReduceMemberInit(Expression objExpression, ReadOnlyCollection<MemberBinding> bindings, bool keepOnStack)
		{
			ParameterExpression parameterExpression = Expression.Variable(objExpression.Type);
			int count = bindings.Count;
			Expression[] array = new Expression[count + 2];
			array[0] = Expression.Assign(parameterExpression, objExpression);
			for (int i = 0; i < count; i++)
			{
				array[i + 1] = MemberInitExpression.ReduceMemberBinding(parameterExpression, bindings[i]);
			}
			array[count + 1] = (keepOnStack ? parameterExpression : Utils.Empty);
			return Expression.Block(new ParameterExpression[] { parameterExpression }, array);
		}

		internal static Expression ReduceListInit(Expression listExpression, ReadOnlyCollection<ElementInit> initializers, bool keepOnStack)
		{
			ParameterExpression parameterExpression = Expression.Variable(listExpression.Type);
			int count = initializers.Count;
			Expression[] array = new Expression[count + 2];
			array[0] = Expression.Assign(parameterExpression, listExpression);
			for (int i = 0; i < count; i++)
			{
				ElementInit elementInit = initializers[i];
				array[i + 1] = Expression.Call(parameterExpression, elementInit.AddMethod, elementInit.Arguments);
			}
			array[count + 1] = (keepOnStack ? parameterExpression : Utils.Empty);
			return Expression.Block(new ParameterExpression[] { parameterExpression }, array);
		}

		internal static Expression ReduceMemberBinding(ParameterExpression objVar, MemberBinding binding)
		{
			MemberExpression memberExpression = Expression.MakeMemberAccess(objVar, binding.Member);
			switch (binding.BindingType)
			{
			case MemberBindingType.Assignment:
				return Expression.Assign(memberExpression, ((MemberAssignment)binding).Expression);
			case MemberBindingType.MemberBinding:
				return MemberInitExpression.ReduceMemberInit(memberExpression, ((MemberMemberBinding)binding).Bindings, false);
			case MemberBindingType.ListBinding:
				return MemberInitExpression.ReduceListInit(memberExpression, ((MemberListBinding)binding).Initializers, false);
			default:
				throw ContractUtils.Unreachable;
			}
		}

		public MemberInitExpression Update(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
		{
			if (((newExpression == this.NewExpression) & (bindings != null)) && ExpressionUtils.SameElements<MemberBinding>(ref bindings, this.Bindings))
			{
				return this;
			}
			return Expression.MemberInit(newExpression, bindings);
		}

		internal MemberInitExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
