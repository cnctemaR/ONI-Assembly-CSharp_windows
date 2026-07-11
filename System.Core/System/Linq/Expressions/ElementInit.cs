using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class ElementInit
	{
		internal ElementInit(MethodInfo add_method, ReadOnlyCollection<Expression> arguments)
		{
			this.add_method = add_method;
			this.arguments = arguments;
		}

		public MethodInfo AddMethod
		{
			get
			{
				return this.add_method;
			}
		}

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public override string ToString()
		{
			return ExpressionPrinter.ToString(this);
		}

		private void EmitPopIfNeeded(EmitContext ec)
		{
			if (this.add_method.ReturnType == typeof(void))
			{
				return;
			}
			ec.ig.Emit(OpCodes.Pop);
		}

		internal void Emit(EmitContext ec, LocalBuilder local)
		{
			ec.EmitCall(local, this.arguments, this.add_method);
			this.EmitPopIfNeeded(ec);
		}

		private MethodInfo add_method;

		private ReadOnlyCollection<Expression> arguments;
	}
}
