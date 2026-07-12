using System;
using System.Threading;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9SigningTime : Pkcs9AttributeObject
	{
		public Pkcs9SigningTime()
			: this(DateTime.Now)
		{
		}

		public Pkcs9SigningTime(DateTime signingTime)
			: base("1.2.840.113549.1.9.5", Pkcs9SigningTime.Encode(signingTime))
		{
			this._lazySigningTime = new DateTime?(signingTime);
		}

		public Pkcs9SigningTime(byte[] encodedSigningTime)
			: base("1.2.840.113549.1.9.5", encodedSigningTime)
		{
		}

		public DateTime SigningTime
		{
			get
			{
				if (this._lazySigningTime == null)
				{
					this._lazySigningTime = new DateTime?(Pkcs9SigningTime.Decode(base.RawData));
					Interlocked.MemoryBarrier();
				}
				return this._lazySigningTime.Value;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			base.CopyFrom(asnEncodedData);
			this._lazySigningTime = null;
		}

		private static DateTime Decode(byte[] rawData)
		{
			if (rawData == null)
			{
				return default(DateTime);
			}
			return PkcsPal.Instance.DecodeUtcTime(rawData);
		}

		private static byte[] Encode(DateTime signingTime)
		{
			return PkcsPal.Instance.EncodeUtcTime(signingTime);
		}

		private DateTime? _lazySigningTime;
	}
}
