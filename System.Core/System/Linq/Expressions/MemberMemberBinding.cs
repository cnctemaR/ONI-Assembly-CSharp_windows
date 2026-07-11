using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;
using Unity;

namespace System.Linq.Expressions
{
	public sealed class MemberMemberBinding : MemberBinding
	{
		internal MemberMemberBinding(MemberInfo member, ReadOnlyCollection<MemberBinding> bindings)
			: base(MemberBindingType.MemberBinding, member)
		{
			this.Bindings = bindings;
		}

		public ReadOnlyCollection<MemberBinding> Bindings { get; }

		public MemberMemberBinding Update(IEnumerable<MemberBinding> bindings)
		{
			if (bindings != null && ExpressionUtils.SameElements<MemberBinding>(ref bindings, this.Bindings))
			{
				return this;
			}
			return Expression.MemberBind(base.Member, bindings);
		}

		internal override void ValidateAsDefinedHere(int index)
		{
		}

		internal MemberMemberBinding()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
