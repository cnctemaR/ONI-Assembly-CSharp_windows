using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace System.ComponentModel
{
	public class MaskedTextProvider : ICloneable
	{
		public MaskedTextProvider(string mask)
			: this(mask, null, true, MaskedTextProvider.default_prompt_char, MaskedTextProvider.default_password_char, false)
		{
		}

		public MaskedTextProvider(string mask, bool restrictToAscii)
			: this(mask, null, true, MaskedTextProvider.default_prompt_char, MaskedTextProvider.default_password_char, restrictToAscii)
		{
		}

		public MaskedTextProvider(string mask, CultureInfo culture)
			: this(mask, culture, true, MaskedTextProvider.default_prompt_char, MaskedTextProvider.default_password_char, false)
		{
		}

		public MaskedTextProvider(string mask, char passwordChar, bool allowPromptAsInput)
			: this(mask, null, allowPromptAsInput, MaskedTextProvider.default_prompt_char, passwordChar, false)
		{
		}

		public MaskedTextProvider(string mask, CultureInfo culture, bool restrictToAscii)
			: this(mask, culture, true, MaskedTextProvider.default_prompt_char, MaskedTextProvider.default_password_char, restrictToAscii)
		{
		}

		public MaskedTextProvider(string mask, CultureInfo culture, char passwordChar, bool allowPromptAsInput)
			: this(mask, culture, allowPromptAsInput, MaskedTextProvider.default_prompt_char, passwordChar, false)
		{
		}

		public MaskedTextProvider(string mask, CultureInfo culture, bool allowPromptAsInput, char promptChar, char passwordChar, bool restrictToAscii)
		{
			this.SetMask(mask);
			if (culture == null)
			{
				this.culture = Thread.CurrentThread.CurrentCulture;
			}
			else
			{
				this.culture = culture;
			}
			this.allow_prompt_as_input = allowPromptAsInput;
			this.PromptChar = promptChar;
			this.PasswordChar = passwordChar;
			this.ascii_only = restrictToAscii;
			this.include_literals = true;
			this.reset_on_prompt = true;
			this.reset_on_space = true;
			this.skip_literals = true;
		}

		private void SetMask(string mask)
		{
			if (mask == null || mask == string.Empty)
			{
				throw new ArgumentException("The Mask value cannot be null or empty.\r\nParameter name: mask");
			}
			this.mask = mask;
			List<MaskedTextProvider.EditPosition> list = new List<MaskedTextProvider.EditPosition>(mask.Length);
			MaskedTextProvider.EditState editState = MaskedTextProvider.EditState.None;
			bool flag = false;
			for (int i = 0; i < mask.Length; i++)
			{
				if (flag)
				{
					list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.Literal, editState, mask[i]));
					flag = false;
				}
				else
				{
					char c = mask[i];
					switch (c)
					{
					case '#':
						list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.DigitOrSpaceOptional_Blank, editState, mask[i]));
						break;
					case '$':
						list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.CurrencySymbol, editState, mask[i]));
						break;
					default:
						switch (c)
						{
						case '9':
							list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.DigitOrSpaceOptional, editState, mask[i]));
							break;
						case ':':
							list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.TimeSeparator, editState, mask[i]));
							break;
						default:
							if (c != 'L')
							{
								if (c != '\\')
								{
									if (c != 'a')
									{
										if (c != '|')
										{
											list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.Literal, editState, mask[i]));
										}
										else
										{
											editState = MaskedTextProvider.EditState.None;
										}
									}
									else
									{
										list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.AlphanumericOptional, editState, mask[i]));
									}
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.LetterRequired, editState, mask[i]));
							}
							break;
						case '<':
							editState = MaskedTextProvider.EditState.LowerCase;
							break;
						case '>':
							editState = MaskedTextProvider.EditState.UpperCase;
							break;
						case '?':
							list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.LetterOptional, editState, mask[i]));
							break;
						case 'A':
							list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.AlphanumericRequired, editState, mask[i]));
							break;
						case 'C':
							list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.CharacterOptional, editState, mask[i]));
							break;
						}
						break;
					case '&':
						list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.CharacterRequired, editState, mask[i]));
						break;
					case ',':
						list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.ThousandsPlaceholder, editState, mask[i]));
						break;
					case '.':
						list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.DecimalPlaceholder, editState, mask[i]));
						break;
					case '/':
						list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.DateSeparator, editState, mask[i]));
						break;
					case '0':
						list.Add(new MaskedTextProvider.EditPosition(this, MaskedTextProvider.EditType.DigitRequired, editState, mask[i]));
						break;
					}
				}
			}
			this.edit_positions = list.ToArray();
		}

		private MaskedTextProvider.EditPosition[] ClonePositions()
		{
			MaskedTextProvider.EditPosition[] array = new MaskedTextProvider.EditPosition[this.edit_positions.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.edit_positions[i].Clone();
			}
			return array;
		}

		private bool AddInternal(string str_input, out int testPosition, out MaskedTextResultHint resultHint, bool only_test)
		{
			MaskedTextProvider.EditPosition[] array;
			if (only_test)
			{
				array = this.ClonePositions();
			}
			else
			{
				array = this.edit_positions;
			}
			if (str_input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (str_input.Length == 0)
			{
				resultHint = MaskedTextResultHint.NoEffect;
				testPosition = this.LastAssignedPosition + 1;
				return true;
			}
			resultHint = MaskedTextResultHint.Unknown;
			testPosition = 0;
			int num = this.LastAssignedPosition;
			MaskedTextResultHint maskedTextResultHint = MaskedTextResultHint.Unknown;
			if (num >= array.Length)
			{
				testPosition = num;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return false;
			}
			foreach (char c in str_input)
			{
				num++;
				testPosition = num;
				if (maskedTextResultHint > resultHint)
				{
					resultHint = maskedTextResultHint;
				}
				if (this.VerifyEscapeChar(c, num))
				{
					maskedTextResultHint = MaskedTextResultHint.CharacterEscaped;
				}
				else
				{
					num = this.FindEditPositionFrom(num, true);
					testPosition = num;
					if (num == MaskedTextProvider.InvalidIndex)
					{
						testPosition = array.Length;
						resultHint = MaskedTextResultHint.UnavailableEditPosition;
						return false;
					}
					if (!MaskedTextProvider.IsValidInputChar(c))
					{
						testPosition = num;
						resultHint = MaskedTextResultHint.InvalidInput;
						return false;
					}
					if (!array[num].Match(c, out maskedTextResultHint, false))
					{
						testPosition = num;
						resultHint = maskedTextResultHint;
						return false;
					}
				}
			}
			if (maskedTextResultHint > resultHint)
			{
				resultHint = maskedTextResultHint;
			}
			return true;
		}

		private bool AddInternal(char input, out int testPosition, out MaskedTextResultHint resultHint, bool check_available_positions_first, bool check_escape_char_first)
		{
			testPosition = 0;
			int num = this.LastAssignedPosition + 1;
			if (check_available_positions_first)
			{
				int i = num;
				bool flag = false;
				while (i < this.edit_positions.Length)
				{
					if (this.edit_positions[i].Editable)
					{
						flag = true;
						break;
					}
					i++;
				}
				if (!flag)
				{
					testPosition = i;
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					return MaskedTextProvider.GetOperationResultFromHint(resultHint);
				}
			}
			if (check_escape_char_first && this.VerifyEscapeChar(input, num))
			{
				testPosition = num;
				resultHint = MaskedTextResultHint.CharacterEscaped;
				return true;
			}
			num = this.FindEditPositionFrom(num, true);
			if (num > this.edit_positions.Length - 1 || num == MaskedTextProvider.InvalidIndex)
			{
				testPosition = num;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return MaskedTextProvider.GetOperationResultFromHint(resultHint);
			}
			if (!MaskedTextProvider.IsValidInputChar(input))
			{
				testPosition = num;
				resultHint = MaskedTextResultHint.InvalidInput;
				return MaskedTextProvider.GetOperationResultFromHint(resultHint);
			}
			if (!this.edit_positions[num].Match(input, out resultHint, false))
			{
				testPosition = num;
				return MaskedTextProvider.GetOperationResultFromHint(resultHint);
			}
			testPosition = num;
			return MaskedTextProvider.GetOperationResultFromHint(resultHint);
		}

		private bool VerifyStringInternal(string input, out int testPosition, out MaskedTextResultHint resultHint, int startIndex, bool only_test)
		{
			int num = startIndex;
			resultHint = MaskedTextResultHint.Unknown;
			for (int i = 0; i < input.Length; i++)
			{
				int num2 = this.FindEditPositionFrom(num, true);
				if (num2 == MaskedTextProvider.InvalidIndex)
				{
					testPosition = this.edit_positions.Length;
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					return false;
				}
				MaskedTextResultHint maskedTextResultHint;
				if (!this.VerifyCharInternal(input[i], num2, out maskedTextResultHint, only_test))
				{
					testPosition = num2;
					resultHint = maskedTextResultHint;
					return false;
				}
				if (maskedTextResultHint > resultHint)
				{
					resultHint = maskedTextResultHint;
				}
				num = num2 + 1;
			}
			if (!only_test)
			{
				for (num = this.FindEditPositionFrom(num, true); num != MaskedTextProvider.InvalidIndex; num = this.FindEditPositionFrom(num + 1, true))
				{
					if (this.edit_positions[num].FilledIn)
					{
						this.edit_positions[num].Reset();
						if (resultHint != MaskedTextResultHint.NoEffect)
						{
							resultHint = MaskedTextResultHint.Success;
						}
					}
				}
			}
			if (input.Length > 0)
			{
				testPosition = startIndex + input.Length - 1;
			}
			else
			{
				testPosition = startIndex;
				if (resultHint < MaskedTextResultHint.NoEffect)
				{
					resultHint = MaskedTextResultHint.NoEffect;
				}
			}
			return true;
		}

		private bool VerifyCharInternal(char input, int position, out MaskedTextResultHint hint, bool only_test)
		{
			hint = MaskedTextResultHint.Unknown;
			if (position < 0 || position >= this.edit_positions.Length)
			{
				hint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (!MaskedTextProvider.IsValidInputChar(input))
			{
				hint = MaskedTextResultHint.InvalidInput;
				return false;
			}
			if (input == ' ' && this.ResetOnSpace && this.edit_positions[position].Editable && this.edit_positions[position].FilledIn)
			{
				if (!only_test)
				{
					this.edit_positions[position].Reset();
				}
				hint = MaskedTextResultHint.SideEffect;
				return true;
			}
			if (this.edit_positions[position].Editable && this.edit_positions[position].FilledIn && this.edit_positions[position].input == input)
			{
				hint = MaskedTextResultHint.NoEffect;
				return true;
			}
			if (this.SkipLiterals && !this.edit_positions[position].Editable && this.edit_positions[position].Text == input.ToString())
			{
				hint = MaskedTextResultHint.CharacterEscaped;
				return true;
			}
			return this.edit_positions[position].Match(input, out hint, only_test);
		}

		private bool IsInsertableString(string str_input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			int num = position;
			resultHint = MaskedTextResultHint.UnavailableEditPosition;
			testPosition = MaskedTextProvider.InvalidIndex;
			foreach (char c in str_input)
			{
				int num2 = this.FindEditPositionFrom(num, true);
				if (num2 != MaskedTextProvider.InvalidIndex && this.VerifyEscapeChar(c, num2))
				{
					num = num2 + 1;
				}
				else if (this.VerifyEscapeChar(c, num))
				{
					num++;
				}
				else
				{
					if (num2 == MaskedTextProvider.InvalidIndex)
					{
						resultHint = MaskedTextResultHint.UnavailableEditPosition;
						testPosition = this.edit_positions.Length;
						return false;
					}
					testPosition = num2;
					if (!this.edit_positions[num2].Match(c, out resultHint, true))
					{
						return false;
					}
					num = num2 + 1;
				}
			}
			resultHint = MaskedTextResultHint.Success;
			return true;
		}

		private bool ShiftPositionsRight(MaskedTextProvider.EditPosition[] edit_positions, int start, out int testPosition, out MaskedTextResultHint resultHint)
		{
			int num = this.FindAssignedEditPositionFrom(edit_positions.Length, false);
			int i = this.FindUnassignedEditPositionFrom(num, true);
			testPosition = start;
			resultHint = MaskedTextResultHint.Unknown;
			if (i == MaskedTextProvider.InvalidIndex)
			{
				testPosition = edit_positions.Length;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return false;
			}
			while (i > start)
			{
				int num2 = this.FindEditPositionFrom(i - 1, false);
				char input = edit_positions[num2].input;
				if (input == '\0')
				{
					edit_positions[i].input = input;
				}
				else if (!edit_positions[i].Match(input, out resultHint, false))
				{
					testPosition = i;
					return false;
				}
				i = num2;
			}
			if (i != MaskedTextProvider.InvalidIndex)
			{
				edit_positions[i].Reset();
			}
			return true;
		}

		private bool ReplaceInternal(string input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint, bool only_test, bool dont_remove_at_end)
		{
			resultHint = MaskedTextResultHint.Unknown;
			MaskedTextProvider.EditPosition[] array;
			if (only_test)
			{
				array = this.ClonePositions();
			}
			else
			{
				array = this.edit_positions;
			}
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (endPosition >= array.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition >= array.Length)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (input.Length == 0)
			{
				return this.RemoveAtInternal(startPosition, endPosition, out testPosition, out resultHint, only_test);
			}
			int num = startPosition;
			int num2 = num;
			MaskedTextResultHint maskedTextResultHint = MaskedTextResultHint.Unknown;
			testPosition = MaskedTextProvider.InvalidIndex;
			foreach (char c in input)
			{
				num2 = num;
				if (this.VerifyEscapeChar(c, num2))
				{
					if ((array[num2].FilledIn && array[num2].Editable && c == ' ' && this.ResetOnSpace) || (c == this.PromptChar && this.ResetOnPrompt))
					{
						array[num2].Reset();
						maskedTextResultHint = MaskedTextResultHint.SideEffect;
					}
					else
					{
						maskedTextResultHint = MaskedTextResultHint.CharacterEscaped;
					}
				}
				else if (num2 < array.Length && !array[num2].Editable && this.FindAssignedEditPositionInRange(num2, endPosition, true) == MaskedTextProvider.InvalidIndex)
				{
					num2 = this.FindEditPositionFrom(num2, true);
					if (num2 == MaskedTextProvider.InvalidIndex)
					{
						resultHint = MaskedTextResultHint.UnavailableEditPosition;
						testPosition = array.Length;
						return false;
					}
					if (!this.InsertAtInternal(c.ToString(), num2, out testPosition, out maskedTextResultHint, only_test))
					{
						resultHint = maskedTextResultHint;
						return false;
					}
				}
				else
				{
					num2 = this.FindEditPositionFrom(num2, true);
					if (num2 == MaskedTextProvider.InvalidIndex)
					{
						testPosition = array.Length;
						resultHint = MaskedTextResultHint.UnavailableEditPosition;
						return false;
					}
					if (!MaskedTextProvider.IsValidInputChar(c))
					{
						testPosition = num2;
						resultHint = MaskedTextResultHint.InvalidInput;
						return false;
					}
					if (!this.ReplaceInternal(array, c, num2, out testPosition, out maskedTextResultHint, false))
					{
						resultHint = maskedTextResultHint;
						return false;
					}
				}
				if (maskedTextResultHint > resultHint)
				{
					resultHint = maskedTextResultHint;
				}
				num = num2 + 1;
			}
			testPosition = num2;
			int num3;
			if (!dont_remove_at_end && num <= endPosition && !this.RemoveAtInternal(num, endPosition, out num3, out maskedTextResultHint, only_test))
			{
				testPosition = num3;
				resultHint = maskedTextResultHint;
				return false;
			}
			if (maskedTextResultHint == MaskedTextResultHint.Success && resultHint < MaskedTextResultHint.SideEffect)
			{
				resultHint = MaskedTextResultHint.SideEffect;
			}
			return true;
		}

		private bool ReplaceInternal(MaskedTextProvider.EditPosition[] edit_positions, char input, int position, out int testPosition, out MaskedTextResultHint resultHint, bool only_test)
		{
			testPosition = position;
			if (!MaskedTextProvider.IsValidInputChar(input))
			{
				resultHint = MaskedTextResultHint.InvalidInput;
				return false;
			}
			if (this.VerifyEscapeChar(input, position))
			{
				if ((edit_positions[position].FilledIn && edit_positions[position].Editable && input == ' ' && this.ResetOnSpace) || (input == this.PromptChar && this.ResetOnPrompt))
				{
					edit_positions[position].Reset();
					resultHint = MaskedTextResultHint.SideEffect;
				}
				else
				{
					resultHint = MaskedTextResultHint.CharacterEscaped;
				}
				testPosition = position;
				return true;
			}
			if (!edit_positions[position].Editable)
			{
				resultHint = MaskedTextResultHint.NonEditPosition;
				return false;
			}
			bool filledIn = edit_positions[position].FilledIn;
			if (filledIn && edit_positions[position].input == input)
			{
				if (this.VerifyEscapeChar(input, position))
				{
					resultHint = MaskedTextResultHint.CharacterEscaped;
				}
				else
				{
					resultHint = MaskedTextResultHint.NoEffect;
				}
			}
			else
			{
				if (input == ' ' && this.ResetOnSpace)
				{
					if (filledIn)
					{
						resultHint = MaskedTextResultHint.SideEffect;
						edit_positions[position].Reset();
					}
					else
					{
						resultHint = MaskedTextResultHint.CharacterEscaped;
					}
					return true;
				}
				if (this.VerifyEscapeChar(input, position))
				{
					resultHint = MaskedTextResultHint.SideEffect;
				}
				else
				{
					resultHint = MaskedTextResultHint.Success;
				}
			}
			MaskedTextResultHint maskedTextResultHint;
			if (!edit_positions[position].Match(input, out maskedTextResultHint, false))
			{
				resultHint = maskedTextResultHint;
				return false;
			}
			return true;
		}

		private bool RemoveAtInternal(int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint, bool only_testing)
		{
			testPosition = -1;
			resultHint = MaskedTextResultHint.Unknown;
			MaskedTextProvider.EditPosition[] array;
			if (only_testing)
			{
				array = this.ClonePositions();
			}
			else
			{
				array = this.edit_positions;
			}
			if (endPosition < 0 || endPosition >= array.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0 || startPosition >= array.Length)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			int num = 0;
			for (int i = startPosition; i <= endPosition; i++)
			{
				if (array[i].Editable)
				{
					num++;
				}
			}
			if (num == 0)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			for (int num2 = this.FindEditPositionFrom(startPosition, true); num2 != MaskedTextProvider.InvalidIndex; num2 = this.FindEditPositionFrom(num2 + 1, true))
			{
				int num3 = this.FindEditPositionFrom(num2 + 1, true);
				int num4 = 1;
				while (num4 < num && num3 != MaskedTextProvider.InvalidIndex)
				{
					num3 = this.FindEditPositionFrom(num3 + 1, true);
					num4++;
				}
				if (num3 == MaskedTextProvider.InvalidIndex)
				{
					if (array[num2].FilledIn)
					{
						array[num2].Reset();
						resultHint = MaskedTextResultHint.Success;
					}
					else if (resultHint < MaskedTextResultHint.NoEffect)
					{
						resultHint = MaskedTextResultHint.NoEffect;
					}
				}
				else
				{
					if (!array[num3].FilledIn)
					{
						if (array[num2].FilledIn)
						{
							array[num2].Reset();
							resultHint = MaskedTextResultHint.Success;
						}
						else if (resultHint < MaskedTextResultHint.NoEffect)
						{
							resultHint = MaskedTextResultHint.NoEffect;
						}
					}
					else
					{
						MaskedTextResultHint maskedTextResultHint = MaskedTextResultHint.Unknown;
						if (array[num2].FilledIn)
						{
							resultHint = MaskedTextResultHint.Success;
						}
						else if (resultHint < MaskedTextResultHint.SideEffect)
						{
							resultHint = MaskedTextResultHint.SideEffect;
						}
						if (!array[num2].Match(array[num3].input, out maskedTextResultHint, false))
						{
							resultHint = maskedTextResultHint;
							testPosition = num2;
							return false;
						}
					}
					array[num3].Reset();
				}
			}
			if (resultHint == MaskedTextResultHint.Unknown)
			{
				resultHint = MaskedTextResultHint.NoEffect;
			}
			testPosition = startPosition;
			return true;
		}

		private bool InsertAtInternal(string str_input, int position, out int testPosition, out MaskedTextResultHint resultHint, bool only_testing)
		{
			testPosition = -1;
			resultHint = MaskedTextResultHint.Unknown;
			MaskedTextProvider.EditPosition[] array;
			if (only_testing)
			{
				array = this.ClonePositions();
			}
			else
			{
				array = this.edit_positions;
			}
			if (position < 0 || position >= array.Length)
			{
				testPosition = 0;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (!this.IsInsertableString(str_input, position, out testPosition, out resultHint))
			{
				return false;
			}
			resultHint = MaskedTextResultHint.Unknown;
			int num = position;
			int i = 0;
			while (i < str_input.Length)
			{
				char c = str_input[i];
				int num2 = this.FindEditPositionFrom(num, true);
				int num3 = this.FindUnassignedEditPositionFrom(num, true);
				bool flag = false;
				if (!this.VerifyEscapeChar(c, num))
				{
					goto IL_00DF;
				}
				flag = true;
				if (!(c.ToString() == array[num].Text))
				{
					goto IL_00DF;
				}
				if (this.FindAssignedEditPositionInRange(0, num - 1, true) != MaskedTextProvider.InvalidIndex && num3 == MaskedTextProvider.InvalidIndex)
				{
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					testPosition = array.Length;
					return false;
				}
				resultHint = MaskedTextResultHint.CharacterEscaped;
				testPosition = num;
				num++;
				IL_01FA:
				i++;
				continue;
				IL_00DF:
				if (!flag && num2 == MaskedTextProvider.InvalidIndex)
				{
					testPosition = array.Length;
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					return false;
				}
				if (num2 == MaskedTextProvider.InvalidIndex)
				{
					num2 = num;
				}
				bool filledIn = array[num2].FilledIn;
				bool flag2 = filledIn;
				if (flag2 && !this.ShiftPositionsRight(array, num2, out testPosition, out resultHint))
				{
					return false;
				}
				testPosition = num2;
				if (flag)
				{
					if (filledIn)
					{
						resultHint = MaskedTextResultHint.Success;
					}
					else if (!array[num2].Editable && c.ToString() == array[num2].Text)
					{
						resultHint = MaskedTextResultHint.CharacterEscaped;
						testPosition = num;
					}
					else
					{
						int num4 = this.FindEditPositionFrom(num2, true);
						if (num4 == MaskedTextProvider.InvalidIndex)
						{
							resultHint = MaskedTextResultHint.UnavailableEditPosition;
							testPosition = array.Length;
							return false;
						}
						resultHint = MaskedTextResultHint.CharacterEscaped;
						if (c.ToString() == array[num].Text)
						{
							testPosition = num;
						}
					}
				}
				else
				{
					MaskedTextResultHint maskedTextResultHint;
					if (!array[num2].Match(c, out maskedTextResultHint, false))
					{
						resultHint = maskedTextResultHint;
						return false;
					}
					if (resultHint < maskedTextResultHint)
					{
						resultHint = maskedTextResultHint;
					}
				}
				num = num2 + 1;
				goto IL_01FA;
			}
			return true;
		}

		public bool Add(char input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Add(input, out num, out maskedTextResultHint);
		}

		public bool Add(string input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Add(input, out num, out maskedTextResultHint);
		}

		public bool Add(char input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			return this.AddInternal(input, out testPosition, out resultHint, true, false);
		}

		public bool Add(string input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			bool flag = this.AddInternal(input, out testPosition, out resultHint, true);
			if (flag)
			{
				flag = this.AddInternal(input, out testPosition, out resultHint, false);
			}
			return flag;
		}

		public void Clear()
		{
			MaskedTextResultHint maskedTextResultHint;
			this.Clear(out maskedTextResultHint);
		}

		public void Clear(out MaskedTextResultHint resultHint)
		{
			resultHint = MaskedTextResultHint.NoEffect;
			for (int i = 0; i < this.edit_positions.Length; i++)
			{
				if (this.edit_positions[i].Editable && this.edit_positions[i].FilledIn)
				{
					this.edit_positions[i].Reset();
					resultHint = MaskedTextResultHint.Success;
				}
			}
		}

		public object Clone()
		{
			return new MaskedTextProvider(this.mask)
			{
				allow_prompt_as_input = this.allow_prompt_as_input,
				ascii_only = this.ascii_only,
				culture = this.culture,
				edit_positions = this.ClonePositions(),
				include_literals = this.include_literals,
				include_prompt = this.include_prompt,
				is_password = this.is_password,
				mask = this.mask,
				password_char = this.password_char,
				prompt_char = this.prompt_char,
				reset_on_prompt = this.reset_on_prompt,
				reset_on_space = this.reset_on_space,
				skip_literals = this.skip_literals
			};
		}

		public int FindAssignedEditPositionFrom(int position, bool direction)
		{
			if (direction)
			{
				return this.FindAssignedEditPositionInRange(position, this.edit_positions.Length - 1, direction);
			}
			return this.FindAssignedEditPositionInRange(0, position, direction);
		}

		public int FindAssignedEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (endPosition >= this.edit_positions.Length)
			{
				endPosition = this.edit_positions.Length - 1;
			}
			if (startPosition > endPosition)
			{
				return MaskedTextProvider.InvalidIndex;
			}
			int num = ((!direction) ? (-1) : 1);
			int num2 = ((!direction) ? endPosition : startPosition);
			int num3 = ((!direction) ? startPosition : endPosition) + num;
			for (int num4 = num2; num4 != num3; num4 += num)
			{
				if (this.edit_positions[num4].Editable && this.edit_positions[num4].FilledIn)
				{
					return num4;
				}
			}
			return MaskedTextProvider.InvalidIndex;
		}

		public int FindEditPositionFrom(int position, bool direction)
		{
			if (direction)
			{
				return this.FindEditPositionInRange(position, this.edit_positions.Length - 1, direction);
			}
			return this.FindEditPositionInRange(0, position, direction);
		}

		public int FindEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (endPosition >= this.edit_positions.Length)
			{
				endPosition = this.edit_positions.Length - 1;
			}
			if (startPosition > endPosition)
			{
				return MaskedTextProvider.InvalidIndex;
			}
			int num = ((!direction) ? (-1) : 1);
			int num2 = ((!direction) ? endPosition : startPosition);
			int num3 = ((!direction) ? startPosition : endPosition) + num;
			for (int num4 = num2; num4 != num3; num4 += num)
			{
				if (this.edit_positions[num4].Editable)
				{
					return num4;
				}
			}
			return MaskedTextProvider.InvalidIndex;
		}

		public int FindNonEditPositionFrom(int position, bool direction)
		{
			if (direction)
			{
				return this.FindNonEditPositionInRange(position, this.edit_positions.Length - 1, direction);
			}
			return this.FindNonEditPositionInRange(0, position, direction);
		}

		public int FindNonEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (endPosition >= this.edit_positions.Length)
			{
				endPosition = this.edit_positions.Length - 1;
			}
			if (startPosition > endPosition)
			{
				return MaskedTextProvider.InvalidIndex;
			}
			int num = ((!direction) ? (-1) : 1);
			int num2 = ((!direction) ? endPosition : startPosition);
			int num3 = ((!direction) ? startPosition : endPosition) + num;
			for (int num4 = num2; num4 != num3; num4 += num)
			{
				if (!this.edit_positions[num4].Editable)
				{
					return num4;
				}
			}
			return MaskedTextProvider.InvalidIndex;
		}

		public int FindUnassignedEditPositionFrom(int position, bool direction)
		{
			if (direction)
			{
				return this.FindUnassignedEditPositionInRange(position, this.edit_positions.Length - 1, direction);
			}
			return this.FindUnassignedEditPositionInRange(0, position, direction);
		}

		public int FindUnassignedEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (endPosition >= this.edit_positions.Length)
			{
				endPosition = this.edit_positions.Length - 1;
			}
			if (startPosition > endPosition)
			{
				return MaskedTextProvider.InvalidIndex;
			}
			int num = ((!direction) ? (-1) : 1);
			int num2 = ((!direction) ? endPosition : startPosition);
			int num3 = ((!direction) ? startPosition : endPosition) + num;
			for (int num4 = num2; num4 != num3; num4 += num)
			{
				if (this.edit_positions[num4].Editable && !this.edit_positions[num4].FilledIn)
				{
					return num4;
				}
			}
			return MaskedTextProvider.InvalidIndex;
		}

		public static bool GetOperationResultFromHint(MaskedTextResultHint hint)
		{
			return hint == MaskedTextResultHint.CharacterEscaped || hint == MaskedTextResultHint.NoEffect || hint == MaskedTextResultHint.SideEffect || hint == MaskedTextResultHint.Success;
		}

		public bool InsertAt(char input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.InsertAt(input, position, out num, out maskedTextResultHint);
		}

		public bool InsertAt(string input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.InsertAt(input, position, out num, out maskedTextResultHint);
		}

		public bool InsertAt(char input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			return this.InsertAt(input.ToString(), position, out testPosition, out resultHint);
		}

		public bool InsertAt(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (position >= this.edit_positions.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (input == string.Empty)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			bool flag = this.InsertAtInternal(input, position, out testPosition, out resultHint, true);
			if (flag)
			{
				flag = this.InsertAtInternal(input, position, out testPosition, out resultHint, false);
			}
			return flag;
		}

		public bool IsAvailablePosition(int position)
		{
			return position >= 0 && position < this.edit_positions.Length && this.edit_positions[position].Editable && !this.edit_positions[position].FilledIn;
		}

		public bool IsEditPosition(int position)
		{
			return position >= 0 && position < this.edit_positions.Length && this.edit_positions[position].Editable;
		}

		public static bool IsValidInputChar(char c)
		{
			return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' ';
		}

		public static bool IsValidMaskChar(char c)
		{
			return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' ';
		}

		public static bool IsValidPasswordChar(char c)
		{
			return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' ' || c == '\0';
		}

		public bool Remove()
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Remove(out num, out maskedTextResultHint);
		}

		public bool Remove(out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (this.LastAssignedPosition == MaskedTextProvider.InvalidIndex)
			{
				resultHint = MaskedTextResultHint.NoEffect;
				testPosition = 0;
				return true;
			}
			testPosition = this.LastAssignedPosition;
			resultHint = MaskedTextResultHint.Success;
			this.edit_positions[this.LastAssignedPosition].input = '\0';
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
			bool flag = this.RemoveAtInternal(startPosition, endPosition, out testPosition, out resultHint, true);
			if (flag)
			{
				flag = this.RemoveAtInternal(startPosition, endPosition, out testPosition, out resultHint, false);
			}
			return flag;
		}

		public bool Replace(char input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Replace(input, position, out num, out maskedTextResultHint);
		}

		public bool Replace(string input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Replace(input, position, out num, out maskedTextResultHint);
		}

		public bool Replace(char input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (position < 0 || position >= this.edit_positions.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (this.VerifyEscapeChar(input, position))
			{
				if ((this.edit_positions[position].FilledIn && this.edit_positions[position].Editable && input == ' ' && this.ResetOnSpace) || (input == this.PromptChar && this.ResetOnPrompt))
				{
					this.edit_positions[position].Reset();
					resultHint = MaskedTextResultHint.SideEffect;
				}
				else
				{
					resultHint = MaskedTextResultHint.CharacterEscaped;
				}
				testPosition = position;
				return true;
			}
			int num = this.FindEditPositionFrom(position, true);
			if (num == MaskedTextProvider.InvalidIndex)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return false;
			}
			if (!MaskedTextProvider.IsValidInputChar(input))
			{
				testPosition = num;
				resultHint = MaskedTextResultHint.InvalidInput;
				return false;
			}
			return this.ReplaceInternal(this.edit_positions, input, num, out testPosition, out resultHint, false);
		}

		public bool Replace(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (position < 0 || position >= this.edit_positions.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (input.Length == 0)
			{
				return this.RemoveAt(position, position, out testPosition, out resultHint);
			}
			bool flag = this.ReplaceInternal(input, position, this.edit_positions.Length - 1, out testPosition, out resultHint, true, true);
			if (flag)
			{
				flag = this.ReplaceInternal(input, position, this.edit_positions.Length - 1, out testPosition, out resultHint, false, true);
			}
			return flag;
		}

		public bool Replace(char input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (endPosition >= this.edit_positions.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition == endPosition)
			{
				return this.ReplaceInternal(this.edit_positions, input, startPosition, out testPosition, out resultHint, false);
			}
			return this.Replace(input.ToString(), startPosition, endPosition, out testPosition, out resultHint);
		}

		public bool Replace(string input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
		{
			bool flag = this.ReplaceInternal(input, startPosition, endPosition, out testPosition, out resultHint, true, false);
			if (flag)
			{
				flag = this.ReplaceInternal(input, startPosition, endPosition, out testPosition, out resultHint, false, false);
			}
			return flag;
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
			bool flag = this.VerifyStringInternal(input, out testPosition, out resultHint, 0, true);
			if (flag)
			{
				flag = this.VerifyStringInternal(input, out testPosition, out resultHint, 0, false);
			}
			return flag;
		}

		public string ToDisplayString()
		{
			return this.ToString(false, true, true, 0, this.Length);
		}

		public override string ToString()
		{
			return this.ToString(true, this.IncludePrompt, this.IncludeLiterals, 0, this.Length);
		}

		public string ToString(bool ignorePasswordChar)
		{
			return this.ToString(ignorePasswordChar, this.IncludePrompt, this.IncludeLiterals, 0, this.Length);
		}

		public string ToString(bool includePrompt, bool includeLiterals)
		{
			return this.ToString(true, includePrompt, includeLiterals, 0, this.Length);
		}

		public string ToString(int startPosition, int length)
		{
			return this.ToString(true, this.IncludePrompt, this.IncludeLiterals, startPosition, length);
		}

		public string ToString(bool ignorePasswordChar, int startPosition, int length)
		{
			return this.ToString(ignorePasswordChar, this.IncludePrompt, this.IncludeLiterals, startPosition, length);
		}

		public string ToString(bool includePrompt, bool includeLiterals, int startPosition, int length)
		{
			return this.ToString(true, includePrompt, includeLiterals, startPosition, length);
		}

		public string ToString(bool ignorePasswordChar, bool includePrompt, bool includeLiterals, int startPosition, int length)
		{
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (length <= 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = startPosition;
			int num2 = startPosition + length - 1;
			if (num2 >= this.edit_positions.Length)
			{
				num2 = this.edit_positions.Length - 1;
			}
			int num3 = this.FindAssignedEditPositionInRange(num, num2, false);
			if (!includePrompt)
			{
				int num4 = this.FindNonEditPositionInRange(num, num2, false);
				if (includeLiterals)
				{
					num2 = ((num3 <= num4) ? num4 : num3);
				}
				else
				{
					num2 = num3;
				}
			}
			for (int i = num; i <= num2; i++)
			{
				MaskedTextProvider.EditPosition editPosition = this.edit_positions[i];
				if (editPosition.Type == MaskedTextProvider.EditType.Literal)
				{
					if (includeLiterals)
					{
						stringBuilder.Append(editPosition.Text);
					}
				}
				else if (editPosition.Editable)
				{
					if (this.IsPassword)
					{
						if (ignorePasswordChar)
						{
							if (!editPosition.FilledIn)
							{
								if (includePrompt)
								{
									stringBuilder.Append(this.PromptChar);
								}
								else
								{
									stringBuilder.Append(" ");
								}
							}
							else
							{
								stringBuilder.Append(editPosition.Input);
							}
						}
						else
						{
							stringBuilder.Append(this.PasswordChar);
						}
					}
					else if (!editPosition.FilledIn)
					{
						if (includePrompt)
						{
							stringBuilder.Append(this.PromptChar);
						}
						else if (includeLiterals)
						{
							stringBuilder.Append(" ");
						}
						else if (num3 != MaskedTextProvider.InvalidIndex && num3 > i)
						{
							stringBuilder.Append(" ");
						}
					}
					else
					{
						stringBuilder.Append(editPosition.Text);
					}
				}
				else if (includeLiterals)
				{
					stringBuilder.Append(editPosition.Text);
				}
			}
			return stringBuilder.ToString();
		}

		public bool VerifyChar(char input, int position, out MaskedTextResultHint hint)
		{
			return this.VerifyCharInternal(input, position, out hint, true);
		}

		public bool VerifyEscapeChar(char input, int position)
		{
			if (position >= this.edit_positions.Length || position < 0)
			{
				return false;
			}
			if (!this.edit_positions[position].Editable)
			{
				return this.SkipLiterals && input.ToString() == this.edit_positions[position].Text;
			}
			return (this.ResetOnSpace && input == ' ') || (this.ResetOnPrompt && input == this.PromptChar);
		}

		public bool VerifyString(string input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.VerifyString(input, out num, out maskedTextResultHint);
		}

		public bool VerifyString(string input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null || input.Length == 0)
			{
				testPosition = 0;
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			return this.VerifyStringInternal(input, out testPosition, out resultHint, 0, true);
		}

		public bool AllowPromptAsInput
		{
			get
			{
				return this.allow_prompt_as_input;
			}
		}

		public bool AsciiOnly
		{
			get
			{
				return this.ascii_only;
			}
		}

		public int AssignedEditPositionCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.edit_positions.Length; i++)
				{
					if (this.edit_positions[i].FilledIn)
					{
						num++;
					}
				}
				return num;
			}
		}

		public int AvailableEditPositionCount
		{
			get
			{
				int num = 0;
				foreach (MaskedTextProvider.EditPosition editPosition in this.edit_positions)
				{
					if (!editPosition.FilledIn && editPosition.Editable)
					{
						num++;
					}
				}
				return num;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

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
				int num = 0;
				foreach (MaskedTextProvider.EditPosition editPosition in this.edit_positions)
				{
					if (editPosition.Editable)
					{
						num++;
					}
				}
				return num;
			}
		}

		public IEnumerator EditPositions
		{
			get
			{
				List<int> list = new List<int>();
				for (int i = 0; i < this.edit_positions.Length; i++)
				{
					if (this.edit_positions[i].Editable)
					{
						list.Add(i);
					}
				}
				return list.GetEnumerator();
			}
		}

		public bool IncludeLiterals
		{
			get
			{
				return this.include_literals;
			}
			set
			{
				this.include_literals = value;
			}
		}

		public bool IncludePrompt
		{
			get
			{
				return this.include_prompt;
			}
			set
			{
				this.include_prompt = value;
			}
		}

		public static int InvalidIndex
		{
			get
			{
				return -1;
			}
		}

		public bool IsPassword
		{
			get
			{
				return this.password_char != '\0';
			}
			set
			{
				this.password_char = ((!value) ? '\0' : MaskedTextProvider.DefaultPasswordChar);
			}
		}

		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Length)
				{
					throw new IndexOutOfRangeException(index.ToString());
				}
				return this.ToString(true, true, true, 0, this.edit_positions.Length)[index];
			}
		}

		public int LastAssignedPosition
		{
			get
			{
				return this.FindAssignedEditPositionFrom(this.edit_positions.Length - 1, false);
			}
		}

		public int Length
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.edit_positions.Length; i++)
				{
					if (this.edit_positions[i].Visible)
					{
						num++;
					}
				}
				return num;
			}
		}

		public string Mask
		{
			get
			{
				return this.mask;
			}
		}

		public bool MaskCompleted
		{
			get
			{
				for (int i = 0; i < this.edit_positions.Length; i++)
				{
					if (this.edit_positions[i].Required && !this.edit_positions[i].FilledIn)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool MaskFull
		{
			get
			{
				for (int i = 0; i < this.edit_positions.Length; i++)
				{
					if (this.edit_positions[i].Editable && !this.edit_positions[i].FilledIn)
					{
						return false;
					}
				}
				return true;
			}
		}

		public char PasswordChar
		{
			get
			{
				return this.password_char;
			}
			set
			{
				this.password_char = value;
			}
		}

		public char PromptChar
		{
			get
			{
				return this.prompt_char;
			}
			set
			{
				this.prompt_char = value;
			}
		}

		public bool ResetOnPrompt
		{
			get
			{
				return this.reset_on_prompt;
			}
			set
			{
				this.reset_on_prompt = value;
			}
		}

		public bool ResetOnSpace
		{
			get
			{
				return this.reset_on_space;
			}
			set
			{
				this.reset_on_space = value;
			}
		}

		public bool SkipLiterals
		{
			get
			{
				return this.skip_literals;
			}
			set
			{
				this.skip_literals = value;
			}
		}

		private bool allow_prompt_as_input;

		private bool ascii_only;

		private CultureInfo culture;

		private bool include_literals;

		private bool include_prompt;

		private bool is_password;

		private string mask;

		private char password_char;

		private char prompt_char;

		private bool reset_on_prompt;

		private bool reset_on_space;

		private bool skip_literals;

		private MaskedTextProvider.EditPosition[] edit_positions;

		private static char default_prompt_char = '_';

		private static char default_password_char;

		private enum EditState
		{
			None,
			UpperCase,
			LowerCase
		}

		private enum EditType
		{
			DigitRequired,
			DigitOrSpaceOptional,
			DigitOrSpaceOptional_Blank,
			LetterRequired,
			LetterOptional,
			CharacterRequired,
			CharacterOptional,
			AlphanumericRequired,
			AlphanumericOptional,
			DecimalPlaceholder,
			ThousandsPlaceholder,
			TimeSeparator,
			DateSeparator,
			CurrencySymbol,
			Literal
		}

		private class EditPosition
		{
			private EditPosition()
			{
			}

			public EditPosition(MaskedTextProvider Parent, MaskedTextProvider.EditType Type, MaskedTextProvider.EditState State, char MaskCharacter)
			{
				this.Type = Type;
				this.Parent = Parent;
				this.State = State;
				this.MaskCharacter = MaskCharacter;
			}

			public void Reset()
			{
				this.input = '\0';
			}

			internal MaskedTextProvider.EditPosition Clone()
			{
				return new MaskedTextProvider.EditPosition
				{
					Parent = this.Parent,
					Type = this.Type,
					State = this.State,
					MaskCharacter = this.MaskCharacter,
					input = this.input
				};
			}

			public char Input
			{
				get
				{
					return this.input;
				}
				set
				{
					MaskedTextProvider.EditState state = this.State;
					if (state != MaskedTextProvider.EditState.UpperCase)
					{
						if (state != MaskedTextProvider.EditState.LowerCase)
						{
							this.input = value;
						}
						else
						{
							this.input = char.ToLower(value, this.Parent.Culture);
						}
					}
					else
					{
						this.input = char.ToUpper(value, this.Parent.Culture);
					}
				}
			}

			public bool IsAscii(char c)
			{
				return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
			}

			public bool Match(char c, out MaskedTextResultHint resultHint, bool only_test)
			{
				if (!MaskedTextProvider.IsValidInputChar(c))
				{
					resultHint = MaskedTextResultHint.InvalidInput;
					return false;
				}
				if (this.Parent.ResetOnSpace && c == ' ' && this.Editable)
				{
					resultHint = MaskedTextResultHint.CharacterEscaped;
					if (this.FilledIn)
					{
						resultHint = MaskedTextResultHint.Success;
						if (!only_test && this.input != ' ')
						{
							switch (this.Type)
							{
							case MaskedTextProvider.EditType.CharacterRequired:
							case MaskedTextProvider.EditType.CharacterOptional:
							case MaskedTextProvider.EditType.AlphanumericRequired:
							case MaskedTextProvider.EditType.AlphanumericOptional:
								this.Input = c;
								break;
							default:
								this.Input = '\0';
								break;
							}
						}
					}
					return true;
				}
				if (this.Type == MaskedTextProvider.EditType.Literal && this.MaskCharacter == c && this.Parent.SkipLiterals)
				{
					resultHint = MaskedTextResultHint.Success;
					return true;
				}
				if (!this.Editable)
				{
					resultHint = MaskedTextResultHint.NonEditPosition;
					return false;
				}
				switch (this.Type)
				{
				case MaskedTextProvider.EditType.DigitRequired:
					if (char.IsDigit(c))
					{
						if (!only_test)
						{
							this.Input = c;
						}
						resultHint = MaskedTextResultHint.Success;
						return true;
					}
					resultHint = MaskedTextResultHint.DigitExpected;
					return false;
				case MaskedTextProvider.EditType.DigitOrSpaceOptional:
				case MaskedTextProvider.EditType.DigitOrSpaceOptional_Blank:
					if (char.IsDigit(c) || c == ' ')
					{
						if (!only_test)
						{
							this.Input = c;
						}
						resultHint = MaskedTextResultHint.Success;
						return true;
					}
					resultHint = MaskedTextResultHint.DigitExpected;
					return false;
				case MaskedTextProvider.EditType.LetterRequired:
				case MaskedTextProvider.EditType.LetterOptional:
					if (!char.IsLetter(c))
					{
						resultHint = MaskedTextResultHint.LetterExpected;
						return false;
					}
					if (this.Parent.AsciiOnly && !this.IsAscii(c))
					{
						resultHint = MaskedTextResultHint.LetterExpected;
						return false;
					}
					if (!only_test)
					{
						this.Input = c;
					}
					resultHint = MaskedTextResultHint.Success;
					return true;
				case MaskedTextProvider.EditType.CharacterRequired:
				case MaskedTextProvider.EditType.CharacterOptional:
					if (this.Parent.AsciiOnly && !this.IsAscii(c))
					{
						resultHint = MaskedTextResultHint.LetterExpected;
						return false;
					}
					if (!char.IsControl(c))
					{
						if (!only_test)
						{
							this.Input = c;
						}
						resultHint = MaskedTextResultHint.Success;
						return true;
					}
					resultHint = MaskedTextResultHint.LetterExpected;
					return false;
				case MaskedTextProvider.EditType.AlphanumericRequired:
				case MaskedTextProvider.EditType.AlphanumericOptional:
					if (!char.IsLetterOrDigit(c))
					{
						resultHint = MaskedTextResultHint.AlphanumericCharacterExpected;
						return false;
					}
					if (this.Parent.AsciiOnly && !this.IsAscii(c))
					{
						resultHint = MaskedTextResultHint.AsciiCharacterExpected;
						return false;
					}
					if (!only_test)
					{
						this.Input = c;
					}
					resultHint = MaskedTextResultHint.Success;
					return true;
				default:
					resultHint = MaskedTextResultHint.Unknown;
					return false;
				}
			}

			public bool FilledIn
			{
				get
				{
					return this.Input != '\0';
				}
			}

			public bool Required
			{
				get
				{
					char maskCharacter = this.MaskCharacter;
					return maskCharacter == '&' || maskCharacter == '0' || maskCharacter == 'A' || maskCharacter == 'L';
				}
			}

			public bool Editable
			{
				get
				{
					char maskCharacter = this.MaskCharacter;
					switch (maskCharacter)
					{
					case '?':
					case 'A':
					case 'C':
						break;
					default:
						switch (maskCharacter)
						{
						case '#':
						case '&':
							break;
						default:
							if (maskCharacter != '0' && maskCharacter != '9' && maskCharacter != 'L' && maskCharacter != 'a')
							{
								return false;
							}
							break;
						}
						break;
					}
					return true;
				}
			}

			public bool Visible
			{
				get
				{
					char maskCharacter = this.MaskCharacter;
					switch (maskCharacter)
					{
					case '<':
					case '>':
						break;
					default:
						if (maskCharacter != '|')
						{
							return true;
						}
						break;
					}
					return false;
				}
			}

			public string Text
			{
				get
				{
					if (this.Type == MaskedTextProvider.EditType.Literal)
					{
						return this.MaskCharacter.ToString();
					}
					char maskCharacter = this.MaskCharacter;
					switch (maskCharacter)
					{
					case ',':
						return this.Parent.Culture.NumberFormat.NumberGroupSeparator;
					default:
						if (maskCharacter == '$')
						{
							return this.Parent.Culture.NumberFormat.CurrencySymbol;
						}
						if (maskCharacter != ':')
						{
							return (!this.FilledIn) ? this.Parent.PromptChar.ToString() : this.Input.ToString();
						}
						return this.Parent.Culture.DateTimeFormat.TimeSeparator;
					case '.':
						return this.Parent.Culture.NumberFormat.NumberDecimalSeparator;
					case '/':
						return this.Parent.Culture.DateTimeFormat.DateSeparator;
					}
				}
			}

			public MaskedTextProvider Parent;

			public MaskedTextProvider.EditType Type;

			public MaskedTextProvider.EditState State;

			public char MaskCharacter;

			public char input;
		}
	}
}
