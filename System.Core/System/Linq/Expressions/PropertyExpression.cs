using System;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal sealed class PropertyExpression : MemberExpression
	{
		public PropertyExpression(Expression expression, PropertyInfo member)
			: base(expression)
		{
			this._property = member;
		}

		internal override MemberInfo GetMember()
		{
			return this._property;
		}

		public sealed override Type Type
		{
			get
			{
				return this._property.PropertyType;
			}
		}

		private readonly PropertyInfo _property;
	}
}
