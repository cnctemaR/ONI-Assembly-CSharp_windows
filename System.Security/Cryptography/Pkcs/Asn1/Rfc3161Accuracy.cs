using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct Rfc3161Accuracy
	{
		internal Rfc3161Accuracy(long accuracyInMicroseconds)
		{
			if (accuracyInMicroseconds < 0L)
			{
				throw new ArgumentOutOfRangeException("accuracyInMicroseconds");
			}
			long num2;
			long num3;
			long num = Math.DivRem(Math.DivRem(accuracyInMicroseconds, 1000L, out num2), 1000L, out num3);
			if (num != 0L)
			{
				this.Seconds = new int?(checked((int)num));
			}
			else
			{
				this.Seconds = null;
			}
			if (num3 != 0L)
			{
				this.Millis = new int?((int)num3);
			}
			else
			{
				this.Millis = null;
			}
			if (num2 != 0L)
			{
				this.Micros = new int?((int)num2);
				return;
			}
			this.Micros = null;
		}

		internal long TotalMicros
		{
			get
			{
				return 1000000L * (long)this.Seconds.GetValueOrDefault() + 1000L * (long)this.Millis.GetValueOrDefault() + (long)this.Micros.GetValueOrDefault();
			}
		}

		[OptionalValue]
		internal int? Seconds;

		[OptionalValue]
		[ExpectedTag(0)]
		internal int? Millis;

		[ExpectedTag(1)]
		[OptionalValue]
		internal int? Micros;
	}
}
