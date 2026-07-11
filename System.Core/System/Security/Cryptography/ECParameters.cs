using System;

namespace System.Security.Cryptography
{
	public struct ECParameters
	{
		public void Validate()
		{
			throw new NotImplementedException();
		}

		public ECCurve Curve;

		public byte[] D;

		public ECPoint Q;
	}
}
