using System;

namespace System.Xml.Xsl.Runtime
{
	internal enum NumberingSequence
	{
		Nil = -1,
		FirstDecimal,
		Arabic = 0,
		DArabic,
		Hindi3,
		Thai2,
		FEDecimal,
		KorDbNum1,
		LastNum = 5,
		FirstAlpha,
		UCLetter = 6,
		LCLetter,
		UCRus,
		LCRus,
		Thai1,
		Hindi1,
		Hindi2,
		Aiueo,
		DAiueo,
		Iroha,
		DIroha,
		DChosung,
		Ganada,
		ArabicScript,
		LastAlpha = 19,
		FirstSpecial,
		UCRoman = 20,
		LCRoman,
		Hebrew,
		DbNum3,
		ChnCmplx,
		KorDbNum3,
		Zodiac1,
		Zodiac2,
		Zodiac3,
		LastSpecial = 28
	}
}
