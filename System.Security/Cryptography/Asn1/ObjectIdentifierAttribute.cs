using System;

namespace System.Security.Cryptography.Asn1
{
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class ObjectIdentifierAttribute : AsnTypeAttribute
	{
		public bool PopulateFriendlyName { get; set; }
	}
}
