using System;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class ParameterExpression : Expression
	{
		internal ParameterExpression(Type type, string name)
			: base(ExpressionType.Parameter, type)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private void EmitLocalParameter(EmitContext ec, int position)
		{
			ec.ig.Emit(OpCodes.Ldarg, position);
		}

		private void EmitHoistedLocal(EmitContext ec, int level, int position)
		{
			ec.EmitScope();
			for (int i = 0; i < level; i++)
			{
				ec.EmitParentScope();
			}
			ec.EmitLoadLocals();
			ec.ig.Emit(OpCodes.Ldc_I4, position);
			ec.ig.Emit(OpCodes.Ldelem, typeof(object));
			ec.EmitLoadStrongBoxValue(base.Type);
		}

		internal override void Emit(EmitContext ec)
		{
			int num = -1;
			if (ec.IsLocalParameter(this, ref num))
			{
				this.EmitLocalParameter(ec, num);
				return;
			}
			int num2 = 0;
			if (ec.IsHoistedLocal(this, ref num2, ref num))
			{
				this.EmitHoistedLocal(ec, num2, num);
				return;
			}
			throw new InvalidOperationException("Parameter out of scope");
		}

		private string name;
	}
}
