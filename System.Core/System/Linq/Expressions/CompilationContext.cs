using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	internal class CompilationContext
	{
		public int AddGlobal(object global)
		{
			return CompilationContext.AddItemToList<object>(global, this.globals);
		}

		public object[] GetGlobals()
		{
			return this.globals.ToArray();
		}

		private static int AddItemToList<T>(T item, IList<T> list)
		{
			list.Add(item);
			return list.Count - 1;
		}

		public int AddCompilationUnit(LambdaExpression lambda)
		{
			this.DetectHoistedVariables(lambda);
			return this.AddCompilationUnit(null, lambda);
		}

		public int AddCompilationUnit(EmitContext parent, LambdaExpression lambda)
		{
			EmitContext emitContext = new EmitContext(this, parent, lambda);
			int num = CompilationContext.AddItemToList<EmitContext>(emitContext, this.units);
			emitContext.Emit();
			return num;
		}

		private void DetectHoistedVariables(LambdaExpression lambda)
		{
			this.hoisted_map = new CompilationContext.HoistedVariableDetector().Process(lambda);
		}

		public List<ParameterExpression> GetHoistedLocals(LambdaExpression lambda)
		{
			if (this.hoisted_map == null)
			{
				return null;
			}
			List<ParameterExpression> list;
			this.hoisted_map.TryGetValue(lambda, out list);
			return list;
		}

		public object[] CreateHoistedLocals(int unit)
		{
			List<ParameterExpression> hoistedLocals = this.GetHoistedLocals(this.units[unit].Lambda);
			return new object[(hoistedLocals != null) ? hoistedLocals.Count : 0];
		}

		public Expression IsolateExpression(ExecutionScope scope, object[] locals, Expression expression)
		{
			return new CompilationContext.ParameterReplacer(this, scope, locals).Transform(expression);
		}

		public Delegate CreateDelegate()
		{
			return this.CreateDelegate(0, new ExecutionScope(this));
		}

		public Delegate CreateDelegate(int unit, ExecutionScope scope)
		{
			return this.units[unit].CreateDelegate(scope);
		}

		private List<object> globals = new List<object>();

		private List<EmitContext> units = new List<EmitContext>();

		private Dictionary<LambdaExpression, List<ParameterExpression>> hoisted_map;

		private class ParameterReplacer : ExpressionTransformer
		{
			public ParameterReplacer(CompilationContext context, ExecutionScope scope, object[] locals)
			{
				this.context = context;
				this.scope = scope;
				this.locals = locals;
			}

			protected override Expression VisitParameter(ParameterExpression parameter)
			{
				ExecutionScope parent = this.scope;
				object[] array = this.locals;
				while (parent != null)
				{
					int num = this.IndexOfHoistedLocal(parent, parameter);
					if (num != -1)
					{
						return this.ReadHoistedLocalFromArray(array, num);
					}
					array = parent.Locals;
					parent = parent.Parent;
				}
				return parameter;
			}

			private Expression ReadHoistedLocalFromArray(object[] locals, int position)
			{
				return Expression.Field(Expression.Convert(Expression.ArrayIndex(Expression.Constant(locals), Expression.Constant(position)), locals[position].GetType()), "Value");
			}

			private int IndexOfHoistedLocal(ExecutionScope scope, ParameterExpression parameter)
			{
				return this.context.units[scope.compilation_unit].IndexOfHoistedLocal(parameter);
			}

			private CompilationContext context;

			private ExecutionScope scope;

			private object[] locals;
		}

		private class HoistedVariableDetector : ExpressionVisitor
		{
			public Dictionary<LambdaExpression, List<ParameterExpression>> Process(LambdaExpression lambda)
			{
				this.Visit(lambda);
				return this.hoisted_map;
			}

			protected override void VisitLambda(LambdaExpression lambda)
			{
				this.lambda = lambda;
				foreach (ParameterExpression parameterExpression in lambda.Parameters)
				{
					this.parameter_to_lambda[parameterExpression] = lambda;
				}
				base.VisitLambda(lambda);
			}

			protected override void VisitParameter(ParameterExpression parameter)
			{
				if (this.lambda.Parameters.Contains(parameter))
				{
					return;
				}
				this.Hoist(parameter);
			}

			private void Hoist(ParameterExpression parameter)
			{
				LambdaExpression lambdaExpression;
				if (!this.parameter_to_lambda.TryGetValue(parameter, out lambdaExpression))
				{
					return;
				}
				if (this.hoisted_map == null)
				{
					this.hoisted_map = new Dictionary<LambdaExpression, List<ParameterExpression>>();
				}
				List<ParameterExpression> list;
				if (!this.hoisted_map.TryGetValue(lambdaExpression, out list))
				{
					list = new List<ParameterExpression>();
					this.hoisted_map[lambdaExpression] = list;
				}
				list.Add(parameter);
			}

			private Dictionary<ParameterExpression, LambdaExpression> parameter_to_lambda = new Dictionary<ParameterExpression, LambdaExpression>();

			private Dictionary<LambdaExpression, List<ParameterExpression>> hoisted_map;

			private LambdaExpression lambda;
		}
	}
}
