using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Compiler
{
	internal sealed class StackSpiller
	{
		private StackSpiller.Result RewriteExpression(Expression node, StackSpiller.Stack stack)
		{
			if (node == null)
			{
				return new StackSpiller.Result(StackSpiller.RewriteAction.None, null);
			}
			if (!this._guard.TryEnterOnCurrentStack())
			{
				return this._guard.RunOnEmptyStack<StackSpiller, Expression, StackSpiller.Stack, StackSpiller.Result>((StackSpiller @this, Expression n, StackSpiller.Stack s) => @this.RewriteExpression(n, s), this, node, stack);
			}
			StackSpiller.Result result;
			switch (node.NodeType)
			{
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
			case ExpressionType.And:
			case ExpressionType.ArrayIndex:
			case ExpressionType.Divide:
			case ExpressionType.Equal:
			case ExpressionType.ExclusiveOr:
			case ExpressionType.GreaterThan:
			case ExpressionType.GreaterThanOrEqual:
			case ExpressionType.LeftShift:
			case ExpressionType.LessThan:
			case ExpressionType.LessThanOrEqual:
			case ExpressionType.Modulo:
			case ExpressionType.Multiply:
			case ExpressionType.MultiplyChecked:
			case ExpressionType.NotEqual:
			case ExpressionType.Or:
			case ExpressionType.Power:
			case ExpressionType.RightShift:
			case ExpressionType.Subtract:
			case ExpressionType.SubtractChecked:
				result = this.RewriteBinaryExpression(node, stack);
				break;
			case ExpressionType.AndAlso:
			case ExpressionType.Coalesce:
			case ExpressionType.OrElse:
				result = this.RewriteLogicalBinaryExpression(node, stack);
				break;
			case ExpressionType.ArrayLength:
			case ExpressionType.Convert:
			case ExpressionType.ConvertChecked:
			case ExpressionType.Negate:
			case ExpressionType.UnaryPlus:
			case ExpressionType.NegateChecked:
			case ExpressionType.Not:
			case ExpressionType.TypeAs:
			case ExpressionType.Decrement:
			case ExpressionType.Increment:
			case ExpressionType.Unbox:
			case ExpressionType.OnesComplement:
			case ExpressionType.IsTrue:
			case ExpressionType.IsFalse:
				result = this.RewriteUnaryExpression(node, stack);
				break;
			case ExpressionType.Call:
				result = this.RewriteMethodCallExpression(node, stack);
				break;
			case ExpressionType.Conditional:
				result = this.RewriteConditionalExpression(node, stack);
				break;
			case ExpressionType.Constant:
			case ExpressionType.Parameter:
			case ExpressionType.Quote:
			case ExpressionType.DebugInfo:
			case ExpressionType.Default:
			case ExpressionType.RuntimeVariables:
				result = new StackSpiller.Result(StackSpiller.RewriteAction.None, node);
				break;
			case ExpressionType.Invoke:
				result = this.RewriteInvocationExpression(node, stack);
				break;
			case ExpressionType.Lambda:
				result = StackSpiller.RewriteLambdaExpression(node);
				break;
			case ExpressionType.ListInit:
				result = this.RewriteListInitExpression(node, stack);
				break;
			case ExpressionType.MemberAccess:
				result = this.RewriteMemberExpression(node, stack);
				break;
			case ExpressionType.MemberInit:
				result = this.RewriteMemberInitExpression(node, stack);
				break;
			case ExpressionType.New:
				result = this.RewriteNewExpression(node, stack);
				break;
			case ExpressionType.NewArrayInit:
			case ExpressionType.NewArrayBounds:
				result = this.RewriteNewArrayExpression(node, stack);
				break;
			case ExpressionType.TypeIs:
			case ExpressionType.TypeEqual:
				result = this.RewriteTypeBinaryExpression(node, stack);
				break;
			case ExpressionType.Assign:
				result = this.RewriteAssignBinaryExpression(node, stack);
				break;
			case ExpressionType.Block:
				result = this.RewriteBlockExpression(node, stack);
				break;
			case ExpressionType.Dynamic:
				result = this.RewriteDynamicExpression(node);
				break;
			case ExpressionType.Extension:
				result = this.RewriteExtensionExpression(node, stack);
				break;
			case ExpressionType.Goto:
				result = this.RewriteGotoExpression(node, stack);
				break;
			case ExpressionType.Index:
				result = this.RewriteIndexExpression(node, stack);
				break;
			case ExpressionType.Label:
				result = this.RewriteLabelExpression(node, stack);
				break;
			case ExpressionType.Loop:
				result = this.RewriteLoopExpression(node, stack);
				break;
			case ExpressionType.Switch:
				result = this.RewriteSwitchExpression(node, stack);
				break;
			case ExpressionType.Throw:
				result = this.RewriteThrowUnaryExpression(node, stack);
				break;
			case ExpressionType.Try:
				result = this.RewriteTryExpression(node, stack);
				break;
			case ExpressionType.AddAssign:
			case ExpressionType.AndAssign:
			case ExpressionType.DivideAssign:
			case ExpressionType.ExclusiveOrAssign:
			case ExpressionType.LeftShiftAssign:
			case ExpressionType.ModuloAssign:
			case ExpressionType.MultiplyAssign:
			case ExpressionType.OrAssign:
			case ExpressionType.PowerAssign:
			case ExpressionType.RightShiftAssign:
			case ExpressionType.SubtractAssign:
			case ExpressionType.AddAssignChecked:
			case ExpressionType.MultiplyAssignChecked:
			case ExpressionType.SubtractAssignChecked:
			case ExpressionType.PreIncrementAssign:
			case ExpressionType.PreDecrementAssign:
			case ExpressionType.PostIncrementAssign:
			case ExpressionType.PostDecrementAssign:
				result = this.RewriteReducibleExpression(node, stack);
				break;
			default:
				result = this.RewriteExpression(node.ReduceAndCheck(), stack);
				if (result.Action == StackSpiller.RewriteAction.None)
				{
					result = new StackSpiller.Result(result.Action | StackSpiller.RewriteAction.Copy, result.Node);
				}
				break;
			}
			return result;
		}

		private static Expression MakeBlock(global::System.Collections.Generic.ArrayBuilder<Expression> expressions)
		{
			return new SpilledExpressionBlock(expressions.ToArray());
		}

		private static Expression MakeBlock(params Expression[] expressions)
		{
			return new SpilledExpressionBlock(expressions);
		}

		private static Expression MakeBlock(IReadOnlyList<Expression> expressions)
		{
			return new SpilledExpressionBlock(expressions);
		}

		private ParameterExpression MakeTemp(Type type)
		{
			return this._tm.Temp(type);
		}

		private int Mark()
		{
			return this._tm.Mark();
		}

		private void Free(int mark)
		{
			this._tm.Free(mark);
		}

		[Conditional("DEBUG")]
		private void VerifyTemps()
		{
		}

		private ParameterExpression ToTemp(Expression expression, out Expression save, bool byRef)
		{
			Type type = (byRef ? expression.Type.MakeByRefType() : expression.Type);
			ParameterExpression parameterExpression = this.MakeTemp(type);
			save = AssignBinaryExpression.Make(parameterExpression, expression, byRef);
			return parameterExpression;
		}

		internal static LambdaExpression AnalyzeLambda(LambdaExpression lambda)
		{
			return lambda.Accept(new StackSpiller(StackSpiller.Stack.Empty));
		}

		private StackSpiller(StackSpiller.Stack stack)
		{
			this._startingStack = stack;
		}

		internal Expression<T> Rewrite<T>(Expression<T> lambda)
		{
			StackSpiller.Result result = this.RewriteExpressionFreeTemps(lambda.Body, this._startingStack);
			this._lambdaRewrite = result.Action;
			if (result.Action != StackSpiller.RewriteAction.None)
			{
				Expression expression = result.Node;
				if (this._tm.Temps.Count > 0)
				{
					expression = Expression.Block(this._tm.Temps, new TrueReadOnlyCollection<Expression>(new Expression[] { expression }));
				}
				return Expression<T>.Create(expression, lambda.Name, lambda.TailCall, new ParameterList(lambda));
			}
			return lambda;
		}

		[Conditional("DEBUG")]
		private static void VerifyRewrite(StackSpiller.Result result, Expression node)
		{
		}

		private StackSpiller.Result RewriteExpressionFreeTemps(Expression expression, StackSpiller.Stack stack)
		{
			int num = this.Mark();
			StackSpiller.Result result = this.RewriteExpression(expression, stack);
			this.Free(num);
			return result;
		}

		private StackSpiller.Result RewriteDynamicExpression(Expression expr)
		{
			IDynamicExpression dynamicExpression = (IDynamicExpression)expr;
			StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, StackSpiller.Stack.NonEmpty, dynamicExpression.ArgumentCount);
			childRewriter.AddArguments(dynamicExpression);
			if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
			{
				StackSpiller.RequireNoRefArgs(dynamicExpression.DelegateType.GetInvokeMethod());
			}
			return childRewriter.Finish(childRewriter.Rewrite ? dynamicExpression.Rewrite(childRewriter[0, -1]) : expr);
		}

		private StackSpiller.Result RewriteIndexAssignment(BinaryExpression node, StackSpiller.Stack stack)
		{
			IndexExpression indexExpression = (IndexExpression)node.Left;
			StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, stack, 2 + indexExpression.ArgumentCount);
			childRewriter.Add(indexExpression.Object);
			childRewriter.AddArguments(indexExpression);
			childRewriter.Add(node.Right);
			if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
			{
				childRewriter.MarkRefInstance(indexExpression.Object);
			}
			if (childRewriter.Rewrite)
			{
				node = new AssignBinaryExpression(new IndexExpression(childRewriter[0], indexExpression.Indexer, childRewriter[1, -2]), childRewriter[-1]);
			}
			return childRewriter.Finish(node);
		}

		private StackSpiller.Result RewriteLogicalBinaryExpression(Expression expr, StackSpiller.Stack stack)
		{
			BinaryExpression binaryExpression = (BinaryExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(binaryExpression.Left, stack);
			StackSpiller.Result result2 = this.RewriteExpression(binaryExpression.Right, stack);
			StackSpiller.Result result3 = this.RewriteExpression(binaryExpression.Conversion, stack);
			StackSpiller.RewriteAction rewriteAction = result.Action | result2.Action | result3.Action;
			if (rewriteAction != StackSpiller.RewriteAction.None)
			{
				expr = BinaryExpression.Create(binaryExpression.NodeType, result.Node, result2.Node, binaryExpression.Type, binaryExpression.Method, (LambdaExpression)result3.Node);
			}
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteReducibleExpression(Expression expr, StackSpiller.Stack stack)
		{
			StackSpiller.Result result = this.RewriteExpression(expr.Reduce(), stack);
			return new StackSpiller.Result(result.Action | StackSpiller.RewriteAction.Copy, result.Node);
		}

		private StackSpiller.Result RewriteBinaryExpression(Expression expr, StackSpiller.Stack stack)
		{
			BinaryExpression binaryExpression = (BinaryExpression)expr;
			StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, stack, 3);
			childRewriter.Add(binaryExpression.Left);
			childRewriter.Add(binaryExpression.Right);
			childRewriter.Add(binaryExpression.Conversion);
			if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
			{
				StackSpiller.RequireNoRefArgs(binaryExpression.Method);
			}
			return childRewriter.Finish(childRewriter.Rewrite ? BinaryExpression.Create(binaryExpression.NodeType, childRewriter[0], childRewriter[1], binaryExpression.Type, binaryExpression.Method, (LambdaExpression)childRewriter[2]) : expr);
		}

		private StackSpiller.Result RewriteVariableAssignment(BinaryExpression node, StackSpiller.Stack stack)
		{
			StackSpiller.Result result = this.RewriteExpression(node.Right, stack);
			if (result.Action != StackSpiller.RewriteAction.None)
			{
				node = new AssignBinaryExpression(node.Left, result.Node);
			}
			return new StackSpiller.Result(result.Action, node);
		}

		private StackSpiller.Result RewriteAssignBinaryExpression(Expression expr, StackSpiller.Stack stack)
		{
			BinaryExpression binaryExpression = (BinaryExpression)expr;
			ExpressionType nodeType = binaryExpression.Left.NodeType;
			if (nodeType <= ExpressionType.Parameter)
			{
				if (nodeType == ExpressionType.MemberAccess)
				{
					return this.RewriteMemberAssignment(binaryExpression, stack);
				}
				if (nodeType == ExpressionType.Parameter)
				{
					return this.RewriteVariableAssignment(binaryExpression, stack);
				}
			}
			else
			{
				if (nodeType == ExpressionType.Extension)
				{
					return this.RewriteExtensionAssignment(binaryExpression, stack);
				}
				if (nodeType == ExpressionType.Index)
				{
					return this.RewriteIndexAssignment(binaryExpression, stack);
				}
			}
			throw Error.InvalidLvalue(binaryExpression.Left.NodeType);
		}

		private StackSpiller.Result RewriteExtensionAssignment(BinaryExpression node, StackSpiller.Stack stack)
		{
			node = new AssignBinaryExpression(node.Left.ReduceExtensions(), node.Right);
			StackSpiller.Result result = this.RewriteAssignBinaryExpression(node, stack);
			return new StackSpiller.Result(result.Action | StackSpiller.RewriteAction.Copy, result.Node);
		}

		private static StackSpiller.Result RewriteLambdaExpression(Expression expr)
		{
			LambdaExpression lambdaExpression = (LambdaExpression)expr;
			expr = StackSpiller.AnalyzeLambda(lambdaExpression);
			return new StackSpiller.Result((expr == lambdaExpression) ? StackSpiller.RewriteAction.None : StackSpiller.RewriteAction.Copy, expr);
		}

		private StackSpiller.Result RewriteConditionalExpression(Expression expr, StackSpiller.Stack stack)
		{
			ConditionalExpression conditionalExpression = (ConditionalExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(conditionalExpression.Test, stack);
			StackSpiller.Result result2 = this.RewriteExpression(conditionalExpression.IfTrue, stack);
			StackSpiller.Result result3 = this.RewriteExpression(conditionalExpression.IfFalse, stack);
			StackSpiller.RewriteAction rewriteAction = result.Action | result2.Action | result3.Action;
			if (rewriteAction != StackSpiller.RewriteAction.None)
			{
				expr = ConditionalExpression.Make(result.Node, result2.Node, result3.Node, conditionalExpression.Type);
			}
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteMemberAssignment(BinaryExpression node, StackSpiller.Stack stack)
		{
			MemberExpression memberExpression = (MemberExpression)node.Left;
			StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, stack, 2);
			childRewriter.Add(memberExpression.Expression);
			childRewriter.Add(node.Right);
			if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
			{
				childRewriter.MarkRefInstance(memberExpression.Expression);
			}
			if (childRewriter.Rewrite)
			{
				return childRewriter.Finish(new AssignBinaryExpression(MemberExpression.Make(childRewriter[0], memberExpression.Member), childRewriter[1]));
			}
			return new StackSpiller.Result(StackSpiller.RewriteAction.None, node);
		}

		private StackSpiller.Result RewriteMemberExpression(Expression expr, StackSpiller.Stack stack)
		{
			MemberExpression memberExpression = (MemberExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(memberExpression.Expression, stack);
			if (result.Action != StackSpiller.RewriteAction.None)
			{
				if (result.Action == StackSpiller.RewriteAction.SpillStack && memberExpression.Member is PropertyInfo)
				{
					StackSpiller.RequireNotRefInstance(memberExpression.Expression);
				}
				expr = MemberExpression.Make(result.Node, memberExpression.Member);
			}
			return new StackSpiller.Result(result.Action, expr);
		}

		private StackSpiller.Result RewriteIndexExpression(Expression expr, StackSpiller.Stack stack)
		{
			IndexExpression indexExpression = (IndexExpression)expr;
			StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, stack, indexExpression.ArgumentCount + 1);
			childRewriter.Add(indexExpression.Object);
			childRewriter.AddArguments(indexExpression);
			if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
			{
				childRewriter.MarkRefInstance(indexExpression.Object);
			}
			if (childRewriter.Rewrite)
			{
				expr = new IndexExpression(childRewriter[0], indexExpression.Indexer, childRewriter[1, -1]);
			}
			return childRewriter.Finish(expr);
		}

		private StackSpiller.Result RewriteMethodCallExpression(Expression expr, StackSpiller.Stack stack)
		{
			MethodCallExpression methodCallExpression = (MethodCallExpression)expr;
			StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, stack, methodCallExpression.ArgumentCount + 1);
			childRewriter.Add(methodCallExpression.Object);
			childRewriter.AddArguments(methodCallExpression);
			if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
			{
				childRewriter.MarkRefInstance(methodCallExpression.Object);
				childRewriter.MarkRefArgs(methodCallExpression.Method, 1);
			}
			if (childRewriter.Rewrite)
			{
				if (methodCallExpression.Object != null)
				{
					expr = new InstanceMethodCallExpressionN(methodCallExpression.Method, childRewriter[0], childRewriter[1, -1]);
				}
				else
				{
					expr = new MethodCallExpressionN(methodCallExpression.Method, childRewriter[1, -1]);
				}
			}
			return childRewriter.Finish(expr);
		}

		private StackSpiller.Result RewriteNewArrayExpression(Expression expr, StackSpiller.Stack stack)
		{
			NewArrayExpression newArrayExpression = (NewArrayExpression)expr;
			if (newArrayExpression.NodeType == ExpressionType.NewArrayInit)
			{
				stack = StackSpiller.Stack.NonEmpty;
			}
			StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, stack, newArrayExpression.Expressions.Count);
			childRewriter.Add(newArrayExpression.Expressions);
			if (childRewriter.Rewrite)
			{
				expr = NewArrayExpression.Make(newArrayExpression.NodeType, newArrayExpression.Type, new TrueReadOnlyCollection<Expression>(childRewriter[0, -1]));
			}
			return childRewriter.Finish(expr);
		}

		private StackSpiller.Result RewriteInvocationExpression(Expression expr, StackSpiller.Stack stack)
		{
			InvocationExpression invocationExpression = (InvocationExpression)expr;
			LambdaExpression lambdaExpression = invocationExpression.LambdaOperand;
			StackSpiller.ChildRewriter childRewriter;
			if (lambdaExpression != null)
			{
				childRewriter = new StackSpiller.ChildRewriter(this, stack, invocationExpression.ArgumentCount);
				childRewriter.AddArguments(invocationExpression);
				if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
				{
					childRewriter.MarkRefArgs(Expression.GetInvokeMethod(invocationExpression.Expression), 0);
				}
				StackSpiller stackSpiller = new StackSpiller(stack);
				lambdaExpression = lambdaExpression.Accept(stackSpiller);
				if (childRewriter.Rewrite || stackSpiller._lambdaRewrite != StackSpiller.RewriteAction.None)
				{
					invocationExpression = new InvocationExpressionN(lambdaExpression, childRewriter[0, -1], invocationExpression.Type);
				}
				StackSpiller.Result result = childRewriter.Finish(invocationExpression);
				return new StackSpiller.Result(result.Action | stackSpiller._lambdaRewrite, result.Node);
			}
			childRewriter = new StackSpiller.ChildRewriter(this, stack, invocationExpression.ArgumentCount + 1);
			childRewriter.Add(invocationExpression.Expression);
			childRewriter.AddArguments(invocationExpression);
			if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
			{
				childRewriter.MarkRefArgs(Expression.GetInvokeMethod(invocationExpression.Expression), 1);
			}
			return childRewriter.Finish(childRewriter.Rewrite ? new InvocationExpressionN(childRewriter[0], childRewriter[1, -1], invocationExpression.Type) : expr);
		}

		private StackSpiller.Result RewriteNewExpression(Expression expr, StackSpiller.Stack stack)
		{
			NewExpression newExpression = (NewExpression)expr;
			StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, stack, newExpression.ArgumentCount);
			childRewriter.AddArguments(newExpression);
			if (childRewriter.Action == StackSpiller.RewriteAction.SpillStack)
			{
				childRewriter.MarkRefArgs(newExpression.Constructor, 0);
			}
			return childRewriter.Finish(childRewriter.Rewrite ? new NewExpression(newExpression.Constructor, childRewriter[0, -1], newExpression.Members) : expr);
		}

		private StackSpiller.Result RewriteTypeBinaryExpression(Expression expr, StackSpiller.Stack stack)
		{
			TypeBinaryExpression typeBinaryExpression = (TypeBinaryExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(typeBinaryExpression.Expression, stack);
			if (result.Action != StackSpiller.RewriteAction.None)
			{
				expr = new TypeBinaryExpression(result.Node, typeBinaryExpression.TypeOperand, typeBinaryExpression.NodeType);
			}
			return new StackSpiller.Result(result.Action, expr);
		}

		private StackSpiller.Result RewriteThrowUnaryExpression(Expression expr, StackSpiller.Stack stack)
		{
			UnaryExpression unaryExpression = (UnaryExpression)expr;
			StackSpiller.Result result = this.RewriteExpressionFreeTemps(unaryExpression.Operand, StackSpiller.Stack.Empty);
			StackSpiller.RewriteAction rewriteAction = result.Action;
			if (stack != StackSpiller.Stack.Empty)
			{
				rewriteAction = StackSpiller.RewriteAction.SpillStack;
			}
			if (rewriteAction != StackSpiller.RewriteAction.None)
			{
				expr = new UnaryExpression(ExpressionType.Throw, result.Node, unaryExpression.Type, null);
			}
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteUnaryExpression(Expression expr, StackSpiller.Stack stack)
		{
			UnaryExpression unaryExpression = (UnaryExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(unaryExpression.Operand, stack);
			if (result.Action == StackSpiller.RewriteAction.SpillStack)
			{
				StackSpiller.RequireNoRefArgs(unaryExpression.Method);
			}
			if (result.Action != StackSpiller.RewriteAction.None)
			{
				expr = new UnaryExpression(unaryExpression.NodeType, result.Node, unaryExpression.Type, unaryExpression.Method);
			}
			return new StackSpiller.Result(result.Action, expr);
		}

		private StackSpiller.Result RewriteListInitExpression(Expression expr, StackSpiller.Stack stack)
		{
			ListInitExpression listInitExpression = (ListInitExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(listInitExpression.NewExpression, stack);
			Expression node = result.Node;
			StackSpiller.RewriteAction rewriteAction = result.Action;
			ReadOnlyCollection<ElementInit> initializers = listInitExpression.Initializers;
			int count = initializers.Count;
			StackSpiller.ChildRewriter[] array = new StackSpiller.ChildRewriter[count];
			for (int i = 0; i < count; i++)
			{
				ElementInit elementInit = initializers[i];
				StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(this, StackSpiller.Stack.NonEmpty, elementInit.Arguments.Count);
				childRewriter.Add(elementInit.Arguments);
				rewriteAction |= childRewriter.Action;
				array[i] = childRewriter;
			}
			switch (rewriteAction)
			{
			case StackSpiller.RewriteAction.None:
				goto IL_01EA;
			case StackSpiller.RewriteAction.Copy:
			{
				ElementInit[] array2 = new ElementInit[count];
				for (int j = 0; j < count; j++)
				{
					StackSpiller.ChildRewriter childRewriter2 = array[j];
					if (childRewriter2.Action == StackSpiller.RewriteAction.None)
					{
						array2[j] = initializers[j];
					}
					else
					{
						array2[j] = new ElementInit(initializers[j].AddMethod, new TrueReadOnlyCollection<Expression>(childRewriter2[0, -1]));
					}
				}
				expr = new ListInitExpression((NewExpression)node, new TrueReadOnlyCollection<ElementInit>(array2));
				goto IL_01EA;
			}
			case StackSpiller.RewriteAction.SpillStack:
			{
				bool flag = StackSpiller.IsRefInstance(listInitExpression.NewExpression);
				global::System.Collections.Generic.ArrayBuilder<Expression> arrayBuilder = new global::System.Collections.Generic.ArrayBuilder<Expression>(count + 2 + (flag ? 1 : 0));
				ParameterExpression parameterExpression = this.MakeTemp(node.Type);
				arrayBuilder.UncheckedAdd(new AssignBinaryExpression(parameterExpression, node));
				ParameterExpression parameterExpression2 = parameterExpression;
				if (flag)
				{
					parameterExpression2 = this.MakeTemp(parameterExpression.Type.MakeByRefType());
					arrayBuilder.UncheckedAdd(new ByRefAssignBinaryExpression(parameterExpression2, parameterExpression));
				}
				for (int k = 0; k < count; k++)
				{
					StackSpiller.ChildRewriter childRewriter3 = array[k];
					StackSpiller.Result result2 = childRewriter3.Finish(new InstanceMethodCallExpressionN(initializers[k].AddMethod, parameterExpression2, childRewriter3[0, -1]));
					arrayBuilder.UncheckedAdd(result2.Node);
				}
				arrayBuilder.UncheckedAdd(parameterExpression);
				expr = StackSpiller.MakeBlock(arrayBuilder);
				goto IL_01EA;
			}
			}
			throw ContractUtils.Unreachable;
			IL_01EA:
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteMemberInitExpression(Expression expr, StackSpiller.Stack stack)
		{
			MemberInitExpression memberInitExpression = (MemberInitExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(memberInitExpression.NewExpression, stack);
			Expression node = result.Node;
			StackSpiller.RewriteAction rewriteAction = result.Action;
			ReadOnlyCollection<MemberBinding> bindings = memberInitExpression.Bindings;
			int count = bindings.Count;
			StackSpiller.BindingRewriter[] array = new StackSpiller.BindingRewriter[count];
			for (int i = 0; i < count; i++)
			{
				StackSpiller.BindingRewriter bindingRewriter = StackSpiller.BindingRewriter.Create(bindings[i], this, StackSpiller.Stack.NonEmpty);
				array[i] = bindingRewriter;
				rewriteAction |= bindingRewriter.Action;
			}
			switch (rewriteAction)
			{
			case StackSpiller.RewriteAction.None:
				goto IL_0175;
			case StackSpiller.RewriteAction.Copy:
			{
				MemberBinding[] array2 = new MemberBinding[count];
				for (int j = 0; j < count; j++)
				{
					array2[j] = array[j].AsBinding();
				}
				expr = new MemberInitExpression((NewExpression)node, new TrueReadOnlyCollection<MemberBinding>(array2));
				goto IL_0175;
			}
			case StackSpiller.RewriteAction.SpillStack:
			{
				bool flag = StackSpiller.IsRefInstance(memberInitExpression.NewExpression);
				global::System.Collections.Generic.ArrayBuilder<Expression> arrayBuilder = new global::System.Collections.Generic.ArrayBuilder<Expression>(count + 2 + (flag ? 1 : 0));
				ParameterExpression parameterExpression = this.MakeTemp(node.Type);
				arrayBuilder.UncheckedAdd(new AssignBinaryExpression(parameterExpression, node));
				ParameterExpression parameterExpression2 = parameterExpression;
				if (flag)
				{
					parameterExpression2 = this.MakeTemp(parameterExpression.Type.MakeByRefType());
					arrayBuilder.UncheckedAdd(new ByRefAssignBinaryExpression(parameterExpression2, parameterExpression));
				}
				for (int k = 0; k < count; k++)
				{
					Expression expression = array[k].AsExpression(parameterExpression2);
					arrayBuilder.UncheckedAdd(expression);
				}
				arrayBuilder.UncheckedAdd(parameterExpression);
				expr = StackSpiller.MakeBlock(arrayBuilder);
				goto IL_0175;
			}
			}
			throw ContractUtils.Unreachable;
			IL_0175:
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteBlockExpression(Expression expr, StackSpiller.Stack stack)
		{
			BlockExpression blockExpression = (BlockExpression)expr;
			int expressionCount = blockExpression.ExpressionCount;
			StackSpiller.RewriteAction rewriteAction = StackSpiller.RewriteAction.None;
			Expression[] array = null;
			for (int i = 0; i < expressionCount; i++)
			{
				Expression expression = blockExpression.GetExpression(i);
				StackSpiller.Result result = this.RewriteExpression(expression, stack);
				rewriteAction |= result.Action;
				if (array == null && result.Action != StackSpiller.RewriteAction.None)
				{
					array = StackSpiller.Clone<Expression>(blockExpression.Expressions, i);
				}
				if (array != null)
				{
					array[i] = result.Node;
				}
			}
			if (rewriteAction != StackSpiller.RewriteAction.None)
			{
				expr = blockExpression.Rewrite(null, array);
			}
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteLabelExpression(Expression expr, StackSpiller.Stack stack)
		{
			LabelExpression labelExpression = (LabelExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(labelExpression.DefaultValue, stack);
			if (result.Action != StackSpiller.RewriteAction.None)
			{
				expr = new LabelExpression(labelExpression.Target, result.Node);
			}
			return new StackSpiller.Result(result.Action, expr);
		}

		private StackSpiller.Result RewriteLoopExpression(Expression expr, StackSpiller.Stack stack)
		{
			LoopExpression loopExpression = (LoopExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(loopExpression.Body, StackSpiller.Stack.Empty);
			StackSpiller.RewriteAction rewriteAction = result.Action;
			if (stack != StackSpiller.Stack.Empty)
			{
				rewriteAction = StackSpiller.RewriteAction.SpillStack;
			}
			if (rewriteAction != StackSpiller.RewriteAction.None)
			{
				expr = new LoopExpression(result.Node, loopExpression.BreakLabel, loopExpression.ContinueLabel);
			}
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteGotoExpression(Expression expr, StackSpiller.Stack stack)
		{
			GotoExpression gotoExpression = (GotoExpression)expr;
			StackSpiller.Result result = this.RewriteExpressionFreeTemps(gotoExpression.Value, StackSpiller.Stack.Empty);
			StackSpiller.RewriteAction rewriteAction = result.Action;
			if (stack != StackSpiller.Stack.Empty)
			{
				rewriteAction = StackSpiller.RewriteAction.SpillStack;
			}
			if (rewriteAction != StackSpiller.RewriteAction.None)
			{
				expr = Expression.MakeGoto(gotoExpression.Kind, gotoExpression.Target, result.Node, gotoExpression.Type);
			}
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteSwitchExpression(Expression expr, StackSpiller.Stack stack)
		{
			SwitchExpression switchExpression = (SwitchExpression)expr;
			StackSpiller.Result result = this.RewriteExpressionFreeTemps(switchExpression.SwitchValue, stack);
			StackSpiller.RewriteAction rewriteAction = result.Action;
			ReadOnlyCollection<SwitchCase> readOnlyCollection = switchExpression.Cases;
			SwitchCase[] array = null;
			for (int i = 0; i < readOnlyCollection.Count; i++)
			{
				SwitchCase switchCase = readOnlyCollection[i];
				Expression[] array2 = null;
				ReadOnlyCollection<Expression> readOnlyCollection2 = switchCase.TestValues;
				for (int j = 0; j < readOnlyCollection2.Count; j++)
				{
					StackSpiller.Result result2 = this.RewriteExpression(readOnlyCollection2[j], stack);
					rewriteAction |= result2.Action;
					if (array2 == null && result2.Action != StackSpiller.RewriteAction.None)
					{
						array2 = StackSpiller.Clone<Expression>(readOnlyCollection2, j);
					}
					if (array2 != null)
					{
						array2[j] = result2.Node;
					}
				}
				StackSpiller.Result result3 = this.RewriteExpression(switchCase.Body, stack);
				rewriteAction |= result3.Action;
				if (result3.Action != StackSpiller.RewriteAction.None || array2 != null)
				{
					if (array2 != null)
					{
						readOnlyCollection2 = new ReadOnlyCollection<Expression>(array2);
					}
					switchCase = new SwitchCase(result3.Node, readOnlyCollection2);
					if (array == null)
					{
						array = StackSpiller.Clone<SwitchCase>(readOnlyCollection, i);
					}
				}
				if (array != null)
				{
					array[i] = switchCase;
				}
			}
			StackSpiller.Result result4 = this.RewriteExpression(switchExpression.DefaultBody, stack);
			rewriteAction |= result4.Action;
			if (rewriteAction != StackSpiller.RewriteAction.None)
			{
				if (array != null)
				{
					readOnlyCollection = new ReadOnlyCollection<SwitchCase>(array);
				}
				expr = new SwitchExpression(switchExpression.Type, result.Node, result4.Node, switchExpression.Comparison, readOnlyCollection);
			}
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteTryExpression(Expression expr, StackSpiller.Stack stack)
		{
			TryExpression tryExpression = (TryExpression)expr;
			StackSpiller.Result result = this.RewriteExpression(tryExpression.Body, StackSpiller.Stack.Empty);
			ReadOnlyCollection<CatchBlock> readOnlyCollection = tryExpression.Handlers;
			CatchBlock[] array = null;
			StackSpiller.RewriteAction rewriteAction = result.Action;
			if (readOnlyCollection != null)
			{
				for (int i = 0; i < readOnlyCollection.Count; i++)
				{
					StackSpiller.RewriteAction rewriteAction2 = result.Action;
					CatchBlock catchBlock = readOnlyCollection[i];
					Expression expression = catchBlock.Filter;
					if (catchBlock.Filter != null)
					{
						StackSpiller.Result result2 = this.RewriteExpression(catchBlock.Filter, StackSpiller.Stack.Empty);
						rewriteAction |= result2.Action;
						rewriteAction2 |= result2.Action;
						expression = result2.Node;
					}
					StackSpiller.Result result3 = this.RewriteExpression(catchBlock.Body, StackSpiller.Stack.Empty);
					rewriteAction |= result3.Action;
					rewriteAction2 |= result3.Action;
					if (rewriteAction2 != StackSpiller.RewriteAction.None)
					{
						catchBlock = Expression.MakeCatchBlock(catchBlock.Test, catchBlock.Variable, result3.Node, expression);
						if (array == null)
						{
							array = StackSpiller.Clone<CatchBlock>(readOnlyCollection, i);
						}
					}
					if (array != null)
					{
						array[i] = catchBlock;
					}
				}
			}
			StackSpiller.Result result4 = this.RewriteExpression(tryExpression.Fault, StackSpiller.Stack.Empty);
			rewriteAction |= result4.Action;
			StackSpiller.Result result5 = this.RewriteExpression(tryExpression.Finally, StackSpiller.Stack.Empty);
			rewriteAction |= result5.Action;
			if (stack != StackSpiller.Stack.Empty)
			{
				rewriteAction = StackSpiller.RewriteAction.SpillStack;
			}
			if (rewriteAction != StackSpiller.RewriteAction.None)
			{
				if (array != null)
				{
					readOnlyCollection = new ReadOnlyCollection<CatchBlock>(array);
				}
				expr = new TryExpression(tryExpression.Type, result.Node, result5.Node, result4.Node, readOnlyCollection);
			}
			return new StackSpiller.Result(rewriteAction, expr);
		}

		private StackSpiller.Result RewriteExtensionExpression(Expression expr, StackSpiller.Stack stack)
		{
			StackSpiller.Result result = this.RewriteExpression(expr.ReduceExtensions(), stack);
			return new StackSpiller.Result(result.Action | StackSpiller.RewriteAction.Copy, result.Node);
		}

		private static T[] Clone<T>(ReadOnlyCollection<T> original, int max)
		{
			T[] array = new T[original.Count];
			for (int i = 0; i < max; i++)
			{
				array[i] = original[i];
			}
			return array;
		}

		private static void RequireNoRefArgs(MethodBase method)
		{
			if (method != null)
			{
				if (method.GetParametersCached().Any<ParameterInfo>((ParameterInfo p) => p.ParameterType.IsByRef))
				{
					throw Error.TryNotSupportedForMethodsWithRefArgs(method);
				}
			}
		}

		private static void RequireNotRefInstance(Expression instance)
		{
			if (StackSpiller.IsRefInstance(instance))
			{
				throw Error.TryNotSupportedForValueTypeInstances(instance.Type);
			}
		}

		private static bool IsRefInstance(Expression instance)
		{
			return instance != null && instance.Type.IsValueType && instance.Type.GetTypeCode() == TypeCode.Object;
		}

		private readonly StackGuard _guard = new StackGuard();

		private readonly StackSpiller.TempMaker _tm = new StackSpiller.TempMaker();

		private readonly StackSpiller.Stack _startingStack;

		private StackSpiller.RewriteAction _lambdaRewrite;

		private abstract class BindingRewriter
		{
			internal BindingRewriter(MemberBinding binding, StackSpiller spiller)
			{
				this._binding = binding;
				this._spiller = spiller;
			}

			internal StackSpiller.RewriteAction Action
			{
				get
				{
					return this._action;
				}
			}

			internal abstract MemberBinding AsBinding();

			internal abstract Expression AsExpression(Expression target);

			internal static StackSpiller.BindingRewriter Create(MemberBinding binding, StackSpiller spiller, StackSpiller.Stack stack)
			{
				switch (binding.BindingType)
				{
				case MemberBindingType.Assignment:
					return new StackSpiller.MemberAssignmentRewriter((MemberAssignment)binding, spiller, stack);
				case MemberBindingType.MemberBinding:
					return new StackSpiller.MemberMemberBindingRewriter((MemberMemberBinding)binding, spiller, stack);
				case MemberBindingType.ListBinding:
					return new StackSpiller.ListBindingRewriter((MemberListBinding)binding, spiller, stack);
				default:
					throw Error.UnhandledBinding();
				}
			}

			protected void RequireNoValueProperty()
			{
				PropertyInfo propertyInfo = this._binding.Member as PropertyInfo;
				if (propertyInfo != null && propertyInfo.PropertyType.IsValueType)
				{
					throw Error.CannotAutoInitializeValueTypeMemberThroughProperty(propertyInfo);
				}
			}

			protected readonly MemberBinding _binding;

			protected readonly StackSpiller _spiller;

			protected StackSpiller.RewriteAction _action;
		}

		private sealed class MemberMemberBindingRewriter : StackSpiller.BindingRewriter
		{
			internal MemberMemberBindingRewriter(MemberMemberBinding binding, StackSpiller spiller, StackSpiller.Stack stack)
				: base(binding, spiller)
			{
				this._bindings = binding.Bindings;
				int count = this._bindings.Count;
				this._bindingRewriters = new StackSpiller.BindingRewriter[count];
				for (int i = 0; i < count; i++)
				{
					StackSpiller.BindingRewriter bindingRewriter = StackSpiller.BindingRewriter.Create(this._bindings[i], spiller, stack);
					this._action |= bindingRewriter.Action;
					this._bindingRewriters[i] = bindingRewriter;
				}
			}

			internal override MemberBinding AsBinding()
			{
				StackSpiller.RewriteAction action = this._action;
				if (action == StackSpiller.RewriteAction.None)
				{
					return this._binding;
				}
				if (action != StackSpiller.RewriteAction.Copy)
				{
					throw ContractUtils.Unreachable;
				}
				int count = this._bindings.Count;
				MemberBinding[] array = new MemberBinding[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = this._bindingRewriters[i].AsBinding();
				}
				return new MemberMemberBinding(this._binding.Member, new TrueReadOnlyCollection<MemberBinding>(array));
			}

			internal override Expression AsExpression(Expression target)
			{
				base.RequireNoValueProperty();
				Expression expression = MemberExpression.Make(target, this._binding.Member);
				Expression expression2 = this._spiller.MakeTemp(expression.Type);
				int count = this._bindings.Count;
				Expression[] array = new Expression[count + 2];
				array[0] = new AssignBinaryExpression(expression2, expression);
				for (int i = 0; i < count; i++)
				{
					StackSpiller.BindingRewriter bindingRewriter = this._bindingRewriters[i];
					array[i + 1] = bindingRewriter.AsExpression(expression2);
				}
				if (expression2.Type.IsValueType)
				{
					array[count + 1] = Expression.Block(typeof(void), new Expression[]
					{
						new AssignBinaryExpression(MemberExpression.Make(target, this._binding.Member), expression2)
					});
				}
				else
				{
					array[count + 1] = Utils.Empty;
				}
				return StackSpiller.MakeBlock(array);
			}

			private readonly ReadOnlyCollection<MemberBinding> _bindings;

			private readonly StackSpiller.BindingRewriter[] _bindingRewriters;
		}

		private sealed class ListBindingRewriter : StackSpiller.BindingRewriter
		{
			internal ListBindingRewriter(MemberListBinding binding, StackSpiller spiller, StackSpiller.Stack stack)
				: base(binding, spiller)
			{
				this._inits = binding.Initializers;
				int count = this._inits.Count;
				this._childRewriters = new StackSpiller.ChildRewriter[count];
				for (int i = 0; i < count; i++)
				{
					ElementInit elementInit = this._inits[i];
					StackSpiller.ChildRewriter childRewriter = new StackSpiller.ChildRewriter(spiller, stack, elementInit.Arguments.Count);
					childRewriter.Add(elementInit.Arguments);
					this._action |= childRewriter.Action;
					this._childRewriters[i] = childRewriter;
				}
			}

			internal override MemberBinding AsBinding()
			{
				StackSpiller.RewriteAction action = this._action;
				if (action == StackSpiller.RewriteAction.None)
				{
					return this._binding;
				}
				if (action != StackSpiller.RewriteAction.Copy)
				{
					throw ContractUtils.Unreachable;
				}
				int count = this._inits.Count;
				ElementInit[] array = new ElementInit[count];
				for (int i = 0; i < count; i++)
				{
					StackSpiller.ChildRewriter childRewriter = this._childRewriters[i];
					if (childRewriter.Action == StackSpiller.RewriteAction.None)
					{
						array[i] = this._inits[i];
					}
					else
					{
						array[i] = new ElementInit(this._inits[i].AddMethod, new TrueReadOnlyCollection<Expression>(childRewriter[0, -1]));
					}
				}
				return new MemberListBinding(this._binding.Member, new TrueReadOnlyCollection<ElementInit>(array));
			}

			internal override Expression AsExpression(Expression target)
			{
				base.RequireNoValueProperty();
				Expression expression = MemberExpression.Make(target, this._binding.Member);
				Expression expression2 = this._spiller.MakeTemp(expression.Type);
				int count = this._inits.Count;
				Expression[] array = new Expression[count + 2];
				array[0] = new AssignBinaryExpression(expression2, expression);
				for (int i = 0; i < count; i++)
				{
					StackSpiller.ChildRewriter childRewriter = this._childRewriters[i];
					StackSpiller.Result result = childRewriter.Finish(new InstanceMethodCallExpressionN(this._inits[i].AddMethod, expression2, childRewriter[0, -1]));
					array[i + 1] = result.Node;
				}
				if (expression2.Type.IsValueType)
				{
					array[count + 1] = Expression.Block(typeof(void), new Expression[]
					{
						new AssignBinaryExpression(MemberExpression.Make(target, this._binding.Member), expression2)
					});
				}
				else
				{
					array[count + 1] = Utils.Empty;
				}
				return StackSpiller.MakeBlock(array);
			}

			private readonly ReadOnlyCollection<ElementInit> _inits;

			private readonly StackSpiller.ChildRewriter[] _childRewriters;
		}

		private sealed class MemberAssignmentRewriter : StackSpiller.BindingRewriter
		{
			internal MemberAssignmentRewriter(MemberAssignment binding, StackSpiller spiller, StackSpiller.Stack stack)
				: base(binding, spiller)
			{
				StackSpiller.Result result = spiller.RewriteExpression(binding.Expression, stack);
				this._action = result.Action;
				this._rhs = result.Node;
			}

			internal override MemberBinding AsBinding()
			{
				StackSpiller.RewriteAction action = this._action;
				if (action == StackSpiller.RewriteAction.None)
				{
					return this._binding;
				}
				if (action != StackSpiller.RewriteAction.Copy)
				{
					throw ContractUtils.Unreachable;
				}
				return new MemberAssignment(this._binding.Member, this._rhs);
			}

			internal override Expression AsExpression(Expression target)
			{
				Expression expression = MemberExpression.Make(target, this._binding.Member);
				Expression expression2 = this._spiller.MakeTemp(expression.Type);
				return StackSpiller.MakeBlock(new Expression[]
				{
					new AssignBinaryExpression(expression2, this._rhs),
					new AssignBinaryExpression(expression, expression2),
					Utils.Empty
				});
			}

			private readonly Expression _rhs;
		}

		private sealed class ChildRewriter
		{
			internal ChildRewriter(StackSpiller self, StackSpiller.Stack stack, int count)
			{
				this._self = self;
				this._stack = stack;
				this._expressions = new Expression[count];
			}

			internal void Add(Expression expression)
			{
				int num;
				if (expression == null)
				{
					Expression[] expressions = this._expressions;
					num = this._expressionsCount;
					this._expressionsCount = num + 1;
					expressions[num] = null;
					return;
				}
				StackSpiller.Result result = this._self.RewriteExpression(expression, this._stack);
				this._action |= result.Action;
				this._stack = StackSpiller.Stack.NonEmpty;
				if (result.Action == StackSpiller.RewriteAction.SpillStack)
				{
					this._lastSpillIndex = this._expressionsCount;
				}
				Expression[] expressions2 = this._expressions;
				num = this._expressionsCount;
				this._expressionsCount = num + 1;
				expressions2[num] = result.Node;
			}

			internal void Add(ReadOnlyCollection<Expression> expressions)
			{
				int i = 0;
				int count = expressions.Count;
				while (i < count)
				{
					this.Add(expressions[i]);
					i++;
				}
			}

			internal void AddArguments(IArgumentProvider expressions)
			{
				int i = 0;
				int argumentCount = expressions.ArgumentCount;
				while (i < argumentCount)
				{
					this.Add(expressions.GetArgument(i));
					i++;
				}
			}

			private void EnsureDone()
			{
				if (!this._done)
				{
					this._done = true;
					if (this._action == StackSpiller.RewriteAction.SpillStack)
					{
						Expression[] expressions = this._expressions;
						int num = this._lastSpillIndex + 1;
						List<Expression> list = new List<Expression>(num + 1);
						for (int i = 0; i < num; i++)
						{
							Expression expression = expressions[i];
							if (StackSpiller.ChildRewriter.ShouldSaveToTemp(expression))
							{
								Expression[] array = expressions;
								int num2 = i;
								StackSpiller self = this._self;
								Expression expression2 = expression;
								bool[] byRefs = this._byRefs;
								Expression expression3;
								array[num2] = self.ToTemp(expression2, out expression3, byRefs != null && byRefs[i]);
								list.Add(expression3);
							}
						}
						list.Capacity = list.Count + 1;
						this._comma = list;
					}
				}
			}

			private static bool ShouldSaveToTemp(Expression expression)
			{
				if (expression == null)
				{
					return false;
				}
				ExpressionType nodeType = expression.NodeType;
				if (nodeType <= ExpressionType.MemberAccess)
				{
					if (nodeType != ExpressionType.Constant)
					{
						if (nodeType != ExpressionType.MemberAccess)
						{
							return true;
						}
						FieldInfo fieldInfo = ((MemberExpression)expression).Member as FieldInfo;
						if (!(fieldInfo != null))
						{
							return true;
						}
						if (fieldInfo.IsLiteral)
						{
							return false;
						}
						if (fieldInfo.IsInitOnly && fieldInfo.IsStatic)
						{
							return false;
						}
						return true;
					}
				}
				else if (nodeType != ExpressionType.Default)
				{
					if (nodeType != ExpressionType.RuntimeVariables)
					{
						return true;
					}
					return false;
				}
				return false;
			}

			internal bool Rewrite
			{
				get
				{
					return this._action > StackSpiller.RewriteAction.None;
				}
			}

			internal StackSpiller.RewriteAction Action
			{
				get
				{
					return this._action;
				}
			}

			internal void MarkRefInstance(Expression expr)
			{
				if (StackSpiller.IsRefInstance(expr))
				{
					this.MarkRef(0);
				}
			}

			internal void MarkRefArgs(MethodBase method, int startIndex)
			{
				ParameterInfo[] parametersCached = method.GetParametersCached();
				int i = 0;
				int num = parametersCached.Length;
				while (i < num)
				{
					if (parametersCached[i].ParameterType.IsByRef)
					{
						this.MarkRef(startIndex + i);
					}
					i++;
				}
			}

			private void MarkRef(int index)
			{
				if (this._byRefs == null)
				{
					this._byRefs = new bool[this._expressions.Length];
				}
				this._byRefs[index] = true;
			}

			internal StackSpiller.Result Finish(Expression expression)
			{
				this.EnsureDone();
				if (this._action == StackSpiller.RewriteAction.SpillStack)
				{
					this._comma.Add(expression);
					expression = StackSpiller.MakeBlock(this._comma);
				}
				return new StackSpiller.Result(this._action, expression);
			}

			internal Expression this[int index]
			{
				get
				{
					this.EnsureDone();
					if (index < 0)
					{
						index += this._expressions.Length;
					}
					return this._expressions[index];
				}
			}

			internal Expression[] this[int first, int last]
			{
				get
				{
					this.EnsureDone();
					if (last < 0)
					{
						last += this._expressions.Length;
					}
					int num = last - first + 1;
					ContractUtils.RequiresArrayRange<Expression>(this._expressions, first, num, "first", "last");
					if (num == this._expressions.Length)
					{
						return this._expressions;
					}
					Expression[] array = new Expression[num];
					Array.Copy(this._expressions, first, array, 0, num);
					return array;
				}
			}

			private readonly StackSpiller _self;

			private readonly Expression[] _expressions;

			private int _expressionsCount;

			private int _lastSpillIndex;

			private List<Expression> _comma;

			private StackSpiller.RewriteAction _action;

			private StackSpiller.Stack _stack;

			private bool _done;

			private bool[] _byRefs;
		}

		private sealed class TempMaker
		{
			internal List<ParameterExpression> Temps { get; } = new List<ParameterExpression>();

			internal ParameterExpression Temp(Type type)
			{
				ParameterExpression parameterExpression;
				if (this._freeTemps != null)
				{
					for (int i = this._freeTemps.Count - 1; i >= 0; i--)
					{
						parameterExpression = this._freeTemps[i];
						if (parameterExpression.Type == type)
						{
							this._freeTemps.RemoveAt(i);
							return this.UseTemp(parameterExpression);
						}
					}
				}
				string text = "$temp$";
				int temp = this._temp;
				this._temp = temp + 1;
				parameterExpression = ParameterExpression.Make(type, text + temp.ToString(), false);
				this.Temps.Add(parameterExpression);
				return this.UseTemp(parameterExpression);
			}

			private ParameterExpression UseTemp(ParameterExpression temp)
			{
				if (this._usedTemps == null)
				{
					this._usedTemps = new Stack<ParameterExpression>();
				}
				this._usedTemps.Push(temp);
				return temp;
			}

			private void FreeTemp(ParameterExpression temp)
			{
				if (this._freeTemps == null)
				{
					this._freeTemps = new List<ParameterExpression>();
				}
				this._freeTemps.Add(temp);
			}

			internal int Mark()
			{
				Stack<ParameterExpression> usedTemps = this._usedTemps;
				if (usedTemps == null)
				{
					return 0;
				}
				return usedTemps.Count;
			}

			internal void Free(int mark)
			{
				if (this._usedTemps != null)
				{
					while (mark < this._usedTemps.Count)
					{
						this.FreeTemp(this._usedTemps.Pop());
					}
				}
			}

			[Conditional("DEBUG")]
			internal void VerifyTemps()
			{
			}

			private int _temp;

			private List<ParameterExpression> _freeTemps;

			private Stack<ParameterExpression> _usedTemps;
		}

		private enum Stack
		{
			Empty,
			NonEmpty
		}

		[Flags]
		private enum RewriteAction
		{
			None = 0,
			Copy = 1,
			SpillStack = 3
		}

		private readonly struct Result
		{
			internal Result(StackSpiller.RewriteAction action, Expression node)
			{
				this.Action = action;
				this.Node = node;
			}

			internal readonly StackSpiller.RewriteAction Action;

			internal readonly Expression Node;
		}
	}
}
