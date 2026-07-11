using System;
using System.Collections;
using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal class RxCompiler : ICompiler
	{
		private void MakeRoom(int bytes)
		{
			while (this.curpos + bytes > this.program.Length)
			{
				int num = this.program.Length * 2;
				byte[] array = new byte[num];
				Buffer.BlockCopy(this.program, 0, array, 0, this.program.Length);
				this.program = array;
			}
		}

		private void Emit(byte val)
		{
			this.MakeRoom(1);
			this.program[this.curpos] = val;
			this.curpos++;
		}

		private void Emit(RxOp opcode)
		{
			this.Emit((byte)opcode);
		}

		private void Emit(ushort val)
		{
			this.MakeRoom(2);
			this.program[this.curpos] = (byte)val;
			this.program[this.curpos + 1] = (byte)(val >> 8);
			this.curpos += 2;
		}

		private void Emit(int val)
		{
			this.MakeRoom(4);
			this.program[this.curpos] = (byte)val;
			this.program[this.curpos + 1] = (byte)(val >> 8);
			this.program[this.curpos + 2] = (byte)(val >> 16);
			this.program[this.curpos + 3] = (byte)(val >> 24);
			this.curpos += 4;
		}

		private void BeginLink(LinkRef lref)
		{
			RxLinkRef rxLinkRef = lref as RxLinkRef;
			rxLinkRef.PushInstructionBase(this.curpos);
		}

		private void EmitLink(LinkRef lref)
		{
			RxLinkRef rxLinkRef = lref as RxLinkRef;
			rxLinkRef.PushOffsetPosition(this.curpos);
			this.Emit(0);
		}

		public void Reset()
		{
			this.curpos = 0;
		}

		public IMachineFactory GetMachineFactory()
		{
			byte[] array = new byte[this.curpos];
			Buffer.BlockCopy(this.program, 0, array, 0, this.curpos);
			return new RxInterpreterFactory(array, null);
		}

		public void EmitFalse()
		{
			this.Emit(RxOp.False);
		}

		public void EmitTrue()
		{
			this.Emit(RxOp.True);
		}

		public virtual void EmitOp(RxOp op, bool negate, bool ignore, bool reverse)
		{
			int num = 0;
			if (negate)
			{
				num++;
			}
			if (ignore)
			{
				num += 2;
			}
			if (reverse)
			{
				num += 4;
			}
			this.Emit(op + (byte)num);
		}

		public virtual void EmitOpIgnoreReverse(RxOp op, bool ignore, bool reverse)
		{
			int num = 0;
			if (ignore)
			{
				num++;
			}
			if (reverse)
			{
				num += 2;
			}
			this.Emit(op + (byte)num);
		}

		public virtual void EmitOpNegateReverse(RxOp op, bool negate, bool reverse)
		{
			int num = 0;
			if (negate)
			{
				num++;
			}
			if (reverse)
			{
				num += 2;
			}
			this.Emit(op + (byte)num);
		}

		public void EmitCharacter(char c, bool negate, bool ignore, bool reverse)
		{
			if (ignore)
			{
				c = char.ToLower(c);
			}
			if (c < 'Ā')
			{
				this.EmitOp(RxOp.Char, negate, ignore, reverse);
				this.Emit((byte)c);
			}
			else
			{
				this.EmitOp(RxOp.UnicodeChar, negate, ignore, reverse);
				this.Emit((ushort)c);
			}
		}

		private void EmitUniCat(UnicodeCategory cat, bool negate, bool reverse)
		{
			this.EmitOpNegateReverse(RxOp.CategoryUnicode, negate, reverse);
			this.Emit((byte)cat);
		}

		private void EmitCatGeneral(Category cat, bool negate, bool reverse)
		{
			this.EmitOpNegateReverse(RxOp.CategoryGeneral, negate, reverse);
			this.Emit((byte)cat);
		}

		public void EmitCategory(Category cat, bool negate, bool reverse)
		{
			switch (cat)
			{
			case Category.Any:
			case Category.EcmaAny:
				this.EmitOpNegateReverse(RxOp.CategoryAny, negate, reverse);
				return;
			case Category.AnySingleline:
				this.EmitOpNegateReverse(RxOp.CategoryAnySingleline, negate, reverse);
				return;
			case Category.Word:
				this.EmitOpNegateReverse(RxOp.CategoryWord, negate, reverse);
				return;
			case Category.Digit:
				this.EmitOpNegateReverse(RxOp.CategoryDigit, negate, reverse);
				return;
			case Category.WhiteSpace:
				this.EmitOpNegateReverse(RxOp.CategoryWhiteSpace, negate, reverse);
				return;
			case Category.EcmaWord:
				this.EmitOpNegateReverse(RxOp.CategoryEcmaWord, negate, reverse);
				return;
			case Category.EcmaDigit:
				this.EmitRange('0', '9', negate, false, reverse);
				return;
			case Category.EcmaWhiteSpace:
				this.EmitOpNegateReverse(RxOp.CategoryEcmaWhiteSpace, negate, reverse);
				return;
			case Category.UnicodeL:
			case Category.UnicodeM:
			case Category.UnicodeN:
			case Category.UnicodeZ:
			case Category.UnicodeP:
			case Category.UnicodeS:
			case Category.UnicodeC:
				this.EmitCatGeneral(cat, negate, reverse);
				return;
			case Category.UnicodeLu:
				this.EmitUniCat(UnicodeCategory.UppercaseLetter, negate, reverse);
				return;
			case Category.UnicodeLl:
				this.EmitUniCat(UnicodeCategory.LowercaseLetter, negate, reverse);
				return;
			case Category.UnicodeLt:
				this.EmitUniCat(UnicodeCategory.TitlecaseLetter, negate, reverse);
				return;
			case Category.UnicodeLm:
				this.EmitUniCat(UnicodeCategory.ModifierLetter, negate, reverse);
				return;
			case Category.UnicodeLo:
				this.EmitUniCat(UnicodeCategory.OtherLetter, negate, reverse);
				return;
			case Category.UnicodeMn:
				this.EmitUniCat(UnicodeCategory.NonSpacingMark, negate, reverse);
				return;
			case Category.UnicodeMe:
				this.EmitUniCat(UnicodeCategory.EnclosingMark, negate, reverse);
				return;
			case Category.UnicodeMc:
				this.EmitUniCat(UnicodeCategory.SpacingCombiningMark, negate, reverse);
				return;
			case Category.UnicodeNd:
				this.EmitUniCat(UnicodeCategory.DecimalDigitNumber, negate, reverse);
				return;
			case Category.UnicodeNl:
				this.EmitUniCat(UnicodeCategory.LetterNumber, negate, reverse);
				return;
			case Category.UnicodeNo:
				this.EmitUniCat(UnicodeCategory.OtherNumber, negate, reverse);
				return;
			case Category.UnicodeZs:
				this.EmitUniCat(UnicodeCategory.SpaceSeparator, negate, reverse);
				return;
			case Category.UnicodeZl:
				this.EmitUniCat(UnicodeCategory.LineSeparator, negate, reverse);
				return;
			case Category.UnicodeZp:
				this.EmitUniCat(UnicodeCategory.ParagraphSeparator, negate, reverse);
				return;
			case Category.UnicodePd:
				this.EmitUniCat(UnicodeCategory.DashPunctuation, negate, reverse);
				return;
			case Category.UnicodePs:
				this.EmitUniCat(UnicodeCategory.OpenPunctuation, negate, reverse);
				return;
			case Category.UnicodePi:
				this.EmitUniCat(UnicodeCategory.InitialQuotePunctuation, negate, reverse);
				return;
			case Category.UnicodePe:
				this.EmitUniCat(UnicodeCategory.ClosePunctuation, negate, reverse);
				return;
			case Category.UnicodePf:
				this.EmitUniCat(UnicodeCategory.FinalQuotePunctuation, negate, reverse);
				return;
			case Category.UnicodePc:
				this.EmitUniCat(UnicodeCategory.ConnectorPunctuation, negate, reverse);
				return;
			case Category.UnicodePo:
				this.EmitUniCat(UnicodeCategory.OtherPunctuation, negate, reverse);
				return;
			case Category.UnicodeSm:
				this.EmitUniCat(UnicodeCategory.MathSymbol, negate, reverse);
				return;
			case Category.UnicodeSc:
				this.EmitUniCat(UnicodeCategory.CurrencySymbol, negate, reverse);
				return;
			case Category.UnicodeSk:
				this.EmitUniCat(UnicodeCategory.ModifierSymbol, negate, reverse);
				return;
			case Category.UnicodeSo:
				this.EmitUniCat(UnicodeCategory.OtherSymbol, negate, reverse);
				return;
			case Category.UnicodeCc:
				this.EmitUniCat(UnicodeCategory.Control, negate, reverse);
				return;
			case Category.UnicodeCf:
				this.EmitUniCat(UnicodeCategory.Format, negate, reverse);
				return;
			case Category.UnicodeCo:
				this.EmitUniCat(UnicodeCategory.PrivateUse, negate, reverse);
				return;
			case Category.UnicodeCs:
				this.EmitUniCat(UnicodeCategory.Surrogate, negate, reverse);
				return;
			case Category.UnicodeCn:
				this.EmitUniCat(UnicodeCategory.OtherNotAssigned, negate, reverse);
				return;
			case Category.UnicodeBasicLatin:
				this.EmitRange('\0', '\u007f', negate, false, reverse);
				return;
			case Category.UnicodeLatin1Supplement:
				this.EmitRange('\u0080', 'ÿ', negate, false, reverse);
				return;
			case Category.UnicodeLatinExtendedA:
				this.EmitRange('Ā', 'ſ', negate, false, reverse);
				return;
			case Category.UnicodeLatinExtendedB:
				this.EmitRange('ƀ', 'ɏ', negate, false, reverse);
				return;
			case Category.UnicodeIPAExtensions:
				this.EmitRange('ɐ', 'ʯ', negate, false, reverse);
				return;
			case Category.UnicodeSpacingModifierLetters:
				this.EmitRange('ʰ', '\u02ff', negate, false, reverse);
				return;
			case Category.UnicodeCombiningDiacriticalMarks:
				this.EmitRange('\u0300', '\u036f', negate, false, reverse);
				return;
			case Category.UnicodeGreek:
				this.EmitRange('Ͱ', 'Ͽ', negate, false, reverse);
				return;
			case Category.UnicodeCyrillic:
				this.EmitRange('Ѐ', 'ӿ', negate, false, reverse);
				return;
			case Category.UnicodeArmenian:
				this.EmitRange('\u0530', '֏', negate, false, reverse);
				return;
			case Category.UnicodeHebrew:
				this.EmitRange('\u0590', '\u05ff', negate, false, reverse);
				return;
			case Category.UnicodeArabic:
				this.EmitRange('\u0600', 'ۿ', negate, false, reverse);
				return;
			case Category.UnicodeSyriac:
				this.EmitRange('܀', 'ݏ', negate, false, reverse);
				return;
			case Category.UnicodeThaana:
				this.EmitRange('ހ', '\u07bf', negate, false, reverse);
				return;
			case Category.UnicodeDevanagari:
				this.EmitRange('\u0900', 'ॿ', negate, false, reverse);
				return;
			case Category.UnicodeBengali:
				this.EmitRange('ঀ', '\u09ff', negate, false, reverse);
				return;
			case Category.UnicodeGurmukhi:
				this.EmitRange('\u0a00', '\u0a7f', negate, false, reverse);
				return;
			case Category.UnicodeGujarati:
				this.EmitRange('\u0a80', '\u0aff', negate, false, reverse);
				return;
			case Category.UnicodeOriya:
				this.EmitRange('\u0b00', '\u0b7f', negate, false, reverse);
				return;
			case Category.UnicodeTamil:
				this.EmitRange('\u0b80', '\u0bff', negate, false, reverse);
				return;
			case Category.UnicodeTelugu:
				this.EmitRange('\u0c00', '౿', negate, false, reverse);
				return;
			case Category.UnicodeKannada:
				this.EmitRange('ಀ', '\u0cff', negate, false, reverse);
				return;
			case Category.UnicodeMalayalam:
				this.EmitRange('\u0d00', 'ൿ', negate, false, reverse);
				return;
			case Category.UnicodeSinhala:
				this.EmitRange('\u0d80', '\u0dff', negate, false, reverse);
				return;
			case Category.UnicodeThai:
				this.EmitRange('\u0e00', '\u0e7f', negate, false, reverse);
				return;
			case Category.UnicodeLao:
				this.EmitRange('\u0e80', '\u0eff', negate, false, reverse);
				return;
			case Category.UnicodeTibetan:
				this.EmitRange('ༀ', '\u0fff', negate, false, reverse);
				return;
			case Category.UnicodeMyanmar:
				this.EmitRange('က', '႟', negate, false, reverse);
				return;
			case Category.UnicodeGeorgian:
				this.EmitRange('Ⴀ', 'ჿ', negate, false, reverse);
				return;
			case Category.UnicodeHangulJamo:
				this.EmitRange('ᄀ', 'ᇿ', negate, false, reverse);
				return;
			case Category.UnicodeEthiopic:
				this.EmitRange('ሀ', '\u137f', negate, false, reverse);
				return;
			case Category.UnicodeCherokee:
				this.EmitRange('Ꭰ', '\u13ff', negate, false, reverse);
				return;
			case Category.UnicodeUnifiedCanadianAboriginalSyllabics:
				this.EmitRange('᐀', 'ᙿ', negate, false, reverse);
				return;
			case Category.UnicodeOgham:
				this.EmitRange('\u1680', '\u169f', negate, false, reverse);
				return;
			case Category.UnicodeRunic:
				this.EmitRange('ᚠ', '\u16ff', negate, false, reverse);
				return;
			case Category.UnicodeKhmer:
				this.EmitRange('ក', '\u17ff', negate, false, reverse);
				return;
			case Category.UnicodeMongolian:
				this.EmitRange('᠀', '\u18af', negate, false, reverse);
				return;
			case Category.UnicodeLatinExtendedAdditional:
				this.EmitRange('Ḁ', 'ỿ', negate, false, reverse);
				return;
			case Category.UnicodeGreekExtended:
				this.EmitRange('ἀ', '\u1fff', negate, false, reverse);
				return;
			case Category.UnicodeGeneralPunctuation:
				this.EmitRange('\u2000', '\u206f', negate, false, reverse);
				return;
			case Category.UnicodeSuperscriptsandSubscripts:
				this.EmitRange('⁰', '\u209f', negate, false, reverse);
				return;
			case Category.UnicodeCurrencySymbols:
				this.EmitRange('₠', '\u20cf', negate, false, reverse);
				return;
			case Category.UnicodeCombiningMarksforSymbols:
				this.EmitRange('\u20d0', '\u20ff', negate, false, reverse);
				return;
			case Category.UnicodeLetterlikeSymbols:
				this.EmitRange('℀', '⅏', negate, false, reverse);
				return;
			case Category.UnicodeNumberForms:
				this.EmitRange('⅐', '\u218f', negate, false, reverse);
				return;
			case Category.UnicodeArrows:
				this.EmitRange('←', '⇿', negate, false, reverse);
				return;
			case Category.UnicodeMathematicalOperators:
				this.EmitRange('∀', '⋿', negate, false, reverse);
				return;
			case Category.UnicodeMiscellaneousTechnical:
				this.EmitRange('⌀', '⏿', negate, false, reverse);
				return;
			case Category.UnicodeControlPictures:
				this.EmitRange('␀', '\u243f', negate, false, reverse);
				return;
			case Category.UnicodeOpticalCharacterRecognition:
				this.EmitRange('⑀', '\u245f', negate, false, reverse);
				return;
			case Category.UnicodeEnclosedAlphanumerics:
				this.EmitRange('①', '⓿', negate, false, reverse);
				return;
			case Category.UnicodeBoxDrawing:
				this.EmitRange('─', '╿', negate, false, reverse);
				return;
			case Category.UnicodeBlockElements:
				this.EmitRange('▀', '▟', negate, false, reverse);
				return;
			case Category.UnicodeGeometricShapes:
				this.EmitRange('■', '◿', negate, false, reverse);
				return;
			case Category.UnicodeMiscellaneousSymbols:
				this.EmitRange('☀', '⛿', negate, false, reverse);
				return;
			case Category.UnicodeDingbats:
				this.EmitRange('✀', '➿', negate, false, reverse);
				return;
			case Category.UnicodeBraillePatterns:
				this.EmitRange('⠀', '⣿', negate, false, reverse);
				return;
			case Category.UnicodeCJKRadicalsSupplement:
				this.EmitRange('⺀', '\u2eff', negate, false, reverse);
				return;
			case Category.UnicodeKangxiRadicals:
				this.EmitRange('⼀', '\u2fdf', negate, false, reverse);
				return;
			case Category.UnicodeIdeographicDescriptionCharacters:
				this.EmitRange('⿰', '⿿', negate, false, reverse);
				return;
			case Category.UnicodeCJKSymbolsandPunctuation:
				this.EmitRange('\u3000', '〿', negate, false, reverse);
				return;
			case Category.UnicodeHiragana:
				this.EmitRange('\u3040', 'ゟ', negate, false, reverse);
				return;
			case Category.UnicodeKatakana:
				this.EmitRange('゠', 'ヿ', negate, false, reverse);
				return;
			case Category.UnicodeBopomofo:
				this.EmitRange('\u3100', 'ㄯ', negate, false, reverse);
				return;
			case Category.UnicodeHangulCompatibilityJamo:
				this.EmitRange('\u3130', '\u318f', negate, false, reverse);
				return;
			case Category.UnicodeKanbun:
				this.EmitRange('㆐', '㆟', negate, false, reverse);
				return;
			case Category.UnicodeBopomofoExtended:
				this.EmitRange('ㆠ', 'ㆿ', negate, false, reverse);
				return;
			case Category.UnicodeEnclosedCJKLettersandMonths:
				this.EmitRange('㈀', '㋿', negate, false, reverse);
				return;
			case Category.UnicodeCJKCompatibility:
				this.EmitRange('㌀', '㏿', negate, false, reverse);
				return;
			case Category.UnicodeCJKUnifiedIdeographsExtensionA:
				this.EmitRange('㐀', '䶵', negate, false, reverse);
				return;
			case Category.UnicodeCJKUnifiedIdeographs:
				this.EmitRange('一', '鿿', negate, false, reverse);
				return;
			case Category.UnicodeYiSyllables:
				this.EmitRange('ꀀ', '\ua48f', negate, false, reverse);
				return;
			case Category.UnicodeYiRadicals:
				this.EmitRange('꒐', '\ua4cf', negate, false, reverse);
				return;
			case Category.UnicodeHangulSyllables:
				this.EmitRange('가', '힣', negate, false, reverse);
				return;
			case Category.UnicodeHighSurrogates:
				this.EmitRange('\ud800', '\udb7f', negate, false, reverse);
				return;
			case Category.UnicodeHighPrivateUseSurrogates:
				this.EmitRange('\udb80', '\udbff', negate, false, reverse);
				return;
			case Category.UnicodeLowSurrogates:
				this.EmitRange('\udc00', '\udfff', negate, false, reverse);
				return;
			case Category.UnicodePrivateUse:
				this.EmitRange('\ue000', '\uf8ff', negate, false, reverse);
				return;
			case Category.UnicodeCJKCompatibilityIdeographs:
				this.EmitRange('豈', '\ufaff', negate, false, reverse);
				return;
			case Category.UnicodeAlphabeticPresentationForms:
				this.EmitRange('ﬀ', 'ﭏ', negate, false, reverse);
				return;
			case Category.UnicodeArabicPresentationFormsA:
				this.EmitRange('ﭐ', '﷿', negate, false, reverse);
				return;
			case Category.UnicodeCombiningHalfMarks:
				this.EmitRange('\ufe20', '\ufe2f', negate, false, reverse);
				return;
			case Category.UnicodeCJKCompatibilityForms:
				this.EmitRange('︰', '\ufe4f', negate, false, reverse);
				return;
			case Category.UnicodeSmallFormVariants:
				this.EmitRange('﹐', '\ufe6f', negate, false, reverse);
				return;
			case Category.UnicodeArabicPresentationFormsB:
				this.EmitRange('ﹰ', '\ufefe', negate, false, reverse);
				return;
			case Category.UnicodeSpecials:
				this.EmitOpNegateReverse(RxOp.CategoryUnicodeSpecials, negate, reverse);
				return;
			case Category.UnicodeHalfwidthandFullwidthForms:
				this.EmitRange('\uff00', '\uffef', negate, false, reverse);
				return;
			}
			throw new NotImplementedException("Missing category: " + cat);
		}

		public void EmitNotCategory(Category cat, bool negate, bool reverse)
		{
			if (negate)
			{
				this.EmitCategory(cat, false, reverse);
			}
			else
			{
				this.EmitCategory(cat, true, reverse);
			}
		}

		public void EmitRange(char lo, char hi, bool negate, bool ignore, bool reverse)
		{
			if (lo < 'Ā' && hi < 'Ā')
			{
				this.EmitOp(RxOp.Range, negate, ignore, reverse);
				this.Emit((byte)lo);
				this.Emit((byte)hi);
			}
			else
			{
				this.EmitOp(RxOp.UnicodeRange, negate, ignore, reverse);
				this.Emit((ushort)lo);
				this.Emit((ushort)hi);
			}
		}

		public void EmitSet(char lo, BitArray set, bool negate, bool ignore, bool reverse)
		{
			int num = set.Length + 7 >> 3;
			if (lo < 'Ā' && num < 256)
			{
				this.EmitOp(RxOp.Bitmap, negate, ignore, reverse);
				this.Emit((byte)lo);
				this.Emit((byte)num);
			}
			else
			{
				this.EmitOp(RxOp.UnicodeBitmap, negate, ignore, reverse);
				this.Emit((ushort)lo);
				this.Emit((ushort)num);
			}
			int num2 = 0;
			while (num-- != 0)
			{
				int num3 = 0;
				for (int i = 0; i < 8; i++)
				{
					if (num2 >= set.Length)
					{
						break;
					}
					if (set[num2++])
					{
						num3 |= 1 << i;
					}
				}
				this.Emit((byte)num3);
			}
		}

		public void EmitString(string str, bool ignore, bool reverse)
		{
			bool flag = false;
			int num = 0;
			if (ignore)
			{
				num++;
			}
			if (reverse)
			{
				num += 2;
			}
			if (ignore)
			{
				str = str.ToLower();
			}
			if (str.Length < 256)
			{
				flag = true;
				for (int i = 0; i < str.Length; i++)
				{
					if (str[i] >= 'Ā')
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.EmitOpIgnoreReverse(RxOp.String, ignore, reverse);
				this.Emit((byte)str.Length);
				for (int i = 0; i < str.Length; i++)
				{
					this.Emit((byte)str[i]);
				}
			}
			else
			{
				this.EmitOpIgnoreReverse(RxOp.UnicodeString, ignore, reverse);
				if (str.Length > 65535)
				{
					throw new NotSupportedException();
				}
				this.Emit((ushort)str.Length);
				for (int i = 0; i < str.Length; i++)
				{
					this.Emit((ushort)str[i]);
				}
			}
		}

		public void EmitPosition(Position pos)
		{
			switch (pos)
			{
			case Position.Any:
				this.Emit(RxOp.AnyPosition);
				break;
			case Position.Start:
				this.Emit(RxOp.StartOfString);
				break;
			case Position.StartOfString:
				this.Emit(RxOp.StartOfString);
				break;
			case Position.StartOfLine:
				this.Emit(RxOp.StartOfLine);
				break;
			case Position.StartOfScan:
				this.Emit(RxOp.StartOfScan);
				break;
			case Position.End:
				this.Emit(RxOp.End);
				break;
			case Position.EndOfString:
				this.Emit(RxOp.EndOfString);
				break;
			case Position.EndOfLine:
				this.Emit(RxOp.EndOfLine);
				break;
			case Position.Boundary:
				this.Emit(RxOp.WordBoundary);
				break;
			case Position.NonBoundary:
				this.Emit(RxOp.NoWordBoundary);
				break;
			default:
				throw new NotSupportedException();
			}
		}

		public void EmitOpen(int gid)
		{
			if (gid > 65535)
			{
				throw new NotSupportedException();
			}
			this.Emit(RxOp.OpenGroup);
			this.Emit((ushort)gid);
		}

		public void EmitClose(int gid)
		{
			if (gid > 65535)
			{
				throw new NotSupportedException();
			}
			this.Emit(RxOp.CloseGroup);
			this.Emit((ushort)gid);
		}

		public void EmitBalanceStart(int gid, int balance, bool capture, LinkRef tail)
		{
			this.BeginLink(tail);
			this.Emit(RxOp.BalanceStart);
			this.Emit((ushort)gid);
			this.Emit((ushort)balance);
			this.Emit((!capture) ? 0 : 1);
			this.EmitLink(tail);
		}

		public void EmitBalance()
		{
			this.Emit(RxOp.Balance);
		}

		public void EmitReference(int gid, bool ignore, bool reverse)
		{
			if (gid > 65535)
			{
				throw new NotSupportedException();
			}
			this.EmitOpIgnoreReverse(RxOp.Reference, ignore, reverse);
			this.Emit((ushort)gid);
		}

		public void EmitIfDefined(int gid, LinkRef tail)
		{
			if (gid > 65535)
			{
				throw new NotSupportedException();
			}
			this.BeginLink(tail);
			this.Emit(RxOp.IfDefined);
			this.EmitLink(tail);
			this.Emit((ushort)gid);
		}

		public void EmitSub(LinkRef tail)
		{
			this.BeginLink(tail);
			this.Emit(RxOp.SubExpression);
			this.EmitLink(tail);
		}

		public void EmitTest(LinkRef yes, LinkRef tail)
		{
			this.BeginLink(yes);
			this.BeginLink(tail);
			this.Emit(RxOp.Test);
			this.EmitLink(yes);
			this.EmitLink(tail);
		}

		public void EmitBranch(LinkRef next)
		{
			this.BeginLink(next);
			this.Emit(RxOp.Branch);
			this.EmitLink(next);
		}

		public void EmitJump(LinkRef target)
		{
			this.BeginLink(target);
			this.Emit(RxOp.Jump);
			this.EmitLink(target);
		}

		public void EmitIn(LinkRef tail)
		{
			this.BeginLink(tail);
			this.Emit(RxOp.TestCharGroup);
			this.EmitLink(tail);
		}

		public void EmitRepeat(int min, int max, bool lazy, LinkRef until)
		{
			this.BeginLink(until);
			this.Emit((!lazy) ? RxOp.Repeat : RxOp.RepeatLazy);
			this.EmitLink(until);
			this.Emit(min);
			this.Emit(max);
		}

		public void EmitUntil(LinkRef repeat)
		{
			this.ResolveLink(repeat);
			this.Emit(RxOp.Until);
		}

		public void EmitInfo(int count, int min, int max)
		{
			this.Emit(RxOp.Info);
			if (count > 65535)
			{
				throw new NotSupportedException();
			}
			this.Emit((ushort)count);
			this.Emit(min);
			this.Emit(max);
		}

		public void EmitFastRepeat(int min, int max, bool lazy, LinkRef tail)
		{
			this.BeginLink(tail);
			this.Emit((!lazy) ? RxOp.FastRepeat : RxOp.FastRepeatLazy);
			this.EmitLink(tail);
			this.Emit(min);
			this.Emit(max);
		}

		public void EmitAnchor(bool reverse, int offset, LinkRef tail)
		{
			this.BeginLink(tail);
			if (reverse)
			{
				this.Emit(RxOp.AnchorReverse);
			}
			else
			{
				this.Emit(RxOp.Anchor);
			}
			this.EmitLink(tail);
			if (offset > 65535)
			{
				throw new NotSupportedException();
			}
			this.Emit((ushort)offset);
		}

		public void EmitBranchEnd()
		{
		}

		public void EmitAlternationEnd()
		{
		}

		public LinkRef NewLink()
		{
			return new RxLinkRef();
		}

		public void ResolveLink(LinkRef link)
		{
			RxLinkRef rxLinkRef = link as RxLinkRef;
			for (int i = 0; i < rxLinkRef.current; i += 2)
			{
				int num = this.curpos - rxLinkRef.offsets[i];
				if (num > 65535)
				{
					throw new NotSupportedException();
				}
				int num2 = rxLinkRef.offsets[i + 1];
				this.program[num2] = (byte)num;
				this.program[num2 + 1] = (byte)(num >> 8);
			}
		}

		protected byte[] program = new byte[32];

		protected int curpos;
	}
}
