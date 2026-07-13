using System;
using System.Collections.Generic;

namespace FuzzySharp.Extractor
{
	public class ExtractedResult<T> : IComparable<ExtractedResult<T>>
	{
		public ExtractedResult(T value, int score)
		{
			this.Value = value;
			this.Score = score;
		}

		public ExtractedResult(T value, int score, int index)
		{
			this.Value = value;
			this.Score = score;
			this.Index = index;
		}

		public int CompareTo(ExtractedResult<T> other)
		{
			return Comparer<int>.Default.Compare(this.Score, other.Score);
		}

		public override string ToString()
		{
			if (typeof(T) == typeof(string))
			{
				return string.Format("(string: {0}, score: {1}, index: {2})", this.Value, this.Score, this.Index);
			}
			string text = "(value: {0}, score: {1}, index: {2})";
			T value = this.Value;
			return string.Format(text, value.ToString(), this.Score, this.Index);
		}

		public readonly T Value;

		public readonly int Score;

		public readonly int Index;
	}
}
