using System;
using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal class CategoryUtils
	{
		public static Category CategoryFromName(string name)
		{
			Category category;
			try
			{
				if (name.StartsWith("Is"))
				{
					name = name.Substring(2);
				}
				category = (Category)((ushort)Enum.Parse(typeof(Category), "Unicode" + name, false));
			}
			catch (ArgumentException)
			{
				category = Category.None;
			}
			return category;
		}

		public static bool IsCategory(Category cat, char c)
		{
			switch (cat)
			{
			case Category.None:
				return false;
			case Category.Any:
				return c != '\n';
			case Category.AnySingleline:
				return true;
			case Category.Word:
				return char.IsLetterOrDigit(c) || CategoryUtils.IsCategory(UnicodeCategory.ConnectorPunctuation, c);
			case Category.Digit:
				return char.IsDigit(c);
			case Category.WhiteSpace:
				return char.IsWhiteSpace(c);
			case Category.EcmaAny:
				return c != '\n';
			case Category.EcmaAnySingleline:
				return true;
			case Category.EcmaWord:
				return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || ('0' <= c && c <= '9') || '_' == c;
			case Category.EcmaDigit:
				return '0' <= c && c <= '9';
			case Category.EcmaWhiteSpace:
				return c == ' ' || c == '\f' || c == '\n' || c == '\r' || c == '\t' || c == '\v';
			case Category.UnicodeL:
				return CategoryUtils.IsCategory(UnicodeCategory.UppercaseLetter, c) || CategoryUtils.IsCategory(UnicodeCategory.LowercaseLetter, c) || CategoryUtils.IsCategory(UnicodeCategory.TitlecaseLetter, c) || CategoryUtils.IsCategory(UnicodeCategory.ModifierLetter, c) || CategoryUtils.IsCategory(UnicodeCategory.OtherLetter, c);
			case Category.UnicodeM:
				return CategoryUtils.IsCategory(UnicodeCategory.NonSpacingMark, c) || CategoryUtils.IsCategory(UnicodeCategory.EnclosingMark, c) || CategoryUtils.IsCategory(UnicodeCategory.SpacingCombiningMark, c);
			case Category.UnicodeN:
				return CategoryUtils.IsCategory(UnicodeCategory.DecimalDigitNumber, c) || CategoryUtils.IsCategory(UnicodeCategory.LetterNumber, c) || CategoryUtils.IsCategory(UnicodeCategory.OtherNumber, c);
			case Category.UnicodeZ:
				return CategoryUtils.IsCategory(UnicodeCategory.SpaceSeparator, c) || CategoryUtils.IsCategory(UnicodeCategory.LineSeparator, c) || CategoryUtils.IsCategory(UnicodeCategory.ParagraphSeparator, c);
			case Category.UnicodeP:
				return CategoryUtils.IsCategory(UnicodeCategory.DashPunctuation, c) || CategoryUtils.IsCategory(UnicodeCategory.OpenPunctuation, c) || CategoryUtils.IsCategory(UnicodeCategory.InitialQuotePunctuation, c) || CategoryUtils.IsCategory(UnicodeCategory.ClosePunctuation, c) || CategoryUtils.IsCategory(UnicodeCategory.FinalQuotePunctuation, c) || CategoryUtils.IsCategory(UnicodeCategory.ConnectorPunctuation, c) || CategoryUtils.IsCategory(UnicodeCategory.OtherPunctuation, c);
			case Category.UnicodeS:
				return CategoryUtils.IsCategory(UnicodeCategory.MathSymbol, c) || CategoryUtils.IsCategory(UnicodeCategory.CurrencySymbol, c) || CategoryUtils.IsCategory(UnicodeCategory.ModifierSymbol, c) || CategoryUtils.IsCategory(UnicodeCategory.OtherSymbol, c);
			case Category.UnicodeC:
				return CategoryUtils.IsCategory(UnicodeCategory.Control, c) || CategoryUtils.IsCategory(UnicodeCategory.Format, c) || CategoryUtils.IsCategory(UnicodeCategory.PrivateUse, c) || CategoryUtils.IsCategory(UnicodeCategory.Surrogate, c) || CategoryUtils.IsCategory(UnicodeCategory.OtherNotAssigned, c);
			case Category.UnicodeLu:
				return CategoryUtils.IsCategory(UnicodeCategory.UppercaseLetter, c);
			case Category.UnicodeLl:
				return CategoryUtils.IsCategory(UnicodeCategory.LowercaseLetter, c);
			case Category.UnicodeLt:
				return CategoryUtils.IsCategory(UnicodeCategory.TitlecaseLetter, c);
			case Category.UnicodeLm:
				return CategoryUtils.IsCategory(UnicodeCategory.ModifierLetter, c);
			case Category.UnicodeLo:
				return CategoryUtils.IsCategory(UnicodeCategory.OtherLetter, c);
			case Category.UnicodeMn:
				return CategoryUtils.IsCategory(UnicodeCategory.NonSpacingMark, c);
			case Category.UnicodeMe:
				return CategoryUtils.IsCategory(UnicodeCategory.EnclosingMark, c);
			case Category.UnicodeMc:
				return CategoryUtils.IsCategory(UnicodeCategory.SpacingCombiningMark, c);
			case Category.UnicodeNd:
				return CategoryUtils.IsCategory(UnicodeCategory.DecimalDigitNumber, c);
			case Category.UnicodeNl:
				return CategoryUtils.IsCategory(UnicodeCategory.LetterNumber, c);
			case Category.UnicodeNo:
				return CategoryUtils.IsCategory(UnicodeCategory.OtherNumber, c);
			case Category.UnicodeZs:
				return CategoryUtils.IsCategory(UnicodeCategory.SpaceSeparator, c);
			case Category.UnicodeZl:
				return CategoryUtils.IsCategory(UnicodeCategory.LineSeparator, c);
			case Category.UnicodeZp:
				return CategoryUtils.IsCategory(UnicodeCategory.ParagraphSeparator, c);
			case Category.UnicodePd:
				return CategoryUtils.IsCategory(UnicodeCategory.DashPunctuation, c);
			case Category.UnicodePs:
				return CategoryUtils.IsCategory(UnicodeCategory.OpenPunctuation, c);
			case Category.UnicodePi:
				return CategoryUtils.IsCategory(UnicodeCategory.InitialQuotePunctuation, c);
			case Category.UnicodePe:
				return CategoryUtils.IsCategory(UnicodeCategory.ClosePunctuation, c);
			case Category.UnicodePf:
				return CategoryUtils.IsCategory(UnicodeCategory.FinalQuotePunctuation, c);
			case Category.UnicodePc:
				return CategoryUtils.IsCategory(UnicodeCategory.ConnectorPunctuation, c);
			case Category.UnicodePo:
				return CategoryUtils.IsCategory(UnicodeCategory.OtherPunctuation, c);
			case Category.UnicodeSm:
				return CategoryUtils.IsCategory(UnicodeCategory.MathSymbol, c);
			case Category.UnicodeSc:
				return CategoryUtils.IsCategory(UnicodeCategory.CurrencySymbol, c);
			case Category.UnicodeSk:
				return CategoryUtils.IsCategory(UnicodeCategory.ModifierSymbol, c);
			case Category.UnicodeSo:
				return CategoryUtils.IsCategory(UnicodeCategory.OtherSymbol, c);
			case Category.UnicodeCc:
				return CategoryUtils.IsCategory(UnicodeCategory.Control, c);
			case Category.UnicodeCf:
				return CategoryUtils.IsCategory(UnicodeCategory.Format, c);
			case Category.UnicodeCo:
				return CategoryUtils.IsCategory(UnicodeCategory.PrivateUse, c);
			case Category.UnicodeCs:
				return CategoryUtils.IsCategory(UnicodeCategory.Surrogate, c);
			case Category.UnicodeCn:
				return CategoryUtils.IsCategory(UnicodeCategory.OtherNotAssigned, c);
			case Category.UnicodeBasicLatin:
				return '\0' <= c && c <= '\u007f';
			case Category.UnicodeLatin1Supplement:
				return '\u0080' <= c && c <= 'ÿ';
			case Category.UnicodeLatinExtendedA:
				return 'Ā' <= c && c <= 'ſ';
			case Category.UnicodeLatinExtendedB:
				return 'ƀ' <= c && c <= 'ɏ';
			case Category.UnicodeIPAExtensions:
				return 'ɐ' <= c && c <= 'ʯ';
			case Category.UnicodeSpacingModifierLetters:
				return 'ʰ' <= c && c <= '\u02ff';
			case Category.UnicodeCombiningDiacriticalMarks:
				return '\u0300' <= c && c <= '\u036f';
			case Category.UnicodeGreek:
				return 'Ͱ' <= c && c <= 'Ͽ';
			case Category.UnicodeCyrillic:
				return 'Ѐ' <= c && c <= 'ӿ';
			case Category.UnicodeArmenian:
				return '\u0530' <= c && c <= '֏';
			case Category.UnicodeHebrew:
				return '\u0590' <= c && c <= '\u05ff';
			case Category.UnicodeArabic:
				return '\u0600' <= c && c <= 'ۿ';
			case Category.UnicodeSyriac:
				return '܀' <= c && c <= 'ݏ';
			case Category.UnicodeThaana:
				return 'ހ' <= c && c <= '\u07bf';
			case Category.UnicodeDevanagari:
				return '\u0900' <= c && c <= 'ॿ';
			case Category.UnicodeBengali:
				return 'ঀ' <= c && c <= '\u09ff';
			case Category.UnicodeGurmukhi:
				return '\u0a00' <= c && c <= '\u0a7f';
			case Category.UnicodeGujarati:
				return '\u0a80' <= c && c <= '\u0aff';
			case Category.UnicodeOriya:
				return '\u0b00' <= c && c <= '\u0b7f';
			case Category.UnicodeTamil:
				return '\u0b80' <= c && c <= '\u0bff';
			case Category.UnicodeTelugu:
				return '\u0c00' <= c && c <= '౿';
			case Category.UnicodeKannada:
				return 'ಀ' <= c && c <= '\u0cff';
			case Category.UnicodeMalayalam:
				return '\u0d00' <= c && c <= 'ൿ';
			case Category.UnicodeSinhala:
				return '\u0d80' <= c && c <= '\u0dff';
			case Category.UnicodeThai:
				return '\u0e00' <= c && c <= '\u0e7f';
			case Category.UnicodeLao:
				return '\u0e80' <= c && c <= '\u0eff';
			case Category.UnicodeTibetan:
				return 'ༀ' <= c && c <= '\u0fff';
			case Category.UnicodeMyanmar:
				return 'က' <= c && c <= '႟';
			case Category.UnicodeGeorgian:
				return 'Ⴀ' <= c && c <= 'ჿ';
			case Category.UnicodeHangulJamo:
				return 'ᄀ' <= c && c <= 'ᇿ';
			case Category.UnicodeEthiopic:
				return 'ሀ' <= c && c <= '\u137f';
			case Category.UnicodeCherokee:
				return 'Ꭰ' <= c && c <= '\u13ff';
			case Category.UnicodeUnifiedCanadianAboriginalSyllabics:
				return '᐀' <= c && c <= 'ᙿ';
			case Category.UnicodeOgham:
				return '\u1680' <= c && c <= '\u169f';
			case Category.UnicodeRunic:
				return 'ᚠ' <= c && c <= '\u16ff';
			case Category.UnicodeKhmer:
				return 'ក' <= c && c <= '\u17ff';
			case Category.UnicodeMongolian:
				return '᠀' <= c && c <= '\u18af';
			case Category.UnicodeLatinExtendedAdditional:
				return 'Ḁ' <= c && c <= 'ỿ';
			case Category.UnicodeGreekExtended:
				return 'ἀ' <= c && c <= '\u1fff';
			case Category.UnicodeGeneralPunctuation:
				return '\u2000' <= c && c <= '\u206f';
			case Category.UnicodeSuperscriptsandSubscripts:
				return '⁰' <= c && c <= '\u209f';
			case Category.UnicodeCurrencySymbols:
				return '₠' <= c && c <= '\u20cf';
			case Category.UnicodeCombiningMarksforSymbols:
				return '\u20d0' <= c && c <= '\u20ff';
			case Category.UnicodeLetterlikeSymbols:
				return '℀' <= c && c <= '⅏';
			case Category.UnicodeNumberForms:
				return '⅐' <= c && c <= '\u218f';
			case Category.UnicodeArrows:
				return '←' <= c && c <= '⇿';
			case Category.UnicodeMathematicalOperators:
				return '∀' <= c && c <= '⋿';
			case Category.UnicodeMiscellaneousTechnical:
				return '⌀' <= c && c <= '⏿';
			case Category.UnicodeControlPictures:
				return '␀' <= c && c <= '\u243f';
			case Category.UnicodeOpticalCharacterRecognition:
				return '⑀' <= c && c <= '\u245f';
			case Category.UnicodeEnclosedAlphanumerics:
				return '①' <= c && c <= '⓿';
			case Category.UnicodeBoxDrawing:
				return '─' <= c && c <= '╿';
			case Category.UnicodeBlockElements:
				return '▀' <= c && c <= '▟';
			case Category.UnicodeGeometricShapes:
				return '■' <= c && c <= '◿';
			case Category.UnicodeMiscellaneousSymbols:
				return '☀' <= c && c <= '⛿';
			case Category.UnicodeDingbats:
				return '✀' <= c && c <= '➿';
			case Category.UnicodeBraillePatterns:
				return '⠀' <= c && c <= '⣿';
			case Category.UnicodeCJKRadicalsSupplement:
				return '⺀' <= c && c <= '\u2eff';
			case Category.UnicodeKangxiRadicals:
				return '⼀' <= c && c <= '\u2fdf';
			case Category.UnicodeIdeographicDescriptionCharacters:
				return '⿰' <= c && c <= '⿿';
			case Category.UnicodeCJKSymbolsandPunctuation:
				return '\u3000' <= c && c <= '〿';
			case Category.UnicodeHiragana:
				return '\u3040' <= c && c <= 'ゟ';
			case Category.UnicodeKatakana:
				return '゠' <= c && c <= 'ヿ';
			case Category.UnicodeBopomofo:
				return '\u3100' <= c && c <= 'ㄯ';
			case Category.UnicodeHangulCompatibilityJamo:
				return '\u3130' <= c && c <= '\u318f';
			case Category.UnicodeKanbun:
				return '㆐' <= c && c <= '㆟';
			case Category.UnicodeBopomofoExtended:
				return 'ㆠ' <= c && c <= 'ㆿ';
			case Category.UnicodeEnclosedCJKLettersandMonths:
				return '㈀' <= c && c <= '㋿';
			case Category.UnicodeCJKCompatibility:
				return '㌀' <= c && c <= '㏿';
			case Category.UnicodeCJKUnifiedIdeographsExtensionA:
				return '㐀' <= c && c <= '䶵';
			case Category.UnicodeCJKUnifiedIdeographs:
				return '一' <= c && c <= '鿿';
			case Category.UnicodeYiSyllables:
				return 'ꀀ' <= c && c <= '\ua48f';
			case Category.UnicodeYiRadicals:
				return '꒐' <= c && c <= '\ua4cf';
			case Category.UnicodeHangulSyllables:
				return '가' <= c && c <= '힣';
			case Category.UnicodeHighSurrogates:
				return '\ud800' <= c && c <= '\udb7f';
			case Category.UnicodeHighPrivateUseSurrogates:
				return '\udb80' <= c && c <= '\udbff';
			case Category.UnicodeLowSurrogates:
				return '\udc00' <= c && c <= '\udfff';
			case Category.UnicodePrivateUse:
				return '\ue000' <= c && c <= '\uf8ff';
			case Category.UnicodeCJKCompatibilityIdeographs:
				return '豈' <= c && c <= '\ufaff';
			case Category.UnicodeAlphabeticPresentationForms:
				return 'ﬀ' <= c && c <= 'ﭏ';
			case Category.UnicodeArabicPresentationFormsA:
				return 'ﭐ' <= c && c <= '﷿';
			case Category.UnicodeCombiningHalfMarks:
				return '\ufe20' <= c && c <= '\ufe2f';
			case Category.UnicodeCJKCompatibilityForms:
				return '︰' <= c && c <= '\ufe4f';
			case Category.UnicodeSmallFormVariants:
				return '﹐' <= c && c <= '\ufe6f';
			case Category.UnicodeArabicPresentationFormsB:
				return 'ﹰ' <= c && c <= '\ufefe';
			case Category.UnicodeSpecials:
				return ('\ufeff' <= c && c <= '\ufeff') || ('\ufff0' <= c && c <= '\ufffd');
			case Category.UnicodeHalfwidthandFullwidthForms:
				return '\uff00' <= c && c <= '\uffef';
			case Category.UnicodeOldItalic:
			case Category.UnicodeGothic:
			case Category.UnicodeDeseret:
			case Category.UnicodeByzantineMusicalSymbols:
			case Category.UnicodeMusicalSymbols:
			case Category.UnicodeMathematicalAlphanumericSymbols:
			case Category.UnicodeCJKUnifiedIdeographsExtensionB:
			case Category.UnicodeCJKCompatibilityIdeographsSupplement:
			case Category.UnicodeTags:
				return false;
			default:
				return false;
			}
		}

		private static bool IsCategory(UnicodeCategory uc, char c)
		{
			return char.GetUnicodeCategory(c) == uc;
		}
	}
}
