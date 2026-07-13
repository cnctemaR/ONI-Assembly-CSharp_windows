using System;
using System.Collections.Generic;
using FuzzySharp.Extractor;
using FuzzySharp.PreProcess;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace FuzzySharp
{
	public static class Process
	{
		public static IEnumerable<ExtractedResult<string>> ExtractAll(string query, IEnumerable<string> choices, Func<string, string> processor = null, IRatioScorer scorer = null, int cutoff = 0)
		{
			if (processor == null)
			{
				processor = Process.s_defaultStringProcessor;
			}
			if (scorer == null)
			{
				scorer = Process.s_defaultScorer;
			}
			return ResultExtractor.ExtractWithoutOrder<string>(query, choices, processor, scorer, cutoff);
		}

		public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer = null, int cutoff = 0)
		{
			if (scorer == null)
			{
				scorer = Process.s_defaultScorer;
			}
			return ResultExtractor.ExtractWithoutOrder<T>(query, choices, processor, scorer, cutoff);
		}

		public static IEnumerable<ExtractedResult<string>> ExtractTop(string query, IEnumerable<string> choices, Func<string, string> processor = null, IRatioScorer scorer = null, int limit = 5, int cutoff = 0)
		{
			if (processor == null)
			{
				processor = Process.s_defaultStringProcessor;
			}
			if (scorer == null)
			{
				scorer = Process.s_defaultScorer;
			}
			return ResultExtractor.ExtractTop<string>(query, choices, processor, scorer, limit, cutoff);
		}

		public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer = null, int limit = 5, int cutoff = 0)
		{
			if (scorer == null)
			{
				scorer = Process.s_defaultScorer;
			}
			return ResultExtractor.ExtractTop<T>(query, choices, processor, scorer, limit, cutoff);
		}

		public static IEnumerable<ExtractedResult<string>> ExtractSorted(string query, IEnumerable<string> choices, Func<string, string> processor = null, IRatioScorer scorer = null, int cutoff = 0)
		{
			if (processor == null)
			{
				processor = Process.s_defaultStringProcessor;
			}
			if (scorer == null)
			{
				scorer = Process.s_defaultScorer;
			}
			return ResultExtractor.ExtractSorted<string>(query, choices, processor, scorer, cutoff);
		}

		public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer = null, int cutoff = 0)
		{
			if (scorer == null)
			{
				scorer = Process.s_defaultScorer;
			}
			return ResultExtractor.ExtractSorted<T>(query, choices, processor, scorer, cutoff);
		}

		public static ExtractedResult<string> ExtractOne(string query, IEnumerable<string> choices, Func<string, string> processor = null, IRatioScorer scorer = null, int cutoff = 0)
		{
			if (processor == null)
			{
				processor = Process.s_defaultStringProcessor;
			}
			if (scorer == null)
			{
				scorer = Process.s_defaultScorer;
			}
			return ResultExtractor.ExtractOne<string>(query, choices, processor, scorer, cutoff);
		}

		public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer = null, int cutoff = 0)
		{
			if (scorer == null)
			{
				scorer = Process.s_defaultScorer;
			}
			return ResultExtractor.ExtractOne<T>(query, choices, processor, scorer, cutoff);
		}

		public static ExtractedResult<string> ExtractOne(string query, params string[] choices)
		{
			return ResultExtractor.ExtractOne<string>(query, choices, Process.s_defaultStringProcessor, Process.s_defaultScorer, 0);
		}

		private static readonly IRatioScorer s_defaultScorer = ScorerCache.Get<WeightedRatioScorer>();

		private static readonly Func<string, string> s_defaultStringProcessor = StringPreprocessorFactory.GetPreprocessor(PreprocessMode.Full);
	}
}
