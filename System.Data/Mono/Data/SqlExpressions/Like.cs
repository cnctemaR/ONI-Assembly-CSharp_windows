using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class Like : UnaryExpression
	{
		public Like(IExpression e, IExpression pattern)
			: base(e)
		{
			this._pattern = pattern;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is Like))
			{
				return false;
			}
			Like like = (Like)obj;
			return this._pattern.Equals(like._pattern);
		}

		public override int GetHashCode()
		{
			return this._pattern.GetHashCode() ^ base.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			object obj = this.expr.Eval(row);
			if (obj == null || obj == DBNull.Value)
			{
				return false;
			}
			string text = (string)obj;
			string text2 = (string)this._pattern.Eval(row);
			string text3 = text2;
			int length = text2.Length;
			bool flag = text2[0] == '*' || text2[0] == '%';
			bool flag2 = text2[length - 1] == '*' || text2[length - 1] == '%';
			text2 = text2.Trim(new char[] { '*', '%' });
			text2 = text2.Replace("[*]", "[[0]]");
			text2 = text2.Replace("[%]", "[[1]]");
			if (text2.IndexOf('*') != -1 || text2.IndexOf('%') != -1)
			{
				throw new EvaluateException(string.Format("Pattern '{0}' is invalid.", text3));
			}
			text2 = text2.Replace("[[0]]", "*");
			text2 = text2.Replace("[[1]]", "%");
			text2 = text2.Replace("[[]", "[");
			text2 = text2.Replace("[]]", "]");
			if (!row.Table.CaseSensitive)
			{
				text = text.ToLower();
				text2 = text2.ToLower();
			}
			int num = text.IndexOf(text2);
			if (num == -1)
			{
				return false;
			}
			return (num == 0 || flag) && (num + text2.Length == text.Length || flag2);
		}

		public override bool EvalBoolean(DataRow row)
		{
			return (bool)this.Eval(row);
		}

		private readonly IExpression _pattern;
	}
}
