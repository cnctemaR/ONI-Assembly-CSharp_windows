using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class Literal : BaseExpression
	{
		public Literal(object val)
		{
			this.val = val;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is Literal))
			{
				return false;
			}
			Literal literal = (Literal)obj;
			if (literal.val != null)
			{
				if (!literal.val.Equals(this.val))
				{
					return false;
				}
			}
			else if (this.val != null)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.val.GetHashCode() ^ base.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			return this.val;
		}

		public override bool DependsOn(DataColumn other)
		{
			return false;
		}

		private object val;
	}
}
