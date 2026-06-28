using System;
using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public class LambdaExpression : Expression
	{
		internal LambdaExpression(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
			: base(ExpressionType.Lambda, delegateType)
		{
			this.body = body;
			this.parameters = parameters;
		}

		public Expression Body
		{
			get
			{
				return this.body;
			}
		}

		public ReadOnlyCollection<ParameterExpression> Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		private void EmitPopIfNeeded(EmitContext ec)
		{
			if (this.GetReturnType() == typeof(void) && this.body.Type != typeof(void))
			{
				ec.ig.Emit(OpCodes.Pop);
			}
		}

		internal override void Emit(EmitContext ec)
		{
			ec.EmitCreateDelegate(this);
		}

		internal void EmitBody(EmitContext ec)
		{
			this.body.Emit(ec);
			this.EmitPopIfNeeded(ec);
			ec.ig.Emit(OpCodes.Ret);
		}

		internal Type GetReturnType()
		{
			return base.Type.GetInvokeMethod().ReturnType;
		}

		public Delegate Compile()
		{
			CompilationContext compilationContext = new CompilationContext();
			compilationContext.AddCompilationUnit(this);
			return compilationContext.CreateDelegate();
		}

		private Expression body;

		private ReadOnlyCollection<ParameterExpression> parameters;
	}
}
