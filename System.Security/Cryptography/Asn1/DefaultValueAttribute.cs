using System;

namespace System.Security.Cryptography.Asn1
{
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class DefaultValueAttribute : AsnEncodingRuleAttribute
	{
		internal byte[] EncodedBytes { get; }

		public DefaultValueAttribute(params byte[] encodedValue)
		{
			this.EncodedBytes = encodedValue;
		}

		public ReadOnlyMemory<byte> EncodedValue
		{
			get
			{
				return this.EncodedBytes;
			}
		}
	}
}
