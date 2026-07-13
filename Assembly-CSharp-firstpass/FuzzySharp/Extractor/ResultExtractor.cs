using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp.Extensions;
using FuzzySharp.SimilarityRatio.Scorer;

namespace FuzzySharp.Extractor
{
	public static class ResultExtractor
	{
		public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff = 0)
		{
			int index = 0;
			string processedQuery = processor(query);
			foreach (T t in choices)
			{
				int num = scorer.Score(processedQuery, processor(t));
				if (num >= cutoff)
				{
					yield return new ExtractedResult<T>(t, num, index);
				}
				int num2 = index;
				index = num2 + 1;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0)
		{
			return ResultExtractor.ExtractWithoutOrder<T>(query, choices, processor, calculator, cutoff).Max<ExtractedResult<T>>();
		}

		public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0)
		{
			return from r in ResultExtractor.ExtractWithoutOrder<T>(query, choices, processor, calculator, cutoff)
				orderby r.Score descending
				select r;
		}

		public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int limit, int cutoff = 0)
		{
			return ResultExtractor.ExtractWithoutOrder<T>(query, choices, processor, calculator, cutoff).MaxN<ExtractedResult<T>>(limit).Reverse<ExtractedResult<T>>();
		}
	}
}
