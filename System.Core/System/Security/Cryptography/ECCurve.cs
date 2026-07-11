using System;

namespace System.Security.Cryptography
{
	public struct ECCurve
	{
		public bool IsCharacteristic2
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsExplicit
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsNamed
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsPrime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Oid Oid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public static ECCurve CreateFromFriendlyName(string oidFriendlyName)
		{
			throw new NotImplementedException();
		}

		public static ECCurve CreateFromOid(Oid curveOid)
		{
			throw new NotImplementedException();
		}

		public static ECCurve CreateFromValue(string oidValue)
		{
			throw new NotImplementedException();
		}

		public void Validate()
		{
			throw new NotImplementedException();
		}

		public byte[] A;

		public byte[] B;

		public byte[] Cofactor;

		public ECCurve.ECCurveType CurveType;

		public ECPoint G;

		public HashAlgorithmName? Hash;

		public byte[] Order;

		public byte[] Polynomial;

		public byte[] Prime;

		public byte[] Seed;

		public enum ECCurveType
		{
			Implicit,
			PrimeShortWeierstrass,
			PrimeTwistedEdwards,
			PrimeMontgomery,
			Characteristic2,
			Named
		}

		public static class NamedCurves
		{
			public static ECCurve brainpoolP160r1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP160t1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP192r1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP192t1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP224r1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP224t1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP256r1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP256t1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP320r1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP320t1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP384r1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP384t1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP512r1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve brainpoolP512t1
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve nistP256
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve nistP384
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public static ECCurve nistP521
			{
				get
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
