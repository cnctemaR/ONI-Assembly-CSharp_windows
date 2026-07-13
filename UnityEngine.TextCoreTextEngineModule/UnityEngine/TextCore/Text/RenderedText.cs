using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule", "UnityEditor.GraphToolsFoundationModule" })]
	internal readonly struct RenderedText : IEquatable<RenderedText>, IEquatable<string>
	{
		public RenderedText(string value)
		{
			this = new RenderedText(value, 0, (value != null) ? value.Length : 0, null);
		}

		public RenderedText(string value, string suffix)
		{
			this = new RenderedText(value, 0, (value != null) ? value.Length : 0, suffix);
		}

		public RenderedText(string value, int start, int length, string suffix = null)
		{
			bool flag = string.IsNullOrEmpty(value);
			if (flag)
			{
				start = 0;
				length = 0;
			}
			else
			{
				bool flag2 = start < 0;
				if (flag2)
				{
					start = 0;
				}
				else
				{
					bool flag3 = start >= value.Length;
					if (flag3)
					{
						start = value.Length;
						length = 0;
					}
				}
				bool flag4 = length < 0;
				if (flag4)
				{
					length = 0;
				}
				else
				{
					bool flag5 = length > value.Length - start;
					if (flag5)
					{
						length = value.Length - start;
					}
				}
			}
			this.value = value;
			this.valueStart = start;
			this.valueLength = length;
			this.suffix = suffix;
			this.repeat = '\0';
			this.repeatCount = 0;
		}

		public RenderedText(char repeat, int repeatCount, string suffix = null)
		{
			bool flag = repeatCount < 0;
			if (flag)
			{
				repeatCount = 0;
			}
			this.value = null;
			this.valueStart = 0;
			this.valueLength = 0;
			this.suffix = suffix;
			this.repeat = repeat;
			this.repeatCount = repeatCount;
		}

		public int CharacterCount
		{
			get
			{
				int num = this.valueLength + this.repeatCount;
				bool flag = this.suffix != null;
				if (flag)
				{
					num += this.suffix.Length;
				}
				return num;
			}
		}

		public RenderedText.Enumerator GetEnumerator()
		{
			return new RenderedText.Enumerator(in this);
		}

		public string CreateString()
		{
			char[] array = new char[this.CharacterCount];
			int num = 0;
			foreach (char c in this)
			{
				array[num++] = c;
			}
			return new string(array);
		}

		public bool Equals(RenderedText other)
		{
			return this.value == other.value && this.valueStart == other.valueStart && this.valueLength == other.valueLength && this.suffix == other.suffix && this.repeat == other.repeat && this.repeatCount == other.repeatCount;
		}

		public bool Equals(string other)
		{
			int num = ((other != null) ? other.Length : 0);
			int characterCount = this.CharacterCount;
			bool flag = num != characterCount;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = num == 0;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					int num2 = 0;
					foreach (char c in this)
					{
						bool flag4 = c != other[num2++];
						if (flag4)
						{
							return false;
						}
					}
					flag2 = true;
				}
			}
			return flag2;
		}

		public override bool Equals(object obj)
		{
			string text = obj as string;
			bool flag;
			if (text == null || !this.Equals(text))
			{
				if (obj is RenderedText)
				{
					RenderedText renderedText = (RenderedText)obj;
					flag = this.Equals(renderedText);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<string, int, int, string, char, int>(this.value, this.valueStart, this.valueLength, this.suffix, this.repeat, this.repeatCount);
		}

		public readonly string value;

		public readonly int valueStart;

		public readonly int valueLength;

		public readonly string suffix;

		public readonly char repeat;

		public readonly int repeatCount;

		public struct Enumerator
		{
			public char Current
			{
				get
				{
					return this.m_Current;
				}
			}

			public Enumerator(in RenderedText source)
			{
				this.m_Source = source;
				this.m_Stage = 0;
				this.m_StageIndex = 0;
				this.m_Current = '\0';
			}

			public bool MoveNext()
			{
				bool flag = this.m_Stage == 0;
				if (flag)
				{
					bool flag2 = this.m_Source.value != null;
					if (flag2)
					{
						int valueStart = this.m_Source.valueStart;
						int num = this.m_Source.valueStart + this.m_Source.valueLength;
						bool flag3 = this.m_StageIndex < valueStart;
						if (flag3)
						{
							this.m_StageIndex = valueStart;
						}
						bool flag4 = this.m_StageIndex < num;
						if (flag4)
						{
							this.m_Current = this.m_Source.value[this.m_StageIndex];
							this.m_StageIndex++;
							return true;
						}
					}
					this.m_Stage = 1;
					this.m_StageIndex = 0;
				}
				bool flag5 = this.m_Stage == 1;
				if (flag5)
				{
					bool flag6 = this.m_StageIndex < this.m_Source.repeatCount;
					if (flag6)
					{
						this.m_Current = this.m_Source.repeat;
						this.m_StageIndex++;
						return true;
					}
					this.m_Stage = 2;
					this.m_StageIndex = 0;
				}
				bool flag7 = this.m_Stage == 2;
				if (flag7)
				{
					bool flag8 = this.m_Source.suffix != null && this.m_StageIndex < this.m_Source.suffix.Length;
					if (flag8)
					{
						this.m_Current = this.m_Source.suffix[this.m_StageIndex];
						this.m_StageIndex++;
						return true;
					}
					this.m_Stage = 3;
					this.m_StageIndex = 0;
				}
				return false;
			}

			public void Reset()
			{
				this.m_Stage = 0;
				this.m_StageIndex = 0;
				this.m_Current = '\0';
			}

			private readonly RenderedText m_Source;

			private const int k_ValueStage = 0;

			private const int k_RepeatStage = 1;

			private const int k_SuffixStage = 2;

			private int m_Stage;

			private int m_StageIndex;

			private char m_Current;
		}
	}
}
