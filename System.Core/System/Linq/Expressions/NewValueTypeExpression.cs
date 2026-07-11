using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal sealed class NewValueTypeExpression : NewExpression
	{
		internal NewValueTypeExpression(Type type, ReadOnlyCollection<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
			: base(null, arguments, members)
		{
			this.Type = type;
		}

		public sealed override Type Type { get; }
	}
}
