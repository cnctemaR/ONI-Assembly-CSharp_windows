using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class MemberListBinding : MemberBinding
	{
		internal MemberListBinding(MemberInfo member, ReadOnlyCollection<ElementInit> initializers)
			: base(MemberBindingType.ListBinding, member)
		{
			this.initializers = initializers;
		}

		public ReadOnlyCollection<ElementInit> Initializers
		{
			get
			{
				return this.initializers;
			}
		}

		internal override void Emit(EmitContext ec, LocalBuilder local)
		{
			LocalBuilder localBuilder = base.EmitLoadMember(ec, local);
			foreach (ElementInit elementInit in this.initializers)
			{
				elementInit.Emit(ec, localBuilder);
			}
		}

		private ReadOnlyCollection<ElementInit> initializers;
	}
}
