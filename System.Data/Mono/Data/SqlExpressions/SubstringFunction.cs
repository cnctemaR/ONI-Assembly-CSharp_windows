using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class SubstringFunction : StringFunction
	{
		public SubstringFunction(IExpression e, IExpression start, IExpression len)
			: base(e)
		{
			this.start = start;
			this.len = len;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is SubstringFunction))
			{
				return false;
			}
			SubstringFunction substringFunction = (SubstringFunction)obj;
			return substringFunction.start == this.start && substringFunction.len == this.len;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			num ^= this.start.GetHashCode();
			return num ^ this.len.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			string text = (string)base.Eval(row);
			object obj = this.start.Eval(row);
			int num = Convert.ToInt32(this.start.Eval(row));
			int num2 = Convert.ToInt32(this.len.Eval(row));
			if (text == null)
			{
				return null;
			}
			if (num > text.Length)
			{
				return string.Empty;
			}
			return text.Substring(num - 1, Math.Min(num2, text.Length - (num - 1)));
		}

		private IExpression start;

		private IExpression len;
	}
}
