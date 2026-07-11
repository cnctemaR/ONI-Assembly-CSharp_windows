using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.MemberExpressionProxy))]
	public class MemberExpression : Expression
	{
		public MemberInfo Member
		{
			get
			{
				return this.GetMember();
			}
		}

		public Expression Expression { get; }

		internal MemberExpression(Expression expression)
		{
			this.Expression = expression;
		}

		internal static PropertyExpression Make(Expression expression, PropertyInfo property)
		{
			return new PropertyExpression(expression, property);
		}

		internal static FieldExpression Make(Expression expression, FieldInfo field)
		{
			return new FieldExpression(expression, field);
		}

		internal static MemberExpression Make(Expression expression, MemberInfo member)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if (!(fieldInfo == null))
			{
				return MemberExpression.Make(expression, fieldInfo);
			}
			return MemberExpression.Make(expression, (PropertyInfo)member);
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.MemberAccess;
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual MemberInfo GetMember()
		{
			throw ContractUtils.Unreachable;
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitMember(this);
		}

		public MemberExpression Update(Expression expression)
		{
			if (expression == this.Expression)
			{
				return this;
			}
			return Expression.MakeMemberAccess(expression, this.Member);
		}
	}
}
