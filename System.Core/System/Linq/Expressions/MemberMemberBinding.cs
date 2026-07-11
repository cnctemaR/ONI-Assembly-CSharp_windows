using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class MemberMemberBinding : MemberBinding
	{
		internal MemberMemberBinding(MemberInfo member, ReadOnlyCollection<MemberBinding> bindings)
			: base(MemberBindingType.MemberBinding, member)
		{
			this.bindings = bindings;
		}

		public ReadOnlyCollection<MemberBinding> Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		internal override void Emit(EmitContext ec, LocalBuilder local)
		{
			LocalBuilder localBuilder = base.EmitLoadMember(ec, local);
			foreach (MemberBinding memberBinding in this.bindings)
			{
				memberBinding.Emit(ec, localBuilder);
			}
		}

		private ReadOnlyCollection<MemberBinding> bindings;
	}
}
