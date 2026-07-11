using System;
using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
	public class ExecutionScope
	{
		private ExecutionScope(CompilationContext context, int compilation_unit)
		{
			this.context = context;
			this.compilation_unit = compilation_unit;
			this.Globals = context.GetGlobals();
		}

		internal ExecutionScope(CompilationContext context)
			: this(context, 0)
		{
		}

		internal ExecutionScope(CompilationContext context, int compilation_unit, ExecutionScope parent, object[] locals)
			: this(context, compilation_unit)
		{
			this.Parent = parent;
			this.Locals = locals;
		}

		public Delegate CreateDelegate(int indexLambda, object[] locals)
		{
			return this.context.CreateDelegate(indexLambda, new ExecutionScope(this.context, indexLambda, this, locals));
		}

		public object[] CreateHoistedLocals()
		{
			return this.context.CreateHoistedLocals(this.compilation_unit);
		}

		public Expression IsolateExpression(Expression expression, object[] locals)
		{
			return this.context.IsolateExpression(this, locals, expression);
		}

		public object[] Globals;

		public object[] Locals;

		public ExecutionScope Parent;

		internal CompilationContext context;

		internal int compilation_unit;
	}
}
