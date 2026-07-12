using System;

namespace System.Security.Cryptography.Asn1
{
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class UtcTimeAttribute : AsnTypeAttribute
	{
		public int TwoDigitYearMax { get; set; }
	}
}
