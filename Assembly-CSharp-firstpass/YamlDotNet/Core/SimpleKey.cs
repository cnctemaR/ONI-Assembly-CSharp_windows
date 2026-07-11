using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class SimpleKey
	{
		public SimpleKey()
		{
			this.cursor = new Cursor();
		}

		public SimpleKey(bool isPossible, bool isRequired, int tokenNumber, Cursor cursor)
		{
			this.IsPossible = isPossible;
			this.IsRequired = isRequired;
			this.TokenNumber = tokenNumber;
			this.cursor = new Cursor(cursor);
		}

		public bool IsPossible { get; set; }

		public bool IsRequired { get; private set; }

		public int TokenNumber { get; private set; }

		public int Index
		{
			get
			{
				return this.cursor.Index;
			}
		}

		public int Line
		{
			get
			{
				return this.cursor.Line;
			}
		}

		public int LineOffset
		{
			get
			{
				return this.cursor.LineOffset;
			}
		}

		public Mark Mark
		{
			get
			{
				return this.cursor.Mark();
			}
		}

		private readonly Cursor cursor;
	}
}
