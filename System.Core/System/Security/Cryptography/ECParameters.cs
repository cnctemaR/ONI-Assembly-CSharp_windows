using System;

namespace System.Security.Cryptography
{
	public struct ECParameters
	{
		public void Validate()
		{
			bool flag = false;
			if (this.Q.X == null || this.Q.Y == null || this.Q.X.Length != this.Q.Y.Length)
			{
				flag = true;
			}
			if (!flag)
			{
				if (this.Curve.IsExplicit)
				{
					flag = this.D != null && this.D.Length != this.Curve.Order.Length;
				}
				else if (this.Curve.IsNamed)
				{
					flag = this.D != null && this.D.Length != this.Q.X.Length;
				}
			}
			if (flag)
			{
				throw new CryptographicException("The specified key parameters are not valid. Q.X and Q.Y are required fields. Q.X, Q.Y must be the same length. If D is specified it must be the same length as Q.X and Q.Y for named curves or the same length as Order for explicit curves.");
			}
			this.Curve.Validate();
		}

		public ECPoint Q;

		public byte[] D;

		public ECCurve Curve;
	}
}
