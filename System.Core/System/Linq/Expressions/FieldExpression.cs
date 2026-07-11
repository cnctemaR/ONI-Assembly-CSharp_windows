using System;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal sealed class FieldExpression : MemberExpression
	{
		public FieldExpression(Expression expression, FieldInfo member)
			: base(expression)
		{
			this._field = member;
		}

		internal override MemberInfo GetMember()
		{
			return this._field;
		}

		public sealed override Type Type
		{
			get
			{
				return this._field.FieldType;
			}
		}

		private readonly FieldInfo _field;
	}
}
