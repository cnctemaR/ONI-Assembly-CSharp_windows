using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class NewArrayExpression : Expression
	{
		internal NewArrayExpression(ExpressionType et, Type type, ReadOnlyCollection<Expression> expressions)
			: base(et, type)
		{
			this.expressions = expressions;
		}

		public ReadOnlyCollection<Expression> Expressions
		{
			get
			{
				return this.expressions;
			}
		}

		private void EmitNewArrayInit(EmitContext ec, Type type)
		{
			int count = this.expressions.Count;
			ec.ig.Emit(OpCodes.Ldc_I4, count);
			ec.ig.Emit(OpCodes.Newarr, type);
			for (int i = 0; i < count; i++)
			{
				ec.ig.Emit(OpCodes.Dup);
				ec.ig.Emit(OpCodes.Ldc_I4, i);
				this.expressions[i].Emit(ec);
				ec.ig.Emit(OpCodes.Stelem, type);
			}
		}

		private void EmitNewArrayBounds(EmitContext ec, Type type)
		{
			int count = this.expressions.Count;
			ec.EmitCollection<Expression>(this.expressions);
			if (count == 1)
			{
				ec.ig.Emit(OpCodes.Newarr, type);
				return;
			}
			ec.ig.Emit(OpCodes.Newobj, NewArrayExpression.GetArrayConstructor(type, count));
		}

		private static ConstructorInfo GetArrayConstructor(Type type, int rank)
		{
			return NewArrayExpression.CreateArray(type, rank).GetConstructor(NewArrayExpression.CreateTypeParameters(rank));
		}

		private static Type[] CreateTypeParameters(int rank)
		{
			return Enumerable.Repeat<Type>(typeof(int), rank).ToArray<Type>();
		}

		private static Type CreateArray(Type type, int rank)
		{
			return type.MakeArrayType(rank);
		}

		internal override void Emit(EmitContext ec)
		{
			Type elementType = base.Type.GetElementType();
			ExpressionType nodeType = base.NodeType;
			if (nodeType == ExpressionType.NewArrayInit)
			{
				this.EmitNewArrayInit(ec, elementType);
				return;
			}
			if (nodeType != ExpressionType.NewArrayBounds)
			{
				throw new NotSupportedException();
			}
			this.EmitNewArrayBounds(ec, elementType);
		}

		private ReadOnlyCollection<Expression> expressions;
	}
}
