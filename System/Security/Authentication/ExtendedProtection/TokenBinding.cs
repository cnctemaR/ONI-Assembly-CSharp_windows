using System;

namespace System.Security.Authentication.ExtendedProtection
{
	public class TokenBinding
	{
		internal TokenBinding(TokenBindingType bindingType, byte[] rawData)
		{
			this.BindingType = bindingType;
			this._rawTokenBindingId = rawData;
		}

		public byte[] GetRawTokenBindingId()
		{
			if (this._rawTokenBindingId == null)
			{
				return null;
			}
			return (byte[])this._rawTokenBindingId.Clone();
		}

		public TokenBindingType BindingType { get; private set; }

		private byte[] _rawTokenBindingId;
	}
}
