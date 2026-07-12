using System;

namespace System.Drawing
{
	public struct CharacterRange
	{
		public CharacterRange(int First, int Length)
		{
			this.first = First;
			this.length = Length;
		}

		public int First
		{
			get
			{
				return this.first;
			}
			set
			{
				this.first = value;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
			set
			{
				this.length = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CharacterRange))
			{
				return false;
			}
			CharacterRange characterRange = (CharacterRange)obj;
			return this == characterRange;
		}

		public override int GetHashCode()
		{
			return this.first ^ this.length;
		}

		public static bool operator ==(CharacterRange cr1, CharacterRange cr2)
		{
			return cr1.first == cr2.first && cr1.length == cr2.length;
		}

		public static bool operator !=(CharacterRange cr1, CharacterRange cr2)
		{
			return cr1.first != cr2.first || cr1.length != cr2.length;
		}

		private int first;

		private int length;
	}
}
