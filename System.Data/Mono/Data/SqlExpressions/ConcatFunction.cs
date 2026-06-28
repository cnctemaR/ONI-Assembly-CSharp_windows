using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class ConcatFunction : StringFunction
	{
		public ConcatFunction(IExpression e, IExpression add)
			: base(e)
		{
			this._add = add;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is ConcatFunction))
			{
				return false;
			}
			ConcatFunction concatFunction = (ConcatFunction)obj;
			return this._add.Equals(concatFunction._add);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			return hashCode ^ this._add.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			string text = (string)base.Eval(row);
			string text2 = (string)this._add.Eval(row);
			return text + text2;
		}

		private readonly IExpression _add;
	}
}
