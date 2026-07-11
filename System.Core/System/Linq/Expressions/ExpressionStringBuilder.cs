using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Linq.Expressions
{
	internal sealed class ExpressionStringBuilder : ExpressionVisitor
	{
		private ExpressionStringBuilder()
		{
			this._out = new StringBuilder();
		}

		public override string ToString()
		{
			return this._out.ToString();
		}

		private int GetLabelId(LabelTarget label)
		{
			return this.GetId(label);
		}

		private int GetParamId(ParameterExpression p)
		{
			return this.GetId(p);
		}

		private int GetId(object o)
		{
			if (this._ids == null)
			{
				this._ids = new Dictionary<object, int>();
			}
			int count;
			if (!this._ids.TryGetValue(o, out count))
			{
				count = this._ids.Count;
				this._ids.Add(o, count);
			}
			return count;
		}

		private void Out(string s)
		{
			this._out.Append(s);
		}

		private void Out(char c)
		{
			this._out.Append(c);
		}

		internal static string ExpressionToString(Expression node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.Visit(node);
			return expressionStringBuilder.ToString();
		}

		internal static string CatchBlockToString(CatchBlock node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.VisitCatchBlock(node);
			return expressionStringBuilder.ToString();
		}

		internal static string SwitchCaseToString(SwitchCase node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.VisitSwitchCase(node);
			return expressionStringBuilder.ToString();
		}

		internal static string MemberBindingToString(MemberBinding node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.VisitMemberBinding(node);
			return expressionStringBuilder.ToString();
		}

		internal static string ElementInitBindingToString(ElementInit node)
		{
			ExpressionStringBuilder expressionStringBuilder = new ExpressionStringBuilder();
			expressionStringBuilder.VisitElementInit(node);
			return expressionStringBuilder.ToString();
		}

		private void VisitExpressions<T>(char open, ReadOnlyCollection<T> expressions, char close) where T : Expression
		{
			this.VisitExpressions<T>(open, expressions, close, ", ");
		}

		private void VisitExpressions<T>(char open, ReadOnlyCollection<T> expressions, char close, string seperator) where T : Expression
		{
			this.Out(open);
			if (expressions != null)
			{
				bool flag = true;
				foreach (T t in expressions)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						this.Out(seperator);
					}
					this.Visit(t);
				}
			}
			this.Out(close);
		}

		protected internal override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.ArrayIndex)
			{
				this.Visit(node.Left);
				this.Out('[');
				this.Visit(node.Right);
				this.Out(']');
			}
			else
			{
				ExpressionType nodeType = node.NodeType;
				string text;
				switch (nodeType)
				{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
					text = "+";
					goto IL_02CE;
				case ExpressionType.And:
					text = (ExpressionStringBuilder.IsBool(node) ? "And" : "&");
					goto IL_02CE;
				case ExpressionType.AndAlso:
					text = "AndAlso";
					goto IL_02CE;
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
					goto IL_02CE;
				case ExpressionType.Divide:
					text = "/";
					goto IL_02CE;
				case ExpressionType.Equal:
					text = "==";
					goto IL_02CE;
				case ExpressionType.ExclusiveOr:
					text = "^";
					goto IL_02CE;
				case ExpressionType.GreaterThan:
					text = ">";
					goto IL_02CE;
				case ExpressionType.GreaterThanOrEqual:
					text = ">=";
					goto IL_02CE;
				case ExpressionType.LeftShift:
					text = "<<";
					goto IL_02CE;
				case ExpressionType.LessThan:
					text = "<";
					goto IL_02CE;
				case ExpressionType.LessThanOrEqual:
					text = "<=";
					goto IL_02CE;
				case ExpressionType.Modulo:
					text = "%";
					goto IL_02CE;
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					text = "*";
					goto IL_02CE;
				case ExpressionType.NotEqual:
					text = "!=";
					goto IL_02CE;
				case ExpressionType.Or:
					text = (ExpressionStringBuilder.IsBool(node) ? "Or" : "|");
					goto IL_02CE;
				case ExpressionType.OrElse:
					text = "OrElse";
					goto IL_02CE;
				case ExpressionType.Power:
					text = "**";
					goto IL_02CE;
				case ExpressionType.RightShift:
					text = ">>";
					goto IL_02CE;
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					text = "-";
					goto IL_02CE;
				case ExpressionType.Assign:
					text = "=";
					goto IL_02CE;
				default:
					switch (nodeType)
					{
					case ExpressionType.AddAssign:
					case ExpressionType.AddAssignChecked:
						text = "+=";
						goto IL_02CE;
					case ExpressionType.AndAssign:
						text = (ExpressionStringBuilder.IsBool(node) ? "&&=" : "&=");
						goto IL_02CE;
					case ExpressionType.DivideAssign:
						text = "/=";
						goto IL_02CE;
					case ExpressionType.ExclusiveOrAssign:
						text = "^=";
						goto IL_02CE;
					case ExpressionType.LeftShiftAssign:
						text = "<<=";
						goto IL_02CE;
					case ExpressionType.ModuloAssign:
						text = "%=";
						goto IL_02CE;
					case ExpressionType.MultiplyAssign:
					case ExpressionType.MultiplyAssignChecked:
						text = "*=";
						goto IL_02CE;
					case ExpressionType.OrAssign:
						text = (ExpressionStringBuilder.IsBool(node) ? "||=" : "|=");
						goto IL_02CE;
					case ExpressionType.PowerAssign:
						text = "**=";
						goto IL_02CE;
					case ExpressionType.RightShiftAssign:
						text = ">>=";
						goto IL_02CE;
					case ExpressionType.SubtractAssign:
					case ExpressionType.SubtractAssignChecked:
						text = "-=";
						goto IL_02CE;
					}
					break;
				}
				throw new InvalidOperationException();
				IL_02CE:
				this.Out('(');
				this.Visit(node.Left);
				this.Out(' ');
				this.Out(text);
				this.Out(' ');
				this.Visit(node.Right);
				this.Out(')');
			}
			return node;
		}

		protected internal override Expression VisitParameter(ParameterExpression node)
		{
			if (node.IsByRef)
			{
				this.Out("ref ");
			}
			string name = node.Name;
			if (string.IsNullOrEmpty(name))
			{
				this.Out("Param_" + this.GetParamId(node));
			}
			else
			{
				this.Out(name);
			}
			return node;
		}

		protected internal override Expression VisitLambda<T>(Expression<T> node)
		{
			if (node.ParameterCount == 1)
			{
				this.Visit(node.GetParameter(0));
			}
			else
			{
				this.Out('(');
				string text = ", ";
				int i = 0;
				int parameterCount = node.ParameterCount;
				while (i < parameterCount)
				{
					if (i > 0)
					{
						this.Out(text);
					}
					this.Visit(node.GetParameter(i));
					i++;
				}
				this.Out(')');
			}
			this.Out(" => ");
			this.Visit(node.Body);
			return node;
		}

		protected internal override Expression VisitListInit(ListInitExpression node)
		{
			this.Visit(node.NewExpression);
			this.Out(" {");
			int i = 0;
			int count = node.Initializers.Count;
			while (i < count)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				this.VisitElementInit(node.Initializers[i]);
				i++;
			}
			this.Out('}');
			return node;
		}

		protected internal override Expression VisitConditional(ConditionalExpression node)
		{
			this.Out("IIF(");
			this.Visit(node.Test);
			this.Out(", ");
			this.Visit(node.IfTrue);
			this.Out(", ");
			this.Visit(node.IfFalse);
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitConstant(ConstantExpression node)
		{
			if (node.Value != null)
			{
				string text = node.Value.ToString();
				if (node.Value is string)
				{
					this.Out('"');
					this.Out(text);
					this.Out('"');
				}
				else if (text == node.Value.GetType().ToString())
				{
					this.Out("value(");
					this.Out(text);
					this.Out(')');
				}
				else
				{
					this.Out(text);
				}
			}
			else
			{
				this.Out("null");
			}
			return node;
		}

		protected internal override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			string text = string.Format(CultureInfo.CurrentCulture, "<DebugInfo({0}: {1}, {2}, {3}, {4})>", new object[]
			{
				node.Document.FileName,
				node.StartLine,
				node.StartColumn,
				node.EndLine,
				node.EndColumn
			});
			this.Out(text);
			return node;
		}

		protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			this.VisitExpressions<ParameterExpression>('(', node.Variables, ')');
			return node;
		}

		private void OutMember(Expression instance, MemberInfo member)
		{
			if (instance != null)
			{
				this.Visit(instance);
			}
			else
			{
				this.Out(member.DeclaringType.Name);
			}
			this.Out('.');
			this.Out(member.Name);
		}

		protected internal override Expression VisitMember(MemberExpression node)
		{
			this.OutMember(node.Expression, node.Member);
			return node;
		}

		protected internal override Expression VisitMemberInit(MemberInitExpression node)
		{
			if (node.NewExpression.ArgumentCount == 0 && node.NewExpression.Type.Name.Contains("<"))
			{
				this.Out("new");
			}
			else
			{
				this.Visit(node.NewExpression);
			}
			this.Out(" {");
			int i = 0;
			int count = node.Bindings.Count;
			while (i < count)
			{
				MemberBinding memberBinding = node.Bindings[i];
				if (i > 0)
				{
					this.Out(", ");
				}
				this.VisitMemberBinding(memberBinding);
				i++;
			}
			this.Out('}');
			return node;
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
		{
			this.Out(assignment.Member.Name);
			this.Out(" = ");
			this.Visit(assignment.Expression);
			return assignment;
		}

		protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
		{
			this.Out(binding.Member.Name);
			this.Out(" = {");
			int i = 0;
			int count = binding.Initializers.Count;
			while (i < count)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				this.VisitElementInit(binding.Initializers[i]);
				i++;
			}
			this.Out('}');
			return binding;
		}

		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			this.Out(binding.Member.Name);
			this.Out(" = {");
			int i = 0;
			int count = binding.Bindings.Count;
			while (i < count)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				this.VisitMemberBinding(binding.Bindings[i]);
				i++;
			}
			this.Out('}');
			return binding;
		}

		protected override ElementInit VisitElementInit(ElementInit initializer)
		{
			this.Out(initializer.AddMethod.ToString());
			string text = ", ";
			this.Out('(');
			int i = 0;
			int argumentCount = initializer.ArgumentCount;
			while (i < argumentCount)
			{
				if (i > 0)
				{
					this.Out(text);
				}
				this.Visit(initializer.GetArgument(i));
				i++;
			}
			this.Out(')');
			return initializer;
		}

		protected internal override Expression VisitInvocation(InvocationExpression node)
		{
			this.Out("Invoke(");
			this.Visit(node.Expression);
			string text = ", ";
			int i = 0;
			int argumentCount = node.ArgumentCount;
			while (i < argumentCount)
			{
				this.Out(text);
				this.Visit(node.GetArgument(i));
				i++;
			}
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitMethodCall(MethodCallExpression node)
		{
			int num = 0;
			Expression expression = node.Object;
			if (node.Method.GetCustomAttribute(typeof(ExtensionAttribute)) != null)
			{
				num = 1;
				expression = node.GetArgument(0);
			}
			if (expression != null)
			{
				this.Visit(expression);
				this.Out('.');
			}
			this.Out(node.Method.Name);
			this.Out('(');
			int i = num;
			int argumentCount = node.ArgumentCount;
			while (i < argumentCount)
			{
				if (i > num)
				{
					this.Out(", ");
				}
				this.Visit(node.GetArgument(i));
				i++;
			}
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitNewArray(NewArrayExpression node)
		{
			ExpressionType nodeType = node.NodeType;
			if (nodeType != ExpressionType.NewArrayInit)
			{
				if (nodeType == ExpressionType.NewArrayBounds)
				{
					this.Out("new ");
					this.Out(node.Type.ToString());
					this.VisitExpressions<Expression>('(', node.Expressions, ')');
				}
			}
			else
			{
				this.Out("new [] ");
				this.VisitExpressions<Expression>('{', node.Expressions, '}');
			}
			return node;
		}

		protected internal override Expression VisitNew(NewExpression node)
		{
			this.Out("new ");
			this.Out(node.Type.Name);
			this.Out('(');
			ReadOnlyCollection<MemberInfo> members = node.Members;
			for (int i = 0; i < node.ArgumentCount; i++)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				if (members != null)
				{
					string name = members[i].Name;
					this.Out(name);
					this.Out(" = ");
				}
				this.Visit(node.GetArgument(i));
			}
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			this.Out('(');
			this.Visit(node.Expression);
			ExpressionType nodeType = node.NodeType;
			if (nodeType != ExpressionType.TypeIs)
			{
				if (nodeType == ExpressionType.TypeEqual)
				{
					this.Out(" TypeEqual ");
				}
			}
			else
			{
				this.Out(" Is ");
			}
			this.Out(node.TypeOperand.Name);
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitUnary(UnaryExpression node)
		{
			ExpressionType expressionType = node.NodeType;
			if (expressionType <= ExpressionType.Quote)
			{
				if (expressionType <= ExpressionType.Convert)
				{
					if (expressionType == ExpressionType.ArrayLength)
					{
						this.Out("ArrayLength(");
						goto IL_019E;
					}
					if (expressionType == ExpressionType.Convert)
					{
						this.Out("Convert(");
						goto IL_019E;
					}
				}
				else
				{
					if (expressionType == ExpressionType.ConvertChecked)
					{
						this.Out("ConvertChecked(");
						goto IL_019E;
					}
					switch (expressionType)
					{
					case ExpressionType.Negate:
					case ExpressionType.NegateChecked:
						this.Out('-');
						goto IL_019E;
					case ExpressionType.UnaryPlus:
						this.Out('+');
						goto IL_019E;
					case ExpressionType.New:
					case ExpressionType.NewArrayInit:
					case ExpressionType.NewArrayBounds:
						break;
					case ExpressionType.Not:
						this.Out("Not(");
						goto IL_019E;
					default:
						if (expressionType == ExpressionType.Quote)
						{
							goto IL_019E;
						}
						break;
					}
				}
			}
			else if (expressionType <= ExpressionType.Increment)
			{
				if (expressionType == ExpressionType.TypeAs)
				{
					this.Out('(');
					goto IL_019E;
				}
				if (expressionType == ExpressionType.Decrement)
				{
					this.Out("Decrement(");
					goto IL_019E;
				}
				if (expressionType == ExpressionType.Increment)
				{
					this.Out("Increment(");
					goto IL_019E;
				}
			}
			else
			{
				if (expressionType == ExpressionType.Throw)
				{
					this.Out("throw(");
					goto IL_019E;
				}
				if (expressionType == ExpressionType.Unbox)
				{
					this.Out("Unbox(");
					goto IL_019E;
				}
				switch (expressionType)
				{
				case ExpressionType.PreIncrementAssign:
					this.Out("++");
					goto IL_019E;
				case ExpressionType.PreDecrementAssign:
					this.Out("--");
					goto IL_019E;
				case ExpressionType.PostIncrementAssign:
				case ExpressionType.PostDecrementAssign:
					goto IL_019E;
				case ExpressionType.OnesComplement:
					this.Out("~(");
					goto IL_019E;
				case ExpressionType.IsTrue:
					this.Out("IsTrue(");
					goto IL_019E;
				case ExpressionType.IsFalse:
					this.Out("IsFalse(");
					goto IL_019E;
				}
			}
			throw new InvalidOperationException();
			IL_019E:
			this.Visit(node.Operand);
			expressionType = node.NodeType;
			if (expressionType <= ExpressionType.NegateChecked)
			{
				if (expressionType - ExpressionType.Convert <= 1)
				{
					this.Out(", ");
					this.Out(node.Type.Name);
					this.Out(')');
					return node;
				}
				if (expressionType - ExpressionType.Negate <= 2)
				{
					return node;
				}
			}
			else
			{
				if (expressionType == ExpressionType.Quote)
				{
					return node;
				}
				if (expressionType == ExpressionType.TypeAs)
				{
					this.Out(" As ");
					this.Out(node.Type.Name);
					this.Out(')');
					return node;
				}
				switch (expressionType)
				{
				case ExpressionType.PreIncrementAssign:
				case ExpressionType.PreDecrementAssign:
					return node;
				case ExpressionType.PostIncrementAssign:
					this.Out("++");
					return node;
				case ExpressionType.PostDecrementAssign:
					this.Out("--");
					return node;
				}
			}
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitBlock(BlockExpression node)
		{
			this.Out('{');
			foreach (ParameterExpression parameterExpression in node.Variables)
			{
				this.Out("var ");
				this.Visit(parameterExpression);
				this.Out(';');
			}
			this.Out(" ... }");
			return node;
		}

		protected internal override Expression VisitDefault(DefaultExpression node)
		{
			this.Out("default(");
			this.Out(node.Type.Name);
			this.Out(')');
			return node;
		}

		protected internal override Expression VisitLabel(LabelExpression node)
		{
			this.Out("{ ... } ");
			this.DumpLabel(node.Target);
			this.Out(':');
			return node;
		}

		protected internal override Expression VisitGoto(GotoExpression node)
		{
			string text;
			switch (node.Kind)
			{
			case GotoExpressionKind.Goto:
				text = "goto";
				break;
			case GotoExpressionKind.Return:
				text = "return";
				break;
			case GotoExpressionKind.Break:
				text = "break";
				break;
			case GotoExpressionKind.Continue:
				text = "continue";
				break;
			default:
				throw new InvalidOperationException();
			}
			this.Out(text);
			this.Out(' ');
			this.DumpLabel(node.Target);
			if (node.Value != null)
			{
				this.Out(" (");
				this.Visit(node.Value);
				this.Out(")");
			}
			return node;
		}

		protected internal override Expression VisitLoop(LoopExpression node)
		{
			this.Out("loop { ... }");
			return node;
		}

		protected override SwitchCase VisitSwitchCase(SwitchCase node)
		{
			this.Out("case ");
			this.VisitExpressions<Expression>('(', node.TestValues, ')');
			this.Out(": ...");
			return node;
		}

		protected internal override Expression VisitSwitch(SwitchExpression node)
		{
			this.Out("switch ");
			this.Out('(');
			this.Visit(node.SwitchValue);
			this.Out(") { ... }");
			return node;
		}

		protected override CatchBlock VisitCatchBlock(CatchBlock node)
		{
			this.Out("catch (");
			this.Out(node.Test.Name);
			ParameterExpression variable = node.Variable;
			if (!string.IsNullOrEmpty((variable != null) ? variable.Name : null))
			{
				this.Out(' ');
				this.Out(node.Variable.Name);
			}
			this.Out(") { ... }");
			return node;
		}

		protected internal override Expression VisitTry(TryExpression node)
		{
			this.Out("try { ... }");
			return node;
		}

		protected internal override Expression VisitIndex(IndexExpression node)
		{
			if (node.Object != null)
			{
				this.Visit(node.Object);
			}
			else
			{
				this.Out(node.Indexer.DeclaringType.Name);
			}
			if (node.Indexer != null)
			{
				this.Out('.');
				this.Out(node.Indexer.Name);
			}
			this.Out('[');
			int i = 0;
			int argumentCount = node.ArgumentCount;
			while (i < argumentCount)
			{
				if (i > 0)
				{
					this.Out(", ");
				}
				this.Visit(node.GetArgument(i));
				i++;
			}
			this.Out(']');
			return node;
		}

		protected internal override Expression VisitExtension(Expression node)
		{
			MethodInfo method = node.GetType().GetMethod("ToString", Type.EmptyTypes);
			if (method.DeclaringType != typeof(Expression) && !method.IsStatic)
			{
				this.Out(node.ToString());
				return node;
			}
			this.Out('[');
			this.Out((node.NodeType == ExpressionType.Extension) ? node.GetType().FullName : node.NodeType.ToString());
			this.Out(']');
			return node;
		}

		private void DumpLabel(LabelTarget target)
		{
			if (!string.IsNullOrEmpty(target.Name))
			{
				this.Out(target.Name);
				return;
			}
			int labelId = this.GetLabelId(target);
			this.Out("UnnamedLabel_" + labelId);
		}

		private static bool IsBool(Expression node)
		{
			return node.Type == typeof(bool) || node.Type == typeof(bool?);
		}

		private readonly StringBuilder _out;

		private Dictionary<object, int> _ids;
	}
}
