using System;

namespace System.Linq.Parallel
{
	internal static class QueryAggregationOptionsExtensions
	{
		public static bool IsValidQueryAggregationOption(this QueryAggregationOptions value)
		{
			return value == QueryAggregationOptions.None || value == QueryAggregationOptions.Associative || value == QueryAggregationOptions.Commutative || value == QueryAggregationOptions.AssociativeCommutative;
		}
	}
}
