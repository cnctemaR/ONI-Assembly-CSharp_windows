using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class NewExpression : Expression
	{
		internal NewExpression(Type type, ReadOnlyCollection<Expression> arguments)
			: base(ExpressionType.New, type)
		{
			this.arguments = arguments;
		}

		internal NewExpression(ConstructorInfo constructor, ReadOnlyCollection<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
			: base(ExpressionType.New, constructor.DeclaringType)
		{
			this.constructor = constructor;
			this.arguments = arguments;
			this.members = members;
		}

		public ConstructorInfo Constructor
		{
			get
			{
				return this.constructor;
			}
		}

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public ReadOnlyCollection<MemberInfo> Members
		{
			get
			{
				return this.members;
			}
		}

		internal override void Emit(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			Type type = base.Type;
			LocalBuilder localBuilder = null;
			if (type.IsValueType)
			{
				localBuilder = ig.DeclareLocal(type);
				ig.Emit(OpCodes.Ldloca, localBuilder);
				if (this.constructor == null)
				{
					ig.Emit(OpCodes.Initobj, type);
					ig.Emit(OpCodes.Ldloc, localBuilder);
					return;
				}
			}
			ec.EmitCollection<Expression>(this.arguments);
			if (type.IsValueType)
			{
				ig.Emit(OpCodes.Call, this.constructor);
				ig.Emit(OpCodes.Ldloc, localBuilder);
			}
			else
			{
				ig.Emit(OpCodes.Newobj, this.constructor ?? NewExpression.GetDefaultConstructor(type));
			}
		}

		private static ConstructorInfo GetDefaultConstructor(Type type)
		{
			return type.GetConstructor(Type.EmptyTypes);
		}

		private ConstructorInfo constructor;

		private ReadOnlyCollection<Expression> arguments;

		private ReadOnlyCollection<MemberInfo> members;
	}
}
