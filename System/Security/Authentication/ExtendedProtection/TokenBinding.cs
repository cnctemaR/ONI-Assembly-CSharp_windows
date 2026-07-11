using System;
using Unity;

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

		internal TokenBinding()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private byte[] _rawTokenBindingId;
	}
}
