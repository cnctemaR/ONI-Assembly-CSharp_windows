using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public class StringInfo
	{
		public StringInfo()
		{
		}

		public StringInfo(string value)
		{
			this.String = value;
		}

		[ComVisible(false)]
		public override bool Equals(object value)
		{
			StringInfo stringInfo = value as StringInfo;
			return stringInfo != null && this.s == stringInfo.s;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return this.s.GetHashCode();
		}

		public int LengthInTextElements
		{
			get
			{
				if (this.length < 0)
				{
					this.length = 0;
					int i = 0;
					while (i < this.s.Length)
					{
						i += StringInfo.GetNextTextElementLength(this.s, i);
						this.length++;
					}
				}
				return this.length;
			}
		}

		public string String
		{
			get
			{
				return this.s;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.length = -1;
				this.s = value;
			}
		}

		public string SubstringByTextElements(int startingTextElement)
		{
			if (startingTextElement < 0 || this.s.Length == 0)
			{
				throw new ArgumentOutOfRangeException("startingTextElement");
			}
			int num = 0;
			for (int i = 0; i < startingTextElement; i++)
			{
				if (num >= this.s.Length)
				{
					throw new ArgumentOutOfRangeException("startingTextElement");
				}
				num += StringInfo.GetNextTextElementLength(this.s, num);
			}
			return this.s.Substring(num);
		}

		public string SubstringByTextElements(int startingTextElement, int lengthInTextElements)
		{
			if (startingTextElement < 0 || this.s.Length == 0)
			{
				throw new ArgumentOutOfRangeException("startingTextElement");
			}
			if (lengthInTextElements < 0)
			{
				throw new ArgumentOutOfRangeException("lengthInTextElements");
			}
			int num = 0;
			for (int i = 0; i < startingTextElement; i++)
			{
				if (num >= this.s.Length)
				{
					throw new ArgumentOutOfRangeException("startingTextElement");
				}
				num += StringInfo.GetNextTextElementLength(this.s, num);
			}
			int num2 = num;
			for (int j = 0; j < lengthInTextElements; j++)
			{
				if (num >= this.s.Length)
				{
					throw new ArgumentOutOfRangeException("lengthInTextElements");
				}
				num += StringInfo.GetNextTextElementLength(this.s, num);
			}
			return this.s.Substring(num2, num - num2);
		}

		public static string GetNextTextElement(string str)
		{
			if (str == null || str.Length == 0)
			{
				throw new ArgumentNullException("string is null");
			}
			return StringInfo.GetNextTextElement(str, 0);
		}

		public static string GetNextTextElement(string str, int index)
		{
			int nextTextElementLength = StringInfo.GetNextTextElementLength(str, index);
			return (nextTextElementLength == 1) ? new string(str[index], 1) : str.Substring(index, nextTextElementLength);
		}

		private static int GetNextTextElementLength(string str, int index)
		{
			if (str == null)
			{
				throw new ArgumentNullException("string is null");
			}
			if (index >= str.Length)
			{
				return 0;
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("Index is not valid");
			}
			char c = str[index];
			UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
			if (unicodeCategory == UnicodeCategory.Surrogate)
			{
				if (c < '\ud800' || c > '\udbff')
				{
					return 1;
				}
				if (index + 1 < str.Length && str[index + 1] >= '\udc00' && str[index + 1] <= '\udfff')
				{
					return 2;
				}
				return 1;
			}
			else
			{
				if (unicodeCategory == UnicodeCategory.NonSpacingMark || unicodeCategory == UnicodeCategory.SpacingCombiningMark || unicodeCategory == UnicodeCategory.EnclosingMark)
				{
					return 1;
				}
				int num = 1;
				while (index + num < str.Length)
				{
					unicodeCategory = char.GetUnicodeCategory(str[index + num]);
					if (unicodeCategory != UnicodeCategory.NonSpacingMark && unicodeCategory != UnicodeCategory.SpacingCombiningMark && unicodeCategory != UnicodeCategory.EnclosingMark)
					{
						break;
					}
					num++;
				}
				return num;
			}
		}

		public static TextElementEnumerator GetTextElementEnumerator(string str)
		{
			if (str == null || str.Length == 0)
			{
				throw new ArgumentNullException("string is null");
			}
			return new TextElementEnumerator(str, 0);
		}

		public static TextElementEnumerator GetTextElementEnumerator(string str, int index)
		{
			if (str == null)
			{
				throw new ArgumentNullException("string is null");
			}
			if (index < 0 || index >= str.Length)
			{
				throw new ArgumentOutOfRangeException("Index is not valid");
			}
			return new TextElementEnumerator(str, index);
		}

		public static int[] ParseCombiningCharacters(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("string is null");
			}
			ArrayList arrayList = new ArrayList(str.Length);
			TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(str);
			textElementEnumerator.Reset();
			while (textElementEnumerator.MoveNext())
			{
				arrayList.Add(textElementEnumerator.ElementIndex);
			}
			return (int[])arrayList.ToArray(typeof(int));
		}

		private string s;

		private int length;
	}
}
