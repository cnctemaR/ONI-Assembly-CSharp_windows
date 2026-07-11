using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
	public sealed class MethodCallExpression : Expression
	{
		internal MethodCallExpression(MethodInfo method, ReadOnlyCollection<Expression> arguments)
			: base(ExpressionType.Call, method.ReturnType)
		{
			this.method = method;
			this.arguments = arguments;
		}

		internal MethodCallExpression(Expression obj, MethodInfo method, ReadOnlyCollection<Expression> arguments)
			: base(ExpressionType.Call, method.ReturnType)
		{
			this.obj = obj;
			this.method = method;
			this.arguments = arguments;
		}

		public Expression Object
		{
			get
			{
				return this.obj;
			}
		}

		public MethodInfo Method
		{
			get
			{
				return this.method;
			}
		}

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		internal override void Emit(EmitContext ec)
		{
			ec.EmitCall(this.obj, this.arguments, this.method);
		}

		private Expression obj;

		private MethodInfo method;

		private ReadOnlyCollection<Expression> arguments;
	}
}
