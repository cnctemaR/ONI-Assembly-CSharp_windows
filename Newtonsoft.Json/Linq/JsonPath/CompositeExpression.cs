using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class CompositeExpression : QueryExpression
	{
		public List<QueryExpression> Expressions { get; set; }

		public CompositeExpression()
		{
			this.Expressions = new List<QueryExpression>();
		}

		public override bool IsMatch(JToken t)
		{
			switch (base.Operator)
			{
			case QueryOperator.And:
				foreach (QueryExpression queryExpression in this.Expressions)
				{
					if (!queryExpression.IsMatch(t))
					{
						return false;
					}
				}
				return true;
			case QueryOperator.Or:
				foreach (QueryExpression queryExpression2 in this.Expressions)
				{
					if (queryExpression2.IsMatch(t))
					{
						return true;
					}
				}
				return false;
			default:
				throw new ArgumentOutOfRangeException();
			}
			bool flag;
			return flag;
		}
	}
}
