using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class BooleanQueryExpression : QueryExpression
	{
		public List<PathFilter> Path { get; set; }

		public JValue Value { get; set; }

		public override bool IsMatch(JToken t)
		{
			IEnumerable<JToken> enumerable = JPath.Evaluate(this.Path, t, false);
			foreach (JToken jtoken in enumerable)
			{
				JValue jvalue = jtoken as JValue;
				switch (base.Operator)
				{
				case QueryOperator.Equals:
					if (jvalue != null && jvalue.Equals(this.Value))
					{
						return true;
					}
					break;
				case QueryOperator.NotEquals:
					if (jvalue != null && !jvalue.Equals(this.Value))
					{
						return true;
					}
					break;
				case QueryOperator.Exists:
					return true;
				case QueryOperator.LessThan:
					if (jvalue != null && jvalue.CompareTo(this.Value) < 0)
					{
						return true;
					}
					break;
				case QueryOperator.LessThanOrEquals:
					if (jvalue != null && jvalue.CompareTo(this.Value) <= 0)
					{
						return true;
					}
					break;
				case QueryOperator.GreaterThan:
					if (jvalue != null && jvalue.CompareTo(this.Value) > 0)
					{
						return true;
					}
					break;
				case QueryOperator.GreaterThanOrEquals:
					if (jvalue != null && jvalue.CompareTo(this.Value) >= 0)
					{
						return true;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			return false;
		}
	}
}
