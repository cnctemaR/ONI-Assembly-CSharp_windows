using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct AlgorithmIdentifierAsn
	{
		internal bool Equals(ref AlgorithmIdentifierAsn other)
		{
			if (this.Algorithm.Value != other.Algorithm.Value)
			{
				return false;
			}
			bool flag = AlgorithmIdentifierAsn.RepresentsNull(this.Parameters);
			bool flag2 = AlgorithmIdentifierAsn.RepresentsNull(other.Parameters);
			return flag == flag2 && (flag || this.Parameters.Value.Span.SequenceEqual<byte>(other.Parameters.Value.Span));
		}

		private unsafe static bool RepresentsNull(ReadOnlyMemory<byte>? parameters)
		{
			if (parameters == null)
			{
				return true;
			}
			ReadOnlySpan<byte> span = parameters.Value.Span;
			return span.Length == 2 && *span[0] == 5 && *span[1] == 0;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AlgorithmIdentifierAsn()
		{
			byte[] array = new byte[2];
			array[0] = 5;
			AlgorithmIdentifierAsn.ExplicitDerNull = array;
		}

		internal static readonly ReadOnlyMemory<byte> ExplicitDerNull;

		[ObjectIdentifier(PopulateFriendlyName = true)]
		public Oid Algorithm;

		[OptionalValue]
		[AnyValue]
		public ReadOnlyMemory<byte>? Parameters;
	}
}
