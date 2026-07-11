using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Mark : IEquatable<Mark>, IComparable<Mark>, IComparable
	{
		public Mark()
		{
			this.Line = 1;
			this.Column = 1;
		}

		public Mark(int index, int line, int column)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index must be greater than or equal to zero.");
			}
			if (line < 1)
			{
				throw new ArgumentOutOfRangeException("line", "Line must be greater than or equal to 1.");
			}
			if (column < 1)
			{
				throw new ArgumentOutOfRangeException("column", "Column must be greater than or equal to 1.");
			}
			this.Index = index;
			this.Line = line;
			this.Column = column;
		}

		public int Index { get; private set; }

		public int Line { get; private set; }

		public int Column { get; private set; }

		public override string ToString()
		{
			return string.Format("Line: {0}, Col: {1}, Idx: {2}", this.Line, this.Column, this.Index);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Mark);
		}

		public bool Equals(Mark other)
		{
			return other != null && this.Index == other.Index && this.Line == other.Line && this.Column == other.Column;
		}

		public override int GetHashCode()
		{
			return HashCode.CombineHashCodes(this.Index.GetHashCode(), HashCode.CombineHashCodes(this.Line.GetHashCode(), this.Column.GetHashCode()));
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			return this.CompareTo(obj as Mark);
		}

		public int CompareTo(Mark other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			int num = this.Line.CompareTo(other.Line);
			if (num == 0)
			{
				num = this.Column.CompareTo(other.Column);
			}
			return num;
		}

		public static readonly Mark Empty = new Mark();
	}
}
