using System;
using System.Collections.Generic;
using System.Linq.Parallel;

namespace System.Linq
{
	internal static class AggregationMinMaxHelpers<T>
	{
		private static T Reduce(IEnumerable<T> source, int sign)
		{
			Func<Pair<bool, T>, T, Pair<bool, T>> func = AggregationMinMaxHelpers<T>.MakeIntermediateReduceFunction(sign);
			Func<Pair<bool, T>, Pair<bool, T>, Pair<bool, T>> func2 = AggregationMinMaxHelpers<T>.MakeFinalReduceFunction(sign);
			Func<Pair<bool, T>, T> func3 = AggregationMinMaxHelpers<T>.MakeResultSelectorFunction();
			return new AssociativeAggregationOperator<T, Pair<bool, T>, T>(source, new Pair<bool, T>(false, default(T)), null, true, func, func2, func3, default(T) != null, QueryAggregationOptions.AssociativeCommutative).Aggregate();
		}

		internal static T ReduceMin(IEnumerable<T> source)
		{
			return AggregationMinMaxHelpers<T>.Reduce(source, -1);
		}

		internal static T ReduceMax(IEnumerable<T> source)
		{
			return AggregationMinMaxHelpers<T>.Reduce(source, 1);
		}

		private static Func<Pair<bool, T>, T, Pair<bool, T>> MakeIntermediateReduceFunction(int sign)
		{
			Comparer<T> comparer = Util.GetDefaultComparer<T>();
			return delegate(Pair<bool, T> accumulator, T element)
			{
				if ((default(T) != null || element != null) && (!accumulator.First || Util.Sign(comparer.Compare(element, accumulator.Second)) == sign))
				{
					return new Pair<bool, T>(true, element);
				}
				return accumulator;
			};
		}

		private static Func<Pair<bool, T>, Pair<bool, T>, Pair<bool, T>> MakeFinalReduceFunction(int sign)
		{
			Comparer<T> comparer = Util.GetDefaultComparer<T>();
			return delegate(Pair<bool, T> accumulator, Pair<bool, T> element)
			{
				if (element.First && (!accumulator.First || Util.Sign(comparer.Compare(element.Second, accumulator.Second)) == sign))
				{
					return new Pair<bool, T>(true, element.Second);
				}
				return accumulator;
			};
		}

		private static Func<Pair<bool, T>, T> MakeResultSelectorFunction()
		{
			return (Pair<bool, T> accumulator) => accumulator.Second;
		}
	}
}
