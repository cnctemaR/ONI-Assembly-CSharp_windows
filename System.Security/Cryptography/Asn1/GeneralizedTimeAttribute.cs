using System;

namespace System.Security.Cryptography.Asn1
{
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class GeneralizedTimeAttribute : AsnTypeAttribute
	{
		public bool DisallowFractions { get; set; }
	}
}
