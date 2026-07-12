using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace System.ComponentModel
{
	public class MaskedTextProvider : ICloneable
	{
		public MaskedTextProvider(string mask)
			: this(mask, null, true, '_', '\0', false)
		{
		}

		public MaskedTextProvider(string mask, bool restrictToAscii)
			: this(mask, null, true, '_', '\0', restrictToAscii)
		{
		}

		public MaskedTextProvider(string mask, CultureInfo culture)
			: this(mask, culture, true, '_', '\0', false)
		{
		}

		public MaskedTextProvider(string mask, CultureInfo culture, bool restrictToAscii)
			: this(mask, culture, true, '_', '\0', restrictToAscii)
		{
		}

		public MaskedTextProvider(string mask, char passwordChar, bool allowPromptAsInput)
			: this(mask, null, allowPromptAsInput, '_', passwordChar, false)
		{
		}

		public MaskedTextProvider(string mask, CultureInfo culture, char passwordChar, bool allowPromptAsInput)
			: this(mask, culture, allowPromptAsInput, '_', passwordChar, false)
		{
		}

		public MaskedTextProvider(string mask, CultureInfo culture, bool allowPromptAsInput, char promptChar, char passwordChar, bool restrictToAscii)
		{
			if (string.IsNullOrEmpty(mask))
			{
				throw new ArgumentException(SR.Format("The Mask value cannot be null or empty.", Array.Empty<object>()), "mask");
			}
			for (int i = 0; i < mask.Length; i++)
			{
				if (!MaskedTextProvider.IsPrintableChar(mask[i]))
				{
					throw new ArgumentException("The specified mask contains invalid characters.");
				}
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			this._flagState = default(BitVector32);
			this.Mask = mask;
			this._promptChar = promptChar;
			this._passwordChar = passwordChar;
			if (culture.IsNeutralCulture)
			{
				foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
				{
					if (culture.Equals(cultureInfo.Parent))
					{
						this.Culture = cultureInfo;
						break;
					}
				}
				if (this.Culture == null)
				{
					this.Culture = CultureInfo.InvariantCulture;
				}
			}
			else
			{
				this.Culture = culture;
			}
			if (!this.Culture.IsReadOnly)
			{
				this.Culture = CultureInfo.ReadOnly(this.Culture);
			}
			this._flagState[MaskedTextProvider.s_ALLOW_PROMPT_AS_INPUT] = allowPromptAsInput;
			this._flagState[MaskedTextProvider.s_ASCII_ONLY] = restrictToAscii;
			this._flagState[MaskedTextProvider.s_INCLUDE_PROMPT] = false;
			this._flagState[MaskedTextProvider.s_INCLUDE_LITERALS] = true;
			this._flagState[MaskedTextProvider.s_RESET_ON_PROMPT] = true;
			this._flagState[MaskedTextProvider.s_SKIP_SPACE] = true;
			this._flagState[MaskedTextProvider.s_RESET_ON_LITERALS] = true;
			this.Initialize();
		}

		private void Initialize()
		{
			this._testString = new StringBuilder();
			this._stringDescriptor = new List<MaskedTextProvider.CharDescriptor>();
			MaskedTextProvider.CaseConversion caseConversion = MaskedTextProvider.CaseConversion.None;
			bool flag = false;
			int num = 0;
			MaskedTextProvider.CharType charType = MaskedTextProvider.CharType.Literal;
			string text = string.Empty;
			int i = 0;
			while (i < this.Mask.Length)
			{
				char c = this.Mask[i];
				if (!flag)
				{
					if (c <= 'C')
					{
						switch (c)
						{
						case '#':
							goto IL_019E;
						case '$':
							text = this.Culture.NumberFormat.CurrencySymbol;
							charType = MaskedTextProvider.CharType.Separator;
							goto IL_01BE;
						case '%':
							goto IL_01B8;
						case '&':
							break;
						default:
							switch (c)
							{
							case ',':
								text = this.Culture.NumberFormat.NumberGroupSeparator;
								charType = MaskedTextProvider.CharType.Separator;
								goto IL_01BE;
							case '-':
								goto IL_01B8;
							case '.':
								text = this.Culture.NumberFormat.NumberDecimalSeparator;
								charType = MaskedTextProvider.CharType.Separator;
								goto IL_01BE;
							case '/':
								text = this.Culture.DateTimeFormat.DateSeparator;
								charType = MaskedTextProvider.CharType.Separator;
								goto IL_01BE;
							case '0':
								break;
							default:
								switch (c)
								{
								case '9':
								case '?':
								case 'C':
									goto IL_019E;
								case ':':
									text = this.Culture.DateTimeFormat.TimeSeparator;
									charType = MaskedTextProvider.CharType.Separator;
									goto IL_01BE;
								case ';':
								case '=':
								case '@':
								case 'B':
									goto IL_01B8;
								case '<':
									caseConversion = MaskedTextProvider.CaseConversion.ToLower;
									goto IL_022A;
								case '>':
									caseConversion = MaskedTextProvider.CaseConversion.ToUpper;
									goto IL_022A;
								case 'A':
									break;
								default:
									goto IL_01B8;
								}
								break;
							}
							break;
						}
					}
					else if (c <= '\\')
					{
						if (c != 'L')
						{
							if (c != '\\')
							{
								goto IL_01B8;
							}
							flag = true;
							charType = MaskedTextProvider.CharType.Literal;
							goto IL_022A;
						}
					}
					else
					{
						if (c == 'a')
						{
							goto IL_019E;
						}
						if (c != '|')
						{
							goto IL_01B8;
						}
						caseConversion = MaskedTextProvider.CaseConversion.None;
						goto IL_022A;
					}
					this._requiredEditChars++;
					c = this._promptChar;
					charType = MaskedTextProvider.CharType.EditRequired;
					goto IL_01BE;
					IL_019E:
					this._optionalEditChars++;
					c = this._promptChar;
					charType = MaskedTextProvider.CharType.EditOptional;
					goto IL_01BE;
					IL_01B8:
					charType = MaskedTextProvider.CharType.Literal;
					goto IL_01BE;
				}
				flag = false;
				goto IL_01BE;
				IL_022A:
				i++;
				continue;
				IL_01BE:
				MaskedTextProvider.CharDescriptor charDescriptor = new MaskedTextProvider.CharDescriptor(i, charType);
				if (MaskedTextProvider.IsEditPosition(charDescriptor))
				{
					charDescriptor.CaseConversion = caseConversion;
				}
				if (charType != MaskedTextProvider.CharType.Separator)
				{
					text = c.ToString();
				}
				foreach (char c2 in text)
				{
					this._testString.Append(c2);
					this._stringDescriptor.Add(charDescriptor);
					num++;
				}
				goto IL_022A;
			}
			this._testString.Capacity = this._testString.Length;
		}

		public bool AllowPromptAsInput
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_ALLOW_PROMPT_AS_INPUT];
			}
		}

		public int AssignedEditPositionCount { get; private set; }

		public int AvailableEditPositionCount
		{
			get
			{
				return this.EditPositionCount - this.AssignedEditPositionCount;
			}
		}

		public object Clone()
		{
			Type type = base.GetType();
			MaskedTextProvider maskedTextProvider;
			if (type == MaskedTextProvider.s_maskTextProviderType)
			{
				maskedTextProvider = new MaskedTextProvider(this.Mask, this.Culture, this.AllowPromptAsInput, this.PromptChar, this.PasswordChar, this.AsciiOnly);
			}
			else
			{
				object[] array = new object[] { this.Mask, this.Culture, this.AllowPromptAsInput, this.PromptChar, this.PasswordChar, this.AsciiOnly };
				maskedTextProvider = SecurityUtils.SecureCreateInstance(type, array) as MaskedTextProvider;
			}
			maskedTextProvider.ResetOnPrompt = false;
			maskedTextProvider.ResetOnSpace = false;
			maskedTextProvider.SkipLiterals = false;
			for (int i = 0; i < this._testString.Length; i++)
			{
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[i];
				if (MaskedTextProvider.IsEditPosition(charDescriptor) && charDescriptor.IsAssigned)
				{
					maskedTextProvider.Replace(this._testString[i], i);
				}
			}
			maskedTextProvider.ResetOnPrompt = this.ResetOnPrompt;
			maskedTextProvider.ResetOnSpace = this.ResetOnSpace;
			maskedTextProvider.SkipLiterals = this.SkipLiterals;
			maskedTextProvider.IncludeLiterals = this.IncludeLiterals;
			maskedTextProvider.IncludePrompt = this.IncludePrompt;
			return maskedTextProvider;
		}

		public CultureInfo Culture { get; }

		public static char DefaultPasswordChar
		{
			get
			{
				return '*';
			}
		}

		public int EditPositionCount
		{
			get
			{
				return this._optionalEditChars + this._requiredEditChars;
			}
		}

		public IEnumerator EditPositions
		{
			get
			{
				List<int> list = new List<int>();
				int num = 0;
				using (List<MaskedTextProvider.CharDescriptor>.Enumerator enumerator = this._stringDescriptor.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (MaskedTextProvider.IsEditPosition(enumerator.Current))
						{
							list.Add(num);
						}
						num++;
					}
				}
				return ((IEnumerable)list).GetEnumerator();
			}
		}

		public bool IncludeLiterals
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_INCLUDE_LITERALS];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_INCLUDE_LITERALS] = value;
			}
		}

		public bool IncludePrompt
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_INCLUDE_PROMPT];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_INCLUDE_PROMPT] = value;
			}
		}

		public bool AsciiOnly
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_ASCII_ONLY];
			}
		}

		public bool IsPassword
		{
			get
			{
				return this._passwordChar > '\0';
			}
			set
			{
				if (this.IsPassword != value)
				{
					this._passwordChar = (value ? MaskedTextProvider.DefaultPasswordChar : '\0');
				}
			}
		}

		public static int InvalidIndex
		{
			get
			{
				return -1;
			}
		}

		public int LastAssignedPosition
		{
			get
			{
				return this.FindAssignedEditPositionFrom(this._testString.Length - 1, false);
			}
		}

		public int Length
		{
			get
			{
				return this._testString.Length;
			}
		}

		public string Mask { get; }

		public bool MaskCompleted
		{
			get
			{
				return this._requiredCharCount == this._requiredEditChars;
			}
		}

		public bool MaskFull
		{
			get
			{
				return this.AssignedEditPositionCount == this.EditPositionCount;
			}
		}

		public char PasswordChar
		{
			get
			{
				return this._passwordChar;
			}
			set
			{
				if (value == this._promptChar)
				{
					throw new InvalidOperationException("The PasswordChar and PromptChar values cannot be the same.");
				}
				if (!MaskedTextProvider.IsValidPasswordChar(value) && value != '\0')
				{
					throw new ArgumentException("The specified character value is not allowed for this property.");
				}
				if (value != this._passwordChar)
				{
					this._passwordChar = value;
				}
			}
		}

		public char PromptChar
		{
			get
			{
				return this._promptChar;
			}
			set
			{
				if (value == this._passwordChar)
				{
					throw new InvalidOperationException("The PasswordChar and PromptChar values cannot be the same.");
				}
				if (!MaskedTextProvider.IsPrintableChar(value))
				{
					throw new ArgumentException("The specified character value is not allowed for this property.");
				}
				if (value != this._promptChar)
				{
					this._promptChar = value;
					for (int i = 0; i < this._testString.Length; i++)
					{
						MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[i];
						if (this.IsEditPosition(i) && !charDescriptor.IsAssigned)
						{
							this._testString[i] = this._promptChar;
						}
					}
				}
			}
		}

		public bool ResetOnPrompt
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_RESET_ON_PROMPT];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_RESET_ON_PROMPT] = value;
			}
		}

		public bool ResetOnSpace
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_SKIP_SPACE];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_SKIP_SPACE] = value;
			}
		}

		public bool SkipLiterals
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_RESET_ON_LITERALS];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_RESET_ON_LITERALS] = value;
			}
		}

		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= this._testString.Length)
				{
					throw new IndexOutOfRangeException(index.ToString(CultureInfo.CurrentCulture));
				}
				return this._testString[index];
			}
		}

		public bool Add(char input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Add(input, out num, out maskedTextResultHint);
		}

		public bool Add(char input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			int lastAssignedPosition = this.LastAssignedPosition;
			if (lastAssignedPosition == this._testString.Length - 1)
			{
				testPosition = this._testString.Length;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return false;
			}
			testPosition = lastAssignedPosition + 1;
			testPosition = this.FindEditPositionFrom(testPosition, true);
			if (testPosition == -1)
			{
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = this._testString.Length;
				return false;
			}
			return this.TestSetChar(input, testPosition, out resultHint);
		}

		public bool Add(string input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Add(input, out num, out maskedTextResultHint);
		}

		public bool Add(string input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			testPosition = this.LastAssignedPosition + 1;
			if (input.Length == 0)
			{
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			return this.TestSetString(input, testPosition, out testPosition, out resultHint);
		}

		public void Clear()
		{
			MaskedTextResultHint maskedTextResultHint;
			this.Clear(out maskedTextResultHint);
		}

		public void Clear(out MaskedTextResultHint resultHint)
		{
			if (this.AssignedEditPositionCount == 0)
			{
				resultHint = MaskedTextResultHint.NoEffect;
				return;
			}
			resultHint = MaskedTextResultHint.Success;
			for (int i = 0; i < this._testString.Length; i++)
			{
				this.ResetChar(i);
			}
		}

		public int FindAssignedEditPositionFrom(int position, bool direction)
		{
			if (this.AssignedEditPositionCount == 0)
			{
				return -1;
			}
			int num;
			int num2;
			if (direction)
			{
				num = position;
				num2 = this._testString.Length - 1;
			}
			else
			{
				num = 0;
				num2 = position;
			}
			return this.FindAssignedEditPositionInRange(num, num2, direction);
		}

		public int FindAssignedEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			if (this.AssignedEditPositionCount == 0)
			{
				return -1;
			}
			return this.FindEditPositionInRange(startPosition, endPosition, direction, 2);
		}

		public int FindEditPositionFrom(int position, bool direction)
		{
			int num;
			int num2;
			if (direction)
			{
				num = position;
				num2 = this._testString.Length - 1;
			}
			else
			{
				num = 0;
				num2 = position;
			}
			return this.FindEditPositionInRange(num, num2, direction);
		}

		public int FindEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			MaskedTextProvider.CharType charType = MaskedTextProvider.CharType.EditOptional | MaskedTextProvider.CharType.EditRequired;
			return this.FindPositionInRange(startPosition, endPosition, direction, charType);
		}

		private int FindEditPositionInRange(int startPosition, int endPosition, bool direction, byte assignedStatus)
		{
			int num;
			for (;;)
			{
				num = this.FindEditPositionInRange(startPosition, endPosition, direction);
				if (num == -1)
				{
					return -1;
				}
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[num];
				if (assignedStatus != 1)
				{
					if (assignedStatus != 2)
					{
						break;
					}
					if (charDescriptor.IsAssigned)
					{
						return num;
					}
				}
				else if (!charDescriptor.IsAssigned)
				{
					return num;
				}
				if (direction)
				{
					startPosition++;
				}
				else
				{
					endPosition--;
				}
				if (startPosition > endPosition)
				{
					return -1;
				}
			}
			return num;
		}

		public int FindNonEditPositionFrom(int position, bool direction)
		{
			int num;
			int num2;
			if (direction)
			{
				num = position;
				num2 = this._testString.Length - 1;
			}
			else
			{
				num = 0;
				num2 = position;
			}
			return this.FindNonEditPositionInRange(num, num2, direction);
		}

		public int FindNonEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			MaskedTextProvider.CharType charType = MaskedTextProvider.CharType.Separator | MaskedTextProvider.CharType.Literal;
			return this.FindPositionInRange(startPosition, endPosition, direction, charType);
		}

		private int FindPositionInRange(int startPosition, int endPosition, bool direction, MaskedTextProvider.CharType charTypeFlags)
		{
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (endPosition >= this._testString.Length)
			{
				endPosition = this._testString.Length - 1;
			}
			if (startPosition > endPosition)
			{
				return -1;
			}
			while (startPosition <= endPosition)
			{
				int num;
				if (!direction)
				{
					endPosition = (num = endPosition) - 1;
				}
				else
				{
					startPosition = (num = startPosition) + 1;
				}
				int num2 = num;
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[num2];
				if ((charDescriptor.CharType & charTypeFlags) == charDescriptor.CharType)
				{
					return num2;
				}
			}
			return -1;
		}

		public int FindUnassignedEditPositionFrom(int position, bool direction)
		{
			int num;
			int num2;
			if (direction)
			{
				num = position;
				num2 = this._testString.Length - 1;
			}
			else
			{
				num = 0;
				num2 = position;
			}
			return this.FindEditPositionInRange(num, num2, direction, 1);
		}

		public int FindUnassignedEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			for (;;)
			{
				int num = this.FindEditPositionInRange(startPosition, endPosition, direction, 0);
				if (num == -1)
				{
					break;
				}
				if (!this._stringDescriptor[num].IsAssigned)
				{
					return num;
				}
				if (direction)
				{
					startPosition++;
				}
				else
				{
					endPosition--;
				}
			}
			return -1;
		}

		public static bool GetOperationResultFromHint(MaskedTextResultHint hint)
		{
			return hint > MaskedTextResultHint.Unknown;
		}

		public bool InsertAt(char input, int position)
		{
			return position >= 0 && position < this._testString.Length && this.InsertAt(input.ToString(), position);
		}

		public bool InsertAt(char input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			return this.InsertAt(input.ToString(), position, out testPosition, out resultHint);
		}

		public bool InsertAt(string input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.InsertAt(input, position, out num, out maskedTextResultHint);
		}

		public bool InsertAt(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (position < 0 || position >= this._testString.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			return this.InsertAtInt(input, position, out testPosition, out resultHint, false);
		}

		private bool InsertAtInt(string input, int position, out int testPosition, out MaskedTextResultHint resultHint, bool testOnly)
		{
			if (input.Length == 0)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			if (!this.TestString(input, position, out testPosition, out resultHint))
			{
				return false;
			}
			int i = this.FindEditPositionFrom(position, true);
			bool flag = this.FindAssignedEditPositionInRange(i, testPosition, true) != -1;
			int lastAssignedPosition = this.LastAssignedPosition;
			if (flag && testPosition == this._testString.Length - 1)
			{
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = this._testString.Length;
				return false;
			}
			int num = this.FindEditPositionFrom(testPosition + 1, true);
			if (flag)
			{
				MaskedTextResultHint maskedTextResultHint = MaskedTextResultHint.Unknown;
				while (num != -1)
				{
					if (this._stringDescriptor[i].IsAssigned && !this.TestChar(this._testString[i], num, out maskedTextResultHint))
					{
						resultHint = maskedTextResultHint;
						testPosition = num;
						return false;
					}
					if (i != lastAssignedPosition)
					{
						i = this.FindEditPositionFrom(i + 1, true);
						num = this.FindEditPositionFrom(num + 1, true);
					}
					else
					{
						if (maskedTextResultHint > resultHint)
						{
							resultHint = maskedTextResultHint;
							goto IL_00EF;
						}
						goto IL_00EF;
					}
				}
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = this._testString.Length;
				return false;
			}
			IL_00EF:
			if (testOnly)
			{
				return true;
			}
			if (flag)
			{
				while (i >= position)
				{
					if (this._stringDescriptor[i].IsAssigned)
					{
						this.SetChar(this._testString[i], num);
					}
					else
					{
						this.ResetChar(num);
					}
					num = this.FindEditPositionFrom(num - 1, false);
					i = this.FindEditPositionFrom(i - 1, false);
				}
			}
			this.SetString(input, position);
			return true;
		}

		private static bool IsAscii(char c)
		{
			return c >= '!' && c <= '~';
		}

		private static bool IsAciiAlphanumeric(char c)
		{
			return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		private static bool IsAlphanumeric(char c)
		{
			return char.IsLetter(c) || char.IsDigit(c);
		}

		private static bool IsAsciiLetter(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		public bool IsAvailablePosition(int position)
		{
			if (position < 0 || position >= this._testString.Length)
			{
				return false;
			}
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[position];
			return MaskedTextProvider.IsEditPosition(charDescriptor) && !charDescriptor.IsAssigned;
		}

		public bool IsEditPosition(int position)
		{
			return position >= 0 && position < this._testString.Length && MaskedTextProvider.IsEditPosition(this._stringDescriptor[position]);
		}

		private static bool IsEditPosition(MaskedTextProvider.CharDescriptor charDescriptor)
		{
			return charDescriptor.CharType == MaskedTextProvider.CharType.EditRequired || charDescriptor.CharType == MaskedTextProvider.CharType.EditOptional;
		}

		private static bool IsLiteralPosition(MaskedTextProvider.CharDescriptor charDescriptor)
		{
			return charDescriptor.CharType == MaskedTextProvider.CharType.Literal || charDescriptor.CharType == MaskedTextProvider.CharType.Separator;
		}

		private static bool IsPrintableChar(char c)
		{
			return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' ';
		}

		public static bool IsValidInputChar(char c)
		{
			return MaskedTextProvider.IsPrintableChar(c);
		}

		public static bool IsValidMaskChar(char c)
		{
			return MaskedTextProvider.IsPrintableChar(c);
		}

		public static bool IsValidPasswordChar(char c)
		{
			return MaskedTextProvider.IsPrintableChar(c) || c == '\0';
		}

		public bool Remove()
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Remove(out num, out maskedTextResultHint);
		}

		public bool Remove(out int testPosition, out MaskedTextResultHint resultHint)
		{
			int lastAssignedPosition = this.LastAssignedPosition;
			if (lastAssignedPosition == -1)
			{
				testPosition = 0;
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			this.ResetChar(lastAssignedPosition);
			testPosition = lastAssignedPosition;
			resultHint = MaskedTextResultHint.Success;
			return true;
		}

		public bool RemoveAt(int position)
		{
			return this.RemoveAt(position, position);
		}

		public bool RemoveAt(int startPosition, int endPosition)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.RemoveAt(startPosition, endPosition, out num, out maskedTextResultHint);
		}

		public bool RemoveAt(int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (endPosition >= this._testString.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0 || startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			return this.RemoveAtInt(startPosition, endPosition, out testPosition, out resultHint, false);
		}

		private bool RemoveAtInt(int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint, bool testOnly)
		{
			int lastAssignedPosition = this.LastAssignedPosition;
			int num = this.FindEditPositionInRange(startPosition, endPosition, true);
			resultHint = MaskedTextResultHint.NoEffect;
			if (num == -1 || num > lastAssignedPosition)
			{
				testPosition = startPosition;
				return true;
			}
			testPosition = startPosition;
			bool flag = endPosition < lastAssignedPosition;
			if (this.FindAssignedEditPositionInRange(startPosition, endPosition, true) != -1)
			{
				resultHint = MaskedTextResultHint.Success;
			}
			if (flag)
			{
				int num2 = this.FindEditPositionFrom(endPosition + 1, true);
				int num3 = num2;
				startPosition = num;
				MaskedTextResultHint maskedTextResultHint;
				for (;;)
				{
					char c = this._testString[num2];
					MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[num2];
					if ((c != this.PromptChar || charDescriptor.IsAssigned) && !this.TestChar(c, num, out maskedTextResultHint))
					{
						break;
					}
					if (num2 == lastAssignedPosition)
					{
						goto IL_00B0;
					}
					num2 = this.FindEditPositionFrom(num2 + 1, true);
					num = this.FindEditPositionFrom(num + 1, true);
				}
				resultHint = maskedTextResultHint;
				testPosition = num;
				return false;
				IL_00B0:
				if (MaskedTextResultHint.SideEffect > resultHint)
				{
					resultHint = MaskedTextResultHint.SideEffect;
				}
				if (testOnly)
				{
					return true;
				}
				num2 = num3;
				num = startPosition;
				for (;;)
				{
					char c2 = this._testString[num2];
					MaskedTextProvider.CharDescriptor charDescriptor2 = this._stringDescriptor[num2];
					if (c2 == this.PromptChar && !charDescriptor2.IsAssigned)
					{
						this.ResetChar(num);
					}
					else
					{
						this.SetChar(c2, num);
						this.ResetChar(num2);
					}
					if (num2 == lastAssignedPosition)
					{
						break;
					}
					num2 = this.FindEditPositionFrom(num2 + 1, true);
					num = this.FindEditPositionFrom(num + 1, true);
				}
				startPosition = num + 1;
			}
			if (startPosition <= endPosition)
			{
				this.ResetString(startPosition, endPosition);
			}
			return true;
		}

		public bool Replace(char input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Replace(input, position, out num, out maskedTextResultHint);
		}

		public bool Replace(char input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (position < 0 || position >= this._testString.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			testPosition = position;
			if (!this.TestEscapeChar(input, testPosition))
			{
				testPosition = this.FindEditPositionFrom(testPosition, true);
			}
			if (testPosition == -1)
			{
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = position;
				return false;
			}
			return this.TestSetChar(input, testPosition, out resultHint);
		}

		public bool Replace(char input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (endPosition >= this._testString.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0 || startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition == endPosition)
			{
				testPosition = startPosition;
				return this.TestSetChar(input, startPosition, out resultHint);
			}
			return this.Replace(input.ToString(), startPosition, endPosition, out testPosition, out resultHint);
		}

		public bool Replace(string input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Replace(input, position, out num, out maskedTextResultHint);
		}

		public bool Replace(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (position < 0 || position >= this._testString.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (input.Length == 0)
			{
				return this.RemoveAt(position, position, out testPosition, out resultHint);
			}
			return this.TestSetString(input, position, out testPosition, out resultHint);
		}

		public bool Replace(string input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (endPosition >= this._testString.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0 || startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (input.Length == 0)
			{
				return this.RemoveAt(startPosition, endPosition, out testPosition, out resultHint);
			}
			if (!this.TestString(input, startPosition, out testPosition, out resultHint))
			{
				return false;
			}
			if (this.AssignedEditPositionCount > 0)
			{
				if (testPosition < endPosition)
				{
					int num;
					MaskedTextResultHint maskedTextResultHint;
					if (!this.RemoveAtInt(testPosition + 1, endPosition, out num, out maskedTextResultHint, false))
					{
						testPosition = num;
						resultHint = maskedTextResultHint;
						return false;
					}
					if (maskedTextResultHint == MaskedTextResultHint.Success && resultHint != maskedTextResultHint)
					{
						resultHint = MaskedTextResultHint.SideEffect;
					}
				}
				else if (testPosition > endPosition)
				{
					int lastAssignedPosition = this.LastAssignedPosition;
					int i = testPosition + 1;
					int num2 = endPosition + 1;
					MaskedTextResultHint maskedTextResultHint;
					for (;;)
					{
						num2 = this.FindEditPositionFrom(num2, true);
						i = this.FindEditPositionFrom(i, true);
						if (i == -1)
						{
							goto Block_12;
						}
						if (!this.TestChar(this._testString[num2], i, out maskedTextResultHint))
						{
							goto Block_13;
						}
						if (maskedTextResultHint == MaskedTextResultHint.Success && resultHint != maskedTextResultHint)
						{
							resultHint = MaskedTextResultHint.Success;
						}
						if (num2 == lastAssignedPosition)
						{
							break;
						}
						num2++;
						i++;
					}
					while (i > testPosition)
					{
						this.SetChar(this._testString[num2], i);
						num2 = this.FindEditPositionFrom(num2 - 1, false);
						i = this.FindEditPositionFrom(i - 1, false);
					}
					goto IL_0162;
					Block_12:
					testPosition = this._testString.Length;
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					return false;
					Block_13:
					testPosition = i;
					resultHint = maskedTextResultHint;
					return false;
				}
			}
			IL_0162:
			this.SetString(input, startPosition);
			return true;
		}

		private void ResetChar(int testPosition)
		{
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[testPosition];
			if (this.IsEditPosition(testPosition) && charDescriptor.IsAssigned)
			{
				charDescriptor.IsAssigned = false;
				this._testString[testPosition] = this._promptChar;
				int assignedEditPositionCount = this.AssignedEditPositionCount;
				this.AssignedEditPositionCount = assignedEditPositionCount - 1;
				if (charDescriptor.CharType == MaskedTextProvider.CharType.EditRequired)
				{
					this._requiredCharCount--;
				}
			}
		}

		private void ResetString(int startPosition, int endPosition)
		{
			startPosition = this.FindAssignedEditPositionFrom(startPosition, true);
			if (startPosition != -1)
			{
				endPosition = this.FindAssignedEditPositionFrom(endPosition, false);
				while (startPosition <= endPosition)
				{
					startPosition = this.FindAssignedEditPositionFrom(startPosition, true);
					this.ResetChar(startPosition);
					startPosition++;
				}
			}
		}

		public bool Set(string input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Set(input, out num, out maskedTextResultHint);
		}

		public bool Set(string input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			resultHint = MaskedTextResultHint.Unknown;
			testPosition = 0;
			if (input.Length == 0)
			{
				this.Clear(out resultHint);
				return true;
			}
			if (!this.TestSetString(input, testPosition, out testPosition, out resultHint))
			{
				return false;
			}
			int num = this.FindAssignedEditPositionFrom(testPosition + 1, true);
			if (num != -1)
			{
				this.ResetString(num, this._testString.Length - 1);
			}
			return true;
		}

		private void SetChar(char input, int position)
		{
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[position];
			this.SetChar(input, position, charDescriptor);
		}

		private void SetChar(char input, int position, MaskedTextProvider.CharDescriptor charDescriptor)
		{
			MaskedTextProvider.CharDescriptor charDescriptor2 = this._stringDescriptor[position];
			if (this.TestEscapeChar(input, position, charDescriptor))
			{
				this.ResetChar(position);
				return;
			}
			if (char.IsLetter(input))
			{
				if (char.IsUpper(input))
				{
					if (charDescriptor.CaseConversion == MaskedTextProvider.CaseConversion.ToLower)
					{
						input = this.Culture.TextInfo.ToLower(input);
					}
				}
				else if (charDescriptor.CaseConversion == MaskedTextProvider.CaseConversion.ToUpper)
				{
					input = this.Culture.TextInfo.ToUpper(input);
				}
			}
			this._testString[position] = input;
			if (!charDescriptor.IsAssigned)
			{
				charDescriptor.IsAssigned = true;
				int assignedEditPositionCount = this.AssignedEditPositionCount;
				this.AssignedEditPositionCount = assignedEditPositionCount + 1;
				if (charDescriptor.CharType == MaskedTextProvider.CharType.EditRequired)
				{
					this._requiredCharCount++;
				}
			}
		}

		private void SetString(string input, int testPosition)
		{
			foreach (char c in input)
			{
				if (!this.TestEscapeChar(c, testPosition))
				{
					testPosition = this.FindEditPositionFrom(testPosition, true);
				}
				this.SetChar(c, testPosition);
				testPosition++;
			}
		}

		private bool TestChar(char input, int position, out MaskedTextResultHint resultHint)
		{
			if (!MaskedTextProvider.IsPrintableChar(input))
			{
				resultHint = MaskedTextResultHint.InvalidInput;
				return false;
			}
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[position];
			if (MaskedTextProvider.IsLiteralPosition(charDescriptor))
			{
				if (this.SkipLiterals && input == this._testString[position])
				{
					resultHint = MaskedTextResultHint.CharacterEscaped;
					return true;
				}
				resultHint = MaskedTextResultHint.NonEditPosition;
				return false;
			}
			else
			{
				if (input == this._promptChar)
				{
					if (this.ResetOnPrompt)
					{
						if (MaskedTextProvider.IsEditPosition(charDescriptor) && charDescriptor.IsAssigned)
						{
							resultHint = MaskedTextResultHint.SideEffect;
						}
						else
						{
							resultHint = MaskedTextResultHint.CharacterEscaped;
						}
						return true;
					}
					if (!this.AllowPromptAsInput)
					{
						resultHint = MaskedTextResultHint.PromptCharNotAllowed;
						return false;
					}
				}
				if (input == ' ' && this.ResetOnSpace)
				{
					if (MaskedTextProvider.IsEditPosition(charDescriptor) && charDescriptor.IsAssigned)
					{
						resultHint = MaskedTextResultHint.SideEffect;
					}
					else
					{
						resultHint = MaskedTextResultHint.CharacterEscaped;
					}
					return true;
				}
				char c = this.Mask[charDescriptor.MaskPosition];
				if (c <= '0')
				{
					if (c != '#')
					{
						if (c != '&')
						{
							if (c == '0')
							{
								if (!char.IsDigit(input))
								{
									resultHint = MaskedTextResultHint.DigitExpected;
									return false;
								}
							}
						}
						else if (!MaskedTextProvider.IsAscii(input) && this.AsciiOnly)
						{
							resultHint = MaskedTextResultHint.AsciiCharacterExpected;
							return false;
						}
					}
					else if (!char.IsDigit(input) && input != '-' && input != '+' && input != ' ')
					{
						resultHint = MaskedTextResultHint.DigitExpected;
						return false;
					}
				}
				else if (c <= 'C')
				{
					if (c != '9')
					{
						switch (c)
						{
						case '?':
							if (!char.IsLetter(input) && input != ' ')
							{
								resultHint = MaskedTextResultHint.LetterExpected;
								return false;
							}
							if (!MaskedTextProvider.IsAsciiLetter(input) && this.AsciiOnly)
							{
								resultHint = MaskedTextResultHint.AsciiCharacterExpected;
								return false;
							}
							break;
						case 'A':
							if (!MaskedTextProvider.IsAlphanumeric(input))
							{
								resultHint = MaskedTextResultHint.AlphanumericCharacterExpected;
								return false;
							}
							if (!MaskedTextProvider.IsAciiAlphanumeric(input) && this.AsciiOnly)
							{
								resultHint = MaskedTextResultHint.AsciiCharacterExpected;
								return false;
							}
							break;
						case 'C':
							if (!MaskedTextProvider.IsAscii(input) && this.AsciiOnly && input != ' ')
							{
								resultHint = MaskedTextResultHint.AsciiCharacterExpected;
								return false;
							}
							break;
						}
					}
					else if (!char.IsDigit(input) && input != ' ')
					{
						resultHint = MaskedTextResultHint.DigitExpected;
						return false;
					}
				}
				else if (c != 'L')
				{
					if (c == 'a')
					{
						if (!MaskedTextProvider.IsAlphanumeric(input) && input != ' ')
						{
							resultHint = MaskedTextResultHint.AlphanumericCharacterExpected;
							return false;
						}
						if (!MaskedTextProvider.IsAciiAlphanumeric(input) && this.AsciiOnly)
						{
							resultHint = MaskedTextResultHint.AsciiCharacterExpected;
							return false;
						}
					}
				}
				else
				{
					if (!char.IsLetter(input))
					{
						resultHint = MaskedTextResultHint.LetterExpected;
						return false;
					}
					if (!MaskedTextProvider.IsAsciiLetter(input) && this.AsciiOnly)
					{
						resultHint = MaskedTextResultHint.AsciiCharacterExpected;
						return false;
					}
				}
				if (input == this._testString[position] && charDescriptor.IsAssigned)
				{
					resultHint = MaskedTextResultHint.NoEffect;
				}
				else
				{
					resultHint = MaskedTextResultHint.Success;
				}
				return true;
			}
		}

		private bool TestEscapeChar(char input, int position)
		{
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[position];
			return this.TestEscapeChar(input, position, charDescriptor);
		}

		private bool TestEscapeChar(char input, int position, MaskedTextProvider.CharDescriptor charDex)
		{
			if (MaskedTextProvider.IsLiteralPosition(charDex))
			{
				return this.SkipLiterals && input == this._testString[position];
			}
			return (this.ResetOnPrompt && input == this._promptChar) || (this.ResetOnSpace && input == ' ');
		}

		private bool TestSetChar(char input, int position, out MaskedTextResultHint resultHint)
		{
			if (this.TestChar(input, position, out resultHint))
			{
				if (resultHint == MaskedTextResultHint.Success || resultHint == MaskedTextResultHint.SideEffect)
				{
					this.SetChar(input, position);
				}
				return true;
			}
			return false;
		}

		private bool TestSetString(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (this.TestString(input, position, out testPosition, out resultHint))
			{
				this.SetString(input, position);
				return true;
			}
			return false;
		}

		private bool TestString(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			resultHint = MaskedTextResultHint.Unknown;
			testPosition = position;
			if (input.Length == 0)
			{
				return true;
			}
			MaskedTextResultHint maskedTextResultHint = resultHint;
			foreach (char c in input)
			{
				if (testPosition >= this._testString.Length)
				{
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					return false;
				}
				if (!this.TestEscapeChar(c, testPosition))
				{
					testPosition = this.FindEditPositionFrom(testPosition, true);
					if (testPosition == -1)
					{
						testPosition = this._testString.Length;
						resultHint = MaskedTextResultHint.UnavailableEditPosition;
						return false;
					}
				}
				if (!this.TestChar(c, testPosition, out maskedTextResultHint))
				{
					resultHint = maskedTextResultHint;
					return false;
				}
				if (maskedTextResultHint > resultHint)
				{
					resultHint = maskedTextResultHint;
				}
				testPosition++;
			}
			testPosition--;
			return true;
		}

		public string ToDisplayString()
		{
			if (!this.IsPassword || this.AssignedEditPositionCount == 0)
			{
				return this._testString.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder(this._testString.Length);
			for (int i = 0; i < this._testString.Length; i++)
			{
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[i];
				stringBuilder.Append((MaskedTextProvider.IsEditPosition(charDescriptor) && charDescriptor.IsAssigned) ? this._passwordChar : this._testString[i]);
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return this.ToString(true, this.IncludePrompt, this.IncludeLiterals, 0, this._testString.Length);
		}

		public string ToString(bool ignorePasswordChar)
		{
			return this.ToString(ignorePasswordChar, this.IncludePrompt, this.IncludeLiterals, 0, this._testString.Length);
		}

		public string ToString(int startPosition, int length)
		{
			return this.ToString(true, this.IncludePrompt, this.IncludeLiterals, startPosition, length);
		}

		public string ToString(bool ignorePasswordChar, int startPosition, int length)
		{
			return this.ToString(ignorePasswordChar, this.IncludePrompt, this.IncludeLiterals, startPosition, length);
		}

		public string ToString(bool includePrompt, bool includeLiterals)
		{
			return this.ToString(true, includePrompt, includeLiterals, 0, this._testString.Length);
		}

		public string ToString(bool includePrompt, bool includeLiterals, int startPosition, int length)
		{
			return this.ToString(true, includePrompt, includeLiterals, startPosition, length);
		}

		public string ToString(bool ignorePasswordChar, bool includePrompt, bool includeLiterals, int startPosition, int length)
		{
			if (length <= 0)
			{
				return string.Empty;
			}
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (startPosition >= this._testString.Length)
			{
				return string.Empty;
			}
			int num = this._testString.Length - startPosition;
			if (length > num)
			{
				length = num;
			}
			if ((!this.IsPassword || ignorePasswordChar) && (includePrompt && includeLiterals))
			{
				return this._testString.ToString(startPosition, length);
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = startPosition + length - 1;
			if (!includePrompt)
			{
				int num3 = (includeLiterals ? this.FindNonEditPositionInRange(startPosition, num2, false) : MaskedTextProvider.InvalidIndex);
				int num4 = this.FindAssignedEditPositionInRange((num3 == MaskedTextProvider.InvalidIndex) ? startPosition : num3, num2, false);
				num2 = ((num4 != MaskedTextProvider.InvalidIndex) ? num4 : num3);
				if (num2 == MaskedTextProvider.InvalidIndex)
				{
					return string.Empty;
				}
			}
			int i = startPosition;
			while (i <= num2)
			{
				char c = this._testString[i];
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[i];
				MaskedTextProvider.CharType charType = charDescriptor.CharType;
				if (charType - MaskedTextProvider.CharType.EditOptional > 1)
				{
					if (charType != MaskedTextProvider.CharType.Separator && charType != MaskedTextProvider.CharType.Literal)
					{
						goto IL_012F;
					}
					if (includeLiterals)
					{
						goto IL_012F;
					}
				}
				else if (charDescriptor.IsAssigned)
				{
					if (!this.IsPassword || ignorePasswordChar)
					{
						goto IL_012F;
					}
					stringBuilder.Append(this._passwordChar);
				}
				else
				{
					if (includePrompt)
					{
						goto IL_012F;
					}
					stringBuilder.Append(' ');
				}
				IL_0138:
				i++;
				continue;
				IL_012F:
				stringBuilder.Append(c);
				goto IL_0138;
			}
			return stringBuilder.ToString();
		}

		public bool VerifyChar(char input, int position, out MaskedTextResultHint hint)
		{
			hint = MaskedTextResultHint.NoEffect;
			if (position < 0 || position >= this._testString.Length)
			{
				hint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			return this.TestChar(input, position, out hint);
		}

		public bool VerifyEscapeChar(char input, int position)
		{
			return position >= 0 && position < this._testString.Length && this.TestEscapeChar(input, position);
		}

		public bool VerifyString(string input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.VerifyString(input, out num, out maskedTextResultHint);
		}

		public bool VerifyString(string input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			testPosition = 0;
			if (input == null || input.Length == 0)
			{
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			return this.TestString(input, 0, out testPosition, out resultHint);
		}

		private const char SPACE_CHAR = ' ';

		private const char DEFAULT_PROMPT_CHAR = '_';

		private const char NULL_PASSWORD_CHAR = '\0';

		private const bool DEFAULT_ALLOW_PROMPT = true;

		private const int INVALID_INDEX = -1;

		private const byte EDIT_ANY = 0;

		private const byte EDIT_UNASSIGNED = 1;

		private const byte EDIT_ASSIGNED = 2;

		private const bool FORWARD = true;

		private const bool BACKWARD = false;

		private static int s_ASCII_ONLY = BitVector32.CreateMask();

		private static int s_ALLOW_PROMPT_AS_INPUT = BitVector32.CreateMask(MaskedTextProvider.s_ASCII_ONLY);

		private static int s_INCLUDE_PROMPT = BitVector32.CreateMask(MaskedTextProvider.s_ALLOW_PROMPT_AS_INPUT);

		private static int s_INCLUDE_LITERALS = BitVector32.CreateMask(MaskedTextProvider.s_INCLUDE_PROMPT);

		private static int s_RESET_ON_PROMPT = BitVector32.CreateMask(MaskedTextProvider.s_INCLUDE_LITERALS);

		private static int s_RESET_ON_LITERALS = BitVector32.CreateMask(MaskedTextProvider.s_RESET_ON_PROMPT);

		private static int s_SKIP_SPACE = BitVector32.CreateMask(MaskedTextProvider.s_RESET_ON_LITERALS);

		private static Type s_maskTextProviderType = typeof(MaskedTextProvider);

		private BitVector32 _flagState;

		private StringBuilder _testString;

		private int _requiredCharCount;

		private int _requiredEditChars;

		private int _optionalEditChars;

		private char _passwordChar;

		private char _promptChar;

		private List<MaskedTextProvider.CharDescriptor> _stringDescriptor;

		private enum CaseConversion
		{
			None,
			ToLower,
			ToUpper
		}

		[Flags]
		private enum CharType
		{
			EditOptional = 1,
			EditRequired = 2,
			Separator = 4,
			Literal = 8,
			Modifier = 16
		}

		private class CharDescriptor
		{
			public CharDescriptor(int maskPos, MaskedTextProvider.CharType charType)
			{
				this.MaskPosition = maskPos;
				this.CharType = charType;
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "MaskPosition[{0}] <CaseConversion.{1}><CharType.{2}><IsAssigned: {3}", new object[] { this.MaskPosition, this.CaseConversion, this.CharType, this.IsAssigned });
			}

			public int MaskPosition;

			public MaskedTextProvider.CaseConversion CaseConversion;

			public MaskedTextProvider.CharType CharType;

			public bool IsAssigned;
		}
	}
}
