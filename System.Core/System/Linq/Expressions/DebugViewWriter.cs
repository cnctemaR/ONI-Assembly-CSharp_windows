using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal sealed class DebugViewWriter : ExpressionVisitor
	{
		private DebugViewWriter(TextWriter file)
		{
			this._out = file;
		}

		private int Base
		{
			get
			{
				if (this._stack.Count <= 0)
				{
					return 0;
				}
				return this._stack.Peek();
			}
		}

		private int Delta
		{
			get
			{
				return this._delta;
			}
		}

		private int Depth
		{
			get
			{
				return this.Base + this.Delta;
			}
		}

		private void Indent()
		{
			this._delta += 4;
		}

		private void Dedent()
		{
			this._delta -= 4;
		}

		private void NewLine()
		{
			this._flow = DebugViewWriter.Flow.NewLine;
		}

		private static int GetId<T>(T e, ref Dictionary<T, int> ids)
		{
			if (ids == null)
			{
				ids = new Dictionary<T, int>();
				ids.Add(e, 1);
				return 1;
			}
			int num;
			if (!ids.TryGetValue(e, out num))
			{
				num = ids.Count + 1;
				ids.Add(e, num);
			}
			return num;
		}

		private int GetLambdaId(LambdaExpression le)
		{
			return DebugViewWriter.GetId<LambdaExpression>(le, ref this._lambdaIds);
		}

		private int GetParamId(ParameterExpression p)
		{
			return DebugViewWriter.GetId<ParameterExpression>(p, ref this._paramIds);
		}

		private int GetLabelTargetId(LabelTarget target)
		{
			return DebugViewWriter.GetId<LabelTarget>(target, ref this._labelIds);
		}

		internal static void WriteTo(Expression node, TextWriter writer)
		{
			new DebugViewWriter(writer).WriteTo(node);
		}

		private void WriteTo(Expression node)
		{
			LambdaExpression lambdaExpression = node as LambdaExpression;
			if (lambdaExpression != null)
			{
				this.WriteLambda(lambdaExpression);
			}
			else
			{
				this.Visit(node);
			}
			while (this._lambdas != null && this._lambdas.Count > 0)
			{
				this.WriteLine();
				this.WriteLine();
				this.WriteLambda(this._lambdas.Dequeue());
			}
		}

		private void Out(string s)
		{
			this.Out(DebugViewWriter.Flow.None, s, DebugViewWriter.Flow.None);
		}

		private void Out(DebugViewWriter.Flow before, string s)
		{
			this.Out(before, s, DebugViewWriter.Flow.None);
		}

		private void Out(string s, DebugViewWriter.Flow after)
		{
			this.Out(DebugViewWriter.Flow.None, s, after);
		}

		private void Out(DebugViewWriter.Flow before, string s, DebugViewWriter.Flow after)
		{
			switch (this.GetFlow(before))
			{
			case DebugViewWriter.Flow.Space:
				this.Write(" ");
				break;
			case DebugViewWriter.Flow.NewLine:
				this.WriteLine();
				this.Write(new string(' ', this.Depth));
				break;
			}
			this.Write(s);
			this._flow = after;
		}

		private void WriteLine()
		{
			this._out.WriteLine();
			this._column = 0;
		}

		private void Write(string s)
		{
			this._out.Write(s);
			this._column += s.Length;
		}

		private DebugViewWriter.Flow GetFlow(DebugViewWriter.Flow flow)
		{
			int num = (int)this.CheckBreak(this._flow);
			flow = this.CheckBreak(flow);
			return (DebugViewWriter.Flow)Math.Max(num, (int)flow);
		}

		private DebugViewWriter.Flow CheckBreak(DebugViewWriter.Flow flow)
		{
			if ((flow & DebugViewWriter.Flow.Break) != DebugViewWriter.Flow.None)
			{
				if (this._column > 120 + this.Depth)
				{
					flow = DebugViewWriter.Flow.NewLine;
				}
				else
				{
					flow &= ~DebugViewWriter.Flow.Break;
				}
			}
			return flow;
		}

		private void VisitExpressions<T>(char open, IReadOnlyList<T> expressions) where T : Expression
		{
			this.VisitExpressions<T>(open, ',', expressions);
		}

		private void VisitExpressions<T>(char open, char separator, IReadOnlyList<T> expressions) where T : Expression
		{
			this.VisitExpressions<T>(open, separator, expressions, delegate(T e)
			{
				this.Visit(e);
			});
		}

		private void VisitDeclarations(IReadOnlyList<ParameterExpression> expressions)
		{
			this.VisitExpressions<ParameterExpression>('(', ',', expressions, delegate(ParameterExpression variable)
			{
				this.Out(variable.Type.ToString());
				if (variable.IsByRef)
				{
					this.Out("&");
				}
				this.Out(" ");
				this.VisitParameter(variable);
			});
		}

		private void VisitExpressions<T>(char open, char separator, IReadOnlyList<T> expressions, Action<T> visit)
		{
			this.Out(open.ToString());
			if (expressions != null)
			{
				this.Indent();
				bool flag = true;
				foreach (T t in expressions)
				{
					if (flag)
					{
						if (open == '{' || expressions.Count > 1)
						{
							this.NewLine();
						}
						flag = false;
					}
					else
					{
						this.Out(separator.ToString(), DebugViewWriter.Flow.NewLine);
					}
					visit(t);
				}
				this.Dedent();
			}
			char c;
			if (open != '(')
			{
				if (open != '[')
				{
					if (open != '{')
					{
						throw ContractUtils.Unreachable;
					}
					c = '}';
				}
				else
				{
					c = ']';
				}
			}
			else
			{
				c = ')';
			}
			if (open == '{')
			{
				this.NewLine();
			}
			this.Out(c.ToString(), DebugViewWriter.Flow.Break);
		}

		protected internal override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.ArrayIndex)
			{
				this.ParenthesizedVisit(node, node.Left);
				this.Out("[");
				this.Visit(node.Right);
				this.Out("]");
			}
			else
			{
				bool flag = DebugViewWriter.NeedsParentheses(node, node.Left);
				bool flag2 = DebugViewWriter.NeedsParentheses(node, node.Right);
				DebugViewWriter.Flow flow = DebugViewWriter.Flow.Space;
				ExpressionType nodeType = node.NodeType;
				string text;
				switch (nodeType)
				{
				case ExpressionType.Add:
					text = "+";
					goto IL_02F0;
				case ExpressionType.AddChecked:
					text = "#+";
					goto IL_02F0;
				case ExpressionType.And:
					text = "&";
					goto IL_02F0;
				case ExpressionType.AndAlso:
					text = "&&";
					flow = DebugViewWriter.Flow.Space | DebugViewWriter.Flow.Break;
					goto IL_02F0;
				case ExpressionType.ArrayLength:
				case ExpressionType.ArrayIndex:
				case ExpressionType.Call:
				case ExpressionType.Conditional:
				case ExpressionType.Constant:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.Invoke:
				case ExpressionType.Lambda:
				case ExpressionType.ListInit:
				case ExpressionType.MemberAccess:
				case ExpressionType.MemberInit:
				case ExpressionType.Negate:
				case ExpressionType.UnaryPlus:
				case ExpressionType.NegateChecked:
				case ExpressionType.New:
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
				case ExpressionType.Not:
				case ExpressionType.Parameter:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
				case ExpressionType.TypeIs:
					break;
				case ExpressionType.Coalesce:
					text = "??";
					goto IL_02F0;
				case ExpressionType.Divide:
					text = "/";
					goto IL_02F0;
				case ExpressionType.Equal:
					text = "==";
					goto IL_02F0;
				case ExpressionType.ExclusiveOr:
					text = "^";
					goto IL_02F0;
				case ExpressionType.GreaterThan:
					text = ">";
					goto IL_02F0;
				case ExpressionType.GreaterThanOrEqual:
					text = ">=";
					goto IL_02F0;
				case ExpressionType.LeftShift:
					text = "<<";
					goto IL_02F0;
				case ExpressionType.LessThan:
					text = "<";
					goto IL_02F0;
				case ExpressionType.LessThanOrEqual:
					text = "<=";
					goto IL_02F0;
				case ExpressionType.Modulo:
					text = "%";
					goto IL_02F0;
				case ExpressionType.Multiply:
					text = "*";
					goto IL_02F0;
				case ExpressionType.MultiplyChecked:
					text = "#*";
					goto IL_02F0;
				case ExpressionType.NotEqual:
					text = "!=";
					goto IL_02F0;
				case ExpressionType.Or:
					text = "|";
					goto IL_02F0;
				case ExpressionType.OrElse:
					text = "||";
					flow = DebugViewWriter.Flow.Space | DebugViewWriter.Flow.Break;
					goto IL_02F0;
				case ExpressionType.Power:
					text = "**";
					goto IL_02F0;
				case ExpressionType.RightShift:
					text = ">>";
					goto IL_02F0;
				case ExpressionType.Subtract:
					text = "-";
					goto IL_02F0;
				case ExpressionType.SubtractChecked:
					text = "#-";
					goto IL_02F0;
				case ExpressionType.Assign:
					text = "=";
					goto IL_02F0;
				default:
					switch (nodeType)
					{
					case ExpressionType.AddAssign:
						text = "+=";
						goto IL_02F0;
					case ExpressionType.AndAssign:
						text = "&=";
						goto IL_02F0;
					case ExpressionType.DivideAssign:
						text = "/=";
						goto IL_02F0;
					case ExpressionType.ExclusiveOrAssign:
						text = "^=";
						goto IL_02F0;
					case ExpressionType.LeftShiftAssign:
						text = "<<=";
						goto IL_02F0;
					case ExpressionType.ModuloAssign:
						text = "%=";
						goto IL_02F0;
					case ExpressionType.MultiplyAssign:
						text = "*=";
						goto IL_02F0;
					case ExpressionType.OrAssign:
						text = "|=";
						goto IL_02F0;
					case ExpressionType.PowerAssign:
						text = "**=";
						goto IL_02F0;
					case ExpressionType.RightShiftAssign:
						text = ">>=";
						goto IL_02F0;
					case ExpressionType.SubtractAssign:
						text = "-=";
						goto IL_02F0;
					case ExpressionType.AddAssignChecked:
						text = "#+=";
						goto IL_02F0;
					case ExpressionType.MultiplyAssignChecked:
						text = "#*=";
						goto IL_02F0;
					case ExpressionType.SubtractAssignChecked:
						text = "#-=";
						goto IL_02F0;
					}
					break;
				}
				throw new InvalidOperationException();
				IL_02F0:
				if (flag)
				{
					this.Out("(", DebugViewWriter.Flow.None);
				}
				this.Visit(node.Left);
				if (flag)
				{
					this.Out(DebugViewWriter.Flow.None, ")", DebugViewWriter.Flow.Break);
				}
				this.Out(flow, text, DebugViewWriter.Flow.Space | DebugViewWriter.Flow.Break);
				if (flag2)
				{
					this.Out("(", DebugViewWriter.Flow.None);
				}
				this.Visit(node.Right);
				if (flag2)
				{
					this.Out(DebugViewWriter.Flow.None, ")", DebugViewWriter.Flow.Break);
				}
			}
			return node;
		}

		protected internal override Expression VisitParameter(ParameterExpression node)
		{
			this.Out("$");
			if (string.IsNullOrEmpty(node.Name))
			{
				int paramId = this.GetParamId(node);
				this.Out("var" + paramId);
			}
			else
			{
				this.Out(DebugViewWriter.GetDisplayName(node.Name));
			}
			return node;
		}

		protected internal override Expression VisitLambda<T>(Expression<T> node)
		{
			this.Out(string.Format(CultureInfo.CurrentCulture, ".Lambda {0}<{1}>", this.GetLambdaName(node), node.Type.ToString()));
			if (this._lambdas == null)
			{
				this._lambdas = new Queue<LambdaExpression>();
			}
			if (!this._lambdas.Contains(node))
			{
				this._lambdas.Enqueue(node);
			}
			return node;
		}

		private static bool IsSimpleExpression(Expression node)
		{
			BinaryExpression binaryExpression = node as BinaryExpression;
			return binaryExpression != null && !(binaryExpression.Left is BinaryExpression) && !(binaryExpression.Right is BinaryExpression);
		}

		protected internal override Expression VisitConditional(ConditionalExpression node)
		{
			if (DebugViewWriter.IsSimpleExpression(node.Test))
			{
				this.Out(".If (");
				this.Visit(node.Test);
				this.Out(") {", DebugViewWriter.Flow.NewLine);
			}
			else
			{
				this.Out(".If (", DebugViewWriter.Flow.NewLine);
				this.Indent();
				this.Visit(node.Test);
				this.Dedent();
				this.Out(DebugViewWriter.Flow.NewLine, ") {", DebugViewWriter.Flow.NewLine);
			}
			this.Indent();
			this.Visit(node.IfTrue);
			this.Dedent();
			this.Out(DebugViewWriter.Flow.NewLine, "} .Else {", DebugViewWriter.Flow.NewLine);
			this.Indent();
			this.Visit(node.IfFalse);
			this.Dedent();
			this.Out(DebugViewWriter.Flow.NewLine, "}");
			return node;
		}

		protected internal override Expression VisitConstant(ConstantExpression node)
		{
			object value = node.Value;
			if (value == null)
			{
				this.Out("null");
			}
			else if (value is string && node.Type == typeof(string))
			{
				this.Out(string.Format(CultureInfo.CurrentCulture, "\"{0}\"", value));
			}
			else if (value is char && node.Type == typeof(char))
			{
				this.Out(string.Format(CultureInfo.CurrentCulture, "'{0}'", value));
			}
			else if ((value is int && node.Type == typeof(int)) || (value is bool && node.Type == typeof(bool)))
			{
				this.Out(value.ToString());
			}
			else
			{
				string constantValueSuffix = DebugViewWriter.GetConstantValueSuffix(node.Type);
				if (constantValueSuffix != null)
				{
					this.Out(value.ToString());
					this.Out(constantValueSuffix);
				}
				else
				{
					this.Out(string.Format(CultureInfo.CurrentCulture, ".Constant<{0}>({1})", node.Type.ToString(), value));
				}
			}
			return node;
		}

		private static string GetConstantValueSuffix(Type type)
		{
			if (type == typeof(uint))
			{
				return "U";
			}
			if (type == typeof(long))
			{
				return "L";
			}
			if (type == typeof(ulong))
			{
				return "UL";
			}
			if (type == typeof(double))
			{
				return "D";
			}
			if (type == typeof(float))
			{
				return "F";
			}
			if (type == typeof(decimal))
			{
				return "M";
			}
			return null;
		}

		protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			this.Out(".RuntimeVariables");
			this.VisitExpressions<ParameterExpression>('(', node.Variables);
			return node;
		}

		private void OutMember(Expression node, Expression instance, MemberInfo member)
		{
			if (instance != null)
			{
				this.ParenthesizedVisit(node, instance);
				this.Out("." + member.Name);
				return;
			}
			this.Out(member.DeclaringType.ToString() + "." + member.Name);
		}

		protected internal override Expression VisitMember(MemberExpression node)
		{
			this.OutMember(node, node.Expression, node.Member);
			return node;
		}

		protected internal override Expression VisitInvocation(InvocationExpression node)
		{
			this.Out(".Invoke ");
			this.ParenthesizedVisit(node, node.Expression);
			this.VisitExpressions<Expression>('(', node.Arguments);
			return node;
		}

		private static bool NeedsParentheses(Expression parent, Expression child)
		{
			if (child == null)
			{
				return false;
			}
			ExpressionType expressionType = parent.NodeType;
			if (expressionType <= ExpressionType.Increment)
			{
				if (expressionType != ExpressionType.Decrement && expressionType != ExpressionType.Increment)
				{
					goto IL_002B;
				}
			}
			else if (expressionType != ExpressionType.Unbox && expressionType - ExpressionType.IsTrue > 1)
			{
				goto IL_002B;
			}
			return true;
			IL_002B:
			int operatorPrecedence = DebugViewWriter.GetOperatorPrecedence(child);
			int operatorPrecedence2 = DebugViewWriter.GetOperatorPrecedence(parent);
			if (operatorPrecedence == operatorPrecedence2)
			{
				expressionType = parent.NodeType;
				if (expressionType <= ExpressionType.ExclusiveOr)
				{
					if (expressionType <= ExpressionType.AndAlso)
					{
						if (expressionType <= ExpressionType.AddChecked)
						{
							return false;
						}
						if (expressionType - ExpressionType.And > 1)
						{
							return true;
						}
					}
					else
					{
						if (expressionType == ExpressionType.Divide)
						{
							goto IL_008C;
						}
						if (expressionType != ExpressionType.ExclusiveOr)
						{
							return true;
						}
					}
				}
				else if (expressionType <= ExpressionType.MultiplyChecked)
				{
					if (expressionType == ExpressionType.Modulo)
					{
						goto IL_008C;
					}
					if (expressionType - ExpressionType.Multiply > 1)
					{
						return true;
					}
					return false;
				}
				else if (expressionType - ExpressionType.Or > 1)
				{
					if (expressionType - ExpressionType.Subtract > 1)
					{
						return true;
					}
					goto IL_008C;
				}
				return false;
				IL_008C:
				BinaryExpression binaryExpression = parent as BinaryExpression;
				return child == binaryExpression.Right;
			}
			return (child != null && child.NodeType == ExpressionType.Constant && (parent.NodeType == ExpressionType.Negate || parent.NodeType == ExpressionType.NegateChecked)) || operatorPrecedence < operatorPrecedence2;
		}

		private static int GetOperatorPrecedence(Expression node)
		{
			switch (node.NodeType)
			{
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
			case ExpressionType.Subtract:
			case ExpressionType.SubtractChecked:
				return 10;
			case ExpressionType.And:
				return 6;
			case ExpressionType.AndAlso:
				return 3;
			case ExpressionType.Coalesce:
			case ExpressionType.Assign:
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
				return 1;
			case ExpressionType.Constant:
			case ExpressionType.Parameter:
				return 15;
			case ExpressionType.Convert:
			case ExpressionType.ConvertChecked:
			case ExpressionType.Negate:
			case ExpressionType.UnaryPlus:
			case ExpressionType.NegateChecked:
			case ExpressionType.Not:
			case ExpressionType.Decrement:
			case ExpressionType.Increment:
			case ExpressionType.Throw:
			case ExpressionType.Unbox:
			case ExpressionType.PreIncrementAssign:
			case ExpressionType.PreDecrementAssign:
			case ExpressionType.OnesComplement:
			case ExpressionType.IsTrue:
			case ExpressionType.IsFalse:
				return 12;
			case ExpressionType.Divide:
			case ExpressionType.Modulo:
			case ExpressionType.Multiply:
			case ExpressionType.MultiplyChecked:
				return 11;
			case ExpressionType.Equal:
			case ExpressionType.NotEqual:
				return 7;
			case ExpressionType.ExclusiveOr:
				return 5;
			case ExpressionType.GreaterThan:
			case ExpressionType.GreaterThanOrEqual:
			case ExpressionType.LessThan:
			case ExpressionType.LessThanOrEqual:
			case ExpressionType.TypeAs:
			case ExpressionType.TypeIs:
			case ExpressionType.TypeEqual:
				return 8;
			case ExpressionType.LeftShift:
			case ExpressionType.RightShift:
				return 9;
			case ExpressionType.Or:
				return 4;
			case ExpressionType.OrElse:
				return 2;
			case ExpressionType.Power:
				return 13;
			}
			return 14;
		}

		private void ParenthesizedVisit(Expression parent, Expression nodeToVisit)
		{
			if (DebugViewWriter.NeedsParentheses(parent, nodeToVisit))
			{
				this.Out("(");
				this.Visit(nodeToVisit);
				this.Out(")");
				return;
			}
			this.Visit(nodeToVisit);
		}

		protected internal override Expression VisitMethodCall(MethodCallExpression node)
		{
			this.Out(".Call ");
			if (node.Object != null)
			{
				this.ParenthesizedVisit(node, node.Object);
			}
			else if (node.Method.DeclaringType != null)
			{
				this.Out(node.Method.DeclaringType.ToString());
			}
			else
			{
				this.Out("<UnknownType>");
			}
			this.Out(".");
			this.Out(node.Method.Name);
			this.VisitExpressions<Expression>('(', node.Arguments);
			return node;
		}

		protected internal override Expression VisitNewArray(NewArrayExpression node)
		{
			if (node.NodeType == ExpressionType.NewArrayBounds)
			{
				this.Out(".NewArray " + node.Type.GetElementType().ToString());
				this.VisitExpressions<Expression>('[', node.Expressions);
			}
			else
			{
				this.Out(".NewArray " + node.Type.ToString(), DebugViewWriter.Flow.Space);
				this.VisitExpressions<Expression>('{', node.Expressions);
			}
			return node;
		}

		protected internal override Expression VisitNew(NewExpression node)
		{
			this.Out(".New " + node.Type.ToString());
			this.VisitExpressions<Expression>('(', node.Arguments);
			return node;
		}

		protected override ElementInit VisitElementInit(ElementInit node)
		{
			if (node.Arguments.Count == 1)
			{
				this.Visit(node.Arguments[0]);
			}
			else
			{
				this.VisitExpressions<Expression>('{', node.Arguments);
			}
			return node;
		}

		protected internal override Expression VisitListInit(ListInitExpression node)
		{
			this.Visit(node.NewExpression);
			this.VisitExpressions<ElementInit>('{', ',', node.Initializers, delegate(ElementInit e)
			{
				this.VisitElementInit(e);
			});
			return node;
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
		{
			this.Out(assignment.Member.Name);
			this.Out(DebugViewWriter.Flow.Space, "=", DebugViewWriter.Flow.Space);
			this.Visit(assignment.Expression);
			return assignment;
		}

		protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
		{
			this.Out(binding.Member.Name);
			this.Out(DebugViewWriter.Flow.Space, "=", DebugViewWriter.Flow.Space);
			this.VisitExpressions<ElementInit>('{', ',', binding.Initializers, delegate(ElementInit e)
			{
				this.VisitElementInit(e);
			});
			return binding;
		}

		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			this.Out(binding.Member.Name);
			this.Out(DebugViewWriter.Flow.Space, "=", DebugViewWriter.Flow.Space);
			this.VisitExpressions<MemberBinding>('{', ',', binding.Bindings, delegate(MemberBinding e)
			{
				this.VisitMemberBinding(e);
			});
			return binding;
		}

		protected internal override Expression VisitMemberInit(MemberInitExpression node)
		{
			this.Visit(node.NewExpression);
			this.VisitExpressions<MemberBinding>('{', ',', node.Bindings, delegate(MemberBinding e)
			{
				this.VisitMemberBinding(e);
			});
			return node;
		}

		protected internal override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			this.ParenthesizedVisit(node, node.Expression);
			ExpressionType nodeType = node.NodeType;
			if (nodeType != ExpressionType.TypeIs)
			{
				if (nodeType == ExpressionType.TypeEqual)
				{
					this.Out(DebugViewWriter.Flow.Space, ".TypeEqual", DebugViewWriter.Flow.Space);
				}
			}
			else
			{
				this.Out(DebugViewWriter.Flow.Space, ".Is", DebugViewWriter.Flow.Space);
			}
			this.Out(node.TypeOperand.ToString());
			return node;
		}

		protected internal override Expression VisitUnary(UnaryExpression node)
		{
			ExpressionType expressionType = node.NodeType;
			if (expressionType <= ExpressionType.Quote)
			{
				if (expressionType <= ExpressionType.Convert)
				{
					if (expressionType != ExpressionType.ArrayLength)
					{
						if (expressionType == ExpressionType.Convert)
						{
							this.Out("(" + node.Type.ToString() + ")");
						}
					}
				}
				else if (expressionType != ExpressionType.ConvertChecked)
				{
					switch (expressionType)
					{
					case ExpressionType.Negate:
						this.Out("-");
						break;
					case ExpressionType.UnaryPlus:
						this.Out("+");
						break;
					case ExpressionType.NegateChecked:
						this.Out("#-");
						break;
					case ExpressionType.New:
					case ExpressionType.NewArrayInit:
					case ExpressionType.NewArrayBounds:
						break;
					case ExpressionType.Not:
						this.Out((node.Type == typeof(bool)) ? "!" : "~");
						break;
					default:
						if (expressionType == ExpressionType.Quote)
						{
							this.Out("'");
						}
						break;
					}
				}
				else
				{
					this.Out("#(" + node.Type.ToString() + ")");
				}
			}
			else if (expressionType <= ExpressionType.Increment)
			{
				if (expressionType != ExpressionType.TypeAs)
				{
					if (expressionType != ExpressionType.Decrement)
					{
						if (expressionType == ExpressionType.Increment)
						{
							this.Out(".Increment");
						}
					}
					else
					{
						this.Out(".Decrement");
					}
				}
			}
			else if (expressionType != ExpressionType.Throw)
			{
				if (expressionType != ExpressionType.Unbox)
				{
					switch (expressionType)
					{
					case ExpressionType.PreIncrementAssign:
						this.Out("++");
						break;
					case ExpressionType.PreDecrementAssign:
						this.Out("--");
						break;
					case ExpressionType.OnesComplement:
						this.Out("~");
						break;
					case ExpressionType.IsTrue:
						this.Out(".IsTrue");
						break;
					case ExpressionType.IsFalse:
						this.Out(".IsFalse");
						break;
					}
				}
				else
				{
					this.Out(".Unbox");
				}
			}
			else if (node.Operand == null)
			{
				this.Out(".Rethrow");
			}
			else
			{
				this.Out(".Throw", DebugViewWriter.Flow.Space);
			}
			this.ParenthesizedVisit(node, node.Operand);
			expressionType = node.NodeType;
			if (expressionType <= ExpressionType.TypeAs)
			{
				if (expressionType != ExpressionType.ArrayLength)
				{
					if (expressionType == ExpressionType.TypeAs)
					{
						this.Out(DebugViewWriter.Flow.Space, ".As", DebugViewWriter.Flow.Space | DebugViewWriter.Flow.Break);
						this.Out(node.Type.ToString());
					}
				}
				else
				{
					this.Out(".Length");
				}
			}
			else if (expressionType != ExpressionType.PostIncrementAssign)
			{
				if (expressionType == ExpressionType.PostDecrementAssign)
				{
					this.Out("--");
				}
			}
			else
			{
				this.Out("++");
			}
			return node;
		}

		protected internal override Expression VisitBlock(BlockExpression node)
		{
			this.Out(".Block");
			if (node.Type != node.GetExpression(node.ExpressionCount - 1).Type)
			{
				this.Out(string.Format(CultureInfo.CurrentCulture, "<{0}>", node.Type.ToString()));
			}
			this.VisitDeclarations(node.Variables);
			this.Out(" ");
			this.VisitExpressions<Expression>('{', ';', node.Expressions);
			return node;
		}

		protected internal override Expression VisitDefault(DefaultExpression node)
		{
			this.Out(".Default(" + node.Type.ToString() + ")");
			return node;
		}

		protected internal override Expression VisitLabel(LabelExpression node)
		{
			this.Out(".Label", DebugViewWriter.Flow.NewLine);
			this.Indent();
			this.Visit(node.DefaultValue);
			this.Dedent();
			this.NewLine();
			this.DumpLabel(node.Target);
			return node;
		}

		protected internal override Expression VisitGoto(GotoExpression node)
		{
			this.Out("." + node.Kind.ToString(), DebugViewWriter.Flow.Space);
			this.Out(this.GetLabelTargetName(node.Target), DebugViewWriter.Flow.Space);
			this.Out("{", DebugViewWriter.Flow.Space);
			this.Visit(node.Value);
			this.Out(DebugViewWriter.Flow.Space, "}");
			return node;
		}

		protected internal override Expression VisitLoop(LoopExpression node)
		{
			this.Out(".Loop", DebugViewWriter.Flow.Space);
			if (node.ContinueLabel != null)
			{
				this.DumpLabel(node.ContinueLabel);
			}
			this.Out(" {", DebugViewWriter.Flow.NewLine);
			this.Indent();
			this.Visit(node.Body);
			this.Dedent();
			this.Out(DebugViewWriter.Flow.NewLine, "}");
			if (node.BreakLabel != null)
			{
				this.Out("", DebugViewWriter.Flow.NewLine);
				this.DumpLabel(node.BreakLabel);
			}
			return node;
		}

		protected override SwitchCase VisitSwitchCase(SwitchCase node)
		{
			foreach (Expression expression in node.TestValues)
			{
				this.Out(".Case (");
				this.Visit(expression);
				this.Out("):", DebugViewWriter.Flow.NewLine);
			}
			this.Indent();
			this.Indent();
			this.Visit(node.Body);
			this.Dedent();
			this.Dedent();
			this.NewLine();
			return node;
		}

		protected internal override Expression VisitSwitch(SwitchExpression node)
		{
			this.Out(".Switch ");
			this.Out("(");
			this.Visit(node.SwitchValue);
			this.Out(") {", DebugViewWriter.Flow.NewLine);
			ExpressionVisitor.Visit<SwitchCase>(node.Cases, new Func<SwitchCase, SwitchCase>(this.VisitSwitchCase));
			if (node.DefaultBody != null)
			{
				this.Out(".Default:", DebugViewWriter.Flow.NewLine);
				this.Indent();
				this.Indent();
				this.Visit(node.DefaultBody);
				this.Dedent();
				this.Dedent();
				this.NewLine();
			}
			this.Out("}");
			return node;
		}

		protected override CatchBlock VisitCatchBlock(CatchBlock node)
		{
			this.Out(DebugViewWriter.Flow.NewLine, "} .Catch (" + node.Test.ToString());
			if (node.Variable != null)
			{
				this.Out(DebugViewWriter.Flow.Space, "");
				this.VisitParameter(node.Variable);
			}
			if (node.Filter != null)
			{
				this.Out(") .If (", DebugViewWriter.Flow.Break);
				this.Visit(node.Filter);
			}
			this.Out(") {", DebugViewWriter.Flow.NewLine);
			this.Indent();
			this.Visit(node.Body);
			this.Dedent();
			return node;
		}

		protected internal override Expression VisitTry(TryExpression node)
		{
			this.Out(".Try {", DebugViewWriter.Flow.NewLine);
			this.Indent();
			this.Visit(node.Body);
			this.Dedent();
			ExpressionVisitor.Visit<CatchBlock>(node.Handlers, new Func<CatchBlock, CatchBlock>(this.VisitCatchBlock));
			if (node.Finally != null)
			{
				this.Out(DebugViewWriter.Flow.NewLine, "} .Finally {", DebugViewWriter.Flow.NewLine);
				this.Indent();
				this.Visit(node.Finally);
				this.Dedent();
			}
			else if (node.Fault != null)
			{
				this.Out(DebugViewWriter.Flow.NewLine, "} .Fault {", DebugViewWriter.Flow.NewLine);
				this.Indent();
				this.Visit(node.Fault);
				this.Dedent();
			}
			this.Out(DebugViewWriter.Flow.NewLine, "}");
			return node;
		}

		protected internal override Expression VisitIndex(IndexExpression node)
		{
			if (node.Indexer != null)
			{
				this.OutMember(node, node.Object, node.Indexer);
			}
			else
			{
				this.ParenthesizedVisit(node, node.Object);
			}
			this.VisitExpressions<Expression>('[', node.Arguments);
			return node;
		}

		protected internal override Expression VisitExtension(Expression node)
		{
			this.Out(string.Format(CultureInfo.CurrentCulture, ".Extension<{0}>", node.GetType().ToString()));
			if (node.CanReduce)
			{
				this.Out(DebugViewWriter.Flow.Space, "{", DebugViewWriter.Flow.NewLine);
				this.Indent();
				this.Visit(node.Reduce());
				this.Dedent();
				this.Out(DebugViewWriter.Flow.NewLine, "}");
			}
			return node;
		}

		protected internal override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			this.Out(string.Format(CultureInfo.CurrentCulture, ".DebugInfo({0}: {1}, {2} - {3}, {4})", new object[]
			{
				node.Document.FileName,
				node.StartLine,
				node.StartColumn,
				node.EndLine,
				node.EndColumn
			}));
			return node;
		}

		private void DumpLabel(LabelTarget target)
		{
			this.Out(string.Format(CultureInfo.CurrentCulture, ".LabelTarget {0}:", this.GetLabelTargetName(target)));
		}

		private string GetLabelTargetName(LabelTarget target)
		{
			if (string.IsNullOrEmpty(target.Name))
			{
				return "#Label" + this.GetLabelTargetId(target);
			}
			return DebugViewWriter.GetDisplayName(target.Name);
		}

		private void WriteLambda(LambdaExpression lambda)
		{
			this.Out(string.Format(CultureInfo.CurrentCulture, ".Lambda {0}<{1}>", this.GetLambdaName(lambda), lambda.Type.ToString()));
			this.VisitDeclarations(lambda.Parameters);
			this.Out(DebugViewWriter.Flow.Space, "{", DebugViewWriter.Flow.NewLine);
			this.Indent();
			this.Visit(lambda.Body);
			this.Dedent();
			this.Out(DebugViewWriter.Flow.NewLine, "}");
		}

		private string GetLambdaName(LambdaExpression lambda)
		{
			if (string.IsNullOrEmpty(lambda.Name))
			{
				return "#Lambda" + this.GetLambdaId(lambda);
			}
			return DebugViewWriter.GetDisplayName(lambda.Name);
		}

		private static bool ContainsWhiteSpace(string name)
		{
			for (int i = 0; i < name.Length; i++)
			{
				if (char.IsWhiteSpace(name[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static string QuoteName(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, "'{0}'", name);
		}

		private static string GetDisplayName(string name)
		{
			if (DebugViewWriter.ContainsWhiteSpace(name))
			{
				return DebugViewWriter.QuoteName(name);
			}
			return name;
		}

		private const int Tab = 4;

		private const int MaxColumn = 120;

		private readonly TextWriter _out;

		private int _column;

		private readonly Stack<int> _stack = new Stack<int>();

		private int _delta;

		private DebugViewWriter.Flow _flow;

		private Queue<LambdaExpression> _lambdas;

		private Dictionary<LambdaExpression, int> _lambdaIds;

		private Dictionary<ParameterExpression, int> _paramIds;

		private Dictionary<LabelTarget, int> _labelIds;

		[Flags]
		private enum Flow
		{
			None = 0,
			Space = 1,
			NewLine = 2,
			Break = 32768
		}
	}
}
