using System;

namespace System.Security.Cryptography.Asn1
{
	internal enum UniversalTagNumber
	{
		EndOfContents,
		Boolean,
		Integer,
		BitString,
		OctetString,
		Null,
		ObjectIdentifier,
		ObjectDescriptor,
		External,
		InstanceOf = 8,
		Real,
		Enumerated,
		Embedded,
		UTF8String,
		RelativeObjectIdentifier,
		Time,
		Sequence = 16,
		SequenceOf = 16,
		Set,
		SetOf = 17,
		NumericString,
		PrintableString,
		TeletexString,
		T61String = 20,
		VideotexString,
		IA5String,
		UtcTime,
		GeneralizedTime,
		GraphicString,
		VisibleString,
		ISO646String = 26,
		GeneralString,
		UniversalString,
		UnrestrictedCharacterString,
		BMPString,
		Date,
		TimeOfDay,
		DateTime,
		Duration,
		ObjectIdentifierIRI,
		RelativeObjectIdentifierIRI
	}
}
